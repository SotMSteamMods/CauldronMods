using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class SameTimeAndPlaceCardController : TheMistressOfFateUtilityCardController
    {
        public SameTimeAndPlaceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {TheMistressOfFate} deals each hero 10 melee damage.",
            yield break;
        }

        public override void AddTriggers()
        {
            //"If there is no card beneath this one and an environment card leaves play, put that card beneath this one. When a Day card flips face down, put any cards beneath this one on top of the environment deck."
        }
    }
}
