using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class TakeItOutsideCardController : DynamoUtilityCardController
    {
        public TakeItOutsideCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //{Dynamo} deals the hero target with the highest HP 5 energy damage. If a hero target takes damage this way, destroy 1 environment card.
        //{Dynamo} deals each other hero target 1 sonic damage.
    }
}
