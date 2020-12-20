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

        public CauterizeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            /*
            base.AddTrigger<DealDamageAction>(dealDamageAction => dealDamageAction.DamageSource != null
                    && dealDamageAction.DamageSource.IsSameCard(base.CharacterCard) && dealDamageAction.Amount > 0,
                ChooseDamageOrHealResponse, new TriggerType[] { TriggerType.WouldBeDealtDamage, TriggerType.GainHP }, TriggerTiming.Before, isActionOptional: true);
            */
            BuildConditionalPreventDamageResponse((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsSameCard(this.CharacterCard) && dd.Amount > 0,
                                    GainHPBasedOnDamagePrevented, new TriggerType[] { TriggerType.GainHP }, true);

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {

            return base.Play();
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

        private IEnumerator ChooseDamageOrHealResponse(DealDamageAction dd)
        {
            Card card = dd.Target;

            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();

            IEnumerator coroutine = base.GameController.MakeYesNoCardDecision(this.DecisionMaker,
                SelectionType.GainHP, card, storedResults: storedResults, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // If not true, just return and let the original damage happen
            if (!base.DidPlayerAnswerYes(storedResults))
            {
                yield break;
            }


            // Cancel original damage

            var storedCancel = new List<CancelAction>();
            coroutine = base.CancelAction(dd, storedResults: storedCancel, isPreventEffect: true);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var gainHPStorage = new List<GainHPAction>();

            if (storedCancel.FirstOrDefault() != null && storedCancel.FirstOrDefault().ActionToCancel.IsSuccessful == false)
            {
                // Gain HP instead of dealing damage
                //coroutine = this.GameController.GainHP(card, dd.Amount, storedResults: gainHPStorage, cardSource: GetCardSource());
                coroutine = GameController.SelectAndGainHP(DecisionMaker, dd.Amount, false, (Card c) => c == dd.Target, 1, 1, true, storedResults: gainHPStorage, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }


            //see if we can get the game to wait for Medico Malpractice's effect
            if (IsRealAction())
            {
                if (gainHPStorage.FirstOrDefault() != null)
                {
                    var gainHP = gainHPStorage.FirstOrDefault();
                    Log.Debug($"Did action get canceled? {gainHP.IsSuccessful == false}");
                    Log.Debug($"Caused {gainHP.AmountActuallyGained} HP gain");
                    coroutine = DoNothing();
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
                else
                {
                    Log.Warning("Cauterize rushing ahead");
                }
            }
        

            yield break;
        }

        private ITrigger BuildConditionalPreventDamageResponse(Func<DealDamageAction, bool> damageCriteria, Func<DealDamageAction, IEnumerator> followUpResponse = null, IEnumerable<TriggerType> followUpTriggerTypes = null, bool isPreventEffect = false)
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

        private IEnumerator AskAndMaybeCancelAction(GameAction ga, bool showOutput = true, bool cancelFutureRelatedDecisions = true, List<CancelAction> storedResults = null, bool isPreventEffect= false)
        {
            List<YesNoCardDecision> storedYesNo = new List<YesNoCardDecision>();

            IEnumerator coroutine = base.GameController.MakeYesNoCardDecision(this.DecisionMaker,
                SelectionType.GainHP, this.Card, storedResults: storedYesNo, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // If not true, just return and let the original damage happen
            if (!base.DidPlayerAnswerYes(storedYesNo))
            {
                yield break;
            }

            coroutine = base.CancelAction(ga, showOutput: showOutput, cancelFutureRelatedDecisions: cancelFutureRelatedDecisions, storedResults: storedResults, isPreventEffect: isPreventEffect);
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
