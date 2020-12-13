using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Echelon
{
    public class CommandAndConquerCardController : EchelonBaseCardController
    {
        //==============================================================
        // Reveal cards from the top of your deck until 2 Tactics are revealed.
        // Shuffle the other revealed cards into your deck.
        // Put 1 revealed Tactic into play and the other on the top or bottom of your deck.
        // {Echelon} deals 1 target 2 lightning or 2 melee damage.
        //==============================================================

        public static string Identifier = "CommandAndConquer";

        public CommandAndConquerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {

            // Reveal cards from the top of your deck until 2 Tactics are revealed.
            List<RevealCardsAction> revealedCardActions = new List<RevealCardsAction>();
            IEnumerator routine = base.GameController.RevealCards(base.HeroTurnTakerController, base.TurnTaker.Deck, IsTactic, 2, 
                revealedCardActions, RevealedCardDisplay.ShowMatchingCards, this.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            List<Card> tacticCards = GetRevealedCards(revealedCardActions).Where(IsTactic).Take(2).ToList();
            List<Card> otherCards = GetRevealedCards(revealedCardActions).Where(c => !tacticCards.Contains(c)).ToList();
            List<PlayCardAction> playCardActions = new List<PlayCardAction>();
            
            if (tacticCards.Any())
            {
                Card tactic = tacticCards.FirstOrDefault();
                IEnumerable<Function> functionChoices = new[]
                {
                    // Put it into play...
                    new Function(base.HeroTurnTakerController, $"Put {tactic.Title} into play", SelectionType.PlayCard, 
                        () => base.GameController.PlayCard(base.HeroTurnTakerController, tacticCards.First(), true, storedResults: playCardActions, cardSource: GetCardSource())),

                    //...or into your trash
                    new Function(base.HeroTurnTakerController, $"Put {tactic.Title} in trash", SelectionType.MoveCardToTrash, () => base.GameController.MoveCard(this.HeroTurnTakerController, tacticCards.First(), base.HeroTurnTaker.Trash, cardSource: GetCardSource()))
                };

                SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, base.HeroTurnTakerController, functionChoices, false);
                
                routine = base.GameController.SelectAndPerformFunction(selectFunction, associatedCards: tacticCards);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }
            }
            else
            {
                routine = GameController.SendMessageAction($"There were no Tactics in {TurnTaker.Deck.GetFriendlyName()}", Priority.Medium, GetCardSource());
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

            // {Echelon} deals 1 target 2 lightning or 2 melee damage.


        }
    }
}