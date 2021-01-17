using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class HiddenDetourCardController : GyrosaurUtilityCardController
    {
        public HiddenDetourCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {Gyrosaur} regains 2HP. Then, reveal the top card of the environment deck and place it beneath this card.",
            yield break;
        }

        public override void AddTriggers()
        {
            //"Cards beneath this one are not considered in play. When an environment card would enter play, you may first switch it with the card beneath this one."
        }
    }
}
