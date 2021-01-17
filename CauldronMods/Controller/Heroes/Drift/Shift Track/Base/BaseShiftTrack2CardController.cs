using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class BaseShiftTrack2CardController : ShiftTrackUtilityCardController
    {
        public BaseShiftTrack2CardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
