using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class CellularIrradiationCardController : PyreUtilityCardController
    {
        public CellularIrradiationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Select a hero. They activate each of the following in order if they have at least that many {PyreIrradiate} cards in their hand:",
            //"{1: Use a power.",
            //"{2: Draw a card.",
            //"{3: Deal themselves 2 energy damage.",
            //"{4: Play a card."
            yield break;
        }
    }
}
