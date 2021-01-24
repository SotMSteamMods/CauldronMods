using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class HullCladdingCardController : PyreUtilityCardController
    {
        public HullCladdingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Reduce damage dealt to and by {Pyre} by 1.",
            //"If Containment Breach is ever in play, destroy it or destroy this card."
        }
    }
}
