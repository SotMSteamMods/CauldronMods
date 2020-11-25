using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Northspar
{
    public class FrostbiteCardController : NorthsparCardController
    {

        public FrostbiteCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the {H + 1} non-environment targets with the lowest HP 4 cold damage each and is destroyed.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, new TriggerType[]
            {
                TriggerType.DealDamage,
                TriggerType.DestroySelf
            });
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            // this card deals the {H + 1} non-environment targets with the lowest HP 4 cold damage each...
            IEnumerator coroutine = base.DealDamageToLowestHP(base.Card, 1, (Card c) => c.IsNonEnvironmentTarget, (Card c) => 4, DamageType.Cold, numberOfTargets: Game.H + 1);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            // ...and is destroyed.
            coroutine = base.DestroyThisCardResponse(pca);
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