using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class StalwartShieldCardController : SingleHandEquipmentCardController
    {
        public StalwartShieldCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Reduce damage taken by {TheKnight} and your Equipment cards by 1."
            base.AddReduceDamageTrigger(c => IsEquipmentEffectingCard(c) || this.IsOwnEquipment(c), 1);
            base.AddTriggers();
        }

        private bool IsOwnEquipment(Card c)
        {
            if(this.TurnTakerControllerWithoutReplacements.HasMultipleCharacterCards)
            {
                if(c.Owner != this.TurnTaker || !IsEquipment(c))
                {
                    return false;
                }
                
                return GetKnightCardUser(this.Card) == GetKnightCardUser(c);
            }
            return base.IsEquipment(c) && c.Owner == base.TurnTaker;
        }
    }
}
