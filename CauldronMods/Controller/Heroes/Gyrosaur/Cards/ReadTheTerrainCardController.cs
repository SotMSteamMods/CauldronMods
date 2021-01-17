using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class ReadTheTerrainCardController : GyrosaurUtilityCardController
    {
        public ReadTheTerrainCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"At the start of your turn, reveal the top card of your deck and replace or discard it.",
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"If {Gyrosaur} deals no damage this turn, increase damage dealt by {Gyrosaur} during your next turn to non-hero targets by 1."
            yield break;
        }
    }
}
