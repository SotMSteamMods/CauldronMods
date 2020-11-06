using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace Cauldron.TheKnight
{
    public class PlateMailCardController : TheKnightCardController
    {
        public PlateMailCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //TODO - Promo Support
            base.AddRedirectDamageTrigger(dd => dd.Target == base.CharacterCard, c => base.Card, true);
        }
    }
}
