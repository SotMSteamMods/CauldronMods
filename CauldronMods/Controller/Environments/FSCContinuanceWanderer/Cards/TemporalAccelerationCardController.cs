using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.FSCContinuanceWanderer
{
    public class TemporalAccelerationCardController : CardController
    {

        public TemporalAccelerationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
     
        public override IEnumerator Play()
        {
            //When this card enters play, play the top card of the villain deck. Then, play the top card of each hero deck in turn order.
            IEnumerator coroutine = base.GameController.SendMessageAction("Temporal Acceleration puts the top card of the villain deck, and then the top card of each hero deck into play in turn order.", Priority.Low, base.GetCardSource());
            IEnumerator e2 = base.PlayTopCardOfEachDeckInTurnOrder((TurnTakerController ttc) => ttc.IsVillain && !ttc.TurnTaker.IsScion, (Location location) => location.IsVillain, responsibleTurnTaker: base.TurnTaker);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(e2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(e2);
            }
            IEnumerator coroutine2 = base.PlayTopCardOfEachDeckInTurnOrder((TurnTakerController ttc) => IsHero(ttc.TurnTaker) && GameController.IsTurnTakerVisibleToCardSource(ttc.TurnTaker, GetCardSource()), (Location location) => location.IsHero, responsibleTurnTaker: base.TurnTaker);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, destroy this card.
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);
        }
    }
}