using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Titan
{
    public class PaybackTimeCardController : CardController
    {
        public PaybackTimeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Increase damage dealt by {Titan} by 1 to non-hero targets that have dealt him damage since the end of his last turn.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.Target == base.CharacterCard, this.IncreaseDamageStatusEffectResponse, TriggerType.IncreaseDamage, TriggerTiming.After);
            //At the end of your turn {Titan} regains 1HP.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.GainHPResponse, TriggerType.GainHP);
        }

        private IEnumerator GainHPResponse(PhaseChangeAction action)
        {
            IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, 1, cardSource: base.GetCardSource());
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

        private IEnumerator IncreaseDamageStatusEffectResponse(DealDamageAction action)
        {
            IncreaseDamageStatusEffect statusEffect = new IncreaseDamageStatusEffect(1)
            {
                SourceCriteria = { IsSpecificCard = base.CharacterCard },
                TargetCriteria = { IsSpecificCard = action.DamageSource.Card }
            };
            statusEffect.UntilEndOfNextTurn(base.TurnTaker);
            IEnumerator coroutine = base.AddStatusEffect(statusEffect);
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