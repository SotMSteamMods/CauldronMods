using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class CherenkovDriveCardController : PyreUtilityCardController
    {
        public CherenkovDriveCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"At the end of your turn, 1 player may select 1 non-{PyreIrradiate} card in their hand. {PyreIrradiate} that card it until it leaves their hand. Then, they may use a power on that card.",
            //"If that power destroys that card, discard it instead."
        }
    }
}
