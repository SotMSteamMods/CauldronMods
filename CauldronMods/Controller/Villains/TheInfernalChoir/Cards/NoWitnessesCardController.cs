using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class NoWitnessesCardController : TheInfernalChoirUtilityCardController
    {
        public NoWitnessesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
    }
}
