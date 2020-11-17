using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class EventHorizonCardController : StarlightCardController
    {
        public EventHorizonCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Destroy any number of constellation cards." 
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            IEnumerator coroutine = GameController.SelectAndDestroyCards(HeroTurnTakerController, new LinqCardCriteria((Card c) => IsConstellation(c), "constellation"), null, optional: false, 0, null, storedResults, null, ignoreBattleZone: false, null, null, null, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            int numberOfConstellationsDestroyed = GetNumberOfCardsDestroyed(storedResults);

            //"For each card destroyed this way, destroy 1 ongoing or environment card."
            IEnumerator coroutine2;
            if (numberOfConstellationsDestroyed > 0)
            {
                List<DestroyCardAction> storedResults2 = new List<DestroyCardAction>();
                coroutine2 = GameController.SelectAndDestroyCards(HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsOngoing || c.IsEnvironment, "environment or ongoing"), numberOfConstellationsDestroyed, optional: false, numberOfConstellationsDestroyed, null, storedResults2, null, ignoreBattleZone: false, null, null, null, GetCardSource());
            }
            else
            {
                coroutine2 = GameController.SendMessageAction("No constellations were destroyed, so no ongoing or environment cards will be destroyed.", Priority.High, GetCardSource());
            }

            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine2);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }

    }
}