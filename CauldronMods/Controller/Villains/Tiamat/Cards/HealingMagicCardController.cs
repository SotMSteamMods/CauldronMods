using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class HealingMagicCardController : SpellCardController
    {
        public HealingMagicCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowLowestHP(ranking: 1, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("head"), "head", false));

            base.SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => c.Identifier == "HealingMagic", "healing magic"));

        }

        public override IEnumerator Play()
        {
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            LinqCardCriteria criteria = new LinqCardCriteria((Card c) => base.CanCardBeConsideredLowestHitPoints(c, (Card card) => card.DoKeywordsContain("head") && card.IsTarget));
            IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.GainHP, criteria, storedResults, false, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(DidSelectCard(storedResults))
            {
                //The Head with the lowest HP regains {H} + X HP, where X is the number of Healing Magic cards in the villain trash.
                Card lowestHPHead = GetSelectedCard(storedResults);
                Func<int> X = () => PlusNumberOfThisCardInTrash(base.H);
                coroutine = base.GameController.GainHP(lowestHPHead, PlusNumberOfThisCardInTrash(base.H), X, cardSource: GetCardSource());
            } else
            {
                coroutine = base.GameController.SendMessageAction("There are no active heads to regain HP.", Priority.Medium, base.GetCardSource(), showCardSource: true);
            }

            //Play the top card of the villain deck.
            IEnumerator coroutine2 = base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, cardSource: base.GetCardSource());
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
            yield break;
        }
    }
}
