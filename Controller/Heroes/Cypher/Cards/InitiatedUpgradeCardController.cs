using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Cypher
{
    public class InitiatedUpgradeCardController : CardController
    {
        //==============================================================
        // Search your deck or trash for an Augment card and put it into play.
        // If you searched your deck, shuffle it.
        // You may draw a card.
        //==============================================================

        public static string Identifier = "InitiatedUpgrade";

        public InitiatedUpgradeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}