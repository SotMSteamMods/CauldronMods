using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.ScreaMachine
{
    public class ScreaMachineTurnTakerController : TurnTakerController
    {
        public ScreaMachineTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            yield break;
        }
    }
}