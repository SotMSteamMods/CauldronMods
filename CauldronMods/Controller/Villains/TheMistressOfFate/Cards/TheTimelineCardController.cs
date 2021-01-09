using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class TheTimelineCardController : TheMistressOfFateUtilityCardController
    {

        /*
         *    "This card is indestructible.",
         *    "Day cards cannot be affected by non-villain cards. Cards beneath villain cards are not considered in play.",
         *    "Immediately after the environment turn, flip {TheMistressOfFate}'s villain character cards if one of these things occurs:",
         *    "{Bulletpoint} The players choose to flip her.",
         *    "{Bulletpoint} All heroes are incapacitated.",
         *    "{Bulletpoint} All Day cards are face up.",
         *    "{MistressOfFateDayCards}"
         */
        public TheTimelineCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }


    }
}
