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
            IEnumerator coroutine;
            List<Card> cardsToMove = new List<Card>();

            cardsToMove = base.GameController.TurnTakerControllers.Where(ttc => ttc != base.TurnTakerController && !ttc.IsIncapacitatedOrOutOfGame).Select((ttc) => ttc.TurnTaker.Trash.TopCard).ToList();

            coroutine = base.GameController.BulkMoveCards(base.TurnTakerController, cardsToMove, base.Card.UnderLocation, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.DealDamage(base.Card, (card) => card != base.Card, 1, DamageType.Toxic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.DealDamage(base.Card, (card) => card == base.Card, 1, DamageType.Fire);
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
