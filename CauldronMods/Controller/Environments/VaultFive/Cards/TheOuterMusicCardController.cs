using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.VaultFive
{
    public class TheOuterMusicCardController : VaultFiveUtilityCardController
    {
        public TheOuterMusicCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, each hero target deals itself 1 psychic damage.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, (PhaseChangeAction pca) => GameController.DealDamageToSelf(DecisionMaker, (Card c) => IsHeroTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1, DamageType.Psychic, cardSource: GetCardSource()), TriggerType.DealDamage);
            //When a player plays an Artifact card from their hand, destroy this card."
            AddTrigger<PlayCardAction>((PlayCardAction pca) => pca.CardToPlay != null && pca.IsSuccessful && IsArtifact(pca.CardToPlay) && pca.CardToPlay.Owner.Identifier != pca.CardToPlay.ParentDeck.Identifier && IsHero(pca.CardToPlay.Owner) && pca.Origin == pca.CardToPlay.Owner.ToHero().Hand, DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);
        }
    }
}
