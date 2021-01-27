using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class CopperheadCardController : DynamoUtilityCardController
    {
        public CopperheadCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //If this card has 10 or fewer HP, increase damage it deals by 2.
        //At the end of the villain turn, this card deals the 2 hero targets with the highest HP {H} melee damage each.
    }
}
