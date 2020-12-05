using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Cypher
{
    public class RapidPrototypingCardController : CardController
    {
        //==============================================================
        // Draw 2 cards.
        // Play any number of Augments from your hand.
        //==============================================================

        public static string Identifier = "RapidPrototyping";

        public RapidPrototypingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}