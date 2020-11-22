﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class BrawlerCardController : CardController
    {
        public static string Identifier = "Brawler";

        public BrawlerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
    }
}
