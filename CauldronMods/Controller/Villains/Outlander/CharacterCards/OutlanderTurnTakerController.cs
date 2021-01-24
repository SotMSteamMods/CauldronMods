using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Outlander
{
    public class OutlanderTurnTakerController : TurnTakerController
    {
        public OutlanderTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            //Search the villain deck for all Trace cards and put them beneath this card. Put 1 random Trace card from beneath this one into play. Shuffle the villain deck.
            yield break;
        }
    }
}