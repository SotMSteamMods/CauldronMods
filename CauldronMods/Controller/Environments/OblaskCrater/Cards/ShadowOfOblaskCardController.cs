using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class ShadowOfOblaskCardController : OblaskCraterUtilityCardController
    {
        /*
         * At the end of the environment turn, this card deals the hero target with the second lowest HP {H} energy damage.
         * If no other predator cards are in play, increase damage dealt by this card by 1.
         */
        public ShadowOfOblaskCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((tt)=>tt.IsEnvironment, PhaseChangeActionResponse, TriggerType.DealDamage);
            base.AddTrigger<DealDamageAction>((dda) => dda.DamageSource.Card == base.Card, DealDamageActionResponse, TriggerType.ModifyDamageAmount, TriggerTiming.Before, isConditional: true, isActionOptional: true);
        }

        private IEnumerator PhaseChangeActionResponse(PhaseChangeAction phaseChangeAction)
        {
            IEnumerator coroutine;
            IEnumerable<Card> targetsWith2ndLowestHP;

            targetsWith2ndLowestHP = base.GameController.FindAllTargetsWithLowestHitPoints(2, (card) => card.IsHero, base.GetCardSource());

            if (targetsWith2ndLowestHP != null && targetsWith2ndLowestHP.Count() > 0)
            {
                coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.Card), base.H, DamageType.Energy, 1, false, 1, additionalCriteria: (card) => targetsWith2ndLowestHP.Contains(card), cardSource: base.GetCardSource());
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

        private IEnumerator DealDamageActionResponse(DealDamageAction dealDamageAction)
        {
            IEnumerator coroutine;

            if (base.GameController.GetAllCards().Count(card => card != base.Card && card.IsInPlay && card.DoKeywordsContain("predator")) == 0)
            {
                coroutine = base.GameController.IncreaseDamage(dealDamageAction, 1, cardSource: base.GetCardSource());
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
