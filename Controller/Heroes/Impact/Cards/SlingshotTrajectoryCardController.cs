using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class SlingshotTrajectoryCardController : CardController
    {
        public SlingshotTrajectoryCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {Impact} deals 1 target 2 infernal damage.",
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Destroy any number of your ongoing cards. {Impact} deals X targets 2 projectile damage each, where X is 1 plus the number of ongoing cards destroyed this way."
            yield break;
        }
    }
}