using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class WishCardController : StarlightCardController
    {
        public WishCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"1 player may..." 
            List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
            IEnumerator coroutine = GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.RevealCardsFromDeck, optional: true, allowAutoDecide: false, storedResults, new LinqTurnTakerCriteria((TurnTaker tt) => GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()) && !tt.IsIncapacitatedOrOutOfGame, "active heroes"), cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            TurnTaker hero = GetSelectedTurnTaker(storedResults);
            if (hero == null || !IsHero(hero))
            {
                yield break;
            }
            HeroTurnTakerController heroTTC = FindHeroTurnTakerController(hero.ToHero());

            if (heroTTC != null)
            {
                //"...look at the top 5 cards of their deck, put 1 of them into play, then put the rest on the bottom of their deck in any order."

                IEnumerator coroutine3 = RevealCardsWithCustomLogic(heroTTC);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine3);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine3);
                }
            }

            yield break;
        }

        private IEnumerator RevealCardsWithCustomLogic(HeroTurnTakerController heroTTC)
        {
            //"reveal 5 cards from the top of their deck..."
            List<Card> revealedCards = new List<Card> { };
            IEnumerator reveal = GameController.RevealCards(heroTTC, heroTTC.TurnTaker.Deck, 5, revealedCards, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(reveal);
            }
            else
            {
                GameController.ExhaustCoroutine(reveal);
            }

            //if there aren't enough cards, warn the player
            string message = null;
            switch (revealedCards.Count)
            {
                case 5:
                    break;
                case 0:
                    message = "No cards were revealed! There is no further effect.";
                    break;
                case 1:
                    message = "Only one card was revealed! It will automatically be put into play.";
                    break;
                default:
                    message = $"Only {revealedCards.Count} cards were revealed!";
                    break;
            }

            if (message != null)
            {
                IEnumerator warnNotEnoughCards = GameController.SendMessageAction(message, Priority.High, this.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(warnNotEnoughCards);
                }
                else
                {
                    GameController.ExhaustCoroutine(warnNotEnoughCards);
                }

                if (revealedCards.Count() == 0)
                {
                    yield break;
                }
            }

            //"...put 1 into play..."
            var playCardStorage = new List<PlayCardAction> { };
            IEnumerator playCard = GameController.SelectAndPlayCard(heroTTC, revealedCards, false, isPutIntoPlay: true, playCardStorage, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(playCard);
            }
            else
            {
                GameController.ExhaustCoroutine(playCard);
            }

            Card playedCard = null;
            if (DidPlayCards(playCardStorage))
            {
                playedCard = playCardStorage.FirstOrDefault().CardToPlay;
                revealedCards.Remove(playedCard);
            }

            int numCardsLeft = revealedCards.Count;
            if (numCardsLeft > 0)
            {
                //"...and the rest on the bottom of their deck in any order"
                Location heroRevealedLocation = heroTTC.TurnTaker.Revealed;
                var moveRest = GameController.SelectCardsFromLocationAndMoveThem(heroTTC,
                                                                    heroRevealedLocation,
                                                                    numCardsLeft,
                                                                    numCardsLeft,
                                                                    new LinqCardCriteria((Card c) => revealedCards.Contains(c), "remaining"),
                                                                    new[] { new MoveCardDestination(heroTTC.TurnTaker.Deck, toBottom: true) },
                                                                    allowAutoDecide: true,
                                                                    cardSource: this.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(moveRest);
                }
                else
                {
                    GameController.ExhaustCoroutine(moveRest);
                }
            }

            //clean up RevealedCards if we have to
            revealedCards.Add(playedCard);
            List<Location> cleanup = new List<Location> { heroTTC.TurnTaker.Revealed };
            IEnumerator cleanupRoutine = CleanupCardsAtLocations(cleanup, heroTTC.TurnTaker.Deck, toBottom: false, addInhibitorException: true, shuffleAfterwards: false, sendMessage: false, isDiscard: false, isReturnedToOriginalLocation: true, revealedCards);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(cleanupRoutine);
            }
            else
            {
                GameController.ExhaustCoroutine(cleanupRoutine);
            }

            yield break;
        }

        /*
        private IEnumerator RevealCardsWithArgentAdept(HeroTurnTakerController heroTTC)
        {
            //This appropriates the function used for Argent Adept's Arcane Cadence.
            //It is likely to have better error handling that my custom logic,
            //but the "put the rest on the bottom of their deck" routine works differently from most such cards.
            //Usually the *first* card you pick ends up on the bottom of the deck after everything is done, here it is the *last* card.

            //I'm keeping this around in case the custom logic ends up breaking horribly
            List<MoveCardDestination> list = new List<MoveCardDestination>
            {
                new MoveCardDestination(heroTTC.TurnTaker.PlayArea),
                new MoveCardDestination(heroTTC.TurnTaker.Deck, toBottom: true),
                new MoveCardDestination(heroTTC.TurnTaker.Deck, toBottom: true),
                new MoveCardDestination(heroTTC.TurnTaker.Deck, toBottom: true),
                new MoveCardDestination(heroTTC.TurnTaker.Deck, toBottom: true)
            };
            IEnumerator coroutine3 = RevealCardsFromDeckToMoveToOrderedDestinations(heroTTC, heroTTC.TurnTaker.Deck, list, numberOfCardsToReveal: 5);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine3);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine3);
            }
            yield break;
        }
        */
    }
}