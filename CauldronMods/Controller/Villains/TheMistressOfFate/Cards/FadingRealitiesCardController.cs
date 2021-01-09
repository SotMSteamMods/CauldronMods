using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class FadingRealitiesCardController : TheMistressOfFateUtilityCardController
    {
        public FadingRealitiesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"The hero with the second lowest HP may select 1 card in their hand and 1 of their cards in play. Remove the selected cards from the game.",
            //"If 2 cards were not removed from the game this way, {TheMistressOfFate} deals that hero 20 infernal damage."
            yield break;
        }
    }
}
