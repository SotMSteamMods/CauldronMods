using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class MisdirectionCardController : CardController
    {
        public MisdirectionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"{MagnificentMara} deals 1 target 1 sonic damage.",
            //"One other hero target deals that same target 2 damage of a type of their choosing."
            yield break;
        }
    }
}