using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class OverwatchCardController : CardController
    {
        //==============================================================
        //==============================================================

        public static string Identifier = "Overwatch";

        public OverwatchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}