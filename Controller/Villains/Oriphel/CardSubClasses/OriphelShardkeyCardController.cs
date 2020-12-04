using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class OriphelShardkeyCardController : OriphelUtilityCardController
    {
        public OriphelShardkeyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"At the start of the villain turn, reveal the top card of the villain deck. If a Transformation card is revealed, play it, 
            //otherwise put it on the bottom of the villain deck.",
        }
    }
}