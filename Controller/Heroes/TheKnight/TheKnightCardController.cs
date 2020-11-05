using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.TheKnight
{
    public abstract class TheKnightCardController : CardController
    {
        public const string SingleHandKeyword = "single hand";

        protected TheKnightCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected bool IsSingleHandCard(Card card)
        {
            return card.DoKeywordsContain(SingleHandKeyword, evenIfUnderCard: true);
        }
    }
}
