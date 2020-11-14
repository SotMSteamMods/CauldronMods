using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheWanderingIsle
{
    public abstract class TheWanderingIsleCardController : CardController
    {
        protected TheWanderingIsleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public Card FindTeryx()
        {
            return base.TurnTaker.GetAllCards().FirstOrDefault(c => IsTeryx(c));
        }

        protected bool IsTeryx(Card card)
        {
            return card.IsInPlayAndHasGameText && card.Identifier == "Teryx";
        }
    }
}
