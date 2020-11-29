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
            if (base.Card.IsInPlayAndNotUnderCard)
            {
                base.SpecialStringMaker.ShowHeroTargetWithHighestHP();
            }
            else
            {
                base.SpecialStringMaker.ShowHighestHP(cardCriteria: new LinqCardCriteria((Card c) => c != this.CardThatAbsorbedThis()));
            }
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, this card deals the hero target with the highest HP 2 lightning damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, new TriggerType[] { TriggerType.DealDamage }, (PhaseChangeAction action) => base.Card.IsInPlayAndNotUnderCard);

            //Absorb: at the end of the villain turn, {SwarmEater} deals the target other than itself with the highest HP 2 lightning damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.AbsorbDealDamageResponse, new TriggerType[] { TriggerType.DealDamage }, (PhaseChangeAction action) => base.Card.Location.IsUnderCard);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...this card deals the hero target with the highest HP 2 lightning damage.
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => c.IsHero, (Card c) => new int?(2), DamageType.Lightning);
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

        private IEnumerator AbsorbDealDamageResponse(PhaseChangeAction action)
        {
            //...{SwarmEater} deals the target other than itself with the highest HP 2 lightning damage.
            IEnumerator coroutine = base.DealDamageToHighestHP(this.CardThatAbsorbedThis(), 1, (Card c) => c != this.CardThatAbsorbedThis(), (Card c) => new int?(2), DamageType.Lightning);
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