using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    /*
     * Select a target. Reduce the next damage it deals by 1. Increase the next damage {Gargoyle} deals by 1.
     * Search your deck and trash for a Hunter card and put it into play or into your hand. If you searched your deck, shuffle it.
     */
    public class SomethingToFearCardController : GargoyleUtilityCardController
    {
        private const string HUNTER = "hunter";
        public SomethingToFearCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowListOfCardsAtLocation(base.TurnTaker.Deck, new LinqCardCriteria((lcc) => lcc.DoKeywordsContain(HUNTER), "Hunter"));
            base.SpecialStringMaker.ShowListOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((lcc) => lcc.DoKeywordsContain(HUNTER), "Hunter"));
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            ReduceDamageStatusEffect reduceDamageStatusEffect;
            List<SelectCardDecision> storedResult = new List<SelectCardDecision>();

            // Select a target.
            coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.None, new LinqCardCriteria(c => c.IsTarget && c.IsInPlayAndHasGameText, "Reduce next damage dealt by 1"), storedResult, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResult != null && storedResult.Count() > 0)
            {
                // Reduce the next damage it deals by 1.
                reduceDamageStatusEffect = new ReduceDamageStatusEffect(1);
                reduceDamageStatusEffect.SourceCriteria.IsSpecificCard = storedResult.FirstOrDefault().SelectedCard;
                reduceDamageStatusEffect.NumberOfUses = 1;
                reduceDamageStatusEffect.UntilTargetLeavesPlay(storedResult.FirstOrDefault().SelectedCard);

                coroutine = base.AddStatusEffect(reduceDamageStatusEffect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            // Increase the next damage {Gargoyle} deals by 1.
            coroutine = IncreaseGargoyleNextDamage(1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // Search your deck and trash for a Hunter card and put it into play or into your hand. If you searched your deck, shuffle it.
            coroutine = base.SearchForCards(DecisionMaker, true, true, 1, 1, new LinqCardCriteria((card) => card.DoKeywordsContain(HUNTER), "Hunter"), true, true, false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}
