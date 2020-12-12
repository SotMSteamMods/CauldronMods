using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class CrushingRiftCardController : CardController
    {
        public CrushingRiftCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Select a non-character target in play with current HP less than its maximum HP.",
            //"Reduce that target to half of its current HP, rounded up."
            yield break;
        }
    }
}