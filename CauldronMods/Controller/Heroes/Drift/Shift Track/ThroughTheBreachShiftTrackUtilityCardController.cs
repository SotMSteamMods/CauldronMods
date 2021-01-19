using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class ThroughTheBreachShiftTrackUtilityCardController : ShiftTrackUtilityCardController
    {
        public ThroughTheBreachShiftTrackUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //Cards added to your shift track are placed face up next to 1 of its 4 spaces. Each space may only have 1 card next to it. They are not considered in play.
        //When you discard a card from the track, you may play it or {Drift} may deal 1 target 3 radiant damage.
    }
}
