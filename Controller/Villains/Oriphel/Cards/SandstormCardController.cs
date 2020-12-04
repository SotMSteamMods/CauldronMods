using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class SandstormCardController : OriphelUtilityCardController
    {
        public SandstormCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Shuffle the villain trash and reveal cards until X Goons are revealed, where X is 1 plus the number of environment cards in play.",
            //"Put the revealed Goons into play and discard the other cards."
            yield break;
        }
    }
}