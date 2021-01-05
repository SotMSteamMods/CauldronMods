using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Mythos
{
    public class MythosTurnTakerController : TurnTakerController
    {
        public MythosTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            var a = TurnTaker;
            yield break;
        }
    }
}