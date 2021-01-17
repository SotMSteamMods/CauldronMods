using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class ProtectiveEscortCardController : GyrosaurUtilityCardController
    {
        public ProtectiveEscortCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Draw 2 cards.",
            //"Select a target and a damage type. That target is immune to that damage type until the start of your next turn."
            yield break;
        }
    }
}
