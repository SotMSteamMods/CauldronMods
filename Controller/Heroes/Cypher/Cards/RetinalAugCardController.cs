using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Cypher
{
    public class RetinalAugCardController : AugBaseCardController
    {
        //==============================================================
        // Play this card next to a hero. The hero next to this card is augmented.
        // During their play phase, that hero may play an additional card.
        //==============================================================

        public static string Identifier = "RetinalAug";

        public RetinalAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator routine = base.IncreasePhaseActionCountIfInPhase(tt => tt == base.TurnTaker, 
                Phase.PlayCard, 1);
            
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