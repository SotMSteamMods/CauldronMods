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
            base.AddTrigger<RemoveTokensFromPoolAction>((RemoveTokensFromPoolAction action) => base.IsTimeMatching(Past) && action.IsSuccessful && action.TokenPool.Identifier == ShiftPoolIdentifier && action.TokenPool.CurrentValue == 1, this.ShiftResponse, TriggerType.ReduceDamageOneUse, TriggerTiming.After);

            //{DriftFuture} Whenever you shift {DriftR}, select a hero target. Increase the next damage dealt by that target by 2.
            base.AddTrigger<AddTokensToPoolAction>((AddTokensToPoolAction action) => base.IsTimeMatching(Future) && action.IsSuccessful && action.TokenPool.Identifier == ShiftPoolIdentifier && action.TokenPool.CurrentValue == 4, this.ShiftResponse, TriggerType.IncreaseDamage, TriggerTiming.After);
        }

        private IEnumerator ShiftResponse(ModifyTokensAction action)
        {
            SelectionType type = SelectionType.ReduceNextDamageTaken;
            if (action is AddTokensToPoolAction)
            {
                type = SelectionType.IncreaseNextDamage;
            }
            //...select a hero target. 
            List<SelectTargetDecision> targetDecision = new List<SelectTargetDecision>();
            IEnumerator coroutine = base.GameController.SelectTargetAndStoreResults(base.HeroTurnTakerController, base.FindCardsWhere(new LinqCardCriteria((Card c) => IsHeroTarget(c) && c.IsInPlayAndHasGameText)), targetDecision, selectionType: type, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (targetDecision.Any())
            {
                Card selectedTarget = targetDecision.FirstOrDefault().SelectedCard;
                if (action is AddTokensToPoolAction)
                {
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
                }
                if (action is RemoveTokensFromPoolAction)
                {
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
                }
            }
            yield break;
        }
    }
}
