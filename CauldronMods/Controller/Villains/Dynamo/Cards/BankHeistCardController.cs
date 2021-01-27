using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class BankHeistCardController : DynamoUtilityCardController
    {
        public BankHeistCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //At the end of the villain turn, each player may discard 1 card. If fewer than {H - 1} cards were discarded this way, discard the top card of the villain deck.
    }
}
