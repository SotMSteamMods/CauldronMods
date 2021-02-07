using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public abstract class OutlanderUtilityCardController : CardController
    {
        protected OutlanderUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public bool IsTrace(Card c)
        {
            return c.DoKeywordsContain("trace");
        }
    }
}
