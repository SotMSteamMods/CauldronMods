using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class OutlanderCharacterCardController : CardController
    {
        public OutlanderCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
    }
}
