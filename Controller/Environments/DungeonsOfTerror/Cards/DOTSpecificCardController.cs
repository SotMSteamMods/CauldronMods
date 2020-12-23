using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DungeonsOfTerror
{
    public class DungeonsOfTerrorSpecificCardController : DungeonsOfTerrorUtilityCardController
    {
        protected DungeonsOfTerrorSpecificCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
    }
}
