using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class GammaBurstCardController : PyreUtilityCardController
    {
        public GammaBurstCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            //"Select up to 2 non-{PyreIrradiate} cards in 1 player's hand. {PyreIrradiate} those cards until they leave that player's hand.",
            //"{Pyre} deals that hero and each non-hero target X energy damage, where X is the number of cards {PyreIrradiate} this way."
            yield break;
        }
    }
}
