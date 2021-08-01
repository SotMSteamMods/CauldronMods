using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vanish
{
    public class FlashReconCardController : CardController
    {
        public FlashReconCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        /* 
         * Reveal the top card of each deck, then replace it. 
         * Select a deck and put its top card into play.
         */
        public override IEnumerator Play()
        {
            List<Location> decks = new List<Location>();
            foreach (TurnTaker tt in Game.TurnTakers)
            {
                if (tt.IsIncapacitatedOrOutOfGame) continue;
                if (tt.Deck.IsRealDeck && GameController.IsLocationVisibleToSource(tt.Deck, GetCardSource()))
                {
                    decks.Add(tt.Deck);
                }
                decks = decks.Concat(tt.SubDecks.Where(l => l.IsRealDeck && GameController.IsLocationVisibleToSource(l, GetCardSource()))).ToList();
            }

            List<Card> revealedCards = new List<Card>();
            var coroutine = GameController.SelectLocationsAndDoAction(DecisionMaker, SelectionType.RevealTopCardOfDeck, l => decks.Contains(l) && l.NumberOfCards > 0, (Location loc) => RevealTopCardAndReturn(loc, revealedCards), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }


            IEnumerable<Card> choices = decks.Where(deck => deck.NumberOfCards > 0).Select(deck => deck.TopCard);
            IEnumerable<CardController> cardsToFlip = choices.Except(revealedCards).Select(c => FindCardController(c));
            coroutine = GameController.FlipCards(cardsToFlip, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = SelectAndPlayCardWithFlipBack(DecisionMaker, choices, cardsToFlip);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private IEnumerator RevealTopCardAndReturn(Location loc, List<Card> revealedCards)
        {
            List<Card> result = new List<Card>();
            List<RevealCardsAction> actionResult = new List<RevealCardsAction>();
            var coroutine = GameController.RevealCards(this.TurnTakerController, loc, 1, result, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, storedResultsAction: actionResult, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = base.CleanupCardsAtLocations(new List<Location> { loc.OwnerTurnTaker.Revealed }, loc, cardsInList: result);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (!actionResult.Any())
            {
                yield break;
            }

            RevealCardsAction revealAction = actionResult.First();

            if (!revealAction.RevealedCards.Any() && !revealAction.RemovedFromRevealedCards.Any())
            {
                yield break;
            }

            Card revealedCard = revealAction.RevealedCards.Any() ? revealAction.RevealedCards.First() : revealAction.RemovedFromRevealedCards.First();
            revealedCards.Add(revealedCard);

            yield break;
        }

        public IEnumerator SelectAndPlayCardWithFlipBack(HeroTurnTakerController hero, IEnumerable<Card> choices, IEnumerable<CardController> cardsToFlip)
        {
            if (!choices.Any((Card c) => GameController.CanPlayCard(FindCardController(c), isPutIntoPlay: true) == CanPlayCardResult.CanPlay))
            {
                IEnumerator coroutine = GameController.SendMessageAction("None of the cards can be played.", Priority.Medium, GetCardSource(), choices, showCardSource: true);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                yield break;
            }
            SelectCardDecision selectCardDecision = new SelectCardDecision(GameController, hero, SelectionType.PutIntoPlay, choices, cardSource: GetCardSource());
            IEnumerator coroutine2 = GameController.SelectCardAndDoAction(selectCardDecision, (SelectCardDecision d) => FlipAndPlayCard(hero, d.SelectedCard, cardsToFlip));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine2);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine2);
            }
        }

        private IEnumerator FlipAndPlayCard(HeroTurnTakerController hero, Card selectedCard, IEnumerable<CardController> cardsToFlip)
        {
            IEnumerator coroutine = GameController.FlipCards(cardsToFlip, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.PlayCard(hero, selectedCard, isPutIntoPlay: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}
        
