using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class CrimeSpreeCardController : DynamoUtilityCardController
    {
        public CrimeSpreeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //At the end of the villain turn, the players may choose to play the top card of the environment deck. If they do not, discard the top card of the villain deck and {Dynamo} deals each hero target 1 energy damage.
    }
}
