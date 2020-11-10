using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Cauldron.Malichae
{
    public abstract class DjinnOngoingController : MalichaeCardController
    {
        private readonly string _requiredIdentifier;
        private readonly string _attachIdentifier;

        protected DjinnOngoingController(Card card, TurnTakerController turnTakerController, string requiredIdentifier, string attachIdentifier)
            : base(card, turnTakerController)
        {
            _requiredIdentifier = requiredIdentifier;
            _attachIdentifier = attachIdentifier;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Place this card next to a target
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText && c.Identifier == _attachIdentifier, _attachIdentifier), storedResults, isPutIntoPlay, decisionSources);
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
            AddTrigger<MoveCardAction>(MoveCriteria, DestroyCardReponse, TriggerType.DestroySelf, TriggerTiming.After);
            AddTrigger<DestroyCardAction>(DestroyCriteria, DestroyCardReponse, TriggerType.DestroySelf, TriggerTiming.After);
        }

        private bool DestroyCriteria(DestroyCardAction dca)
        {
            //this should only apply to the GrandOngoings that need High

            //your are my requiredIdentifer
            //me and you are both in play
            //you are moving to the hand or trash
            return (dca.CardToDestroy.Card.Identifier == _requiredIdentifier || dca.CardToDestroy.Card.Identifier == _attachIdentifier) &&
                   dca.WasCardDestroyed &&
                   dca.CardToDestroy.Card.Location.IsHeroPlayAreaRecursive;
        }

        private bool MoveCriteria(MoveCardAction mca)
        {
            //your are my requiredIdentifer
            //me and you are both in play
            //you are moving to the hand or trash
            return (mca.CardToMove.Identifier == _requiredIdentifier || mca.CardToMove.Identifier == _attachIdentifier) &&
                   mca.Origin == this.Card.Location &&
                   mca.Origin.IsHeroPlayAreaRecursive &&
                   (mca.Destination.IsHand || mca.Destination.IsTrash);
        }

        private IEnumerator DestroyCardReponse(GameAction ga)
        {
            IEnumerator coroutine = base.GameController.DestroyCard(this.DecisionMaker, Card,
                actionSource: ga,
                cardSource: GetCardSource());
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
