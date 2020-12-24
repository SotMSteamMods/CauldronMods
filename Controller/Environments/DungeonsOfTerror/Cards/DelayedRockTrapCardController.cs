using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DungeonsOfTerror
{
    public class DelayedRockTrapCardController : DungeonsOfTerrorUtilityCardController
    {
        public DelayedRockTrapCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
    }
}
