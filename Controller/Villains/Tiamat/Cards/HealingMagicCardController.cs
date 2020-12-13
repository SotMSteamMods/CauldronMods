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
            base.SpecialStringMaker.ShowVillainTargetWithLowestHP();
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            LinqCardCriteria criteria = new LinqCardCriteria((Card c) => base.CanCardBeConsideredLowestHitPoints(c, (Card card) => card.DoKeywordsContain("head") && card.IsTarget));
            coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.GainHP, criteria, storedResults, false, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card lowestHPHead = storedResults.FirstOrDefault().SelectedCard;

            //The Head with the lowest HP regains {H} + X HP, where X is the number of Healing Magic cards in the villain trash.
            Func<int> X = () => PlusNumberOfThisCardInTrash(base.H);
            coroutine = base.GameController.GainHP(lowestHPHead, PlusNumberOfThisCardInTrash(base.H), X, cardSource: GetCardSource());
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