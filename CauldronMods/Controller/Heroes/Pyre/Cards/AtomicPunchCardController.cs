using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class AtomicPunchCardController : PyreUtilityCardController
    {
        public AtomicPunchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Select 2 non-{PyreIrradiate} cards in 1 player's hand. {PyreIrradiate} those cards until they leave that player's hand. If 2 of your cards were {PyreIrradiate} this way, increase energy damage dealt by {Pyre} by 1 until the end of your turn.",
            //"{Pyre} deals 1 target 2 energy damage."
            yield break;
        }
    }
}
