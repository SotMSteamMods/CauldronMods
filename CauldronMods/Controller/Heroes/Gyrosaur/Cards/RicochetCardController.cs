using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class RicochetCardController : GyrosaurUtilityCardController
    {
        public RicochetCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"{Gyrosaur} deals 1 target 2 melee damage. ,
            //"Reduce the next damage dealt by non-hero targets damaged this way to 0."
            var storedDamage = new List<DealDamageAction>();
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 2, DamageType.Melee, 1, false, 1, storedResultsDamage: storedDamage, addStatusEffect: AddReduceNextDamageEffect, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var firstDamage = storedDamage.FirstOrDefault();
            if(firstDamage == null)
            {
                yield break;
            }

            //{Gyrosaur} deals a second target X melee damage, where X is the amount of damage she dealt to the first target."
            var secondDamageAmount = firstDamage.DidDealDamage ? firstDamage.Amount : 0;
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), secondDamageAmount, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => c != firstDamage.Target, addStatusEffect: AddReduceNextDamageEffect, cardSource: GetCardSource());
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

        private IEnumerator AddReduceNextDamageEffect(DealDamageAction dd)
        {
            //"Reduce the next damage dealt by non-hero targets damaged this way to 0."
            if (dd.DidDealDamage && !IsHero(dd.Target))
            {
                var reduceEffect = new OnDealDamageStatusEffect(CardWithoutReplacements, nameof(ReduceDamageToZero), $"Reduce the next damage dealt by {dd.Target.Title} to zero", new TriggerType[] { TriggerType.ReduceDamageOneUse }, DecisionMaker.TurnTaker, this.Card);
                reduceEffect.NumberOfUses = 1;
                reduceEffect.SourceCriteria.IsSpecificCard = dd.Target;
                reduceEffect.CardSource = Card;
                reduceEffect.DamageAmountCriteria.GreaterThan = 0;
                reduceEffect.UntilTargetLeavesPlay(dd.Target);

                IEnumerator coroutine = AddStatusEffect(reduceEffect);
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

        public IEnumerator ReduceDamageToZero(DealDamageAction dd, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            IEnumerator coroutine = GameController.ReduceDamage(dd, dd.Amount, null, GetCardSource());
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
