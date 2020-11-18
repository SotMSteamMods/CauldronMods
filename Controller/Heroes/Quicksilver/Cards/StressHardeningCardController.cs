﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron
{
    public class StressHardeningCardController : CardController
    {
        public StressHardeningCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //If {Quicksilver} currently has less than her max HP, increase damage she deals to non-hero targets by 1.
            //If {Quicksilver} has 10 or fewer HP, increase damage she deals to non-hero targets by an additional 1.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource.Card == base.CharacterCard && !action.Target.IsHero, (DealDamageAction action) => DamageIncrease(action));
        }

        private int DamageIncrease(DealDamageAction action)
        {
            int increase = 0;
            //If {Quicksilver} currently has less than her max HP, increase damage she deals to non-hero targets by 1.
            if (base.CharacterCard.MaximumHitPoints > base.CharacterCard.HitPoints)
            {
                increase++;
            }
            //If {Quicksilver} has 10 or fewer HP, increase damage she deals to non-hero targets by an additional 1.
            if (base.CharacterCard.HitPoints <= 10)
            {
                increase++;
            }
            return increase;
        }
    }
}