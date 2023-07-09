using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public class JumperAugCardController : AugCardController
    {
        public JumperAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithLowestHP(numberOfTargets: 2).Condition = () => base.Card.IsInPlayAndNotUnderCard;
        }

        public override void AddTriggers()
        {
            //Reduce damage dealt to this card by non-villain cards by 1.
            base.AddReduceDamageTrigger((DealDamageAction action) => base.Card.IsInPlayAndNotUnderCard && action.Target == base.Card && !action.DamageSource.IsVillain, (DealDamageAction action) => 1);
            //At the end of the villain turn this card deals the 2 hero targets with the lowest HP {H - 2} melee damage each.
            base.AddEndOfTurnTrigger((TurnTaker tt) => base.Card.IsInPlayAndNotUnderCard && tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
        }
        public override void AddAbsorbTriggers(Card absorbingCard)
        {
            //Absorb: reduce damage dealt to {SwarmEater} by 1.
            base.AddReduceDamageTrigger((Card c) => CanAbsorbEffectTrigger() && c == absorbingCard, 1);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...this card deals the 2 hero targets with the lowest HP {H - 2} melee damage each.
            IEnumerator coroutine = base.DealDamageToLowestHP(base.Card, 1, (Card c) => IsHeroTarget(c), (Card c) => Game.H - 2, DamageType.Melee, numberOfTargets: 2);
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