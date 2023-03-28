using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.SwarmEater
{
    public class VenomAugCardController : AugCardController
    {
        public VenomAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => base.Card.IsInPlayAndNotUnderCard;
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn this card deals the hero target with the highest HP {H} projectile damage. Any target dealt damage this way deals itself 1 toxic damage.
            base.AddEndOfTurnTrigger(tt => base.Card.IsInPlayAndNotUnderCard && tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
        }
        public override void AddAbsorbTriggers(Card absorbingCard)
        {
            //Absorb: whenever {SwarmEater} deals damage to another target, that target deals itself 1 toxic damage.
            base.AddTrigger((DealDamageAction action) => CanAbsorbEffectTrigger() && action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.Card == absorbingCard && action.Target != action.DamageSource.Card && action.DidDealDamage, dda => this.AbsorbDealDamageResponse(dda, absorbingCard), TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction phaseChange)
        {
            //...this card deals the hero target with the highest HP {H} projectile damage.
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, c => IsHero(c), c => Game.H, DamageType.Projectile, storedResults: storedResults, numberOfTargets: () => 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (!storedResults.Any())
            {
                yield break;
            }
            var damage = storedResults.FirstOrDefault();
            if (damage != null && damage.DidDealDamage)
            {
                Card target = damage.Target;
                //...Any target dealt damage this way deals itself 1 toxic damage.
                coroutine = base.DealDamage(target, target, 1, DamageType.Toxic, cardSource: base.GetCardSource());
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

        public IEnumerator AbsorbDealDamageResponse(DealDamageAction action, Card absorbingCard)
        {
            //...that target deals itself 1 toxic damage.
            IEnumerator coroutine = base.DealDamage(action.Target, action.Target, 1, DamageType.Toxic, cardSource: base.GetCardSource());
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