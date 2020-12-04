using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class MirageCardController : OriphelUtilityCardController
    {
        public MirageCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Reveal the top 2 cards of the villain deck. Put any revealed targets or Transformation cards into play and discard the rest.",
            //"Each Goon deals the hero target with the highest HP 1 fire damage."
            yield break;
        }
    }
}