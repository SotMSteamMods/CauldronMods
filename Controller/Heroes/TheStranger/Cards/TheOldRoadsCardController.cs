using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class TheOldRoadsCardController : CardController
    {
        #region Constructors

        public TheOldRoadsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override IEnumerator Play()
        {
            //Put a Glyph from your trash into your hand, or reveal cards from the top of your deck until you reveal a Glyph, put it into play, and shuffle the other revealed cards into your deck.
            string option1 = "Put a Glyph from your trash into your hand";
            string option2 = "reveal cards from the top of your deck until you reveal a Glyph, put it into play, and shuffle the other revealed cards into your deck";
            List<Function> list = new List<Function>();
            list.Add(new Function(this.DecisionMaker, option1, SelectionType.MoveCardToHandFromTrash, () => base.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, base.TurnTaker.Trash, new LinqCardCriteria((Card c) => this.IsGlyph(c), "glyph"), new MoveCardDestination[] { new MoveCardDestination(base.Card.Owner.ToHero().Hand, false, false, false) })));
            list.Add(new Function(this.DecisionMaker, option2, SelectionType.RevealCardsFromDeck, () => base.RevealCards_MoveMatching_ReturnNonMatchingCards(this.DecisionMaker, base.TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => this.IsGlyph(c), "glyph", true, false, null, null, false), new int?(1), null, true, false, RevealedCardDisplay.None, false, false, null, false, false), new bool?(true), null, option2));
            SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, this.DecisionMaker, list, false, null, null, null, base.GetCardSource(null));
            IEnumerator coroutine = base.GameController.SelectAndPerformFunction(selectFunction, null, null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //You may draw a card.
            IEnumerator draw = base.DrawCard(base.HeroTurnTaker, true,null, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(draw);
            }
            else
            {
                base.GameController.ExhaustCoroutine(draw);
            }
            yield break;
        }
        private bool IsGlyph(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "glyph", false, false);
        }

        #endregion Methods
    }
}