using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Cypher
{
    public class RetinalAugCardController : CardController
    {
        //==============================================================
        // Play this card next to a hero. The hero next to this card is augmented.
        // During their play phase, that hero may play an additional card.
        //==============================================================

        public static string Identifier = "RetinalAug";

        public RetinalAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}