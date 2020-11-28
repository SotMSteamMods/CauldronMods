using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

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
            AddTrigger((UsePowerAction up) => up.Power.CardController.Card == GetCardThisCardIsNextTo(),
                            (UsePowerAction up) => ModifyDamageFromPowerResponse(up,
                                                        (Func<DealDamageAction, bool> c) => AddIncreaseDamageTrigger(c, 1),
                                                        1),
                            TriggerType.IncreaseDamage,
                            TriggerTiming.Before);
            //"If that card would be destroyed, destroy this card instead."
            AddTrigger((DestroyCardAction dc) => dc.CardToDestroy.Card == GetCardThisCardIsNextTo() && !GameController.IsCardIndestructible(dc.CardToDestroy.Card), DestroyThisCardInsteadResponse, TriggerType.CancelAction, TriggerTiming.Before);
        }

        public IEnumerator DestroyThisCardInsteadResponse(DestroyCardAction dc)
        {
            IEnumerator coroutine = CancelAction(dc);
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

        //WITNESS THE POWER OF COPY-PASTING PRIVATE FUNCTIONS!
        private IEnumerator ModifyDamageFromPowerResponse(UsePowerAction power, Func<Func<DealDamageAction, bool>, ITrigger> addDealDamageTrigger, int? increaseDamageAmount = null, bool makeDamageIrreducible = false)
        {
            RemoveTemporaryTriggers();
            CardController powerCardController = (CardController)AddTemporaryVariable("SelectedPowerCardControllerForDealDamageModification", power.Power.IsContributionFromCardSource ? power.Power.CardSource.CardController : power.Power.CardController);
            Func<DealDamageAction, bool> arg = (DealDamageAction dd) => dd.CardSource.PowerSource != null && dd.CardSource.PowerSource == power.Power && (dd.CardSource.CardController == powerCardController || dd.CardSource.AssociatedCardSources.Any((CardSource cs) => cs.CardController == powerCardController)) && !dd.DamageModifiers.Select((ModifyDealDamageAction md) => md.CardSource.CardController).Contains(this) && !powerCardController.Card.IsBeingDestroyed;
            AddToTemporaryTriggerList(addDealDamageTrigger(arg));
            if (increaseDamageAmount.HasValue)
            {
                AddToTemporaryTriggerList(AddTrigger((AddStatusEffectAction se) => se.StatusEffect.DoesDealDamage && se.CardSource.PowerSource != null && se.CardSource.PowerSource == power.Power, (AddStatusEffectAction se) => IncreaseDamageFromEffectResponse(se, increaseDamageAmount.Value, power.Power), TriggerType.Hidden, TriggerTiming.Before));
            }
            yield return null;
            yield break;
        }

        private IEnumerator IncreaseDamageFromEffectResponse(AddStatusEffectAction se, int increaseAmount, Power power)
        {
            IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(increaseAmount);
            increaseDamageStatusEffect.StatusEffectCriteria.Effect = se.StatusEffect;
            if (power != null && power.CardController != null)
            {
                increaseDamageStatusEffect.StatusEffectCriteria.CardWithPower = power.CardController.Card;
            }
            IEnumerator coroutine = AddStatusEffect(increaseDamageStatusEffect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}