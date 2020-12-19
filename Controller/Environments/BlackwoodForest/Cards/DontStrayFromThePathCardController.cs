using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.BlackwoodForest
{
    public class DontStrayFromThePathCardController : CardController
    {
        //==============================================================
        // This card is indestructible.
        // At the start of the environment turn, if there is at least 1 other environment card in play,
        // reveal the top card of the environment deck.
        // If the Hound is revealed, put it into play, otherwise discard that card.
        //==============================================================

        public static readonly string Identifier = "DontStrayFromThePath";

        private const int CardsToReveal = 1;

        public DontStrayFromThePathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            // Make this card indestructible
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override void AddTriggers()
        {
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, StartOfTurnResponse, TriggerType.RevealCard);

            base.AddTriggers();
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == base.Card;
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            var criteria = new LinqCardCriteria(card => this.Card != card && card.IsInPlayAndHasGameText && card.IsEnvironment && GameController.IsCardVisibleToCardSource(card, GetCardSource()));
            IEnumerable<Card> cardResults = FindCardsWhere(criteria);
            if (cardResults.Any())
            {
                // At least one other env. card is in play that isn't this card, proceed
                List<Card> revealedCards = new List<Card>();
                var coroutine = GameController.RevealCards(TurnTakerController, FindEnvironment().TurnTaker.Deck, CardsToReveal, revealedCards, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                //deck could be empty
                if (revealedCards.Any())
                {
                    var revealedCard = revealedCards.First();
                    if (IsHound(revealedCard))
                    {
                        // The Hound revealed, put into play
                        coroutine = base.GameController.PlayCard(TurnTakerController, revealedCard, true);
                    }
                    else
                    {
                        // Discard
                        coroutine = base.GameController.MoveCard(TurnTakerController, revealedCard, FindEnvironment().TurnTaker.Trash);
                    }

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
        }

        private bool IsHound(Card card)
        {
            return card != null && card.Identifier.Equals(TheHoundCardController.Identifier);
        }
    }
}