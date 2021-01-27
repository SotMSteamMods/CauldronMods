using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class HeresThePlanCardController : DynamoUtilityCardController
    {
        public HeresThePlanCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //Reveal cards from the top of the villain deck until a Plot is revealed. Put it into play and shuffle the other revealed cards back into the villain deck.
        //The villain target with the highest HP deals each hero target {H - 1} melee damage.
    }
}
