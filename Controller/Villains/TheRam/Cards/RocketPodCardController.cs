using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class RocketPodCardController : TheRamUtilityCardController
    {
        public RocketPodCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {

            //"This card is immune to damage from heroes that are not Up Close.",
            //"At the end of the villain turn, this card deals each hero that is not Up Close {H - 2} cold damage."
        }
    }
}