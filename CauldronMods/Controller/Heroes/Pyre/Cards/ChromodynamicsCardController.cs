using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class ChromodynamicsCardController : PyreUtilityCardController
    {
        public ChromodynamicsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Whenever a player plays a {PyreIrradiate} card, {Pyre} deals 1 target 1 energy damage."
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"Discard a card. {Pyre} deals 2 targets 2 lightning damage each."
            yield break;
        }
    }
}
