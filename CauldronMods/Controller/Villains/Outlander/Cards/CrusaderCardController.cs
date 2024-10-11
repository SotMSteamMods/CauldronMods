using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class CrusaderCardController : OutlanderTraceCardController
    {
        public CrusaderCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonVillainTargetWithHighestHP(numberOfTargets: 2);
        }

        public override void AddTriggers()
        {
            //Increase damage dealt by {Outlander} by 1.
            AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.Card == CharacterCard, 1);

            //At the end of the villain turn, {Outlander} deals the 2 non-villain targets with the highest HP 2 irreducible melee damage each.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...{Outlander} deals the 2 non-villain targets with the highest HP 2 irreducible melee damage each.
            var coroutine = DealDamageToHighestHP(CharacterCard, 1, (Card c) => !IsVillainTarget(c), (Card c) => 2, DamageType.Melee, isIrreducible: true, numberOfTargets: () => 2);
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
