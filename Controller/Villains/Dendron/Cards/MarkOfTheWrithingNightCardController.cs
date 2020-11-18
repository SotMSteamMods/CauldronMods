using System;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class MarkOfTheWrithingNightCardController : CardController
    {
        public static string Identifier = "MarkOfTheWrithingNight";

        public MarkOfTheWrithingNightCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}