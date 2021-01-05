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

        protected bool DoesPlayerHaveArtifactInHand(HeroTurnTaker htt)
        {
            return htt.Hand.Cards.Any((Card c) => IsArtifact(c));
        }
    }
}
