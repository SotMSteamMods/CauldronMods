using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class TeslaKnucklesCardController : CardController
    {
        //==============================================================
        //"At the end of your turn, {Echelon} may deal 1 target X lightning damage, where X is the number of Tactics destroyed during your turn."
        //Power: "{Echelon} deals each non-hero target 1 lightning damage."
        //==============================================================

        public static string Identifier = "TeslaKnuckles";

        public TeslaKnucklesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}