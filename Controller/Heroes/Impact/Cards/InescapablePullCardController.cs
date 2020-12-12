using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class InescapablePullCardController : CardController
    {
        public InescapablePullCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, draw a card.",
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Select a target that {Impact} damaged this turn. {Impact} deals that target 4 infernal damage."
            yield break;
        }
    }
}