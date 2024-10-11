using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class TakeItOutsideCardController : DynamoUtilityCardController
    {
        public TakeItOutsideCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override IEnumerator Play()
        {
            //{Dynamo} deals the hero target with the highest HP 5 energy damage.
            List<Card> storeHighest = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card c) => IsHeroTarget(c), storeHighest, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<DealDamageAction> dealDamageActions = new List<DealDamageAction>();
            coroutine = base.DealDamage(base.CharacterCard, storeHighest.FirstOrDefault(), 5, DamageType.Energy, storedResults: dealDamageActions, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDealDamage(dealDamageActions))
            {
                DealDamageAction damageAction = dealDamageActions.FirstOrDefault();
                //If a hero target takes damage this way...
                if (IsHeroTarget(damageAction.Target) && damageAction.Amount > 0)
                {
                    //...destroy 1 environment card.
                    coroutine = base.GameController.SelectAndDestroyCard(base.DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment), false, cardSource: GetCardSource());
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

            //{Dynamo} deals each other hero target 1 sonic damage.
            coroutine = base.DealDamage(base.CharacterCard, (Card c) => IsHeroTarget(c) && !storeHighest.Contains(c), (Card c) => 1, DamageType.Sonic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);

                yield break;
            }
        }
    }
}
