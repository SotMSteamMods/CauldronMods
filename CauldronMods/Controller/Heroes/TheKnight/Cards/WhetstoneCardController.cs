using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class WhetstoneCardController : RoninAssignableCardController
    {
        public WhetstoneCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddIncreaseDamageTrigger(dd => dd.DamageType == DamageType.Melee && dd.DamageSource != null && dd.DamageSource.Card != null && IsEquipmentEffectingCard(dd.DamageSource.Card), 1);
            base.AddTriggers();
        }
    }
}
