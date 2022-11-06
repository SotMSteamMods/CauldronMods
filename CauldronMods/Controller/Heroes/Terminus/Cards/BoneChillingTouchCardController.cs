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
        }

        public override void AddTriggers()
        {
            // A non-character target next to this card cannot have its current HP increased and cannot deal damage to {Terminus}.
            AddPreventDamageTrigger(DealDamageActionCriteria, isPreventEffect: false);
            AddTrigger<GainHPAction>(GainHPActionCriteria, GainHPActionResponse, TriggerType.ModifyHPGain, TriggerTiming.Before);
            AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(alsoRemoveTriggersFromThisCard: true);
        }

        private bool DealDamageActionCriteria(DealDamageAction dealDamageAction)
        {
            return dealDamageAction.DamageSource != null && dealDamageAction.DamageSource.Card != null && !dealDamageAction.DamageSource.Card.IsCharacter && dealDamageAction.DamageSource.Card.NextToLocation.Cards.Contains(base.Card) && dealDamageAction.DamageSource.Card.IsTarget && dealDamageAction.Target == base.CharacterCard;
        }

        private bool GainHPActionCriteria(GainHPAction gainHPAction)
        {
            return !gainHPAction.HpGainer.IsCharacter && gainHPAction.HpGainer.NextToLocation.Cards.Contains(base.Card);
        }

        private IEnumerator GainHPActionResponse(GainHPAction gainHPAction)
        {
            return GameController.CancelAction(gainHPAction, cardSource: base.GetCardSource());
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

            if (DidSelectCard(storedResultsDecisions) && DidDealDamage(storedDamageActions))
            {
                target = GetSelectedCard(storedResultsDecisions);
                var damageAction = storedDamageActions.FirstOrDefault();
                if (target.IsInPlayAndNotUnderCard)
                {
                    // You may move this card next to that target.
                    coroutine = base.GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.MoveCard, base.Card, storedResults: yesNoCardDecisions, associatedCards: new Card[] { target }, action: damageAction, cardSource: base.GetCardSource());
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
                        coroutine = GameController.MoveCard(DecisionMaker, base.CardWithoutReplacements, target.NextToLocation, playCardIfMovingToPlayArea: false, cardSource: GetCardSource());
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
    }
}
