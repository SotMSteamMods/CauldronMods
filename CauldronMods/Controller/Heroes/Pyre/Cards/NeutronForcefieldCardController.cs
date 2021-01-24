using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class NeutronForcefieldCardController : PyreUtilityCardController
    {
        public NeutronForcefieldCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"If this card is {PyreIrradiate} when you play it, it becomes indestructible until the end of your turn.",
            yield break;
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"Select a hero target. Until the start of your next turn, that target is immune to damage. Destroy this card."
            yield break;
        }
    }
}
