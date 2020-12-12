using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class HurledObstructionCardController : CardController
    {
        public HurledObstructionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {Impact} deals up to 3 targets 1 projectile damage each.",
            yield break;
        }

        public override void AddTriggers()
        {
            //"Reduce damage dealt by villain targets by 1.",
            //"At the start of your turn, destroy this card."
        }
    }
}