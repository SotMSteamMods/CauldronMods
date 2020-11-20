using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Northspar
{
    public class ThirdWaypointCardController : NorthsparCardController
    {

        public ThirdWaypointCardController(Card card, TurnTakerController turnTakerController, string cardToRemoveIdentifier) : base(card, turnTakerController)
        {
            this.CardToRemoveIdentifier = cardToRemoveIdentifier;
        }

        public override IEnumerator Play()
        {
            // When this card enters play, play the top card of the environment deck...
            IEnumerator coroutine = base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

           // ...and destroy this card
           coroutine = base.GameController.DestroyCard(this.DecisionMaker, base.Card, cardSource: base.GetCardSource());
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

        public override void AddTriggers()
        {
            //If the other Third Waypoint would enter play, instead remove it from the game.
            Func<CardEntersPlayAction, bool> criteria = (CardEntersPlayAction cp) => cp.CardEnteringPlay != null && cp.CardEnteringPlay.Identifier == CardToRemoveIdentifier;
            base.AddTrigger<CardEntersPlayAction>(criteria , this.RemoveOtherThirdWaypointResponse, TriggerType.RemoveFromGame, TriggerTiming.Before);
        }

        private IEnumerator RemoveOtherThirdWaypointResponse(CardEntersPlayAction cp)
        {
            //grab the card to move
            Card otherThirdWaypoint = cp.CardEnteringPlay;

            //cancel entering play
            IEnumerator coroutine = base.CancelAction(cp);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //move it out of the game

            //send the message
            coroutine = base.GameController.SendMessageAction(base.Card.Title + " removes " + otherThirdWaypoint.Title + " from the game!", Priority.Medium, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //do the move
            coroutine = base.GameController.MoveCard(base.TurnTakerController, otherThirdWaypoint, base.TurnTaker.OutOfGame,evenIfIndestructible: true, cardSource: base.GetCardSource());
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

        protected string CardToRemoveIdentifier { get; private set; }
    }
}