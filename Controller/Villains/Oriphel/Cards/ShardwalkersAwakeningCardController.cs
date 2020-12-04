using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class ShardwalkersAwakeningCardController : OriphelUtilityCardController
    {
        public ShardwalkersAwakeningCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"If {Oriphel} is in play, he deals each hero target 1 infernal and 1 projectile damage.",
            //"If Jade is in play, flip her villain character cards."
            yield break;
        }
    }
}