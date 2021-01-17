using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class RapturianShellCardController : GyrosaurUtilityCardController
    {
        public RapturianShellCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"At the end of your turn, if you have 0 Crash cards in your hand, {Gyrosaur} deals 1 other hero 2 psychic damage.",
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"Play a Crash card, or discard cards from the top of your deck until you discard a Crash card and put it into your hand."
            yield break;
        }
    }
}
