using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class WantonDestructionCardController : DynamoUtilityCardController
    {
        public WantonDestructionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //At the end of the villain turn, each player may destroy 1 of their non-character cards. If fewer than {H - 1} cards were destroyed this way, discard the top card of the villain deck.
    }
}
