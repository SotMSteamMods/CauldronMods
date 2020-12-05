using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Cypher
{
    public class DermalAugCardController : CardController
    {
        //==============================================================
        // Play this card next to a hero. The hero next to this card is augmented.
        // Reduce damage dealt to that hero by 1.
        //==============================================================

        public static string Identifier = "DermalAug";

        public DermalAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}