using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class DragonbornCardController : OutlanderUtilityCardController
    {
        public DragonbornCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //The first time {Outlander} is dealt damage each turn, he deals the source of that damage 2 fire damage.
        //At the end of the villain turn, {Outlander} deals each non-villain target 1 fire damage.
    }
}
