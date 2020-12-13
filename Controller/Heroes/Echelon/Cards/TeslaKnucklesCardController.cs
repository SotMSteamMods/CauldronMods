using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class TeslaKnucklesCardController : CardController
    {
        //==============================================================
        //==============================================================

        public static string Identifier = "TeslaKnuckles";

        public TeslaKnucklesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}