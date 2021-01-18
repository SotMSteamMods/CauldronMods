using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public class GrayPharmaceuticalBuildingCardController : WindmillCityUtilityCardController
    {
        public GrayPharmaceuticalBuildingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Whenever a hero uses a power that deals damage, increase that damage by 2.
            AddTrigger((UsePowerAction up) => GameController.IsTurnTakerVisibleToCardSource(up.HeroUsingPower.TurnTaker, GetCardSource()),
             (UsePowerAction up) => ModifyDamageAmountFromPowerResponse(up,
                                         (Func<DealDamageAction, bool> c) => AddIncreaseDamageTrigger(c, 2),
                                         2),
             TriggerType.IncreaseDamage,
             TriggerTiming.Before);

            AddEndOfTurnTrigger((TurnTaker tt) => true, ClearTempTriggers, TriggerType.Hidden);

            //After a hero uses a power on a non-character card, destroy that card.
            AddTrigger((UsePowerAction up) => GameController.IsTurnTakerVisibleToCardSource(up.HeroUsingPower.TurnTaker, GetCardSource()) && up.Power != null && up.Power.CardSource != null && !up.Power.CardSource.Card.IsCharacter, UseNonCharacterPowerResponse, TriggerType.DestroyCard, TriggerTiming.After);
            
            //At the start of the environment turn, destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);

        }

        private IEnumerator UseNonCharacterPowerResponse(UsePowerAction up)
        {
            Card powerCardSource = up.Power.CardSource.Card;
          
            IEnumerator coroutine = GameController.DestroyCard(DecisionMaker, powerCardSource, cardSource: GetCardSource());
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

        private IEnumerator ClearTempTriggers(GameAction ga)
        {
            RemoveTemporaryTriggers();
            yield return null;
            yield break;
        }

        private IEnumerator ModifyDamageAmountFromPowerResponse(UsePowerAction power, Func<Func<DealDamageAction, bool>, ITrigger> addDealDamageTrigger, int? increaseDamageAmount = null, bool makeDamageIrreducible = false)
        {
            CardController powerCardController = power.Power.IsContributionFromCardSource ? power.Power.CardSource.CardController : power.Power.CardController;
            Func<DealDamageAction, bool> argIncreaseDamage = (DealDamageAction dd) => dd.CardSource.PowerSource != null && dd.CardSource.PowerSource == power.Power && (dd.CardSource.CardController == powerCardController || dd.CardSource.AssociatedCardSources.Any((CardSource cs) => cs.CardController == powerCardController)) && !dd.DamageModifiers.Where((ModifyDealDamageAction md) => md is IncreaseDamageAction).Select((ModifyDealDamageAction md) => md.CardSource.CardController).Contains(this) && !powerCardController.Card.IsBeingDestroyed;
            AddToTemporaryTriggerList(addDealDamageTrigger(argIncreaseDamage));
            
            AddToTemporaryTriggerList(AddTrigger((AddStatusEffectAction se) => se.StatusEffect.DoesDealDamage && se.CardSource.PowerSource != null && se.CardSource.PowerSource == power.Power, (AddStatusEffectAction se) => ModifyDamageFromEffectResponse(se, increaseDamageAmount.Value, power.Power), TriggerType.Hidden, TriggerTiming.Before));

            yield return null;
            yield break;
        }
     
       
        private IEnumerator ModifyDamageFromEffectResponse(AddStatusEffectAction se, int increaseAmount, Power power)
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

            yield break;
            
        }

    }
}
