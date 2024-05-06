using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class MetalScavengerCardController : OblaskCraterUtilityCardController
    {
        /* 
         * At the end of the environment turn, move the top card of each other trash pile beneath this card.
         * Then, this card deals each other target 1 toxic damage and itself 1 fire damage.
         * Cards beneath this one are not considered in play.
         */
        public MetalScavengerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            // Cards beneath this one are not considered in play.
            base.Card.UnderLocation.OverrideIsInPlay = false;
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((tt) => tt.IsEnvironment, PhaseChangeActionResponse, new TriggerType[] { TriggerType.MoveCard, TriggerType.DealDamage });
        }

        private IEnumerator PhaseChangeActionResponse(PhaseChangeAction phaseChangeAction)
        {
            IEnumerator messageRoutine;
            IEnumerator moveRoutine;
            IEnumerator damageRoutine;
            List<Card> cardsToMove = new List<Card>();



            string message = $"{Card.Title} moves the top card of each other trash pile beneath this card!";

            cardsToMove = base.GameController.FindLocationsWhere((Location L) => L.IsTrash && L != this.TurnTaker.Trash && L.HasCards && !L.OwnerTurnTaker.IsIncapacitatedOrOutOfGame && base.GameController.IsLocationVisibleToSource(L, GetCardSource())).Select((Location L) => L.TopCard).ToList();
            moveRoutine = base.GameController.BulkMoveCards(base.TurnTakerController, cardsToMove, base.Card.UnderLocation, cardSource: base.GetCardSource());
            if (!cardsToMove.Any())
            {
                moveRoutine = DoNothing();
                message = $"There are no cards in any trashes for {Card.Title} to move!";
            }
            messageRoutine = base.GameController.SendMessageAction(message, Priority.Medium, GetCardSource(), showCardSource: true);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(messageRoutine);
                yield return base.GameController.StartCoroutine(moveRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(messageRoutine);
                base.GameController.ExhaustCoroutine(moveRoutine);
            }

            damageRoutine = base.DealDamage(base.Card, (card) => card != base.Card, 1, DamageType.Toxic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageRoutine);
            }

            damageRoutine = base.DealDamage(base.Card, (card) => card == base.Card, 1, DamageType.Fire);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(damageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(damageRoutine);
            }

            yield break;
        }
    }
}
