using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Gray
{
    public class MutatedWildlifeCardController : CardController
    {
        public MutatedWildlifeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            //Increase damage dealt by environment cards by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource.IsEnvironmentCard, 1);
            //Whenever a villain target would be dealt damage by an environment card, redirect that damage to the hero target with the highest HP.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DamageSource.IsEnvironmentSource && IsVillainTarget(action.Target), new Func<DealDamageAction, IEnumerator>(this.RedirectResponse), TriggerType.RedirectDamage, TriggerTiming.Before);
            //At the end of the villain turn, play the top card of the environment deck.
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, base.PlayTheTopCardOfTheEnvironmentDeckResponse, TriggerType.PlayCard);
        }

        private IEnumerator RedirectResponse(DealDamageAction action)
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card c) => IsHero(c), storedResults, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card card = storedResults.FirstOrDefault<Card>();
            if (action.IsRedirectable && card != null)
            {
                coroutine = base.GameController.RedirectDamage(action, card, cardSource: base.GetCardSource());
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