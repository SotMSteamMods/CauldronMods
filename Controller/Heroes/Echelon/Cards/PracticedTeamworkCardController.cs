using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class PracticedTeamworkCardController : CardController
    {
        //==============================================================
        //==============================================================

        public static string Identifier = "PracticedTeamwork";

        public PracticedTeamworkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}