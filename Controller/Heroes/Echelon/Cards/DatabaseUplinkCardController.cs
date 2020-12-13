using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class DatabaseUplinkCardController : CardController
    {
        //==============================================================
        //==============================================================

        public static string Identifier = "DatabaseUplink";

        public DatabaseUplinkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}