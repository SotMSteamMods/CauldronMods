using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class RiftbladeStrikesCardController : OutlanderUtilityCardController
    {
        public RiftbladeStrikesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //{Outlander} deals the non-villain target with the second highest HP 2 fire damage.
        //{Outlander} deals the non-villain target with the highest HP 4 melee damage.
    }
}
