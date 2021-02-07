using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.NightloreCitadel
{
    public class TalinBroskCardController : NightloreCitadelUtilityCardController
    {
        public TalinBroskCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals 1 other target 3 sonic damage.
            //Increase damage dealt by a target damaged this way by 1 until the start of the next environment turn.

            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, EndOfTurnResponse, new TriggerType[]
            {
                TriggerType.DealDamage,
                TriggerType.IncreaseDamage
            });
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, Card), 3, DamageType.Sonic, 1, false, 1, additionalCriteria: (Card c) => c != Card, addStatusEffect: (DealDamageAction dd) => IncreaseDamageDealtIfDamagedByThatTargetBy1UntilTheStartOfYourNextTurnResponse(dd), cardSource: GetCardSource());
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

        private IEnumerator IncreaseDamageDealtIfDamagedByThatTargetBy1UntilTheStartOfYourNextTurnResponse(DealDamageAction dd)
        {
            if(dd.DidDealDamage)
            {
                IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
                increaseDamageStatusEffect.SourceCriteria.IsSpecificCard = dd.Target;
                increaseDamageStatusEffect.UntilStartOfNextTurn(TurnTaker);
                increaseDamageStatusEffect.UntilCardLeavesPlay(dd.Target);
                IEnumerator coroutine = AddStatusEffect(increaseDamageStatusEffect);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }
    }
}
