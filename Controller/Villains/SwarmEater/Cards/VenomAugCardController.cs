using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.SwarmEater
{
    public class VenomAugCardController : AugCardController
    {
        public VenomAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            if (base.Card.IsInPlayAndNotUnderCard)
            {
                base.SpecialStringMaker.ShowHeroTargetWithHighestHP();
            }
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn this card deals the hero target with the highest HP {H} projectile damage. Any target dealt damage this way deals itself 1 toxic damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealDamageResponse), TriggerType.DealDamage);
        }
        private IEnumerator DealDamageResponse(PhaseChangeAction phaseChange)
        {
            //this card deals the hero target with the highest HP {H} projectile damage.
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => c.IsHero, (Card c) => Game.H, DamageType.Projectile, storedResults: storedResults, numberOfTargets: () => 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            IEnumerable<Card> damagedTargets = (from d in storedResults
                                                where d.DidDealDamage
                                                select d.Target).Distinct<Card>();
            if (storedResults != null)
            {
                Card target = storedResults.FirstOrDefault().Target;
                //Any target dealt damage this way deals itself 1 toxic damage.
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

        public override IEnumerator ActivateAbsorb(Card cardThisIsUnder)
        {
            //Absorb: whenever {SwarmEater} deals damage to another target, that target deals itself 1 toxic damage.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DamageSource.Card == cardThisIsUnder && action.DidDealDamage, this.AbsorbDealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
            yield break;
        }

        public IEnumerator AbsorbDealDamageResponse(DealDamageAction action)
        {
            IEnumerator coroutine = base.DealDamage(action.Target, action.Target, 1, DamageType.Toxic, cardSource: base.GetCardSource());
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