using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Cypher
{
    public class CyborgPunchCardController : CardController
    {
        //==============================================================
        // You may move 1 Augment in play next to a new hero.
        // One augmented hero deals 1 target 1 melee damage and draws a card now.
        //==============================================================

        public static string Identifier = "CyborgPunch";

        public CyborgPunchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}