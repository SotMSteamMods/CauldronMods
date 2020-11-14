using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.BlackwoodForest
{
    public class VengefulSpiritsCardController : CardController
    {
        //==============================================================
        // At the start of the environment turn, shuffle the villain trash
        // and reveal cards until a target is revealed.
        // Put that target into play, then this card deals that
        // target 2 infernal damage.
        // At the end of the environment turn, each player may
        // discard 2 cards to destroy this card.
        //==============================================================

        public static string Identifier = "VengefulSpirits";

        private const int DamageToDeal = 2;
        private const int NumberOfCardMatches = 1;
        private const int NumberOfCardsToDiscard = 2;


        public VengefulSpiritsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // At the start of the environment turn, shuffle the villain trash and reveal cards until a target is revealed.
            // Put that target into play, then this card deals that target 2 infernal damage.
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, StartOfTurnShuffleVillianTrashResponse,
                TriggerType.ShuffleDeck, null, false);

            // At the end of the environment turn, each player may discard 2 cards to destroy this card.
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, EndOfTurnOptionalDestruction,
                TriggerType.DestroyCard, null, false);

            base.AddTriggers();
        }

        private IEnumerator StartOfTurnShuffleVillianTrashResponse(PhaseChangeAction pca)
        {
            Location villainTrash = base.FindLocationsWhere(location => location.IsTrash && location.IsVillain).First();

            // Shuffle villain trash
            IEnumerator shuffleTrashRoutine = base.GameController.ShuffleLocation(villainTrash);

            // Reveal cards until target is revealed (if any)
            List<RevealCardsAction> revealedCards = new List<RevealCardsAction>();
            IEnumerator revealCardsRoutine = base.GameController.RevealCards(this.TurnTakerController,
                villainTrash, card => card.IsTarget, NumberOfCardMatches, revealedCards);


            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(shuffleTrashRoutine);
                yield return base.GameController.StartCoroutine(revealCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(shuffleTrashRoutine);
                base.GameController.ExhaustCoroutine(revealCardsRoutine);
            }

            if (!revealedCards.Any() || !revealedCards.First().FoundMatchingCards)
            {
                yield break;
            }

            // Eligible card found, put it into play and deal it 2 infernal damage
            Card matchedCard = revealedCards.First().MatchingCards.First();

            IEnumerator playCardRoutine = this.GameController.PlayCard(this.TurnTakerController, matchedCard, true);
            IEnumerator dealDamageRoutine = this.DealDamage(this.Card, matchedCard, DamageToDeal, DamageType.Infernal);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCardRoutine);
                yield return base.GameController.StartCoroutine(dealDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCardRoutine);
                base.GameController.ExhaustCoroutine(dealDamageRoutine);
            }
        }

        private IEnumerator EndOfTurnOptionalDestruction(PhaseChangeAction pca)
        {
            List<DiscardCardAction> discardedCards = new List<DiscardCardAction>();
            IEnumerator discardCardsRoutine 
                = this.GameController.EachPlayerDiscardsCards(0, NumberOfCardsToDiscard,
                discardedCards);


            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(discardCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(discardCardsRoutine);
            }

            if (discardedCards.Count != (this.Game.H * NumberOfCardsToDiscard))
            {
                yield break;
            }

            // Required cards discarded, destroy this card
            IEnumerator destroyRoutine = base.GameController.DestroyCard(this.HeroTurnTakerController, this.Card);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyRoutine);
            }
        }
    }
}