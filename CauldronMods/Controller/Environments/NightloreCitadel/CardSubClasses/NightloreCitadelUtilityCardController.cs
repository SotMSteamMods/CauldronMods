using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.NightloreCitadel
{
    public abstract class NightloreCitadelUtilityCardController : CardController
    {
        protected NightloreCitadelUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public static readonly string ConstellationKeyword = "constellation";
        public static readonly string StarlightOfOrosIdentifier = "StarlightOfOros";

        protected bool IsConstellation(Card card)
        {
            return card.DoKeywordsContain(ConstellationKeyword);
        }
        private IEnumerable<Card> FindStarlightOfOros()
        {
            return base.FindCardsWhere(c => c.Identifier == StarlightOfOrosIdentifier);
        }
        protected Card FindStarlightOfOrosInPlay()
        {
            return FindStarlightOfOros().Where(c => c.IsInPlayAndHasGameText).FirstOrDefault();
        }
    }
}
