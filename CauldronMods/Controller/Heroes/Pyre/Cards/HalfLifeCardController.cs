using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class HalfLifeCardController : PyreUtilityCardController
    {
        public HalfLifeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            //"Search your deck for an equipment card and put it into your hand. {PyreIrradiate} that card until it leaves your hand. Shuffle your deck. ",
            //"You may play an equipment card.",
            //"{Pyre} deals each non-hero target 0 energy damage."
            yield break;
        }
    }
}
