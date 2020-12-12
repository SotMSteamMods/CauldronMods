using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class MassDriverCardController : CardController
    {
        public MassDriverCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Destroy 1 environment card.",
            //"{Impact} deals 1 target X projectile damage, where X is the number of your Ongoing cards in play."
            yield break;
        }
    }
}