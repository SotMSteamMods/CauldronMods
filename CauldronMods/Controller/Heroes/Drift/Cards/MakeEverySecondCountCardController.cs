using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class MakeEverySecondCountCardController : DriftUtilityCardController
    {
        public MakeEverySecondCountCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //{DriftPast} Whenever you shift {DriftL}, select a hero target. Reduce the next damage dealt to that target by 2.
            base.AddTrigger<CardPropertiesJournalEntry>((CardPropertiesJournalEntry entry) => base.IsTimeMatching(Past) && entry.Key == HasShifted && entry.BoolValue == true, this.ShiftLResponse, TriggerType.ReduceDamageOneUse, TriggerTiming.After);

            //{DriftFuture} Whenever you shift {DriftR}, select a hero target. Increase the next damage dealt by that target by 2.
        }

        private IEnumerator ShiftLResponse(CardPropertiesJournalEntry entry)
        {
            //...select a hero target. 
            List<SelectTargetDecision> targetDecision = new List<SelectTargetDecision>();
            IEnumerator coroutine = base.GameController.SelectTargetAndStoreResults(base.HeroTurnTakerController, base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsHero)), targetDecision, selectionType: SelectionType.ReduceNextDamageTaken, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            Card selectedTarget = targetDecision.FirstOrDefault().SelectedCard;

            //Reduce the next damage dealt to that target by 2.
            ReduceDamageStatusEffect statusEffect = new ReduceDamageStatusEffect(2);
            statusEffect.NumberOfUses = 1;
            statusEffect.TargetCriteria.IsSpecificCard = selectedTarget;
            statusEffect.UntilCardLeavesPlay(selectedTarget);

            coroutine = base.AddStatusEffect(statusEffect);
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

        private IEnumerator ShiftRResponse(CardPropertiesJournalEntry entry)
        {
            //...select a hero target. 
            List<SelectTargetDecision> targetDecision = new List<SelectTargetDecision>();
            IEnumerator coroutine = base.GameController.SelectTargetAndStoreResults(base.HeroTurnTakerController, base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsHero)), targetDecision, selectionType: SelectionType.ReduceNextDamageTaken, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            Card selectedTarget = targetDecision.FirstOrDefault().SelectedCard;

            //Increase the next damage dealt by that target by 2.
            IncreaseDamageStatusEffect statusEffect = new IncreaseDamageStatusEffect(2);
            statusEffect.NumberOfUses = 1;
            statusEffect.SourceCriteria.IsSpecificCard = selectedTarget;
            statusEffect.UntilCardLeavesPlay(selectedTarget);

            coroutine = base.AddStatusEffect(statusEffect);
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
