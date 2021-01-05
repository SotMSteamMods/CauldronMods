using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.TheCybersphere
{
    public class Fo551lCardController : TheCybersphereCardController
    {

        public Fo551lCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonEnvironmentTargetWithHighestHP(ranking: 2);
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the non-environment target with the second highest HP 3 melee damage and 1 lightning damage.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageResponse, TriggerType.DealDamage);
        }
        private IEnumerator DealDamageResponse(PhaseChangeAction phaseChange)
        {
            //this card deals the non-environment target with the second highest HP 3 melee damage and 1 lightning damage.
            IEnumerator coroutine = base.DealMultipleInstancesOfDamage(new List<DealDamageAction>
            {
                new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.Card), null, 3, DamageType.Melee),
                new DealDamageAction(GetCardSource(), new DamageSource(base.GameController, base.Card), null, 1, DamageType.Lightning)
            }, (Card target) => CanCardBeConsideredHighestHitPoints(target, (Card c) => c.IsNonEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), ranking: 2), numberOfTargets: new int?(1));
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