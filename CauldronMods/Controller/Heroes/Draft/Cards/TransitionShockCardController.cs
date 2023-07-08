using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Draft
{
    public class TransitionShockCardController : CardController
    {
        public TransitionShockCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            ////Whenever you shift from {DriftPast} to {DriftFuture}... 
            //base.AddTrigger<AddTokensToPoolAction>((AddTokensToPoolAction action) => action.IsSuccessful && action.TokenPool.Identifier == ShiftPoolIdentifier && action.TokenPool.CurrentValue == 3, this.DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
            ////...or from {DriftFuture} to {DriftPast}...
            //base.AddTrigger<RemoveTokensFromPoolAction>((RemoveTokensFromPoolAction action) => action.IsSuccessful && action.TokenPool.Identifier == ShiftPoolIdentifier && action.TokenPool.CurrentValue == 2, this.DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
            ////...{Drift} may deal 1 other target and herself 1 psychic damage.
        }

        private IEnumerator DealDamageResponse(ModifyTokensAction action)
        {
            //...{Drift} may deal 1 other target...
            List<SelectCardDecision> targetDecision = new List<SelectCardDecision>();
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, CharacterCard), 1, DamageType.Psychic, 1, true, 1, additionalCriteria: (Card c) => c != CharacterCard, storedResultsDecisions: targetDecision, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //...and herself 1 psychic damage.
            if (targetDecision.Any())
            {
                coroutine = base.DealDamage(CharacterCard, CharacterCard, 1, DamageType.Psychic, cardSource: base.GetCardSource());
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
    }
}
