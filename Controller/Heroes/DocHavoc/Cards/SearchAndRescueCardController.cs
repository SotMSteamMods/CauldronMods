using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class SearchAndRescueCardController : CardController
    {

        //==============================================================
        // Each player may discard a card.
        // Any player that does may reveal the top 3 cards of their deck, put 1 in their hand, 1 on the bottom of their deck, and 1 in their trash.
        //==============================================================

        public static string Identifier = "SearchAndRescue";

        public SearchAndRescueCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator discardCardRoutine = this.GameController.EachPlayerDiscardsCards(0, 1, storedResults, cardSource: GetCardSource());

            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(discardCardRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(discardCardRoutine);
            }

            foreach (DiscardCardAction dca in storedResults.Where(dca => dca.WasCardDiscarded))
            {
                Debug.WriteLine("DEBUG: card was discarded");

                List<YesNoCardDecision> storedYesNoResults = new List<YesNoCardDecision>();
                HeroTurnTaker owner = dca.CardToDiscard.Owner.ToHero();
                HeroTurnTakerController httc = FindHeroTurnTakerController(owner);
                //ask player if they want to reveal cards
                IEnumerator coroutine = base.GameController.MakeYesNoCardDecision(httc,
                    SelectionType.RevealCardsFromDeck, base.Card, storedResults: storedYesNoResults, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (base.DidPlayerAnswerYes(storedYesNoResults))
                {
                    //reveal the top 3 cards of their deck, put 1 in their hand, 1 on the bottom of their deck, and 1 in their trash.
                    IEnumerator orderedDestinationsRoutine = this.CustomizedRevealCardsFromDeckToMoveToOrderedDestinations(httc, owner.Deck, new[]
                        {
                            new MoveCardDestination(owner.Hand),
                            new MoveCardDestination(owner.Deck, true),
                            new MoveCardDestination(owner.Trash)
                        },
                        sendCleanupMessageIfNecessary: true);

                    if (this.UseUnityCoroutines)
                    {
                        yield return this.GameController.StartCoroutine(orderedDestinationsRoutine);
                    }
                    else
                    {
                        this.GameController.ExhaustCoroutine(orderedDestinationsRoutine);
                    }
                }
            }
            yield break;
        }

        private IEnumerator CustomizedRevealCardsFromDeckToMoveToOrderedDestinations(HeroTurnTakerController revealingTurnTaker, Location deck, IEnumerable<MoveCardDestination> orderedDestinations, bool fromBottom = false, bool sendCleanupMessageIfNecessary = false, int? numberOfCardsToReveal = null, bool isPutIntoPlay = false)
        {
            List<Card> revealedCards = new List<Card>();
            int numberOfCards = numberOfCardsToReveal ?? orderedDestinations.Count();
            IEnumerator coroutine = GameController.RevealCards(revealingTurnTaker, deck, numberOfCards, revealedCards, fromBottom, RevealedCardDisplay.None, null, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            List<Card> allRevealedCards = new List<Card>(revealedCards);
            foreach (MoveCardDestination orderedDestination in orderedDestinations)
            {
                if (revealedCards.Count > 1)
                {
                    List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                    coroutine = GameController.SelectCardFromLocationAndMoveIt(revealingTurnTaker, deck.OwnerTurnTaker.Revealed,
                        new LinqCardCriteria((Card c) => revealedCards.Contains(c), ignoreBattleZone: true),
                        new[] { orderedDestination }, isPutIntoPlay: isPutIntoPlay,
                        storedResults: storedResults,
                        responsibleTurnTaker: HeroTurnTaker,
                        cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                    Card selectedCard = GetSelectedCard(storedResults);
                    revealedCards.Remove(selectedCard);
                }
                else if (revealedCards.Count == 1)
                {
                    bool showMessage = !orderedDestination.Location.IsInPlayAndNotUnderCard;
                    Card card = revealedCards.First();
                    coroutine = GameController.MoveCard(TurnTakerController, card, orderedDestination.Location, orderedDestination.ToBottom, isPutIntoPlay,
                        showMessage: showMessage,
                        responsibleTurnTaker: HeroTurnTaker,
                        cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                    revealedCards.Remove(card);
                }
                if (revealedCards.Count == 0)
                {
                    break;
                }
            }
            List<Location> list = new List<Location>()
            {
                deck.OwnerTurnTaker.Revealed
            };
            coroutine = CleanupCardsAtLocations(list, deck, sendMessage: sendCleanupMessageIfNecessary, cardsInList: allRevealedCards);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            if (revealedCards.Count > 0)
            {
                Handelabra.Log.Error(Card.Title + " still has cards left to work with, but should be empty!");
            }
        }
    }
}
