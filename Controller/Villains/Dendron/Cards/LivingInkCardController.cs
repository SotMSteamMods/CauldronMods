using System;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class LivingInkCardController : CardController
    {
        public static string Identifier = "LivingInk";

        public LivingInkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}