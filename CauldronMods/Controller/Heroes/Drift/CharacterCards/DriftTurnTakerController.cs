using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Drift
{
    public class DriftTurnTakerController : HeroTurnTakerController
    {
        public DriftTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public override IEnumerator StartGame()
        {
            //Put Shift Track into play
            Card shiftTrack1 = TurnTaker.FindCard("ShiftTrack1", realCardsOnly: false);
            IEnumerator coroutine = base.GameController.PlayCard(this, shiftTrack1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Put a token in the pool to begin tracking active card
            TokenPool pool1 = shiftTrack1.FindTokenPool("ShiftPool");
            coroutine = base.GameController.AddTokensToPool(pool1, 1, new CardSource(base.CharacterCardController));
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