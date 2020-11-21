using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class CauterizeCardController : CardController
    {
        //==============================================================
        // Whenever {DocHavoc} would deal damage to a target,
        // that target may instead regain that much HP.
        //==============================================================

        public static string Identifier = "Cauterize";

        public CauterizeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {

            base.AddTrigger<DealDamageAction>(dealDamageAction => dealDamageAction.DamageSource != null
                    && dealDamageAction.DamageSource.IsSameCard(base.CharacterCard),
                ChooseDamageOrHealResponse, TriggerType.DealDamage, TriggerTiming.Before);

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {

            return base.Play();
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
            coroutine = base.CancelAction(dd);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // Gain HP instead of dealing damage
            coroutine = this.GameController.GainHP(card, dd.Amount);

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
    }
}
