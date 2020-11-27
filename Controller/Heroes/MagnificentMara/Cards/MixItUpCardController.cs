using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class MixItUpCardController : CardController
    {
        public MixItUpCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Destroy 1 hero ongoing card, equipment card, or environment card.",
            //"If you do, reveal the top 2 cards of the associated deck, put one into play and discard the other."

            yield break;
        }
    }
}