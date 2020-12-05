using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Cypher
{
    public class NeuralInterfaceCardController : CardController
    {
        //==============================================================
        // Power: You may move 1 Augment in play next to a new hero.
        // Draw 2 cards. Discard a card
        //==============================================================

        public static string Identifier = "NeuralInterface";

        public NeuralInterfaceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}