using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class SeeThePatternCardController : TheMistressOfFateUtilityCardController
    {
        public SeeThePatternCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _isStoredCard = false;
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {TheMistressOfFate} deals each hero {H} psychic damage and {H} melee damage.",
            yield break;
        }

        public override void AddTriggers()
        {
            //"When {TheMistressOfFate} flips, each player may move 1 card from their trash to their hand. If any cards were moved this way, destroy this card."
        }
    }
}
