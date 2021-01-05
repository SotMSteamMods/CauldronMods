using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.VaultFive
{
    public abstract class VaultFiveUtilityCardController : CardController
    {
        protected VaultFiveUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public static readonly string ArtifactKeyword = "artifact";

        protected bool IsArtifact(Card card)
        {
            return card.DoKeywordsContain(ArtifactKeyword);
        }

        protected IEnumerable<TurnTaker> GetPlayersWithArtifactsInHand()
        {
            return FindTurnTakersWhere((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame && DoesPlayerHaveArtifactInHand(tt.ToHero()));
        }

        public IEnumerable<TurnTaker> FindHeroWithMostArtifacts(CardSource cardSource)
        {
            IEnumerable<Card> displacedArtifacts = FindCardsWhere(c => IsArtifact(c) && c.Owner.Identifier != c.ParentDeck.Identifier && GameController.IsCardVisibleToCardSource(c, cardSource));
            Dictionary<TurnTaker, int> heroArtifactCounts = new Dictionary<TurnTaker, int>();
            foreach(Card artifact in displacedArtifacts)
            {
                if(heroArtifactCounts.ContainsKey(artifact.Owner))
                {
                    heroArtifactCounts[artifact.Owner]++;
                } else
                {
                    heroArtifactCounts.Add(artifact.Owner, 1);
                }
            }

            int maxCount = heroArtifactCounts.Values.Max();
            return heroArtifactCounts.Where(kvp => kvp.Value == maxCount).Select(kvp => kvp.Key);
        }

        protected bool DoesPlayerHaveArtifactInHand(HeroTurnTaker htt)
        {
            return htt.Hand.Cards.Any((Card c) => IsArtifact(c));
        }
    }
}
