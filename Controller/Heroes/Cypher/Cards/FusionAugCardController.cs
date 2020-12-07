using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Cypher
{
    public class FusionAugCardController : CardController
    {
        //==============================================================
        // Play this card next to a hero. The hero next to this card is augmented.
        // That hero may use an additional power during their power phase.
        //==============================================================

        public static string Identifier = "FusionAug";

        public FusionAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            IEnumerator routine = base.IncreasePhaseActionCountIfInPhase(tt => tt == base.TurnTaker,
                Phase.UsePower, 1);

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