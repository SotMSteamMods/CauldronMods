using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class HourDevourerCardController : TheMistressOfFateUtilityCardController
    {
        public HourDevourerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"At the end of the villain turn, this card deals each non-villain target X sonic damage, where X is 3 times the number of Day cards face up.",
            //"When damage dealt by a target destroys this card, that target becomes immune to damage until the start of its next turn."
        }
    }
}
