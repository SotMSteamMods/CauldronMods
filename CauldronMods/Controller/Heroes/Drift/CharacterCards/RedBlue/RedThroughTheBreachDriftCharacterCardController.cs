using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class RedThroughTheBreachDriftCharacterCardController : ThroughTheBreachDriftCharacterCardController
    {
        public RedThroughTheBreachDriftCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            
        }

        protected override bool shouldRunSetUp => false;

    }
}
