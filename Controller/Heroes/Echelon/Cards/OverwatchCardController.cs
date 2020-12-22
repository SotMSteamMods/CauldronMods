using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class OverwatchCardController : CardController
    {
        //==============================================================
        //Power: "Once before your next turn, when a hero target is dealt damage, {Echelon} may deal the source of that damage 3 melee damage"
        //==============================================================

        public static string Identifier = "Overwatch";

        public OverwatchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}