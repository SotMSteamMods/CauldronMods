using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class CrusaderCardController : OutlanderUtilityCardController
    {
        public CrusaderCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //Increase damage dealt by {Outlander} by 1.
        //At the end of the villain turn, {Outlander} deals the 2 non-villain targets with the highest HP 2 irreducible melee damage each.
    }
}
