using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.DocHavoc
{
    public class CauterizeCardController : CardController
    {
        //==============================================================
        // Whenever {DocHavoc} would deal damage to a target,
        // that target may instead regain that much HP.
        //==============================================================

        public static readonly string Identifier = "Cauterize";

        private bool? DecisionShouldHeal = null;
        private Card RememberedTarget
        {
            get;
            set;
        }

        public CauterizeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            AddOptionalPreventDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsSameCard(this.CharacterCard) && dd.Amount > 0,
                                    GainHPBasedOnDamagePrevented, new TriggerType[] { TriggerType.GainHP }, true);

            base.AddTriggers();
        }

        private IEnumerator GainHPBasedOnDamagePrevented(DealDamageAction dd)
        {
            IEnumerator coroutine = GameController.GainHP(dd.Target, dd.Amount, cardSource: GetCardSource());
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

        private IEnumerator AskAndMaybeCancelAction(DealDamageAction ga, bool showOutput = true, bool cancelFutureRelatedDecisions = true, List<CancelAction> storedResults = null, bool isPreventEffect= false)
        {
            //ask and set 
            if(GameController.PretendMode || ga.Target != RememberedTarget)
            {
                List<YesNoCardDecision> storedYesNo = new List<YesNoCardDecision>();
                IEnumerator coroutine = base.GameController.MakeYesNoCardDecision(this.DecisionMaker,
                    SelectionType.Custom, ga.Target, action: ga, storedResults: storedYesNo,
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
                
                RememberedTarget = ga.Target;
                if(DidPlayerAnswerYes(storedYesNo))
                {
                    DecisionShouldHeal = true;
                }
                else
                {
                    DecisionShouldHeal = false;
                }
            }
            if (DecisionShouldHeal == true)
            {
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

            if(IsRealAction(ga))
            {
                RememberedTarget = null;
                DecisionShouldHeal = null;
            }
            yield break;
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            string target = "the target";
            string hpAmount = "HP";
            if (decision is YesNoCardDecision yncd && yncd.GameAction is DealDamageAction dd)
            {
                target = dd.Target.Title;
                hpAmount = $"{dd.Amount} HP";
            }
            return new CustomDecisionText($"Do you want {target} to regain {hpAmount} instead of dealing damage?", $"{decision.DecisionMaker.Name} is deciding whether to cause HP gain instead of damage...", $"Should {target} regain {hpAmount} instead of taking damage?", "cause HP regain instead of damage");

        }
    }
}
