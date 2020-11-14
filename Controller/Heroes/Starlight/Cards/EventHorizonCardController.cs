using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class EventHorizonCardController : StarlightCardController
    {
        public EventHorizonCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Destroy any number of constellation cards." 
            //"For each card destroyed this way, destroy 1 ongoing or environment card."
            yield break;
        }

    }
}