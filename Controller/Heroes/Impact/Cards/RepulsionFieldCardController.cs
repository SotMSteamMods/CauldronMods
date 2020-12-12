using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class RepulsionFieldCardController : CardController
    {
        public RepulsionFieldCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {Impact} deals each non-hero target 1 energy damage.",
            yield break;
        }

        public override void AddTriggers()
        {
            //"Reduce damage dealt to {Impact} by 1."
        }
    }
}