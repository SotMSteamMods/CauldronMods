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

                Card copperhead = TurnTaker.FindCard("Copperhead");
                Card python = TurnTaker.FindCard("Python");
                CardSource dynamoSource = new CardSource(CharacterCardController);
                IEnumerator copperheadRoutine = GameController.PlayCard(this, copperhead, isPutIntoPlay: true, cardSource: dynamoSource);
                IEnumerator pythonRoutine = GameController.PlayCard(this, python, isPutIntoPlay: true, cardSource: dynamoSource);
                IEnumerator shuffleRoutine = GameController.ShuffleLocation(TurnTaker.Deck);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(copperheadRoutine);
                    yield return base.GameController.StartCoroutine(pythonRoutine);
                    yield return base.GameController.StartCoroutine(shuffleRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(copperheadRoutine);
                    base.GameController.ExhaustCoroutine(pythonRoutine);
                    base.GameController.ExhaustCoroutine(shuffleRoutine);
                }
            }
            yield break;
        }
    }
}
