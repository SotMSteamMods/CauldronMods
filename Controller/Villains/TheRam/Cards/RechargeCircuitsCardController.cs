using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class RechargeCircuitsCardController : TheRamUtilityCardController
    {
        public RechargeCircuitsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {    
            //"body": "At the end of the villain turn, if no heroes are Up Close, {TheRam} regains 10HP and all Devices and Nodes regain 2HP.",
        }

    }
}