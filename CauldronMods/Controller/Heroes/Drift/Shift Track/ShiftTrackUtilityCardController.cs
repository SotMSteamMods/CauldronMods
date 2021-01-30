using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public abstract class ShiftTrackUtilityCardController : DriftBaseCardController
    {
        protected ShiftTrackUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}
