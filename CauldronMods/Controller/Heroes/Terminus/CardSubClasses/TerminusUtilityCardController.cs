using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public abstract class TerminusUtilityCardController : CardController
    {
        protected TerminusUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
    }
}
