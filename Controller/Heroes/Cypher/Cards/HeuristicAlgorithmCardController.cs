using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Cypher
{
    public class HeuristicAlgorithmCardController : CardController
    {
        //==============================================================
        // Reveal cards from the top of your deck until you reveal an Augment.
        // Put it into play or into your trash. Shuffle the rest of the revealed cards into your deck.
        // If you did not put an Augment into play this way, draw 2 cards.
        //==============================================================

        public static string Identifier = "HeuristicAlgorithm";

        public HeuristicAlgorithmCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}