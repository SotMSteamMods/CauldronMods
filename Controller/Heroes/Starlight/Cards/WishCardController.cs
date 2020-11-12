using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class WishCardController : CardController
    {
        public WishCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"1 player may look at the top 5 cards of their deck, put 1 of them into play, then put the rest on the bottom of their deck in any order."
            yield break;
        }
    }
}