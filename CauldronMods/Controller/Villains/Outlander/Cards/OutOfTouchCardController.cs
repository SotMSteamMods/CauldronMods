using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class OutOfTouchCardController : OutlanderUtilityCardController
    {
        public OutOfTouchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //When this card enters play, {Outlander} deals the non-villain target with the highest HP X+3 melee damage, where X is the number of Trace cards in play.
        //Reduce all HP recovery by 1. At the start of the villain turn, destroy this card.
    }
}
