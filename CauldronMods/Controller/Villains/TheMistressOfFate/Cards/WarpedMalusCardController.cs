using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class WarpedMalusCardController : TheMistressOfFateUtilityCardController
    {
        public WarpedMalusCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Reduce damage dealt to this card by {H - 2}.",
            //"At the end of the villain turn, this card deals each non-villain target {H + 1} energy damage and regains 4 HP."
        }
    }
}
