using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.Quicksilver
{
    public class LiquidMetalCardController : CardController
    {
        public LiquidMetalCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //Reveal cards from the top of your deck until you reveal a [Combo or a Finisher]...
            IEnumerator coroutine = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, false, false, true, new LinqCardCriteria((Card c) => c.DoKeywordsContain(new string[] { "combo", "finisher" })), 1, shuffleSourceAfterwards: false);
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
            if (foundCard == null || !foundCard.DoKeywordsContain("finisher"))
            {
                missingKeywords.Add("finisher");
            }
            if (foundCard == null || !foundCard.DoKeywordsContain("combo"))
            {
                missingKeywords.Add("combo");
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

            base.CharacterCardController.SetCardPropertyToTrueIfRealAction("ComboSelfDamage");
            //...{Quicksilver} may deal herself 2 melee damage...
            coroutine = base.DealDamage(base.CharacterCard, base.CharacterCard, 2, DamageType.Melee, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            base.CharacterCardController.SetCardProperty("ComboSelfDamage", false);
            //...play a Combo.
            coroutine = base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, false, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("combo")), cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}