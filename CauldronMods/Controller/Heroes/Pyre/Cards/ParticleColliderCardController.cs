using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class ParticleColliderCardController : PyreUtilityCardController
    {
        public ParticleColliderCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"1 player may play a {PyreIrradiate} card now. {Pyre} deals 1 target 1 energy damage. You may destroy a copy of Thermonuclear Core. If you do, increase this damage by 3."
            yield break;
        }
    }
}
