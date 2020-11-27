using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class LookingForThisCardController : CardController
    {
        public LookingForThisCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"One player may return one of their non-character cards in play to their hand.",
            //"If they do, they may select a card in their trash that shares a keyword with that card and put it into play."

            yield break;
        }
    }
}