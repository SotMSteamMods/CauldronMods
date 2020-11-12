using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class HcBSwarmingAgentCardController : ChemicalTriggerCardController
    {
        #region Constructors

        public HcBSwarmingAgentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //This card is indestructible if at least 1 Test Subject is in play. At the start of the environment turn, destroy this card.
            //In ChemicalTriggerCardController
            base.AddTriggers();
        }

        public override IEnumerator Play()
        {

            //When this card enters play, reveal cards from the top of the environment deck until 2 Test Subjects have been revealed. Put them into play and shuffle the remaining cards into the deck.
            // Reveal cards from the top of your deck until you reveal 2 Test Subjects cards. 
            List<RevealCardsAction> revealedCardActions = new List<RevealCardsAction>();
            IEnumerator coroutine = base.GameController.RevealCards(base.TurnTakerController, base.TurnTaker.Deck, (Card c) => base.IsTestSubject(c), 2, revealedCardActions, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<Card> testSubjectCards = GetRevealedCards(revealedCardActions).Where(c => base.IsTestSubject(c)).Take(2).ToList();
            List<Card> otherCards = GetRevealedCards(revealedCardActions).Where(c => !testSubjectCards.Contains(c)).ToList();
            if (testSubjectCards.Any())
            {
                //Put both into play 
                coroutine = base.GameController.PlayCards(this.DecisionMaker, (Card c) => testSubjectCards.Contains(c), false, true, new int?(2), cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            if (otherCards.Any())
            {
                //Put the remaining revealed cards back on the deck and shuffle
                coroutine = base.GameController.MoveCards(this.DecisionMaker, otherCards, this.TurnTaker.Deck, cardSource: base.GetCardSource());
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = this.ShuffleDeck(this.DecisionMaker, this.TurnTaker.Deck);
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }
        #endregion Methods
    }
}