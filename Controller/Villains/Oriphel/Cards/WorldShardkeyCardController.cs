using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class WorldShardkeyCardController : OriphelShardkeyCardController
    {
        public WorldShardkeyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Shardkey Transformation trigger
            base.AddTriggers();

            //"At the end of the villain turn, the villain target with the highest HP deals the hero target with the lowest HP 2 melee damage."

        }
    }
}