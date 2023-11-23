using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    [Serializable]
    public class GargoyleDamageIncrease : OnDealDamageStatusEffect
    {
        public GargoyleDamageIncrease(CardController controller, int _amount, DealDamageAction _notThisDDA)
            : base(
                  controller.CardWithoutReplacements, 
                  nameof(GargoyleUtilityCardController.GargoyleDamageIncreaseFunc),
                  "Increase the next damage dealt by " + controller.CharacterCard.Title + " by " + _amount + ", excepting damage reduced by leech field.", 
                  new TriggerType[] { TriggerType.IncreaseDamage },
                  controller.TurnTaker,
                  controller.Card
            )
        {
            Amount = _amount;
            NotThisDDA = _notThisDDA.InstanceIdentifier;
        }

        public int Amount;
        public Guid NotThisDDA;
    }


    public abstract class GargoyleUtilityCardController : CardController
    {
        protected GargoyleUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public IEnumerator GargoyleDamageIncreaseFunc(DealDamageAction dda, TurnTaker decisionMaker, OnDealDamageStatusEffect effect, int[] powerNumerals)
        {
            var gdEffect = effect as GargoyleDamageIncrease;
            if (gdEffect == null || gdEffect.NotThisDDA == dda.InstanceIdentifier)
            {
                ++effect.NumberOfUses;
                yield break;
            }

            var e = GameController.IncreaseDamage(dda, gdEffect.Amount, cardSource: GetCardSource());
            if (UseUnityCoroutines) { yield return GameController.StartCoroutine(e); }
            else { GameController.ExhaustCoroutine(e); }
        }

        // Most of Gargoyle's cards increase his next damage dealt by a set amount.
        protected IEnumerator IncreaseGargoyleNextDamage(int amount, DealDamageAction ignoreThisDDA = null)
        {
            StatusEffect increaseDamageStatusEffect;
            if (ignoreThisDDA == null)
            {
                var effect = new IncreaseDamageStatusEffect(amount);
                effect.SourceCriteria.IsSpecificCard = CharacterCard;
                increaseDamageStatusEffect = effect;
                increaseDamageStatusEffect.NumberOfUses = 1;
            }
            else
            {
                var effect = new GargoyleDamageIncrease(this, amount, ignoreThisDDA);
                effect.SourceCriteria.IsSpecificCard = CharacterCard;
                increaseDamageStatusEffect = effect;
            }

            increaseDamageStatusEffect.UntilTargetLeavesPlay(CharacterCard);

            return AddStatusEffect(increaseDamageStatusEffect);
        }

        protected string TotalNextDamageBoostString()
        {
            var allDamageBoosts = GameController.StatusEffectManager.StatusEffectControllers.Where(
                sec => sec.StatusEffect is IncreaseDamageStatusEffect || sec.StatusEffect is GargoyleDamageIncrease
            );
            int totalBoost = 0;
            int conditionalBoost = 0;
            foreach(StatusEffectController sec in allDamageBoosts)
            {
                if (sec.StatusEffect is IncreaseDamageStatusEffect thisEffect)
                {
                    if (thisEffect.SourceCriteria.IsSpecificCard == CharacterCard && thisEffect.NumberOfUses > 0)
                    {
                        totalBoost += thisEffect.Amount;
                    }
                }

                if (sec.StatusEffect is GargoyleDamageIncrease thisGargoyleEffect)
                {
                    if (thisGargoyleEffect.SourceCriteria.IsSpecificCard == CharacterCard && thisGargoyleEffect.NumberOfUses > 0)
                    {
                        totalBoost += thisGargoyleEffect.Amount;
                        conditionalBoost += thisGargoyleEffect.Amount;
                    }
                }
            }
            if(totalBoost == 0)
            {
                return $"{TurnTaker.Name} has no temporary damage boosts.";
            }

            var ret = $"{TurnTaker.Name}'s next damage will be increased by a total of {totalBoost}";
            if (conditionalBoost > 0)
            {
                ret += $"; {conditionalBoost} of which is conditional";
            }

            return ret + ".";
        }
    }
}
