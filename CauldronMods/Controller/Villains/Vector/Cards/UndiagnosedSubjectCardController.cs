﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class UndiagnosedSubjectCardController : VectorBaseCardController
    {
        //==============================================================
        // At the end of the villain turn, play the top card of the villain deck.
        // When this card is destroyed, reveal the top card of the villain deck.
        // If a Virus card is revealed you may put it beneath Supervirus,
        // otherwise discard or replace it.
        //==============================================================

        public static readonly string Identifier = "UndiagnosedSubject";

        private const int CardsToReveal = 1;

        public UndiagnosedSubjectCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker,
                PlayTheTopCardOfTheVillainDeckWithMessageResponse, TriggerType.PlayCard);

            AddWhenDestroyedTrigger(DestroyedResponse, new[]
            {
                TriggerType.RevealCard, TriggerType.MoveCard
            });

            base.AddTriggers();
        }

        private IEnumerator DestroyedResponse(DestroyCardAction destroy)
        {
            List<Card> revealedCards = new List<Card>();
            IEnumerator revealCardsRoutine = base.GameController.RevealCards(this.TurnTakerController, base.TurnTaker.Deck,
                CardsToReveal, revealedCards, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(revealCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(revealCardsRoutine);
            }

            if (!revealedCards.Any())
            {
                yield break;
            }

            IEnumerable<Function> choices = new[]
            {
                new Function(base.DecisionMaker, "Discard Card", SelectionType.DiscardCard,
                    () => base.GameController.MoveCard(base.DecisionMaker, revealedCards.First(), base.TurnTaker.Trash, showMessage: true, cardSource: GetCardSource())),

                new Function(base.DecisionMaker, "Replace Card", SelectionType.MoveCardOnDeck,
                    () => base.GameController.MoveCard(base.DecisionMaker, revealedCards.First(), base.TurnTaker.Deck, showMessage: true, cardSource: GetCardSource()))
            };

            List<MoveCardAction> virusMoveAction = new List<MoveCardAction>();
            if (IsSuperVirusInPlay() && IsVirus(revealedCards.First()))
            {
                choices = choices.Concat(new[]
                {
                    new Function(base.DecisionMaker, "Put under Super Virus", SelectionType.MoveCardToUnderCard,
                        () => base.GameController.MoveCard(base.DecisionMaker, revealedCards.First(),
                            GetSuperVirusCard().UnderLocation, storedResults: virusMoveAction, showMessage: true, cardSource: GetCardSource())),
                });
            }

            SelectFunctionDecision selectFunctionDecision 
                = new SelectFunctionDecision(base.GameController, this.DecisionMaker, choices, false, associatedCards: revealedCards, cardSource: base.GetCardSource());

            IEnumerator routine = base.GameController.SelectAndPerformFunction(selectFunctionDecision);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            yield break;
        }
    }
}