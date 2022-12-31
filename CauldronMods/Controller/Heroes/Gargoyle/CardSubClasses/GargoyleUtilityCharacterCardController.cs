using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    public abstract class GargoyleUtilityCharacterCardController : HeroCharacterCardController
    {
        protected GargoyleUtilityCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(TotalNextDamageBoostString);
        }

        public override void AddStartOfGameTriggers()
        {
            AddTrigger((GameAction ga) => TurnTakerController is GargoyleTurnTakerController ttc && !ttc.ArePromosSetup, SetupPromos, TriggerType.Hidden, TriggerTiming.Before, priority: TriggerPriority.High);
        }

        public IEnumerator SetupPromos(GameAction ga)
        {
            if (TurnTakerController is GargoyleTurnTakerController ttc && !ttc.ArePromosSetup)
            {
                ttc.SetupPromos(ttc.availablePromos);
                ttc.ArePromosSetup = true;
            }

            return DoNothing();
        }

        protected string TotalNextDamageBoostString()
        {
            var allDamageBoosts = GameController.StatusEffectManager.StatusEffectControllers.Where(sec => sec.StatusEffect is IncreaseDamageStatusEffect);
            int totalBoost = 0;
            foreach (StatusEffectController sec in allDamageBoosts)
            {
                var thisEffect = sec.StatusEffect as IncreaseDamageStatusEffect;
                if (thisEffect.SourceCriteria.IsSpecificCard == Card && thisEffect.NumberOfUses == 1)
                {
                    totalBoost += thisEffect.Amount;
                }
            }
            if (totalBoost == 0)
            {
                return $"{Card.Title} has no temporary damage boosts.";
            }
            return $"{Card.Title}'s next damage will be increased by a total of {totalBoost}.";
        }
    }
}
