using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class WarbrandCardController : OutlanderUtilityCardController
    {
        public WarbrandCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowIfElseSpecialString(() => base.HasBeenSetToTrueThisTurn(OncePerTurn), () => "Outlander has dealt damage this turn.", () => "Outlander has not dealt damage this turn.");
        }

        protected const string OncePerTurn = "OncePerTurn";

        public override void AddTriggers()
        {
            //The first time {Outlander} deals damage each turn, he then deals the hero target with the highest HP 2 projectile damage.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => !base.HasBeenSetToTrueThisTurn(OncePerTurn) && action.DamageSource.IsSameCard(base.CharacterCard) && action.DidDealDamage, this.OncePerTurnResponse, TriggerType.DealDamage, TriggerTiming.After);

            //At the end of the villain turn, {Outlander} deals the 2 hero targets with the lowest HP 1 projectile damage each.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator OncePerTurnResponse(DealDamageAction action)
        {
            base.SetCardPropertyToTrueIfRealAction(OncePerTurn);
            //...he then deals the hero target with the highest HP 2 projectile damage.
            IEnumerator coroutine = base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => c.IsHero && c.IsTarget, (Card c) => 2, DamageType.Projectile);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...{Outlander} deals the 2 hero targets with the lowest HP 1 projectile damage each.
            IEnumerator coroutine = base.DealDamageToLowestHP(base.CharacterCard, 1, (Card c) => c.IsHero && c.IsTarget, (Card c) => 1, DamageType.Projectile, numberOfTargets: 2);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
