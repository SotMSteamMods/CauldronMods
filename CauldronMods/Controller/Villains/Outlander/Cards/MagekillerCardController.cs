using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class MagekillerCardController : OutlanderUtilityCardController
    {
        public MagekillerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //The first time a hero one-shot enters play each turn, {Outlander} deals the hero target with the highest HP 1 irreducible lightning damage.
        //At the end of the villain turn, {Outlander} deals the hero target with the highest HP 3 melee damage.
    }
}
