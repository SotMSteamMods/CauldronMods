using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class AcceleratedCollisionCardController : CardController
    {
        public AcceleratedCollisionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"{Impact} deals 1 target 2 infernal damage.",
            //"You may play a card."
            yield break;
        }
    }
}