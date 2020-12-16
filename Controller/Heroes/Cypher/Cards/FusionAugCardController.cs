using System;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class FusionAugCardController : PhaseIncreasesAugCardController
    {
        //==============================================================
        // Play this card next to a hero. The hero next to this card is augmented.
        // That hero may use an additional power during their power phase.
        //==============================================================

        public static string Identifier = "FusionAug";

        public FusionAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, Phase.UsePower)
        {
        }
    }
}