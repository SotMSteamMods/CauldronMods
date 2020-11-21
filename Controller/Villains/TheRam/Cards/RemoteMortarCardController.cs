using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class RemoteMortarCardController : TheRamUtilityCardController
    {
        public RemoteMortarCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {

            //"This card is immune to damage from heroes that are Up Close.",
            //"At the end of the villain turn, this card deals each Up Close hero {H - 1} energy damage and those heroes must each discard a card."
        }
    }
}