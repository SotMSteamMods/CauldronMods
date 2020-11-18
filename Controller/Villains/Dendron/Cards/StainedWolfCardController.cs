using System;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dendron
{
    public class StainedWolfCardController : CardController
    {
        public static string Identifier = "StainedWolf";

        public StainedWolfCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}