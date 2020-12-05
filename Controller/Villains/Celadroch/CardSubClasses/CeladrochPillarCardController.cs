using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public abstract class CeladrochPillarCardController : CardController
    {
        private readonly CeladrochPillarRewards _rewards;
        private readonly TriggerType _rewardType;
        private readonly string PendingTriggerKey = "PendingTriggerKey";

        protected CeladrochPillarCardController(Card card, TurnTakerController turnTakerController, TriggerType rewardType) : base(card, turnTakerController)
        {
            _rewards = new CeladrochPillarRewards(H);
            _rewardType = rewardType;

            SpecialStringMaker.ShowSpecialString(RemainingRewardsSpecialString);
            SpecialStringMaker.ShowSpecialString(HpTilNextRewardSpecialString).Condition = () => _rewards.HpTillNextTrigger(Card.HitPoints.Value) > 0;

            SpecialStringMaker.ShowSpecialString(() => $"DEBUG - Pending Triggers {GetCardPropertyJournalEntryInteger(PendingTriggerKey) ?? 0}");
        }

        // This card may not regain HP.
        public override bool AskIfActionCanBePerformed(GameAction gameAction)
        {
            if (gameAction is GainHPAction hpAction && hpAction.HpGainer == Card)
            {
                return false;
            }

            return base.AskIfActionCanBePerformed(gameAction);
        }

        public override void AddTriggers()
        {
            //Reduce damge dealt to Celadroch by 1.
            AddReduceDamageTrigger(c => c.IsTarget && c.Identifier == "Celadroch", 1);

            //reward trigger
            AddTrigger<DealDamageAction>(dda => dda.Target == Card && dda.DidDealDamage, PillarCardRewardResponse, _rewardType, TriggerTiming.After);

            //when this card would leave play, remove it from the game.
            AddTrigger<MoveCardAction>(mca => mca.CardToMove == Card && mca.Origin.IsPlayArea && !mca.Destination.HighestRecursiveLocation.IsInPlay, MoveOutOfPlayResponse, TriggerType.RemoveFromGame, TriggerTiming.Before);
        }

        private IEnumerator PillarCardRewardResponse(DealDamageAction action)
        {
            //plan:
            // pull pending triggers
            // push new trigger count
            // do loop
            // pull pending triggers
            // if triggers, loop
            // fire the trigger
            // loop

            int pendingTriggers = GetCardPropertyJournalEntryInteger(PendingTriggerKey) ?? 0;
            int newTriggers = _rewards.NumberOfTriggers(action.TargetHitPointsAfterBeingDealtDamage.Value, action.TargetHitPointsBeforeBeingDealtDamage.Value);
            pendingTriggers += newTriggers;

            SetCardProperty(PendingTriggerKey, pendingTriggers);
            while (pendingTriggers > 0)
            {
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

                pendingTriggers = GetCardPropertyJournalEntryInteger(PendingTriggerKey) ?? 0;
                if (pendingTriggers > 0)
                    pendingTriggers--;
                if (!DidSelectTurnTaker(selected))
                {
                    pendingTriggers = 0; // skip == done
                }

                SetCardProperty(PendingTriggerKey, pendingTriggers);
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
            return DoNothing();
        }
    }
}