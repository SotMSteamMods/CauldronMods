using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class WipeoutCardController : GyrosaurUtilityCardController
    {
        public WipeoutCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"{Gyrosaur} deals up to X+1 targets 4 melee damage each, then deals herself X+1 melee damage, where X is the number of Crash cards in your hand."
            yield break;
        }
    }
}
