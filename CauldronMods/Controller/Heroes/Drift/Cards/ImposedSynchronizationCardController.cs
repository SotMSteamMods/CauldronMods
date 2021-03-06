﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class ImposedSynchronizationCardController : DriftUtilityCardController
    {
        public ImposedSynchronizationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Draw a card.
            IEnumerator coroutine = base.DrawCard();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //{DriftPast}
            if (base.IsTimeMatching(Past))
            {
                List<SelectCardDecision> targetDecisions = new List<SelectCardDecision>();
                //Select a target. Reduce damage dealt to that target by 1 until the start of your next turn.
                coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.ReduceDamageTaken, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText), targetDecisions, false, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                ReduceDamageStatusEffect statusEffect = new ReduceDamageStatusEffect(1);
                statusEffect.TargetCriteria.IsSpecificCard = targetDecisions.FirstOrDefault().SelectedCard;
                statusEffect.UntilStartOfNextTurn(base.TurnTaker);
                statusEffect.UntilTargetLeavesPlay(targetDecisions.FirstOrDefault().SelectedCard);
                coroutine = base.AddStatusEffect(statusEffect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //Shift {DriftL}.
                coroutine = base.ShiftL();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            //{DriftFuture} 
            if (base.IsTimeMatching(Future))
            {
                List<SelectCardDecision> targetDecisions = new List<SelectCardDecision>();
                //Select a target. Increase damage dealt by that target by 1 until the start of your next turn. 
                coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.IncreaseDamage, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText), targetDecisions, false, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                IncreaseDamageStatusEffect statusEffect = new IncreaseDamageStatusEffect(1);
                statusEffect.SourceCriteria.IsSpecificCard = targetDecisions.FirstOrDefault().SelectedCard;
                statusEffect.UntilStartOfNextTurn(base.TurnTaker);
                statusEffect.UntilTargetLeavesPlay(targetDecisions.FirstOrDefault().SelectedCard);
                coroutine = base.AddStatusEffect(statusEffect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //Shift {DriftR}.
                coroutine = base.ShiftR();
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
