using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Cypher
{
    public class MuscleAugCardController : CardController
    {
        //==============================================================
        // Play this card next to a hero. The hero next to this card is augmented.
        // Increase damage dealt by that hero by 1.
        //==============================================================

        public static string Identifier = "MuscleAug";

        public MuscleAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}