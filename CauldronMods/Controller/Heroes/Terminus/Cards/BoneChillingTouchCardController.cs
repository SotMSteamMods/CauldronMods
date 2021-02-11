using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class BoneChillingTouchCardController : TerminusBaseCardController
    {
        /* 
         * A non-character target next to this card cannot have its current HP increased and cannot deal damage to {Terminus}.
         * powers
         * {Terminus} deals 1 target 2 cold damage. You may move this card next to that target.
         */

        private int TargetCount => GetPowerNumeral(0, 1);
        private int ColdDamage => GetPowerNumeral(1, 2);

        public BoneChillingTouchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddAsPowerContributor();
        }

        public override void AddTriggers()
        {
            // A non-character target next to this card cannot have its current HP increased and cannot deal damage to {Terminus}.
            AddPreventDamageTrigger(DealDamageActionCriteria, isPreventEffect: false);
            AddTrigger<GainHPAction>(GainHPActionCriteria, GainHPActionResponse, TriggerType.ModifyHPGain, TriggerTiming.Before);
            AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(alsoRemoveTriggersFromThisCard: false);

            AddTrigger((UsePowerAction up) => up.Power != null && up.Power.IsContributionFromCardSource && up.Power.CopiedFromCardController == this,
                            ReplaceWithActualPower,
                            TriggerType.FirstTrigger,
                            TriggerTiming.Before);
        }

        private IEnumerator ReplaceWithActualPower(UsePowerAction up)
        {
            IEnumerator coroutine = CancelAction(up, false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = UsePowerOnOtherCard(this.Card);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private bool DealDamageActionCriteria(DealDamageAction dealDamageAction)
        {
            return !dealDamageAction.DamageSource.Card.IsCharacter && dealDamageAction.DamageSource.Card.NextToLocation.Cards.Contains(base.Card) && dealDamageAction.DamageSource.Card.IsTarget && dealDamageAction.Target == base.CharacterCard;
        }

        private bool GainHPActionCriteria(GainHPAction gainHPAction)
        {
            return !gainHPAction.HpGainer.IsCharacter && gainHPAction.HpGainer.NextToLocation.Cards.Contains(base.Card);
        }

        private IEnumerator GainHPActionResponse(GainHPAction gainHPAction)
        {
            IEnumerator coroutine;

            coroutine = base.GameController.CancelAction(gainHPAction, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<SelectCardDecision> storedResultsDecisions = new List<SelectCardDecision>();
            List<DealDamageAction> storedDamageActions = new List<DealDamageAction>();
            List<YesNoCardDecision> yesNoCardDecisions = new List<YesNoCardDecision>();
            Card target;

            // {Terminus} deals 1 target 2 cold damage. 
            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), ColdDamage, DamageType.Cold, TargetCount, false, TargetCount, storedResultsDecisions: storedResultsDecisions, storedResultsDamage: storedDamageActions, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResultsDecisions != null && storedResultsDecisions.Count() > 0 && storedDamageActions != null && storedDamageActions.Count() > 0)
            {
                target = storedResultsDecisions.FirstOrDefault().SelectedCard;

                if (!storedDamageActions.FirstOrDefault().DidDestroyTarget)
                {
                    // You may move this card next to that target.
                    coroutine = base.GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.MoveCardNextToCard, base.Card, storedResults: yesNoCardDecisions, associatedCards: new Card[] { target }, cardSource: base.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    if (base.DidPlayerAnswerYes(yesNoCardDecisions))
                    {
                        coroutine = GameController.MoveCard(DecisionMaker, base.Card, target.NextToLocation, playCardIfMovingToPlayArea: false, cardSource: GetCardSource());
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
            yield break;
        }

        public override IEnumerable<Power> AskIfContributesPowersToCardController(CardController cardController)
        {
            Power[] powers = null;
            if(cardController == CharacterCardController)
            {
                if (!HasPowerBeenUsedThisTurn(new Power(DecisionMaker, this, CardWithoutReplacements.AllPowers.FirstOrDefault(), this.UsePower(), 0, null, GetCardSource())))
                {
                    return new Power[]
                    {
                        new Power(DecisionMaker, this, "{Terminus} deals 1 target 2 cold damage. You may move Bone-Chilling Touch next to that target.", this.DoNothing(), 0, this, GetCardSource())
                    };
                }
            }
            return powers;
        }

        private bool HasPowerBeenUsedThisTurn(Power power)
        {
            List<UsePowerJournalEntry> source = Game.Journal.UsePowerEntriesThisTurn().ToList();
            Func<UsePowerJournalEntry, bool> predicate = delegate (UsePowerJournalEntry p)
            {
                bool flag = power.CardController.CardWithoutReplacements == p.CardWithPower;
                if (!flag && power.CardController.CardWithoutReplacements.SharedIdentifier != null && power.IsContributionFromCardSource)
                {
                    flag = power.CardController.CardWithoutReplacements.SharedIdentifier == p.CardWithPower.SharedIdentifier;
                }
                if (flag)
                {
                    flag &= p.NumberOfUses == 0;
                }
                if (flag)
                {
                    flag &= power.Index == p.PowerIndex;
                }
                if (flag)
                {
                    flag &= power.IsContributionFromCardSource == p.IsContributionFromCardSource;
                }
                if (flag)
                {
                    bool flag2 = power.TurnTakerController == null && p.PowerUser == null;
                    bool flag3 = false;
                    if (power.TurnTakerController != null && power.TurnTakerController.IsHero)
                    {
                        flag3 = power.TurnTakerController.ToHero().HeroTurnTaker == p.PowerUser;
                    }
                    flag = flag && (flag2 || flag3);
                }
                if (flag)
                {
                    if (!power.IsContributionFromCardSource)
                    {
                        if (flag && power.CardController.CardWithoutReplacements.PlayIndex.HasValue && p.CardWithPowerPlayIndex.HasValue)
                        {
                            flag &= power.CardController.CardWithoutReplacements.PlayIndex.Value == p.CardWithPowerPlayIndex.Value;
                        }
                    }
                    else
                    {
                        flag &= p.CardSource == power.CardSource.Card;
                        if (power.CardSource != null && power.CardSource.Card.PlayIndex.HasValue && p.CardSourcePlayIndex.HasValue)
                        {
                            flag &= power.CardSource.Card.PlayIndex.Value == p.CardSourcePlayIndex.Value;
                        }
                    }
                }
                return flag;
            };
            int num = source.Where(predicate).Count();
            if (num > 0)
            {
                if (!GameController.StatusEffectManager.AskIfPowerCanBeReused(power, num))
                {
                    return true;
                }
                return false;
            }
            return false;
        }
    }
}
