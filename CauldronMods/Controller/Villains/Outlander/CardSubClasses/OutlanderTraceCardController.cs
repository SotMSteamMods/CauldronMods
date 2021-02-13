using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public abstract class OutlanderTraceCardController : OutlanderUtilityCardController
    {
        protected OutlanderTraceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => "This card is indestructible.");
        }

        public override bool CanBeDestroyed => false;
    }
}
