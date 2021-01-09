using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class CantFightFateCardController : TheMistressOfFateUtilityCardController
    {
        public CantFightFateCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"The hero with the highest HP may discard 3 cards that share a keyword.",
            //"If they do not, {TheMistressOfFate} deals each target in that hero's play area 20 psychic damage."
            yield break;
        }
    }
}
