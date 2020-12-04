using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class ScrollsOfZephaerenCardController : OriphelUtilityCardController
    {
        public ScrollsOfZephaerenCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Shuffle all Guardians from the villain trash into the villain deck.",
            //"Play the top card of the villain deck.",
            //"If {Oriphel} is in play, he deals the hero target with the second lowest HP 3 melee damage."
            yield break;
        }
    }
}