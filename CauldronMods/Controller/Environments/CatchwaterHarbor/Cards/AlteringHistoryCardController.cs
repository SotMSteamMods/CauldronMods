using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.CatchwaterHarbor
{
    public class AlteringHistoryCardController : CatchwaterHarborUtilityCardController
    {
        public AlteringHistoryCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonEnvironmentTargetWithLowestHP(numberOfTargets: 2);
        }

        public override void AddTriggers()
        {
            //Whenever this card or any other environment card is destroyed, this card deals the 2 non-environment targets with the lowest HP 2 psychic damage each.
            AddTrigger<DestroyCardAction>((DestroyCardAction dca) => dca.CardToDestroy != null && dca.CardToDestroy.Card != null && dca.CardToDestroy.Card != Card && dca.CardToDestroy.Card.IsEnvironment && GameController.IsCardVisibleToCardSource(dca.CardToDestroy.Card, GetCardSource()), DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
            AddWhenDestroyedTrigger(DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(DestroyCardAction dca)
        {
            IEnumerator coroutine = DealDamageToLowestHP(Card, 1, (Card c) => c.IsNonEnvironmentTarget, (Card c) => 2, DamageType.Psychic, numberOfTargets: 2);
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
