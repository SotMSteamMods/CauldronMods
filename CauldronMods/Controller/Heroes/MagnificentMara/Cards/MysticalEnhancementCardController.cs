using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Handelabra;

namespace Cauldron.MagnificentMara
{
    public class MysticalEnhancementCardController : CardController
    {
        public MysticalEnhancementCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //"Play this card next to a card with a power on it.",
            IEnumerable<Card> viableCards = FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && GameController.GetAllPowersForCardController(FindCardController(c)).Any());
            IEnumerator coroutine = SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => viableCards.Contains(c)), storedResults, isPutIntoPlay, decisionSources);
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

        public override void AddTriggers()
        {
            //"Increase damage dealt by that power by 1.",
            AddIncreaseDamageTrigger(DeterminePowerIncreaseDealDamageAction, 1);
            AddTrigger<AddStatusEffectAction>(DeterminePowerIncreaseAddStatusEffectAction, AddDamageBoostToEffect, TriggerType.Hidden, TriggerTiming.Before);

            //"If that card would be destroyed, destroy this card instead."
            AddTrigger((DestroyCardAction dc) => !this.IsBeingDestroyed && dc.CardToDestroy.Card == GetCardThisCardIsNextTo() && !GameController.IsCardIndestructible(dc.CardToDestroy.Card), DestroyThisCardInsteadResponse, TriggerType.CancelAction, TriggerTiming.Before);
            AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(alsoRemoveTriggersFromThisCard: true);
        }

        private bool DeterminePowerIncreaseDealDamageAction(DealDamageAction dda)
        {
            CardController nextToCardController = FindCardController(GetCardThisCardIsNextTo());
            return  (dda.CardSource != null && dda.CardSource.PowerSource != null) && 
                    (dda.CardSource.PowerSource.CardController == nextToCardController ||
                            (dda.CardSource.PowerSource.CardSource != null && 
                             dda.CardSource.PowerSource.CardSource.CardController == nextToCardController));
        }

        private bool DeterminePowerIncreaseAddStatusEffectAction(AddStatusEffectAction se)
        {
            CardController nextToCardController = FindCardController(GetCardThisCardIsNextTo());
            return  (se.StatusEffect.DoesDealDamage && se.CardSource != null && se.CardSource.PowerSource != null) && 
                    (se.CardSource.PowerSource.CardController == nextToCardController ||
                        (se.CardSource.PowerSource.CardSource != null &&
                         se.CardSource.PowerSource.CardSource.CardController == nextToCardController));
        }

        public IEnumerator DestroyThisCardInsteadResponse(DestroyCardAction dc)
        {
            IEnumerator coroutine = GetCardThisCardIsNextTo().HitPoints == null || !(GetCardThisCardIsNextTo().HitPoints.HasValue) || GetCardThisCardIsNextTo().HitPoints.Value > 0 ? CancelAction(dc, showOutput: true, cancelFutureRelatedDecisions: true, null, isPreventEffect: true) : GameController.SendMessageAction("The card this is next to has less than 0 HP! We can't save it!", Priority.Medium, GetCardSource(), showCardSource: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = DestroyThisCardResponse(dc);
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

        private IEnumerator AddDamageBoostToEffect(AddStatusEffectAction se)
        {
            IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
            increaseDamageStatusEffect.StatusEffectCriteria.Effect = se.StatusEffect;
            increaseDamageStatusEffect.CreateImplicitExpiryConditions();

            IEnumerator coroutine = AddStatusEffect(increaseDamageStatusEffect);
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