using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class WarbrandCardController : OutlanderTraceCardController
    {
        public WarbrandCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfElseSpecialString(() => HasBeenSetToTrueThisTurn(OncePerTurn), () => "Outlander has dealt damage this turn.", () => "Outlander has not dealt damage this turn.");
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
            SpecialStringMaker.ShowHeroTargetWithLowestHP(numberOfTargets: 2);
        }

        protected const string OncePerTurn = "OutlanderWarbrandOncePerTurn";

        public override void AddTriggers()
        {
            //The first time {Outlander} deals damage each turn, he then deals the hero target with the highest HP 2 projectile damage.
            AddTrigger<DealDamageAction>((DealDamageAction action) => !HasBeenSetToTrueThisTurn(OncePerTurn) && action.DamageSource.IsSameCard(CharacterCard) && action.DidDealDamage, OncePerTurnResponse, TriggerType.DealDamage, TriggerTiming.After);

            //At the end of the villain turn, {Outlander} deals the 2 hero targets with the lowest HP 1 projectile damage each.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator OncePerTurnResponse(DealDamageAction action)
        {
            SetCardPropertyToTrueIfRealAction(OncePerTurn);
            //...he then deals the hero target with the highest HP 2 projectile damage.
            IEnumerator coroutine = base.DealDamageToHighestHP(CharacterCard, 1, (Card c) => IsHeroTarget(c), (Card c) => 2, DamageType.Projectile);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...{Outlander} deals the 2 hero targets with the lowest HP 1 projectile damage each.
            IEnumerator coroutine = DealDamageToLowestHP(CharacterCard, 1, (Card c) => IsHeroTarget(c), (Card c) => 1, DamageType.Projectile, numberOfTargets: 2);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}
