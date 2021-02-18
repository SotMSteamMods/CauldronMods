using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public abstract class DynamoUtilityCardController : CardController
    {
        protected DynamoUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public bool IsPlot(Card c)
        {
            return c.DoKeywordsContain("plot");
        }

        public Card FindCopperhead()
        {
            return base.FindCardsWhere(new LinqCardCriteria((Card c) => c.Identifier == "Copperhead")).FirstOrDefault();
        }

        public Card FindPython()
        {
            return base.FindCardsWhere(new LinqCardCriteria((Card c) => c.Identifier == "Python")).FirstOrDefault();
        }
    }
}
