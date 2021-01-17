using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class OmnivoreCardController : GyrosaurUtilityCardController
    {
        public OmnivoreCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Destroy a target with 3 or fewer HP.",
            //"{Gyrosaur} regains X HP, where X is the HP of that target before it was destroyed.",
            //"You may shuffle your trash into your deck."
            yield break;
        }
    }
}
