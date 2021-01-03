using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.TheInfernalChoir
{
    public class TheInfernalChoirTurnTakerController : TurnTakerController
    {
        public TheInfernalChoirTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            yield break;
        }
    }
}