using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class ToDustCardController : TheMistressOfFateUtilityCardController
    {
        public ToDustCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"{TheMistressOfFate} deals the hero target with the second highest HP 15 projectile damage.",
            //"That hero may shuffle 10 cards from their trash into their deck. If they do, they may redirect that damage to another target."
            yield break;
        }
    }
}
