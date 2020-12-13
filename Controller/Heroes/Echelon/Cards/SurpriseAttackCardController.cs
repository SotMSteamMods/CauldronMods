using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class SurpriseAttackCardController : CardController
    {
        //==============================================================
        //==============================================================

        public static string Identifier = "SurpriseAttack";

        public SurpriseAttackCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}