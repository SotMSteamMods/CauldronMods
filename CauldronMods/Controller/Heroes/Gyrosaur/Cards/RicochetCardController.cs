using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class RicochetCardController : GyrosaurUtilityCardController
    {
        public RicochetCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"{Gyrosaur} deals 1 target 2 melee damage. {Gyrosaur} deals a second target X melee damage, where X is the amount of damage she dealt to the first target.",
            //"Reduce the next damage dealt by non-hero targets damaged this way to 0."
            yield break;
        }
    }
}
