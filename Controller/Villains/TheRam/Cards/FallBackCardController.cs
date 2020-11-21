using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class FallBackCardController : TheRamUtilityCardController
    {
        public FallBackCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {TheRam} deals each Up Close hero target {H - 1} melee damage. Destroy all copies of Up Close in play.",
            yield break;
        }

        public override void AddTriggers()
        {
            //"{TheRam} is immune to damage from heroes that are not Up Close."
        }
    }
}