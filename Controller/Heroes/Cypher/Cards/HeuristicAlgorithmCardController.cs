using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class HeuristicAlgorithmCardController : CypherBaseCardController
    {
        //==============================================================
        // Reveal cards from the top of your deck until you reveal an Augment.
        // Put it into play or into your trash. Shuffle the rest of the revealed cards into your deck.
        // If you did not put an Augment into play this way, draw 2 cards.
        //==============================================================

        public static string Identifier = "HeuristicAlgorithm";

        private const int CardsToDraw = 2;

        public HeuristicAlgorithmCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Reveal cards from the top of your deck until you reveal an Augment.
            List<RevealCardsAction> revealedCardActions = new List<RevealCardsAction>();
            IEnumerator routine = base.GameController.RevealCards(base.HeroTurnTakerController, base.TurnTaker.Deck, IsAugment, 1, 
                revealedCardActions, RevealedCardDisplay.ShowRevealedCards, this.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            List<Card> augmentCards = GetRevealedCards(revealedCardActions).Where(IsAugment).Take(1).ToList();
            List<Card> otherCards = GetRevealedCards(revealedCardActions).Where(c => !augmentCards.Contains(c)).ToList();
            List<PlayCardAction> playCardActions = new List<PlayCardAction>();
            
            if (augmentCards.Any())
            {

                IEnumerable<Function> functionChoices = new[]
                {
                    // Put it into play...
                    new Function(base.HeroTurnTakerController, "Put into play", SelectionType.PlayCard, 
                        () => base.GameController.PlayCard(base.HeroTurnTakerController, augmentCards.First(), true, storedResults: playCardActions, cardSource: GetCardSource())),

                    //...or into your trash
                    new Function(base.HeroTurnTakerController, "Put in trash", SelectionType.MoveCardToTrash, () => base.GameController.MoveCard(this.HeroTurnTakerController, augmentCards.First(), base.HeroTurnTaker.Trash, cardSource: GetCardSource()))
                };

                SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, base.HeroTurnTakerController, functionChoices, false);
                
                routine = base.GameController.SelectAndPerformFunction(selectFunction);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }
            }

            if (otherCards.Any())
            {
                // Shuffle the rest of the revealed cards into your deck.
                routine = base.GameController.MoveCards(this.DecisionMaker, otherCards, this.TurnTaker.Deck, cardSource: base.GetCardSource());
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(routine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(routine);
                }

                routine = this.ShuffleDeck(this.DecisionMaker, this.TurnTaker.Deck);
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(routine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(routine);
                }
            }

            if (augmentCards.Any() && playCardActions.Any())
            {
                yield break;
            }


            // If you did not put an Augment into play this way, draw 2 cards.
            routine = base.DrawCards(this.HeroTurnTakerController, CardsToDraw);
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(routine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(routine);
            }
        }
    }
}