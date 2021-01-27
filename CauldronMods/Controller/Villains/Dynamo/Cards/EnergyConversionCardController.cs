using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class EnergyConversionCardController : DynamoUtilityCardController
    {
        public EnergyConversionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //When this card enters play, discard the top card of the villain deck.
        //When {Dynamo} is dealt 4 or more damage from a single source, discard the top card of the villain deck and {Dynamo} deals each hero target {H} energy damage. Then, destroy this card.
    }
}
