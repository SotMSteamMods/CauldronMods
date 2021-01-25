﻿using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class TheOldRoadsCardController : TheStrangerBaseCardController
    {
        public TheOldRoadsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, IsGlyphCriteria());
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, IsGlyphCriteria());
        }

        public override IEnumerator Play()
        {
            //Put a Glyph from your trash into your hand, or reveal cards from the top of your deck until you reveal a Glyph, put it into play, and shuffle the other revealed cards into your deck.
            string option1 = "Put a Glyph from your trash into your hand";
            string option2 = "Reveal cards from the top of your deck until you reveal a Glyph, put it into play, and shuffle the other revealed cards into your deck";
            List<Function> list = new List<Function>();
            list.Add(new Function(this.DecisionMaker, option1, SelectionType.MoveCardToHandFromTrash, () => base.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, base.TurnTaker.Trash, IsGlyphCriteria(), new MoveCardDestination[] { new MoveCardDestination(HeroTurnTaker.Hand, false, false, false) }), null, null, option1));
            list.Add(new Function(this.DecisionMaker, option2, SelectionType.RevealCardsFromDeck, () => base.RevealCards_MoveMatching_ReturnNonMatchingCards(this.DecisionMaker, base.TurnTaker.Deck, false, true, false, IsGlyphCriteria(), new int?(1), null, true, false, RevealedCardDisplay.None, false, false, null, false, false), new bool?(true), null, option2));
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
            IEnumerator draw = base.DrawCard(base.HeroTurnTaker, true, null, true);
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
    }
}
