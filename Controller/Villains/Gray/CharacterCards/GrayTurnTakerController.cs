using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron
{
    public class GrayTurnTakerController : TurnTakerController
    {
        public GrayTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            //Search the villain deck for 1 copy of Chain Reaction and put it into play.
            IEnumerator coroutine = base.GameController.PlayCard(this, base.TurnTaker.GetCardByIdentifier("ChainReaction"), cardSource: new CardSource(base.CharacterCardController));
            //Shuffle the villain deck.
            IEnumerator coroutine2 = base.GameController.ShuffleLocation(base.TurnTaker.Deck, cardSource: new CardSource(base.CharacterCardController));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            coroutine2 = null;
            yield break;
        }
    }
}