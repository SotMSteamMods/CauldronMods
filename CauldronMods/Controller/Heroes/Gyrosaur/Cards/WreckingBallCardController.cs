using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class WreckingBallCardController : GyrosaurUtilityCardController
    {
        public WreckingBallCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {Gyrosaur} deals each target 1 melee damage.",
            yield break;
        }
        public override void AddTriggers()
        {
            //"Increase damage dealt to environment targets by 1."
        }
    }
}
