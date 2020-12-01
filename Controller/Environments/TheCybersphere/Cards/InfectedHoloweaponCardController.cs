using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.TheCybersphere
{
    public class InfectedHoloweaponCardController : TheCybersphereCardController
    {

        public InfectedHoloweaponCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the 2 non-environment targets with the highest HP 2 irreducible energy damage each.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageResponse, new TriggerType[]
                {
                    TriggerType.DealDamage,
                    TriggerType.CreateStatusEffect
                });
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            //This card deals the 2 non-environment targets with the highest HP 2 irreducible energy damage each.
            //Damage dealt by those targets is irreducible until the start of the environment turn.
            IEnumerator coroutine = DealDamageToHighestHP(base.Card, 1, (Card c) => c.IsNonEnvironmentTarget, (Card c) => new int?(2), DamageType.Energy, isIrreducible: true, numberOfTargets: () => 2, addStatusEffect: MakeDamageIrreducible);
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

        private IEnumerator MakeDamageIrreducible(DealDamageAction dd)
        {
            //Damage dealt by those targets is irreducible until the start of the environment turn.

            MakeDamageIrreducibleStatusEffect effect = new MakeDamageIrreducibleStatusEffect();
            effect.UntilCardLeavesPlay(dd.Target);
            effect.UntilStartOfNextTurn(base.TurnTaker);
            effect.SourceCriteria.IsSpecificCard = dd.Target;
            IEnumerator coroutine = AddStatusEffect(effect);
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