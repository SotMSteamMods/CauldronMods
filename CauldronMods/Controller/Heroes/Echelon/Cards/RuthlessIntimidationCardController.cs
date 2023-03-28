using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Echelon
{
    public class RuthlessIntimidationCardController : TacticBaseCardController
    {
        //==============================================================
        // At the start of your turn, you may discard a card. If you do not, draw a card and destroy this card.
        // Increase damage dealt to the non-hero target with the lowest HP by 1.
        //==============================================================

        public static string Identifier = "RuthlessIntimidation";
        private bool? PerformReduce
        {
            get;
            set;
        }

        public RuthlessIntimidationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AllowFastCoroutinesDuringPretend = false;
            RunModifyDamageAmountSimulationForThisCard = false;
            SpecialStringMaker.ShowNonHeroTargetWithLowestHP(1, 1);
        }

        protected override void AddTacticEffectTrigger()
        {
            //Increase damage dealt to the non-hero target with the lowest HP by 1.
            AddTrigger((DealDamageAction dd) => !IsHero(dd.Target) && CanCardBeConsideredLowestHitPoints(dd.Target, (Card c) => !IsHero(c)), MaybeIncreaseDamageResponse, TriggerType.IncreaseDamage, TriggerTiming.Before);
        }

        private IEnumerator MaybeIncreaseDamageResponse(DealDamageAction dd)
        {
            if (GameController.PretendMode)
            {
                List<bool> storedResults = new List<bool>();
                IEnumerator coroutine = DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints(dd.Target, highest: false, (Card card) => !IsHero(card), dd, storedResults);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                PerformReduce = storedResults.Count() > 0 && storedResults.First();
            }
            if (PerformReduce.HasValue && PerformReduce.Value)
            {
                IEnumerator coroutine2 = GameController.IncreaseDamage(dd, 1, false, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine2);
                }
            }
            if (!GameController.PretendMode)
            {
                PerformReduce = null;
            }
            yield break;
        }
    }
}