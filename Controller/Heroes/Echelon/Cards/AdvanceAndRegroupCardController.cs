using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class AdvanceAndRegroupCardController : CardController
    {
        //==============================================================
        //==============================================================

        public static string Identifier = "AdvanceAndRegroup";

        public AdvanceAndRegroupCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}