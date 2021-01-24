using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class KnightsHatredCardController : OutlanderUtilityCardController
    {
        public KnightsHatredCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //Increase damage dealt by {Outlander} by 1.
        //Reduce damage dealt to {Outlander} by 1.
        //At the start of the villain turn, destroy this card.
    }
}
