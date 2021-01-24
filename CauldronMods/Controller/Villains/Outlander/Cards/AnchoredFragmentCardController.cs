using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class AnchoredFragmentCardController : OutlanderUtilityCardController
    {
        public AnchoredFragmentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //When this card enters play, {Outlander} deals the hero target with the highest HP 1 melee damage.
        //At the start of the villain turn, if {Outlander} was not dealt at least {H} times 2 damage in the last round, destroy {H} hero ongoing and/or equipment cards.
    }
}
