using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class PowerNodeCardController : TheRamUtilityCardController
    {
        public PowerNodeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"This card is immune to damage from heroes that are not Up Close.",
            //"At the end of the villain turn, play the top card of the villain deck, and all Devices and Nodes regain 1HP."
        }

    }
}