using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class DayOfSorrowsCardController : TheMistressOfFateUtilityCardController
    {
        public DayOfSorrowsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        /*
         * "When this card flips face up, put any cards beneath it into play. If there are none, reveal cards from the top of the villain deck until an Anomaly is revealed, put it into play, and shuffle the other revealed cards into the villain deck.",
         * "When {TheMistressOfFate} flips or that card leaves play, put it beneath this card."
         */
    }
}
