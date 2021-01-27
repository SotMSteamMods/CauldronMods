using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class HardenedCriminalsCardController : DynamoUtilityCardController
    {
        public HardenedCriminalsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //Reduce damage dealt to villain targets by 1.
    }
}
