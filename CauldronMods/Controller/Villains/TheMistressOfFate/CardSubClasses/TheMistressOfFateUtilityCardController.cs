using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public abstract class TheMistressOfFateUtilityCardController : CardController
    {
        private Location _dayDeck;
        private Location dayDeck
        {
            get
            {
                if (_dayDeck == null)
                {
                    _dayDeck = TurnTaker.FindSubDeck("DayDeck");
                }
                return _dayDeck;
            }
        }
        protected TheMistressOfFateUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected bool IsDay(Card c)
        {
            if(c != null && c.IsRealCard && c.NativeDeck == dayDeck && c.Identifier != "TheTimeline")
            {
                return true;
            }
            return false;
        }
    }
}
