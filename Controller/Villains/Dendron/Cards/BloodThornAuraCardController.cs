using System;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class BloodThornAuraCardController : CardController
    {
        public static string Identifier = "BloodThornAura";

        public BloodThornAuraCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}