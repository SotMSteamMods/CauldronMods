using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Menagerie
{
    public class MenagerieTurnTakerController : TurnTakerController
    {
        public MenagerieTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            //Search the villain deck for the card Prized Catch and put it into play.
            IEnumerator coroutine = base.GameController.PlayCard(this, base.GameController.FindCard("PrizedCatch"), isPutIntoPlay: true, cardSource: CharacterCardController.GetCardSource());
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