using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    public abstract class GargoyleUtilityCardController : CardController
    {
        protected GargoyleUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        // Most of Gargoyle's cards increase his next damage dealt by a set amount.
        protected IEnumerator IncreaseGargoyleNextDamage(int amount)
        {
            IncreaseDamageStatusEffect increaseDamageStatusEffect;

            increaseDamageStatusEffect = new IncreaseDamageStatusEffect(amount);
            increaseDamageStatusEffect.SourceCriteria.IsSpecificCard = base.CharacterCard;
            increaseDamageStatusEffect.NumberOfUses = 1;
            increaseDamageStatusEffect.UntilTargetLeavesPlay(base.CharacterCard);

            return base.AddStatusEffect(increaseDamageStatusEffect);
        }
    }
}
