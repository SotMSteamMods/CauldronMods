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

        public static string Identifier = "DontStrayFromThePath";

        private const int CardsToReveal = 1;

        public DontStrayFromThePathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            // Make this card indestructible
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override void AddTriggers()
        {
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker,
                new Func<PhaseChangeAction, IEnumerator>(StartOfTurnResponse),
                TriggerType.RevealCard, null, false);

            base.AddTriggers();
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == base.Card;
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            LinqCardCriteria envCardsInPlay =
                new LinqCardCriteria(card => !card.Equals(this.Card) && card.IsInPlayAndHasGameText && card.IsEnvironment);

            IEnumerable<Card> cardResults = FindCardsWhere(envCardsInPlay);

            if (!cardResults.Any())
            {
                yield break;
            }

            // At least one other env. card is in play that isn't this card, proceed
            List<Card> revealedCards = new List<Card>();
            IEnumerator revealCardsRoutine = base.GameController.RevealCards(this.TurnTakerController, base.FindEnvironment(null).TurnTaker.Deck, 
                CardsToReveal, revealedCards, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(revealCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(revealCardsRoutine);
            }

            IEnumerator cardActionRoutine;
            if (revealedCards.Any() && IsHound(revealedCards.First()))
            {
                // The Hound revealed, put into play
                cardActionRoutine = base.GameController.PlayCard(this.TurnTakerController, revealedCards.First(), true);
            }
            else
            {
                // Discard
                cardActionRoutine = base.GameController.MoveCard(this.TurnTakerController, revealedCards.First(),
                    FindEnvironment().TurnTaker.Trash);
            }

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(cardActionRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(cardActionRoutine);
            }
        }

        private bool IsHound(Card card)
        {
            return card != null 
                && card.Identifier.Equals("TheHound") 
                && base.GameController.DoesCardContainKeyword(card, "grim", false, false);
        }
    }
}