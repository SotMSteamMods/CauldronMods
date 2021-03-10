using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{

    public class DynamoTurnTakerController : TurnTakerController
    {
        public DynamoTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {
        }

        public override IEnumerator StartGame()
        {

            if(GameController.Game.IsChallenge)
            {
                //At the start of the game, put Python and Copperhead into play.

                IEnumerator copperhead = PutCardIntoPlay("Copperhead", shuffleDeckAfter: false);
                IEnumerator python = PutCardIntoPlay("Python");
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(copperhead);
                    yield return base.GameController.StartCoroutine(python);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(copperhead);
                    base.GameController.ExhaustCoroutine(python);
                }
            }
            yield break;
        }
    }
}
