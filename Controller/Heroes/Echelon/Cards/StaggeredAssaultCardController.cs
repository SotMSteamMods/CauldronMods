using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class StaggeredAssaultCardController : CardController
    {
        //==============================================================
        //==============================================================

        public static string Identifier = "StaggeredAssault";

        public StaggeredAssaultCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}