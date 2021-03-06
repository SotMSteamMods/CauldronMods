using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class UnknownHerdsCardController : OblaskCraterUtilityCardController
    {
        /* 
         * [ORIGNIAL TEXT - The current engine does not provide a way to determine if a power will or will not damage]
         * At the end of the environment turn, if there are 0 or 1 predator cards in play, 1 hero may use a power that 
         * does not deal damage. 
         * When this card is destroyed, each predator and villain target regains {H} HP. 
         */
        /* [Alternate Text - Use until the engine supports additional metadata for powers]
         * At the end of the environment turn, if there are 0 or 1 predator cards in play, 1 hero may use a power. If that 
         * power would deal damage, prevent it. 
         * When this card is destroyed, each predator and villain target regains {H} HP. 
         */
        public UnknownHerdsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(c => IsPredator(c), "predator"));
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((tt)=>tt.IsEnvironment, PhaseChangeActionResponse, TriggerType.UsePower);
            base.AddWhenDestroyedTrigger(DestroyCardActionResponse, TriggerType.GainHP);

            //prevent the damage from powers caused by this card
            AddTrigger((UsePowerAction up) => up.CardSource != null && up.CardSource.Card == this.Card, PreventDamageFromPowerResponse, TriggerType.Hidden, TriggerTiming.Before);
        }

        private IEnumerator PhaseChangeActionResponse(PhaseChangeAction phaseChangeAction)
        {
            IEnumerator coroutine;
            int totalPredators = base.GameController.GetAllCards().Count(card => card != base.Card && card.IsInPlay && IsPredator(card));
            List<UsePowerDecision> storedResultsAction = new List<UsePowerDecision>();
            List<SelectCardDecision> selectCardDecisions = new List<SelectCardDecision>();

            if (totalPredators == 0 || totalPredators == 1)
            {
                coroutine = GameController.SelectHeroToUsePower(DecisionMaker, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                coroutine = GameController.SendMessageAction($"There are {totalPredators} predators in play, so {Card.Title} does not give a power.", Priority.Medium, GetCardSource(), showCardSource: true);
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

        private IEnumerator PreventDamageFromPowerResponse(UsePowerAction up)
        {
            RemoveTemporaryTriggers();
            AddToTemporaryTriggerList(AddPreventDamageTrigger((DealDamageAction dd) => dd.CardSource != null && dd.CardSource.PowerSource != null && dd.CardSource.PowerSource == up.Power));
            AddToTemporaryTriggerList(AddTrigger((AddStatusEffectAction se) => se.StatusEffect.DoesDealDamage && se.CardSource != null && se.CardSource.PowerSource == up.Power, PreventDamageFromEffectResponse, TriggerType.Hidden, TriggerTiming.After));
            yield return null;
            yield break;
        }

        private IEnumerator PreventDamageFromEffectResponse(AddStatusEffectAction se)
        {
            var preventEffect = new CannotDealDamageStatusEffect();
            preventEffect.IsPreventEffect = true;
            preventEffect.StatusEffectCriteria.Effect = se.StatusEffect;
            preventEffect.UntilEffectExpires(se.StatusEffect);
            return GameController.AddStatusEffect(preventEffect, true, GetCardSource());
        }

        private IEnumerator UsePowerActionResponse(UsePowerAction usePowerAction)
        {
            base.AddToTemporaryTriggerList(AddPreventDamageTrigger((dda) => DamageCritera(dda, usePowerAction), isPreventEffect: true));

            yield break;
        }

        private bool DamageCritera(DealDamageAction dealDamageAction, UsePowerAction usePowerAction)
        {
            return usePowerAction.Power.CardSource.Card == dealDamageAction.CardSource.Card;
        }

        private IEnumerator DestroyCardActionResponse(DestroyCardAction destroyCardAction)
        {
            IEnumerator coroutine;

            coroutine = base.GameController.GainHP(base.HeroTurnTakerController, (card) => card.IsVillain || card.DoKeywordsContain("predator"), base.H, cardSource: base.GetCardSource());
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
