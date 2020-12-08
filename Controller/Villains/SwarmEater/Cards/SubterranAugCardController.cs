using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.SwarmEater
{
    public class SubterranAugCardController : AugCardController
    {
        public SubterranAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        private ITrigger _reduceDamage;
        private const string FirstTimeDamageDealt = "FirstTimeDamageDealt";

        public override void AddTriggers()
        {
            //At the start of the villain turn, put a random target from the villain trash into play.
            base.AddStartOfTurnTrigger((TurnTaker tt) => base.Card.IsInPlayAndNotUnderCard && tt == base.TurnTaker, this.PlayVillainTargetResponse, TriggerType.PlayCard);

            //Absorb: The first time {SwarmEater} would be dealt damage each turn, reduce that damage by 1.
            this._reduceDamage = base.AddReduceDamageTrigger((DealDamageAction action) => action.Amount > 0 && base.Card.Location.IsUnderCard && !base.HasBeenSetToTrueThisTurn(FirstTimeDamageDealt), this.ReduceDamageResponse, (Card c) => c == this.CardThatAbsorbedThis(), true);
        }

        private IEnumerator PlayVillainTargetResponse(PhaseChangeAction p)
        {
            IEnumerable<Card> source = base.FindCardsWhere((Card c) => c.IsVillainTarget && c.Location.IsVillain && c.Location.IsTrash);
            if (source.Count<Card>() == 1)
            {
                string message = $"{base.Card.Title} moves {source.First<Card>().Title} from the villain trash into play.";
                IEnumerator coroutine = base.GameController.SendMessageAction(message, Priority.Low, base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            GameController gameController = base.GameController;
            bool optional = false;
            IEnumerator coroutine2 = null;
            Random rng = Game.RNG;
            IEnumerable<Card> trashTargets = base.FindCardsWhere(new LinqCardCriteria(c => c.IsVillainTarget && c.IsInTrash));
            Card cardToPlay = trashTargets.ElementAt(rng.Next(0, trashTargets.Count()));
            if (cardToPlay != null)
            {
                coroutine2 = gameController.PlayCard(base.TurnTakerController, cardToPlay, true, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }
            else
            {
                coroutine2 = base.GameController.SendMessageAction("There are no villain targets in any villain trash to put into play.", Priority.Low, base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }
        }

        private IEnumerator ReduceDamageResponse(DealDamageAction action)
        {
            base.SetCardPropertyToTrueIfRealAction(FirstTimeDamageDealt);
            IEnumerator coroutine = base.GameController.ReduceDamage(action, 1, this._reduceDamage, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}