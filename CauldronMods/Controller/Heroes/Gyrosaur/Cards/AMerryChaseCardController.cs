using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class AMerryChaseCardController : GyrosaurUtilityCardController
    {
        public AMerryChaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Whenever a target deals damage to {Gyrosaur}, all other hero targets become immune to damage dealt by that target until this card leaves play.",
            //"At the start of your turn, destroy this card."
        }
    }
}
