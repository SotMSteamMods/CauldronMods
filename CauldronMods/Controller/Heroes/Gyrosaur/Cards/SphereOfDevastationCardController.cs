using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class SphereOfDevastationCardController : GyrosaurUtilityCardController
    {
        public SphereOfDevastationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Discard all Crash cards in your hand. {Gyrosaur} deals 1 target X+4 melee damage, where X is 4 times the number of cards discarded this way.",
            //"If {Gyrosaur} dealt more than 10 damage this way, destroy all environment cards and each other player discards a card."
            yield break;
        }
    }
}
