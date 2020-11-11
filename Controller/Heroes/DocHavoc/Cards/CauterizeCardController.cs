using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        private IEnumerator ChooseDamageOrHealResponse(DealDamageAction dda)
        {
            Card card = dda.Target;

            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();

            IEnumerator routine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController,
                SelectionType.GainHP, card, null, storedResults, null, base.GetCardSource(null));

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // If not true, just return and let the original damage happen
            if (!base.DidPlayerAnswerYes(storedResults))
            {
                yield break;
            }


            // Cancel original damage
            routine = base.CancelAction(dda);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // Gain HP instead of dealing damage
            routine = this.GameController.GainHP(card, dda.Amount);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            yield break;
        }
    }
}
