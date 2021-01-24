using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class ArchangelCardController : OutlanderUtilityCardController
    {
        public ArchangelCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //The first time {Outlander} is dealt 4 or more damage from a single source each turn, play the top card of the villain deck.
        //At the end of the villain turn, {Outlander} deals each non-villain target irreducible 1 projectile damage.
    }
}
