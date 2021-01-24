using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class IonTraceCardController : PyreUtilityCardController
    {
        public IonTraceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Two players may each select a non-{PyreIrradiate} card in their hand and move a card that shares a keyword with it from their trash to their hand. {PyreIrradiate} any cards selected or moved this way until they leave their hands.",
            //"{Pyre} deals each non-hero target 0 energy damage."
            yield break;
        }
    }
}
