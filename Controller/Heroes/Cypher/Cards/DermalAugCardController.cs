using System;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class DermalAugCardController : AugBaseCardController
    {
        //==============================================================
        // Play this card next to a hero. The hero next to this card is augmented.
        // Reduce damage dealt to that hero by 1.
        //==============================================================

        public static string Identifier = "DermalAug";

        private const int DamageReduction = 1;

        public DermalAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {

            ReduceDamageStatusEffect rdse = new ReduceDamageStatusEffect(DamageReduction);
            rdse.TargetCriteria.IsSpecificCard = base.GetCardThisCardIsNextTo();
            rdse.UntilCardLeavesPlay(this.Card);

            IEnumerator routine = base.AddStatusEffect(rdse, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
    }
}