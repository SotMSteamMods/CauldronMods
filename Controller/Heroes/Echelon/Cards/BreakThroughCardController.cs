using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class BreakThroughCardController : CardController
    {
        //==============================================================
        //==============================================================

        public static string Identifier = "BreakThrough";

        public BreakThroughCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}