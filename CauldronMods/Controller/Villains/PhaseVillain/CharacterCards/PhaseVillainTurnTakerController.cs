using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.PhaseVillain
{
    public class PhaseVillainTurnTakerController : TurnTakerController
    {
        public PhaseVillainTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            //Search the villain deck for the card Reinforced Wall and put it into play.
            IEnumerator coroutine = base.GameController.PlayCard(this, base.GameController.FindCard("ReinforcedWall"), isPutIntoPlay: true, cardSource: CharacterCardController.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Shuffle the villain deck.
            coroutine = base.GameController.ShuffleLocation(base.TurnTaker.Deck, cardSource: CharacterCardController.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}