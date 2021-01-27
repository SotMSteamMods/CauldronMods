using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class CatharticDemolitionCardController : DynamoUtilityCardController
    {
        public CatharticDemolitionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //At the start of the villain turn, destroy all Plot cards and this card.
        //When this card is destroyed, {Dynamo} deals each non-villain target X energy damage, where X is 2 times the number of villain cards destroyed this turn.
    }
}
