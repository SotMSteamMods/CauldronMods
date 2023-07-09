using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public class BladeAugCardController : AugCardController
    {
        public BladeAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => base.Card.IsInPlayAndNotUnderCard;
            base.SpecialStringMaker.ShowHighestHP(cardCriteria: new LinqCardCriteria((Card c) => c != this.CardThatAbsorbedThis())).Condition = () => CanAbsorbEffectTrigger();
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, this card deals the hero target with the highest HP 2 lightning damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => base.Card.IsInPlayAndNotUnderCard && tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);

        }
        public override void AddAbsorbTriggers(Card absorbingCard)
        {
            //Absorb: at the end of the villain turn, {SwarmEater} deals the target other than itself with the highest HP 2 lightning damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => CanAbsorbEffectTrigger() && tt == base.TurnTaker, pca => this.AbsorbDealDamageResponse(pca, absorbingCard), TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...this card deals the hero target with the highest HP 2 lightning damage.
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => IsHero(c), (Card c) => new int?(2), DamageType.Lightning);
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
        private IEnumerator AbsorbDealDamageResponse(PhaseChangeAction action, Card absorbingCard)
        {
            //...{SwarmEater} deals the target other than itself with the highest HP 2 lightning damage.
            IEnumerator coroutine = base.DealDamageToHighestHP(absorbingCard, 1, (Card c) => c != absorbingCard, (Card c) => new int?(2), DamageType.Lightning);
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