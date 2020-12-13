using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class FindAWayInCardController : CardController
    {
        //==============================================================
        //==============================================================

        public static string Identifier = "FindAWayIn";

        public FindAWayInCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}