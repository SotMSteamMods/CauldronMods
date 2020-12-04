using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class SunShardkeyCardController : OriphelShardkeyCardController
    {
        public SunShardkeyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Shardkey transformation trigger
            base.AddTriggers();

            //"Whenever a hero uses a power, that hero deals themselves 2 psychic damage."
        }
    }
}