using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class EscapeVelocityCardController : CardController
    {
        public EscapeVelocityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"You may destroy 1 ongoing card.",
            //"Select up to 3 non-character targets in play with 2 or fewer HP. Place those targets on the bottom of their associated decks in any order."
            yield break;
        }
    }
}