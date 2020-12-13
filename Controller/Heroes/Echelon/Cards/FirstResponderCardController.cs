using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class FirstResponderCardController : CardController
    {
        //==============================================================
        //==============================================================

        public static string Identifier = "FirstResponder";

        public FirstResponderCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}