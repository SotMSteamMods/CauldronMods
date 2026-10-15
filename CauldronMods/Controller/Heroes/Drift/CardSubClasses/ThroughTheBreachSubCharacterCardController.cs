using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class ThroughTheBreachSubCharacterCardController : DriftSubCharacterCardController
    {
        public ThroughTheBreachSubCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        { }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            IEnumerator coroutine = base.GameController.MoveCard(base.TurnTakerController, this.GetShiftTrack(), base.TurnTaker.OutOfGame, cardSource: base.GetCardSource());
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
