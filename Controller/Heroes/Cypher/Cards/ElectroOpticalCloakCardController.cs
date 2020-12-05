using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Cypher
{
    public class ElectroOpticalCloakCardController : CardController
    {
        //==============================================================
        // Augmented heroes are immune to damage.
        // At the start of your turn, destroy this card.
        //==============================================================

        public static string Identifier = "ElectroOpticalCloak";

        public ElectroOpticalCloakCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}