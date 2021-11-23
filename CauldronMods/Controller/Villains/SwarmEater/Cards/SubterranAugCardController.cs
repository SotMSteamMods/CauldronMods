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
            var ss = SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria(c => IsVillainTarget(c), "villain target"));
            ss.Condition = () => base.Card.IsInPlayAndNotUnderCard;
        }

        private ITrigger _reduceDamage;
        private const string FirstTimeDamageDealt = "FirstTimeDamageDealt";

        public override void AddTriggers()
        {
            //At the start of the villain turn, put a random target from the villain trash into play.
            base.AddStartOfTurnTrigger((TurnTaker tt) => base.Card.IsInPlayAndNotUnderCard && tt == base.TurnTaker, this.PlayVillainTargetResponse, TriggerType.PlayCard);
        }
        public override void AddAbsorbTriggers(Card absorbingCard)
        {
            //Absorb: The first time {SwarmEater} would be dealt damage each turn, reduce that damage by 1.
            this._reduceDamage = base.AddReduceDamageTrigger((DealDamageAction action) => action.Amount > 0 && CanAbsorbEffectTrigger() && !base.HasBeenSetToTrueThisTurn(GeneratePerTargetKey(FirstTimeDamageDealt, absorbingCard)), dda => this.ReduceDamageResponse(dda, absorbingCard), (Card c) => c == absorbingCard, true);

            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagsAfterLeavesPlay(FirstTimeDamageDealt), TriggerType.Hidden);
        }

        private IEnumerator PlayVillainTargetResponse(PhaseChangeAction p)
        {
            IEnumerable<Card> source = base.FindCardsWhere((Card c) => IsVillainTarget(c) && c.Location.IsVillain && c.Location.IsTrash);
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
            IEnumerator coroutine2 = null;
            Random rng = Game.RNG;
            IEnumerable<Card> trashTargets = base.FindCardsWhere(new LinqCardCriteria(c => IsVillainTarget(c) && c.IsInTrash, "villain targets", false));
            if (trashTargets.Count() > 0)
            {
                Card cardToPlay = trashTargets.ElementAt(rng.Next(0, trashTargets.Count()));
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

        private IEnumerator ReduceDamageResponse(DealDamageAction action, Card absorbingCard)
        {
            base.SetCardPropertyToTrueIfRealAction(GeneratePerTargetKey(FirstTimeDamageDealt, absorbingCard));
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