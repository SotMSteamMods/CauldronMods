using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.NightloreCitadel
{
    public class StarlightOfNoomeCardController : NightloreCitadelUtilityCardController
    {
        public StarlightOfNoomeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonEnvironmentTargetWithLowestHP(numberOfTargets: 2);
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, select the 2 non - environment targets with the lowest HP.This card deals 1 of those targets 2 melee damage, and the other target regains 2HP.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, EndOfTurnResponse, TriggerType.DealDamage);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            //select the 2 non - environment targets with the lowest HP.

            List<Card> storedResult = new List<Card>() ;
            IEnumerator coroutine = GameController.FindTargetsWithLowestHitPoints(1, 2, (Card c) => c.IsNonEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), storedResult, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //This card deals 1 of those targets 2 melee damage, and the other target regains 2HP
            if(storedResult != null && storedResult.Any())
            {

                List<SelectCardDecision> storedDecision = new List<SelectCardDecision>() ;
                coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), 2, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => storedResult.Contains(c), storedResultsDecisions: storedDecision, selectTargetsEvenIfCannotDealDamage: true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (storedResult.Count > 1 && DidSelectCard(storedDecision))
                {
                    Card selectedCard = GetSelectedCard(storedDecision);
                    Card otherCard = storedResult.Find((Card c) => c != selectedCard);
                    coroutine = GameController.GainHP(otherCard, 2, cardSource: GetCardSource());
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
