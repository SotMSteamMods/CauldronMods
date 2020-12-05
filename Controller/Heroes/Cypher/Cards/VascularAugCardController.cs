using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Cypher
{
    public class VascularAugCardController : CardController
    {
        //==============================================================
        // Play this card next to a hero. The hero next to this card is augmented.
        // That hero regains 1HP at the end of their turn.
        //==============================================================

        public static string Identifier = "VascularAug";

        public VascularAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}