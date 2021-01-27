using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class KineticEnergyBeamCardController : DynamoUtilityCardController
    {
        public KineticEnergyBeamCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //{Dynamo} deals the hero target with the second highest HP {H} energy damage.
        //Increase damage dealt to that target by environment cards by 1 until the start of the next villain turn.
    }
}
