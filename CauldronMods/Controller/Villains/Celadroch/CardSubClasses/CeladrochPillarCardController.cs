using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public abstract class CeladrochPillarCardController : CardController
    {
        private readonly CeladrochPillarRewards _rewards;
        private readonly TriggerType _rewardType;
        private readonly string PendingTriggersKey = "PendingTriggersKey";
        private readonly string CompletedTriggersKey = "CompletedTriggersKey";

        protected CeladrochPillarCardController(Card card, TurnTakerController turnTakerController, TriggerType rewardType) : base(card, turnTakerController)
        {
            _rewards = new CeladrochPillarRewards(H);
            _rewardType = rewardType;

            SpecialStringMaker.ShowSpecialString(RemainingRewardsSpecialString);
            SpecialStringMaker.ShowSpecialString(HpTilNextRewardSpecialString).Condition = () => _rewards.HpTillNextTrigger(Card.HitPoints.Value) > 0;

            //SpecialStringMaker.ShowSpecialString(() => $"DEBUG - Pending Triggers {GetCardPropertyJournalEntryInteger(PendingTriggersKey) ?? 0}");
            //SpecialStringMaker.ShowSpecialString(() => $"DEBUG - Completed Triggers {GetCardPropertyJournalEntryInteger(CompletedTriggersKey) ?? 0}");
        }

        // This card may not regain HP.
        public override bool AskIfActionCanBePerformed(GameAction gameAction)
        {
            //prevent HP gain via GainHP Action
            if (gameAction is GainHPAction ghpAction && ghpAction.HpGainer == Card)
            {
                return false;
            }

            //prevent HP gain via SetHP Action.  Hp decrease via Set Hp Action is allowed.
            if (gameAction is SetHPAction shpAction && shpAction.HpGainer == Card && shpAction.AmountActuallyChanged > 0)
            {
                return false;
            }

            return base.AskIfActionCanBePerformed(gameAction);
        }

        public override void AddTriggers()
        {
            //Reduce damge dealt to Celadroch by 1.
            base.AddReduceDamageTrigger(c => c.IsTarget && c == FindCard("CeladrochCharacter"), 1);

            //reward triggers
            //incremental damage trigger
            AddTrigger<DealDamageAction>(ga => ga.Target == Card && ga.DidDealDamage && DidDamageCrossThreshold(ga), PillarCardRewardDamageResponse, _rewardType, TriggerTiming.After);
            AddTrigger<SetHPAction>(ga => ga.HpGainer == Card && ga.AmountActuallyChanged < 0 && DidSetHpCrossThreshold(ga), PillarCardRewardSetHpResponse, _rewardType, TriggerTiming.After);
            //destroyed by HP damage final
            AddTrigger<DestroyCardAction>(dca => dca.CardToDestroy.Card == Card && dca.DealDamageAction != null && dca.DealDamageAction.DidDealDamage, dca => PillarCardRewardDamageResponse(dca.DealDamageAction), _rewardType, TriggerTiming.Before);

            //when this card would leave play, remove it from the game.
            AddTrigger<MoveCardAction>(mca => mca.CardToMove == Card && mca.Origin.IsPlayArea && (!mca.Destination.HighestRecursiveLocation.IsInPlay || !mca.Destination.IsUnderCard), MoveOutOfPlayResponse, TriggerType.RemoveFromGame, TriggerTiming.Before);
            AddTrigger<FlipCardAction>(fca => fca.CardToFlip.Card == Card, fca => GameController.MoveCard(TurnTakerController, Card, TurnTaker.OutOfGame, cardSource: GetCardSource()), TriggerType.RemoveFromGame, TriggerTiming.Before);
        }

        private bool DidDamageCrossThreshold(DealDamageAction ga)
        {
            return _rewards.NumberOfTriggers(ga.TargetHitPointsBeforeBeingDealtDamage.Value, ga.TargetHitPointsAfterBeingDealtDamage.Value) > 0;
        }

        private bool DidSetHpCrossThreshold(SetHPAction ga)
        {
            return _rewards.NumberOfTriggers((ga.AmountActuallyChanged * -1) + Card.HitPoints.Value, Card.HitPoints.Value) > 0;
        }

        private IEnumerator PillarCardRewardSetHpResponse(SetHPAction action)
        {
            int beforeHp = action.Amount - action.AmountActuallyChanged;
            int afterHp = action.Amount;

            return PillarCardRewardResponse(action, beforeHp, afterHp);
        }

        private IEnumerator PillarCardRewardDamageResponse(DealDamageAction action)
        {
            int beforeHp = action.TargetHitPointsBeforeBeingDealtDamage.Value;
            int afterHp = action.TargetHitPointsAfterBeingDealtDamage.Value;

            return PillarCardRewardResponse(action, beforeHp, afterHp);
        }

        private IEnumerator PillarCardRewardResponse(GameAction action, int beforeHp, int afterHp)
        {
            //plan:
            // pull pending triggers
            // push new trigger count
            // pull completed triggers
            // do loop, if pending > completed
            // push +1 complted
            // fire the trigger
            // pull pending & completed
            // loop

            int pendingTriggers = GetCardPropertyJournalEntryInteger(PendingTriggersKey) ?? 0;

            System.Console.WriteLine($"{Card.Title} - before = {beforeHp}, after = {afterHp}");
            int newTriggers = _rewards.NumberOfTriggers(beforeHp, afterHp);
            pendingTriggers += newTriggers;

            SetCardProperty(PendingTriggersKey, pendingTriggers);
            int completedTriggers = GetCardPropertyJournalEntryInteger(CompletedTriggersKey) ?? 0;
            while (pendingTriggers > completedTriggers)
            {
                System.Console.WriteLine($"{Card.Title} - Reward Trigger");
                               
                SetCardProperty(CompletedTriggersKey, completedTriggers + 1);

                List<SelectTurnTakerDecision> selected = new List<SelectTurnTakerDecision>();
                var coroutine = SelectHeroAndGrantReward(selected);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                pendingTriggers = GetCardPropertyJournalEntryInteger(PendingTriggersKey) ?? 0;
                completedTriggers = GetCardPropertyJournalEntryInteger(CompletedTriggersKey) ?? 0;

                if (!DidSelectTurnTaker(selected))
                {
                    //skip remaining triggers if any remain
                    SetCardProperty(CompletedTriggersKey, pendingTriggers);
                    completedTriggers = pendingTriggers;
                }
            }
        }

        //When this card would leave play, remove it from the game
        public override MoveCardDestination GetTrashDestination()
        {
            return new MoveCardDestination(TurnTaker.OutOfGame, false, true, true);
        }

        protected abstract IEnumerator SelectHeroAndGrantReward(List<SelectTurnTakerDecision> selected);

        private string RemainingRewardsSpecialString()
        {
            int num = _rewards.RemainingRewards(Card.HitPoints.Value);
            switch (num)
            {
                case 0: return $"{Card.Title} has used all it's rewards.";
                case 1: return $"{Card.Title} has 1 reward remaining.";
                default:
                    return $"{Card.Title} has {num} rewards remaining.";
            }
        }

        private string HpTilNextRewardSpecialString()
        {
            int num = _rewards.HpTillNextTrigger(Card.HitPoints.Value);
            switch (num)
            {
                case 0: return "";
                case 1: return $"{Card.Title} will give a reward after losing 1 more HP.";
                default:
                    return $"{Card.Title} will give a reward after losing {num} more HPs.";
            }
        }

        private IEnumerator MoveOutOfPlayResponse(MoveCardAction action)
        {
            action.SetDestination(TurnTaker.OutOfGame);
            var coroutine = ResetFlagAfterLeavesPlay(PendingTriggersKey);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = ResetFlagAfterLeavesPlay(CompletedTriggersKey);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}