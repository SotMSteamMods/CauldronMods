using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class WishCardController : CardController
    {
        public WishCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

    }
}