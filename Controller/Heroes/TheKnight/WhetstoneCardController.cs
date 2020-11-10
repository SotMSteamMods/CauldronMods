using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace Cauldron.TheKnight
{
    public class WhetstoneCardController : TheKnightCardController
    {
        public WhetstoneCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddIncreaseDamageTrigger(dd => dd.DamageType == DamageType.Melee && IsEquipmentEffectingCard(dd.DamageSource.Card), 1);
        }
    }
}
