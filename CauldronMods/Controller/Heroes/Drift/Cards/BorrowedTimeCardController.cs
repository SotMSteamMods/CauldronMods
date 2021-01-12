using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class BorrowedTimeCardController : DriftUtilityCardController
    {
        public BorrowedTimeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Select {DriftL} or {DriftR}. Shift that direction up to 3 times. X is the number of times you shifted this way.
            //If you shifted at least {DriftL} this way, X targets regain 2 HP each. If you shifted {DriftR} this way, {Drift} deals X targets 3 radiant damage each.
            yield break;
        }
    }
}
