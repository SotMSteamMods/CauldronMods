using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class WarbrandCardController : OutlanderUtilityCardController
    {
        public WarbrandCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //The first time {Outlander} deals damage each turn, he then deals the hero target with the highest HP 2 projectile damage.
        //At the end of the villain turn, {Outlander} deals the 2 hero targets with the lowest HP 1 projectile damage each.
    }
}
