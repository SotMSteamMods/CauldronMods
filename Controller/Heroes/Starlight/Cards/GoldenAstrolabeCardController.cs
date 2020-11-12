using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class GoldenAstrolabeCardController : CardController
    {
        public GoldenAstrolabeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Starlight deals herself 2 energy damage. One hero character next to a constellation may use a power now."
            yield break;
        }

    }
}