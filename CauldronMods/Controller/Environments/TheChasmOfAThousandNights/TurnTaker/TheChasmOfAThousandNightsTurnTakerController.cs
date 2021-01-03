using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.Achievements;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class TheChasmOfAThousandNightsTurnTakerController : TurnTakerController
    {
        public TheChasmOfAThousandNightsTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {
        }

        public override IEnumerator StartGame()
        {
            yield break;
        }
    }
}
