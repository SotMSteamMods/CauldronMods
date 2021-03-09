using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public abstract class OblaskCraterUtilityCardController : CardController
    {
        protected OblaskCraterUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public static readonly string PredatorKeyword = "predator";

        protected bool IsPredator(Card card)
        {
            return card != null && card.DoKeywordsContain(PredatorKeyword);
        }
    }
}
