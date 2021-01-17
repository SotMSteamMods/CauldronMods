using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class HyperspinCardController : GyrosaurUtilityCardController
    {
        public HyperspinCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, you may play a card.",
            yield break;
        }

        public override void AddTriggers()
        {
            //"Increase damage dealt by {Gyrosaur} to non-hero targets by 1.",
            //"If you would draw a Crash card, play it instead. Then, destroy all copies of Hyperspin."
        }
    }
}
