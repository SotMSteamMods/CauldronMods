using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    public class LeechFieldCardController : GargoyleUtilityCardController
    {
        // "Once per turn, when {Gargoyle} deals or is dealt damage, or when a non-hero target is dealt damage, 
        // you may reduce that damage by 1 and increase the next damage dealt by {Gargoyle} by 1."
        private const string FirstTimeWouldBeDealtDamage = "OncePerTurn";

        public LeechFieldCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeWouldBeDealtDamage);
        }

        public override bool AllowFastCoroutinesDuringPretend
        {
            get
            {
                // This allows us to make decisions during damage actions.
                return false;
            }
        }

        public override void AddTriggers()
        {
            //ReduceDamageTrigger reduceDamageTrigger;

            base.AddTrigger<DealDamageAction>(DealDamageCritera, DealDamageResponse, new TriggerType[] { TriggerType.ModifyDamageAmount }, TriggerTiming.Before, isActionOptional: true);

            //reduceDamageTrigger = new ReduceDamageTrigger(base.GameController, DealDamageCritera, (card) => true, DealDamageResponse, true, false, base.GetCardSource());
            //base.AddTrigger(reduceDamageTrigger);

            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(FirstTimeWouldBeDealtDamage), TriggerType.Hidden);
        }

        private bool DealDamageCritera(DealDamageAction dealDamageAction)
        {
            // "Once per turn, when {Gargoyle} deals or is dealt damage, or when a non-hero target is dealt damage, 
            return !base.HasBeenSetToTrueThisTurn(FirstTimeWouldBeDealtDamage) && dealDamageAction.Amount > 0 && !dealDamageAction.IsPretend && ((dealDamageAction.DamageSource != null && dealDamageAction.DamageSource.Card != null && dealDamageAction.DamageSource.Card == base.CharacterCard) || dealDamageAction.Target == base.CharacterCard || !IsHero(dealDamageAction.Target));
        }

        private bool DealDamageTargetCriteria(Card card)
        {
            return card == base.CharacterCard || !IsHero(card);
        }
        private IEnumerator DealDamageResponse(DealDamageAction dealDamageAction)
        {
            IEnumerator coroutine;
            ITrigger trigger = null;
            YesNoDecision decision;
           
            decision = new YesNoDecision(base.GameController, DecisionMaker, SelectionType.ReduceDamageTaken, gameAction: dealDamageAction, cardSource: GetCardSource());
            coroutine = base.GameController.MakeDecisionAction(decision);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (base.DidPlayerAnswerYes(decision))
            { 
                base.SetCardPropertyToTrueIfRealAction(FirstTimeWouldBeDealtDamage);
                // you may reduce that damage by 1 
                coroutine = base.GameController.ReduceDamage(dealDamageAction, 1, trigger, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                // and increase the next damage dealt by {Gargoyle} by 1.
                coroutine = base.IncreaseGargoyleNextDamage(1);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }


            yield break;
        }
    }
}
