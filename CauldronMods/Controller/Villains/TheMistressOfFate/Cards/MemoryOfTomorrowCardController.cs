using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class MemoryOfTomorrowCardController : TheMistressOfFateUtilityCardController
    {
        public MemoryOfTomorrowCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _isStoredCard = false;
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, move it next to the right-most face up Day card and {TheMistressOfFate} deals each hero 5 sonic damage and 5 cold damage.",
            yield break;
        }

        public override void AddTriggers()
        {
            //"Each time the Day card next to this one is flipped face up, 1 hero may draw a card and play a card."
        }
    }
}
