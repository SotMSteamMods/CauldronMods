using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class NecessaryCorrectionCardController : TheMistressOfFateUtilityCardController
    {
        public NecessaryCorrectionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _isStoredCard = false;
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {TheMistressOfFate} deals the {H - 1} hero targets with the highest HP 10 psychic damage each.",
            yield break;
        }

        public override void AddTriggers()
        {
            //"At the end of the environment turn, if there are at least 5 cards remaining in the villain deck, destroy this card and flip {TheMistressOfFate}'s villain character cards."
        }
    }
}
