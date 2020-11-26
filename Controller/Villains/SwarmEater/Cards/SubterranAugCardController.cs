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
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.PlayVillainTargetResponse), TriggerType.PlayCard);
        }

        private IEnumerator PlayVillainTargetResponse(PhaseChangeAction p)
        {
            IEnumerable<Card> source = base.FindCardsWhere((Card c) => c.IsVillainTarget && c.Location.IsVillain && c.Location.IsTrash);
            if (source.Count<Card>() == 1)
            {
                string message = string.Format("{0} moves {1} from the villain trash into play.", base.Card.Title, source.First<Card>().Title);
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
            Location trash = base.TurnTaker.Trash;
            GameController gameController = base.GameController;
            HeroTurnTakerController decisionMaker = this.DecisionMaker;
            Func<Card, bool> criteria = (Card c) => c.IsVillainTarget && c.Location == trash;
            bool optional = false;
            bool isPutIntoPlay = true;
            string noValidCardsMessage = "There are no villain targets in " + trash.GetFriendlyName() + " to put into play.";
            IEnumerator coroutine2 = null;
            Random rng = Game.RNG;
            IEnumerable<Card> trashTargets = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsVillainTarget && c.IsInTrash));
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
            yield break;
        }

        public override IEnumerator AddAbsorbTriggers(Card cardThisIsUnder)
        {
            //Absorb: The first time {SwarmEater} would be dealt damage each turn, reduce that damage by 1.
            this._reduceDamage = base.AddReduceDamageTrigger((DealDamageAction action) => !base.HasBeenSetToTrueThisTurn("FirstTimeDamageDealt"), new Func<DealDamageAction, IEnumerator>(this.ReduceDamageResponse), (Card c) => c == cardThisIsUnder, true);
            yield break;
        }

        private IEnumerator ReduceDamageResponse(DealDamageAction action)
        {
            base.SetCardPropertyToTrueIfRealAction("FirstTimeDamageDealt");
            IEnumerator coroutine = base.GameController.ReduceDamage(action, 1, this._reduceDamage, base.GetCardSource());
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