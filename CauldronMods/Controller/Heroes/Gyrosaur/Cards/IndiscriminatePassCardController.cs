using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class IndiscriminatePassCardController : GyrosaurUtilityCardController
    {
        public IndiscriminatePassCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"If you have at least 1 Crash card in your hand, {Gyrosaur} deals another hero target 2 melee damage.",
            //"{Gyrosaur} deals 1 non-hero target 4 melee damage."
            yield break;
        }
    }
}
