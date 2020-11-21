using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheRam
{
    public class TheRamCharacterCardController : TheRamUtilityCharacterCardController
    {
        public TheRamCharacterCardController(Card card, TurnTakerController ttc) : base(card, ttc)
        {
        }
    }
}
