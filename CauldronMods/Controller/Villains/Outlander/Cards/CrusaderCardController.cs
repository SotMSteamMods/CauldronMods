using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class CrusaderCardController : OutlanderUtilityCardController
    {
        public CrusaderCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNonVillainTargetWithHighestHP(numberOfTargets: 2);
        }

        public override void AddTriggers()
        {
            //Increase damage dealt by {Outlander} by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource.Card == base.CharacterCard, 1);

            //At the end of the villain turn, {Outlander} deals the 2 non-villain targets with the highest HP 2 irreducible melee damage each.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...{Outlander} deals the 2 non-villain targets with the highest HP 2 irreducible melee damage each.
            IEnumerator coroutine = base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => !base.IsVillain(c) && c.IsTarget, (Card c) => 2, DamageType.Melee, true, numberOfTargets: () => 2);
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
