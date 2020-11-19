using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class PlateMailCardController : TheKnightCardController
    {
        public PlateMailCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddRedirectDamageTrigger(dd => IsEquipmentEffectingCard(dd.Target), c => base.Card, true);
        }
    }
}
