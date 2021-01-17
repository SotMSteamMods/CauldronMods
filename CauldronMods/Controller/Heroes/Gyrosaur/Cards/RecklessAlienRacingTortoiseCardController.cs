using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class RecklessAlienRacingTortoiseCardController : GyrosaurUtilityCardController
    {
        public RecklessAlienRacingTortoiseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"During your turn, the first time you have more than 3 Crash cards in your hand, immediately use this card's power and then destroy it.",
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{Gyrosaur} deals 1 target X+1 melee damage, where X is the number of Crash cards in your hand."
            yield break;
        }
    }
}
