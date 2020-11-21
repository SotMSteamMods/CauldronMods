using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class ForcefieldNodeCardController : TheRamUtilityCardController
    {
        public ForcefieldNodeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"This card is immune to damage from heroes that are not Up Close.",
            //"Reduce damage dealt to {TheRam} by 2.",
            //"Whenever a copy of Up Close enters play next to a hero, this card deals that hero {H - 2} energy damage."
        }
    }
}