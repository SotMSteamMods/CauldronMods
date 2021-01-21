using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    /* 
     * Once per turn when {Gargoyle} would be dealt damage by another hero target, you may prevent that damage.
     * If you do, increase the next damage dealt by {Gargoyle} by X, where X is the amount of damage prevented this way.
     */
    public class ViolentAssistCardController : GargoyleUtilityCardController
    {
        private const string FirstTimeWouldBeDealtDamage = "OncePerTurn";

        public ViolentAssistCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddTrigger<DealDamageAction>((dda) => !base.HasBeenSetToTrueThisTurn(FirstTimeWouldBeDealtDamage) && dda.DamageSource.IsHero && dda.DamageSource.Card != base.CharacterCard && dda.DamageSource.IsTarget && dda.Target == base.CharacterCard, IncreaseNextDamageResponse, TriggerType.WouldBeDealtDamage, TriggerTiming.Before);

            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(FirstTimeWouldBeDealtDamage), TriggerType.Hidden);
        }

        private IEnumerator IncreaseNextDamageResponse(DealDamageAction dealDamageAction)
        {
            IEnumerator coroutine;
            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
            int valueOfX = 0;

            coroutine = base.GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.PreventDamage, base.Card, storedResults: storedResults, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (base.DidPlayerAnswerYes(storedResults))
            {
                valueOfX = dealDamageAction.Amount;

                coroutine = CancelAction(dealDamageAction, showOutput: true, cancelFutureRelatedDecisions: true, isPreventEffect: false);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = base.IncreaseGargoyleNextDamage(dealDamageAction.Amount);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            base.SetCardPropertyToTrueIfRealAction(FirstTimeWouldBeDealtDamage);

            yield break;
        }
    }
}
