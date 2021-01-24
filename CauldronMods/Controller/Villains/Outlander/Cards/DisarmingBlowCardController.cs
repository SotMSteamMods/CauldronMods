using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class DisarmingBlowCardController : OutlanderUtilityCardController
    {
        public DisarmingBlowCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //{Outlander} deals the 2 non-villain targets with the highest HP 3 melee damage each.
        //Any hero damaged this way discards 1 card.
    }
}
