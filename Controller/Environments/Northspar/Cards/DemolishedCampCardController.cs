using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Northspar
{
    public class DemolishedCampCardController : ThirdWaypointCardController
    {

        public DemolishedCampCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "LandingSite")
        {

        }

        public override void AddTriggers()
        {
            // Increase damage dealt to Tak Ahab by 1
            Func<DealDamageAction, bool> increaseCriteria = (DealDamageAction dd) => dd.Target != null && dd.Target.Identifier == TakAhabIdentifier;
            base.AddIncreaseDamageTrigger(increaseCriteria, (DealDamageAction dd) => 1);

            // If Tak Ahab is destroyed, remove him from the game.
            Func<DestroyCardAction, bool> destroyCriteria = (DestroyCardAction dca) => dca.CardToDestroy != null && dca.CardToDestroy.Card.Identifier == TakAhabIdentifier;
            base.AddTrigger<DestroyCardAction>(destroyCriteria, this.RemoveFromGameResponse, TriggerType.RemoveFromGame, TriggerTiming.Before);

            // add all ThirdWaypoint triggers
            base.AddTriggers();
        }

        private IEnumerator RemoveFromGameResponse(DestroyCardAction dca)
        {
            //grab the card to move
            Card takAhab = dca.CardToDestroy.Card;

            //cancel destruction
            IEnumerator coroutine = base.CancelAction(dca);
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
            coroutine = base.GameController.SendMessageAction(base.Card.Title + " removes " + takAhab.Title + " from the game!", Priority.Medium, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //do the move
            coroutine = base.GameController.MoveCard(base.TurnTakerController, takAhab, base.TurnTaker.OutOfGame, cardSource: base.GetCardSource());
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