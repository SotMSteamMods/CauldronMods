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
    }
}
