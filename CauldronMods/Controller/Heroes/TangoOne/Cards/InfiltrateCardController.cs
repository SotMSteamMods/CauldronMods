using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class InfiltrateCardController : TangoOneBaseCardController
    {
        //==============================================================
        // Reveal the top 2 cards of any deck, then replace them in any order.
        // You may draw a card. You may play a card.
        //==============================================================

        public static readonly string Identifier = "Infiltrate";

        private const int CardsToReveal = 2;

        public InfiltrateCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // Select deck
            List<SelectLocationDecision> locationResults = new List<SelectLocationDecision>();
            IEnumerator selectDeckRoutine = base.GameController.SelectADeck(this.DecisionMaker, SelectionType.RevealCardsFromDeck,
                location => location.IsDeck, locationResults);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectDeckRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectDeckRoutine);
            }

            // Reveal the top 2 cards, then replace them in any order
            List<Card> revealedCards = new List<Card>();
            Location deck = base.GetSelectedLocation(locationResults);

            if (deck != null)
            {
                IEnumerator routine = RevealCardsFromTopOfDeck_PutOnTopInChoosenOrder(DecisionMaker, deck);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }
            }

            // You may draw a card, You may play a card
            IEnumerator drawCardRoutine = base.DrawCard(base.HeroTurnTaker, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCardRoutine);
            }

            IEnumerator playCardRoutine = this.SelectAndPlayCardFromHand(this.HeroTurnTakerController);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCardRoutine);
            }
        }

        protected IEnumerator RevealCardsFromTopOfDeck_PutOnTopInChoosenOrder(HeroTurnTakerController hero, Location deck)
        {
            List<Card> revealedCards = new List<Card>();
            IEnumerator coroutine = this.GameController.RevealCards(hero, deck, CardsToReveal, revealedCards, false, RevealedCardDisplay.None, null, this.GetCardSource(null));
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }

            List<Card> allRevealedCards = new List<Card>(revealedCards);
            if (revealedCards.Count == 2)
            {
                List<SelectCardsDecision> selectCardDecisions = new List<SelectCardsDecision>();
                coroutine = GameController.SelectCardsAndStoreResults(hero, SelectionType.MoveCardOnDeck, (Card c) => revealedCards.Contains(c), 1, selectCardDecisions, false,
                    cardSource: GetCardSource(),
                    ignoreBattleZone: true);
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine);
                }
                SelectCardsDecision selectCardsDecision = selectCardDecisions.FirstOrDefault();
                SelectCardDecision selectCardDecision = selectCardsDecision?.SelectCardDecisions.FirstOrDefault();
                Card selectedCard = selectCardDecision?.SelectedCard;
                Card otherCard = revealedCards.First(c => c != selectedCard);

                if (selectedCard != null)
                {
                    coroutine = this.GameController.MoveCard(this.TurnTakerController, otherCard, deck, cardSource: GetCardSource());
                    if (this.UseUnityCoroutines)
                    {
                        yield return this.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        this.GameController.ExhaustCoroutine(coroutine);
                    }
                    coroutine = this.GameController.MoveCard(this.TurnTakerController, selectedCard, deck, decisionSources: new[] { selectCardDecision }, cardSource: GetCardSource());
                    if (this.UseUnityCoroutines)
                    {
                        yield return this.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        this.GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
            else if (revealedCards.Count == 1)
            {
                Card card = revealedCards[0];
                string message = $"{card.Title} is the only card in {deck.GetFriendlyName()}.";
                coroutine = this.GameController.SendMessageAction(message, Priority.High, GetCardSource(), new Card[] { card });
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = this.GameController.MoveCard(this.TurnTakerController, card, deck, cardSource: GetCardSource());
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine);
                }
            }
            coroutine = this.CleanupCardsAtLocations(new List<Location>() { deck.OwnerTurnTaker.Revealed }, deck, cardsInList: allRevealedCards);
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}