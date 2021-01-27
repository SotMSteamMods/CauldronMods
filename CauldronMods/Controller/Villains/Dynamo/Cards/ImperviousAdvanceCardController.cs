using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class ImperviousAdvanceCardController : DynamoUtilityCardController
    {
        public ImperviousAdvanceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //If Copperhead is in play, reduce damage dealt to villain targets by 1 until the start of the next villain turn.
        //The villain target with the highest HP deals the hero target with the second highest HP {H} melee damage.
    }
}
