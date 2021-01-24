using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class RogueFissionCascadeCardController : PyreUtilityCardController
    {
        public RogueFissionCascadeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddStartOfGameTriggers()
        {
            //"When this card enters your hand, put it into play.",
        }

        public override IEnumerator Play()
        {
            
            //"{Pyre} deals each hero with {PyreIrradiate} cards in their hand X energy damage, where X is the number of {PyreIrradiate} cards in all hands.",
            //"Reveal the top card of your deck and draw or discard it."
            yield break;
        }
    }
}
