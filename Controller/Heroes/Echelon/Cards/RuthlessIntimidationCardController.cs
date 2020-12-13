using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class RuthlessIntimidationCardController : CardController
    {
        //==============================================================
        //==============================================================

        public static string Identifier = "RuthlessIntimidation";

        public RuthlessIntimidationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}