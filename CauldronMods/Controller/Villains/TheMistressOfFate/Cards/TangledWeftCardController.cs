using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class TangledWeftCardController : TheMistressOfFateUtilityCardController
    {
        public TangledWeftCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Each player may discard a card that shares a keyword with at least one of their cards in play.",
            //"Then, {TheMistressOfFate} deals each hero that did not discard a card this way 5 infernal damage for each face up Day card."
            yield break;
        }
    }
}
