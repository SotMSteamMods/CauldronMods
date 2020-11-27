using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class GlimpseOfThingsToComeCardController : CardController
    {
        public GlimpseOfThingsToComeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Reveal the top card of the Villain deck, then replace it.",
            //"You may draw a card.",
            //"You may play a card."
            yield break;
        }
    }
}