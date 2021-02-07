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
        public static readonly string AethiumCannonIdentifier = "AethiumCannon";
        public static readonly string RogueConstellationIdentifier = "RogueConstellation";

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

        protected bool IsStarlightOfOrosInPlay()
        {
            return FindStarlightOfOros().Where(c => c.IsInPlayAndHasGameText).Any();
        }

        private IEnumerable<Card> FindAethiumCannon()
        {
            return base.FindCardsWhere(c => c.Identifier == AethiumCannonIdentifier);
        }
        protected Card FindAethiumCannonInPlay()
        {
            return FindAethiumCannon().Where(c => c.IsInPlayAndHasGameText).FirstOrDefault();
        }

        protected bool IsAethiumCannonInPlay()
        {
            return FindAethiumCannon().Where(c => c.IsInPlayAndHasGameText).Any();
        }

        private IEnumerable<Card> FindRogueConstellation()
        {
            return base.FindCardsWhere(c => c.Identifier == RogueConstellationIdentifier);
        }
        protected Card FindRogueConstellationInPlay()
        {
            return FindRogueConstellation().Where(c => c.IsInPlayAndHasGameText).FirstOrDefault();
        }

        protected bool IsRogueConstellationInPlay()
        {
            return FindRogueConstellation().Where(c => c.IsInPlayAndHasGameText).Any();
        }

    }
}
