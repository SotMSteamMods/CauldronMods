using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class HelmetedChargeCardController : DynamoUtilityCardController
    {
        public HelmetedChargeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //If Copperhead is in play, he deals each hero target 2 melee damage.
        //Otherwise, seach the villain deck and trash for Copperhead and put him into play. If you searched the villain deck, shuffle it.
    }
}
