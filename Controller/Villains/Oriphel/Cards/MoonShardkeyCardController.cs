using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class MoonShardkeyCardController : OriphelShardkeyCardController
    {
        public MoonShardkeyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Shardkey transformation trigger
            base.AddTriggers();

            //"At the end of the villain turn, the villain target with the highest HP deals the hero target with the highest HP 2 energy damage."

        }
    }
}