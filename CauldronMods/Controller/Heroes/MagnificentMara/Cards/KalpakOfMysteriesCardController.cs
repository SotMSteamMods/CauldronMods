using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class KalpakOfMysteriesCardController : CardController
    {
        public KalpakOfMysteriesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"Reveal the top card of each other hero deck. One player may put their revealed card into their hand or into play. Replace or discard the other revealed cards. If a card is put into play this way, destroy this card."
            var storedMoveActions = new List<MoveCardAction> { };
            var storedSpecialMove = new List<MoveCardAction> { };
            var storedReveals = new List<RevealCardsAction>();

            bool destroyAtEnd = false;

            //Reveal the top card of each deck.
            var otherHeroControllers = FindActiveHeroTurnTakerControllers().Where((HeroTurnTakerController httc) => httc != this.HeroTurnTakerController);
            foreach(HeroTurnTakerController heroTTC in otherHeroControllers)
            {
                Card cardToReveal = heroTTC.TurnTaker.Deck.TopCard;
                if (cardToReveal != null)
                {
                    var cards = new List<Card>();
                    IEnumerator reveal = GameController.RevealCards(DecisionMaker, heroTTC.TurnTaker.Deck, 1, cards, storedResultsAction: storedReveals, cardSource: GetCardSource());
                    //IEnumerator reveal = GameController.MoveCard(DecisionMaker, cardToReveal, heroTTC.TurnTaker.Revealed, storedResults: storedMoveActions, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(reveal);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(reveal);
                    }
                }
                else
                {
                    IEnumerator message = GameController.SendMessageAction($"There was no card to reveal from {heroTTC.TurnTaker.Deck.GetFriendlyName()}.", Priority.Medium, GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(message);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(message);
                    }
                }
            }

            var revealedCards = storedReveals.SelectMany((RevealCardsAction rc) => rc.RevealedCards).ToList();

            //One player may put their revealed card into play or into their hand.
            var selectSpecial = new List<SelectCardDecision> { };
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.MoveCard, revealedCards, selectSpecial, optional: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(DidSelectCard(selectSpecial))
            {
                coroutine = PutIntoPlayOrHand(selectSpecial.First().SelectedCard, storedSpecialMove);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                if (storedSpecialMove.FirstOrDefault() != null && storedSpecialMove.FirstOrDefault().Destination.IsInPlay)
                {
                    destroyAtEnd = true;
                    revealedCards.Remove(selectSpecial.First().SelectedCard);
                }
            }

            //Replace or discard the other revealed cards.
            foreach (RevealCardsAction reveal in storedReveals)
            {
                if (reveal.RevealedCards.Count() > 0)
                {
                    Card revealedCard = reveal.RevealedCards.FirstOrDefault();
                    if (revealedCard != null && revealedCard.Location.IsRevealed)
                    {
                        IEnumerator handleCard = ReplaceOrDiscard(revealedCard, reveal.SearchLocation);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(handleCard);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(handleCard);
                        }
                    }
                }
            }

            foreach(HeroTurnTakerController heroTTC in GameController.AllHeroControllers)
            {
                IEnumerator cleanup = CleanupCardsAtLocations(new List<Location> { heroTTC.TurnTaker.Revealed },
                                                                heroTTC.TurnTaker.Deck,
                                                                cardsInList: revealedCards);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(cleanup);
                }
                else
                {
                    GameController.ExhaustCoroutine(cleanup);
                }
            }

            //If a card is put into play this way, destroy this card.
            if (destroyAtEnd)
            {
                IEnumerator destroy = DestroyThisCardResponse(null);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(destroy);
                }
                else
                {
                    GameController.ExhaustCoroutine(destroy);
                }
            }

            yield break;
        }

        private IEnumerator PutIntoPlayOrHand(Card cardToMove, List<MoveCardAction> moveCardStorage)
        {
            var functions = new List<Function>
            {
                new Function(DecisionMaker, $"Put {cardToMove.Title} in play", SelectionType.PutIntoPlay, () => GameController.MoveCard(FindHeroTurnTakerController(cardToMove.Owner.ToHero()), cardToMove, cardToMove.Owner.PlayArea, isPutIntoPlay: true, responsibleTurnTaker: DecisionMaker.TurnTaker, storedResults: moveCardStorage, cardSource:GetCardSource())),
                new Function(DecisionMaker, $"Put {cardToMove.Title} in hand", SelectionType.MoveCardToHand, () => GameController.MoveCard(FindHeroTurnTakerController(cardToMove.Owner.ToHero()), cardToMove, cardToMove.Owner.ToHero().Hand, responsibleTurnTaker: DecisionMaker.TurnTaker, storedResults: moveCardStorage, cardSource: GetCardSource())),
            };
            var selectFunction = new SelectFunctionDecision(GameController, DecisionMaker, functions, optional: true, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectAndPerformFunction(selectFunction);
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

        private IEnumerator ReplaceOrDiscard(Card cardToMove, Location deckItCameFrom)
        {
            var deckController = FindHeroTurnTakerController(deckItCameFrom.OwnerTurnTaker.ToHero());
            var functions = new List<Function>
            {
                new Function(deckController, $"Put {cardToMove.Title} back on top of the deck", SelectionType.MoveCardOnDeck, () => GameController.MoveCard(deckController, cardToMove, deckItCameFrom, responsibleTurnTaker: DecisionMaker.TurnTaker, cardSource: GetCardSource())),
                new Function(deckController, $"Discard {cardToMove.Title}", SelectionType.DiscardCard, () => GameController.MoveCard(deckController, cardToMove, cardToMove.NativeTrash, responsibleTurnTaker: DecisionMaker.TurnTaker, cardSource:GetCardSource()))
            };
            var selectFunction = new SelectFunctionDecision(GameController, deckController, functions, optional: false, associatedCards: new List<Card> { cardToMove }, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectAndPerformFunction(selectFunction, associatedCards: new List<Card> { cardToMove });
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
    }
}