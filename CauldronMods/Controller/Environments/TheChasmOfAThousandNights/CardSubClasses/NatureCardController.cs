using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheChasmOfAThousandNights
{
    public abstract class NatureCardController : TheChasmOfAThousandNightsUtilityCardController
    {
        protected NatureCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => "This card is indestructible.");
        }

        public override void AddTriggers()
        {
            AddTrigger((MoveCardAction mca) => mca.CardToMove != null && mca.CardToMove == Card && Card.IsInPlayAndNotUnderCard && mca.CardSource != null && mca.CardSource.CardController != GetCardSource().CardController, CancelMoveResponse, new TriggerType[]
            {
                TriggerType.CancelAction,
                TriggerType.ShowMessage
            }, TriggerTiming.Before);

            AddTrigger((BulkMoveCardsAction bmca) => bmca.CardsToMove != null && bmca.CardsToMove.Contains(Card) && Card.IsInPlayAndNotUnderCard && bmca.CardSource != null && bmca.CardSource.CardController != GetCardSource().CardController, CancelMoveResponse, new TriggerType[]
            {
                TriggerType.CancelAction,
                TriggerType.ShowMessage
            }, TriggerTiming.Before);
            //If a nature ever has no target next to it, put that nature face down beneath this card.
            AddIfTheTargetThatThisCardIsBelowLeavesPlayMoveBackUnderTrigger();
        }

        private IEnumerator CancelMoveResponse(BulkMoveCardsAction bmca)
        {
            bmca.RemoveCardsFromMove(Card.ToEnumerable());
            IEnumerator coroutine = IndestructibleMessage();
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
        private IEnumerator CancelMoveResponse(MoveCardAction mca)
        {
            IEnumerator coroutine = CancelAction(mca);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = IndestructibleMessage();
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

        public override bool CanBeDestroyed => false;

        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            return IndestructibleMessage();
        }

        private IEnumerator IndestructibleMessage()
        {
            return GameController.SendMessageAction($"{TheChasm.Title} makes this card indestructible.", Priority.Medium, FindCardController(TheChasm).GetCardSource());
        }

        public override MoveCardDestination GetTrashDestination()
        {
            //this doesn't matter, but provides some coverage in case some effect does move or destroy the nature.
            return new MoveCardDestination(TheChasm.UnderLocation);
        }

        protected void AddIfTheTargetThatThisCardIsBelowLeavesPlayMoveBackUnderTrigger()
        {
            if (Card.Location.OwnerCard == null || GetCardThisCardIsBelow() == null)
            {
                return;
            }
            AddTrigger((MoveCardAction moveCard) => GetCardThisCardIsBelow() == moveCard.CardToMove && !moveCard.Destination.IsInPlayAndNotUnderCard, (MoveCardAction d) => MoveAfterBelowCardLeavesPlay(), TriggerType.MoveCard, TriggerTiming.After);
            AddTrigger((BulkMoveCardsAction bulkMove) => bulkMove.CardsToMove.Where((Card c) => GetCardThisCardIsBelow() == c).Any(), (BulkMoveCardsAction d) => MoveAfterBelowCardLeavesPlay(), TriggerType.MoveCard, TriggerTiming.After);
            AddTrigger((FlipCardAction flip) => GetCardThisCardIsBelow() == flip.CardToFlip.Card && flip.CardToFlip.Card.IsFaceDownNonCharacter, (FlipCardAction d) => MoveAfterBelowCardLeavesPlay(), TriggerType.MoveCard, TriggerTiming.After);
            AddTrigger((TargetLeavesPlayAction a) => GetCardThisCardIsBelow() == a.TargetLeavingPlay, (TargetLeavesPlayAction d) => MoveAfterBelowCardLeavesPlay(), TriggerType.MoveCard, TriggerTiming.After);

            //Target-ness removed from card
            AddTrigger((RemoveTargetAction remove) => GetCardThisCardIsBelow() == remove.CardToRemoveTarget, (RemoveTargetAction d) => MoveAfterBelowCardLeavesPlay(), TriggerType.MoveCard, TriggerTiming.After);
            AddTrigger((BulkRemoveTargetsAction remove) => remove.CardsToRemoveTargets.Contains(GetCardThisCardIsBelow()), (BulkRemoveTargetsAction d) => MoveAfterBelowCardLeavesPlay(), TriggerType.MoveCard, TriggerTiming.After);
        }

        private IEnumerator MoveAfterBelowCardLeavesPlay()
        {
            string message = Card.Title + " returns itself to " + TheChasm.Title + "!";

            IEnumerator coroutine = base.GameController.SendMessageAction(message, Priority.Medium, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.MoveCard(TurnTakerController, Card, TheChasm.UnderLocation, evenIfIndestructible: true, flipFaceDown: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}