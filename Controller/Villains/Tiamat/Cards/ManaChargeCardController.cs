using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class ManaChargeCardController : CardController
    {
        public ManaChargeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Reveal cards from the top of the villain deck until 3 Spell cards are revealed. Discard those cards. Shuffle the rest of the revealed cards back into the villain deck.
            IEnumerator coroutine = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, false, false, false, new LinqCardCriteria((Card c) => c.DoKeywordsContain("spell"), "spell"), new int?(3), moveMatchingCardsToTrash: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Shuffle all copies of Elemental Frenzy from the villain trash into the villain deck.
            IEnumerable<Card> frenzies = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.Identifier == "ElementalFrenzy" && base.TurnTaker.Trash.Cards.Contains(c)));
            if (frenzies.Count<Card>() > 0)
            {
                coroutine = base.BulkMoveCard(this.TurnTakerController, frenzies, this.TurnTaker.Deck, false, false, this.TurnTakerController, false);
                IEnumerator coroutine2 = base.GameController.ShuffleLocation(this.TurnTaker.Deck, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }
            yield break;
        }
    }
}