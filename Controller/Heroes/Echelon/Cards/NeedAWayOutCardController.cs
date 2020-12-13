using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class NeedAWayOutCardController : CardController
    {
        //==============================================================
        //==============================================================

        public static string Identifier = "NeedAWayOut";

        public NeedAWayOutCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}