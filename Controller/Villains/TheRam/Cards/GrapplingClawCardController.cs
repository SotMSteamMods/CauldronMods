using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class GrapplingClawCardController : TheRamUtilityCardController
    {
        public GrapplingClawCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Increase damage dealt by {TheRam} by 1.",
            //"At the end of the villain turn, {TheRam} deals {H - 2} projectile damage to the hero with the lowest HP that is not Up Close. Put a copy of Up Close from the villain trash into play next to that hero."
        }
    }
}