using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class CommandAndConquerCardController : CardController
    {
        //==============================================================
        //==============================================================

        public static string Identifier = "CommandAndConquer";

        public CommandAndConquerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}