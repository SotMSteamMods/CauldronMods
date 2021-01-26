using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class ContainmentBreachCardController : PyreUtilityCardController
    {
        public ContainmentBreachCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override void AddTriggers()
        {
            //"Whenever a player plays a {PyreIrradiate} card, increase energy damage dealt by {Pyre} by 1 until the end of your turn. Then shuffle a Cascade card from your trash into your deck.",

        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"{Pyre} deals himself and each non-hero target 1 energy damage."
            yield break;
        }
    }
}
