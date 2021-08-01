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
        public override bool AllowFastCoroutinesDuringPretend => HasBeenSetToTrueThisTurn(FirstTimeWouldBeDealtDamage);
        public ViolentAssistCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeWouldBeDealtDamage);
        }

        private int? DamageToPrevent = null;
        private bool? ShouldPreventDamage = null;

        public override void AddTriggers()
        {
            //base.AddTrigger<DealDamageAction>((dda) => !base.HasBeenSetToTrueThisTurn(FirstTimeWouldBeDealtDamage) && dda.DamageSource != null && dda.DamageSource.Card != null && dda.DamageSource.IsHero && dda.DamageSource.Card != base.CharacterCard && dda.DamageSource.IsTarget && dda.Target == base.CharacterCard, IncreaseNextDamageResponse, TriggerType.WouldBeDealtDamage, TriggerTiming.Before, isActionOptional: true);

            AddOptionalPreventDamageTrigger(dda => !base.HasBeenSetToTrueThisTurn(FirstTimeWouldBeDealtDamage) && dda.DamageSource != null && dda.DamageSource.Card != null && dda.DamageSource.IsHero && dda.DamageSource.Card != base.CharacterCard && dda.DamageSource.IsTarget && dda.Target == base.CharacterCard, dda => IncreaseGargoyleNextDamage(dda.Amount), new List<TriggerType> { TriggerType.CreateStatusEffect }, isPreventEffect: true);
            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(FirstTimeWouldBeDealtDamage), TriggerType.Hidden);
        }

        private IEnumerator IncreaseNextDamageResponse(DealDamageAction dealDamageAction)
        {
            IEnumerator coroutine;
            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
            int valueOfX = 0;

            coroutine = base.GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.PreventDamage, base.Card, action: dealDamageAction, storedResults: storedResults, cardSource: base.GetCardSource());
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
                base.SetCardPropertyToTrueIfRealAction(FirstTimeWouldBeDealtDamage);
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
            yield break;
        }

        private ITrigger AddOptionalPreventDamageTrigger(Func<DealDamageAction, bool> damageCriteria, Func<DealDamageAction, IEnumerator> followUpResponse = null, IEnumerable<TriggerType> followUpTriggerTypes = null, bool isPreventEffect = false)
        {
            DealDamageAction preventedDamage = null;
            List<CancelAction> cancelDamage = null;
            Func<DealDamageAction, IEnumerator> response = delegate (DealDamageAction dd)
            {
                preventedDamage = dd;
                cancelDamage = new List<CancelAction>();
                IEnumerator enumerator2 = AskAndMaybeCancelAction(dd, showOutput: true, cancelFutureRelatedDecisions: true, cancelDamage, isPreventEffect);
                if (UseUnityCoroutines)
                {
                    return enumerator2;
                }
                GameController.ExhaustCoroutine(enumerator2);
                return DoNothing();
            };
            Func<DealDamageAction, IEnumerator> response2 = delegate (DealDamageAction dd)
            {
                preventedDamage = null;
                cancelDamage = null;
                IEnumerator enumerator = followUpResponse(dd);
                if (UseUnityCoroutines)
                {
                    return enumerator;
                }
                GameController.ExhaustCoroutine(enumerator);
                return DoNothing();
            };
            ITrigger result = AddTrigger((DealDamageAction dd) => damageCriteria(dd) && dd.Amount > 0 && dd.CanDealDamage, response, TriggerType.WouldBeDealtDamage, TriggerTiming.Before, isActionOptional: true);
            if (followUpResponse != null && followUpTriggerTypes != null)
            {
                AddTrigger((DealDamageAction dd) => dd == preventedDamage && !dd.IsSuccessful && cancelDamage != null && cancelDamage.FirstOrDefault() != null && cancelDamage.First().CardSource.Card == Card, response2, followUpTriggerTypes, TriggerTiming.After, null, isConditional: false, requireActionSuccess: false);
            }
            return result;
        }

        private IEnumerator AskAndMaybeCancelAction(DealDamageAction ga, bool showOutput = true, bool cancelFutureRelatedDecisions = true, List<CancelAction> storedResults = null, bool isPreventEffect = false)
        {
            //ask and set 
            if (GameController.PretendMode || ga.ActionIdentifier != DamageToPrevent)
            {
                List<YesNoCardDecision> storedYesNo = new List<YesNoCardDecision>();
                IEnumerator coroutine = base.GameController.MakeYesNoCardDecision(this.DecisionMaker,
                    SelectionType.PreventDamage, ga.Target, action: ga, storedResults: storedYesNo,
                    associatedCards: new[] { ga.Target, ga.Target },
                    cardSource: base.GetCardSource());

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                DamageToPrevent = ga.ActionIdentifier;
                if (DidPlayerAnswerYes(storedYesNo))
                {
                    ShouldPreventDamage = true;
                }
                else
                {
                    ShouldPreventDamage = false;
                }
            }
            if (ShouldPreventDamage == true)
            {
                if(IsRealAction(ga))
                {
                    SetCardPropertyToTrueIfRealAction(FirstTimeWouldBeDealtDamage);
                }

                IEnumerator coroutine = base.CancelAction(ga, showOutput: showOutput, cancelFutureRelatedDecisions: cancelFutureRelatedDecisions, storedResults: storedResults, isPreventEffect: isPreventEffect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            if (IsRealAction(ga))
            {
                DamageToPrevent = null;
                ShouldPreventDamage = null;
            }
            yield break;
        }
    }
}
