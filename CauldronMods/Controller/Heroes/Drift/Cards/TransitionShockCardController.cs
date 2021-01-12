using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class TransitionShockCardController : DriftUtilityCardController
    {
        public TransitionShockCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Whenever you shift from {DriftPast} to {DriftFuture} or from {DriftFuture} to {DriftPast}, {Drift} may deal 1 other target and herself 1 psychic damage.
            base.AddTrigger<CardPropertiesJournalEntry>((CardPropertiesJournalEntry entry) => base.IsTimeMatching(Past) && entry.Key == HasShifted && entry.BoolValue == true, this.ShiftLResponse, TriggerType.ReduceDamageOneUse, TriggerTiming.After);
        }

        private IEnumerator DealDamageResponse(CardPropertiesJournalEntry entry)
        {
            //...{Drift} may deal 1 other target...
            List<SelectCardDecision> targetDecision = new List<SelectCardDecision>();
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.GetActiveCharacterCard()), 1, DamageType.Psychic, 1, true, 1, storedResultsDecisions: targetDecision, cardSource: base.GetCardSource());
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
                coroutine = base.DealDamage(base.GetActiveCharacterCard(), base.GetActiveCharacterCard(), 1, DamageType.Psychic, cardSource: base.GetCardSource());
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
