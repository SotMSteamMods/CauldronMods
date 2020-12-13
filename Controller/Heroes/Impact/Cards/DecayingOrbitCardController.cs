using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class DecayingOrbitCardController : CardController
    {
        public DecayingOrbitCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {Impact} deals 1 target 2 infernal damage.",
            yield break;
        }

        public override void AddTriggers()
        {
            //"At the start of your turn, {Impact} may deal 1 target 2 projectile damage. If he does, destroy 1 of your ongoing cards."
        }
    }
}