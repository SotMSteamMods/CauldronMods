using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class TheKestrelMarkIICardController : CardController
    {
        //==============================================================
        //==============================================================

        public static string Identifier = "TheKestrelMarkII";

        public TheKestrelMarkIICardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}