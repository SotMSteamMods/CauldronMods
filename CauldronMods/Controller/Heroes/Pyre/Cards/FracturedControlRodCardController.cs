using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class FracturedControlRodCardController : PyreUtilityCardController
    {
        public FracturedControlRodCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"If this card is {PyreIrradiate} when you play it, {Pyre} deals 1 target 3 toxic damage.",
            yield break;
        }

        public override void AddTriggers()
        {
            //"Whenever a player discards a {PyreIrradiate} card, they may destroy this card to play the discarded card."
        }
    }
}
