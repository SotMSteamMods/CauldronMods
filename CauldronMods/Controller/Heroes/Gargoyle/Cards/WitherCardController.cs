using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    public class WitherCardController : GargoyleUtilityCardController
    {
        /*
         * Destroy 1 ongoing or environment card.
         * If a card is destroyed this way, increase the next damage dealt by {Gargoyle} by 2.
         */
        public WitherCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator coroutine;

            // Destroy 1 ongoing or environment card.
            coroutine = base.GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((card) => (card.IsEnvironment || IsOngoing(card)) && card.IsInPlay && GameController.IsCardVisibleToCardSource(card, GetCardSource()), "ongoing or environment"), 1, false, 1, storedResultsAction: storedResults, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // If a card is destroyed this way, increase the next damage dealt by {Gargoyle} by 2.
            if (base.DidDestroyCard(storedResults))
            {
                coroutine = base.IncreaseGargoyleNextDamage(2);
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
