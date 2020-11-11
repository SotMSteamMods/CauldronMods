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
            base.AddTrigger<DealDamageAction>(dealDamageAction => dealDamageAction.Target.IsTarget,
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
                SelectionType.DealDamage, card, null, storedResults, null, base.GetCardSource(null));

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (base.DidPlayerAnswerYes(storedResults))
            {
                // Gain HP instead of dealing damage
            }
            else
            {
                // Deal Damage

                routine = this.GameController.DealDamageToTarget(new DamageSource(this.GameController, this.CharacterCard),
                    dda.Target,
                    dda.Amount, dda.DamageType, dda.IsIrreducible, dda.WasOptional, cardSource: dda.CardSource);

               // routine = this.GameController.SelectTargetsAndDealDamage(this.DecisionMaker,
               //     new DamageSource(this.GameController, this.CharacterCard), dda.Amount,
               //     dda.DamageType, new int?(), dda.WasOptional, new int?(), dda.IsIrreducible, 
               //     cardSource: dda.CardSource);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }
            }

            yield break;
        }
    }
}
