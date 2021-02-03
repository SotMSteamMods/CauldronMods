﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.Quicksilver
{
    public class LiquidMetalCardController : QuicksilverBaseCardController
    {
        public LiquidMetalCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //Reveal cards from the top of your deck until you reveal a [Combo or a Finisher]...
            IEnumerator coroutine = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, false, false, true, new LinqCardCriteria((Card c) => c.DoKeywordsContain(new string[] { ComboKeyword, FinisherKeyword })), 1, shuffleSourceAfterwards: false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var cardToHandEntry = Journal.MoveCardEntriesThisTurn().Where((MoveCardJournalEntry mcje) => mcje.CardSource == this.Card && mcje.ToLocation == HeroTurnTaker.Hand).LastOrDefault();
            Card foundCard = null;
            if (cardToHandEntry != null)
            {
                foundCard = cardToHandEntry.Card;
            }

            List<string> missingKeywords = new List<string> { };
            if (foundCard == null || !foundCard.DoKeywordsContain(FinisherKeyword))
            {
                missingKeywords.Add(FinisherKeyword);
            }
            if (foundCard == null || !foundCard.DoKeywordsContain(ComboKeyword))
            {
                missingKeywords.Add(ComboKeyword);
            }

            //...and [the kind you didn't find first] and put them into your hand. Shuffle the other revealed cards back into your deck.
            coroutine = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, false, false, true, new LinqCardCriteria((Card c) => c.DoKeywordsContain(missingKeywords)), 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
            var fake = new DealDamageAction(GetCardSource(), new DamageSource(GameController, CharacterCard), CharacterCard, 2, DamageType.Melee);
            coroutine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController, SelectionType.DealDamageSelf, base.Card, action: fake, storedResults: storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (base.DidPlayerAnswerYes(storedResults))
            {
                coroutine = base.ContinueWithComboResponse();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}