using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class TerrifyingMomentumCardController : GyrosaurUtilityCardController
    {
        public TerrifyingMomentumCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"{Gyrosaur} deals 1 target X+2 melee damage, where X is the number of Crash cards in your hand.",
            //"If X is more than 4, redirect this damage to the non-hero target with the lowest HP.",
            //"Draw a card."
            yield break;
        }
    }
}
