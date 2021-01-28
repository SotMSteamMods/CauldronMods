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

        }

        public override IEnumerator Play()
        {
            //{Dynamo} deals the hero target with the highest HP 5 energy damage.
            List<DealDamageAction> dealDamageActions = new List<DealDamageAction>();
            IEnumerator coroutine = base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => c.IsHero && c.IsTarget, (Card c) => 5, DamageType.Energy, storedResults: dealDamageActions);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (dealDamageActions.Any())
            {
                DealDamageAction damageAction = dealDamageActions.FirstOrDefault();
                //If a hero target takes damage this way...
                if (damageAction.Target.IsHero && damageAction.Amount > 0)
                {
                    //...destroy 1 environment card.
                    coroutine = base.GameController.SelectAndDestroyCard(base.DecisionMaker, new LinqCardCriteria((Card c) => c.IsEnvironment), false);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }

                //{Dynamo} deals each other hero target 1 sonic damage.
                coroutine = base.DealDamage(base.CharacterCard, (Card c) => c.IsHero && c.IsTarget && c != damageAction.Target, (Card c) => 1, DamageType.Sonic);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                //If somehow no targets were hit, go after all hero targets
                coroutine = base.DealDamage(base.CharacterCard, (Card c) => c.IsHero && c.IsTarget, (Card c) => 1, DamageType.Sonic);
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
