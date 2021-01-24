using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class TransdimensionalOnslaughtCardController : OutlanderUtilityCardController
    {
        public TransdimensionalOnslaughtCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //{Outlander} deals each non-villain target X irreducible psychic damage, where X is the number of Trace cards in play.
    }
}
