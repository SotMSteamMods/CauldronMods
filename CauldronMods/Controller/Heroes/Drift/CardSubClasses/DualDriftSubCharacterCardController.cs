using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Drift
{
    public class DualDriftSubCharacterCardController : DriftSubCharacterCardController
    {
        public DualDriftSubCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            IEnumerator coroutine = base.GameController.MoveCard(base.TurnTakerController, base.GetShiftTrack(), base.TurnTaker.OutOfGame, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield return base.AfterFlipCardImmediateResponse();
            yield break;
        }
    }
}