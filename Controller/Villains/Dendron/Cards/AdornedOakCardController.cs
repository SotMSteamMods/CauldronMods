using System;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class AdornedOakCardController : CardController
    {
        public static string Identifier = "AdornedOak";

        public AdornedOakCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}