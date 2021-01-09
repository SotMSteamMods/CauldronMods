using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class DayOfSaintsCardController : DayCardController
    {
        public DayOfSaintsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        protected override IEnumerator DayFlipFaceUpEffect()
        {
            //"When this card flips face up, increase damage dealt by villain targets by 2 until the start of the next villain turn.",
            //"Then {TheMistressOfFate} deals the hero with the lowest HP {H} times 3 psychic damage."
            yield break;
        }
    }
}
