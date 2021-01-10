using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class NatureCardController : TheChasmOfAThousandNightsUtilityCardController
    {

        public NatureCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override void AddTriggers()
        {
            //If a nature ever has no target next to it, put that nature face down beneath this card.
            AddIfTheTargetThatThisCardIsNextToLeavesPlayMoveBackUnderTrigger();
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //This card is indestructible.
            return card == Card;
        }

        protected void AddIfTheTargetThatThisCardIsNextToLeavesPlayMoveBackUnderTrigger(IEnumerator doThisFirst = null)
        {
            if (Card.Location.OwnerCard == null)
            {
                return;
            }
            AddTrigger((MoveCardAction moveCard) => IsThisCardNextToCard(moveCard.CardToMove) && !moveCard.Destination.IsInPlayAndNotUnderCard, (MoveCardAction d) => MoveAfterNextToCardLeavesPlay(doThisFirst), TriggerType.MoveCard, TriggerTiming.After);
            AddTrigger((BulkMoveCardsAction bulkMove) => bulkMove.CardsToMove.Where((Card c) => IsThisCardNextToCard(c)).Count() > 0, (BulkMoveCardsAction d) => MoveAfterNextToCardLeavesPlay(doThisFirst), TriggerType.MoveCard, TriggerTiming.After);
            AddTrigger((FlipCardAction flip) => IsThisCardNextToCard(flip.CardToFlip.Card) && flip.CardToFlip.Card.IsFaceDownNonCharacter, (FlipCardAction d) => MoveAfterNextToCardLeavesPlay(doThisFirst), TriggerType.MoveCard, TriggerTiming.After);
            AddTrigger((RemoveTargetAction remove) => IsThisCardNextToCard(remove.CardToRemoveTarget), (RemoveTargetAction d) => MoveAfterNextToCardLeavesPlay(doThisFirst), TriggerType.MoveCard, TriggerTiming.After);
            AddTrigger((BulkRemoveTargetsAction remove) => GetCardThisCardIsNextTo() != null && remove.CardsToRemoveTargets.Contains(GetCardThisCardIsNextTo()), (BulkRemoveTargetsAction d) => MoveAfterNextToCardLeavesPlay(doThisFirst), TriggerType.MoveCard, TriggerTiming.After);
            AddTrigger((TargetLeavesPlayAction a) => IsThisCardNextToCard(a.TargetLeavingPlay), (TargetLeavesPlayAction d) => MoveAfterNextToCardLeavesPlay(doThisFirst), TriggerType.MoveCard, TriggerTiming.After);
           
        }

        private IEnumerator MoveAfterNextToCardLeavesPlay(IEnumerator doThisFirst)
        {
            IEnumerator enumerator = doThisFirst;
            if (enumerator != null)
            {
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(enumerator);
                }
                else
                {
                    GameController.ExhaustCoroutine(enumerator);
                }
            }
            Card chasm = TurnTaker.FindCard(TheChasmOfAThousandNightsCardController.Identifier, realCardsOnly: false);
            string message = Card.Title + " returns itself to " + chasm.Title + "!";

            IEnumerator coroutine = base.GameController.SendMessageAction(message, Priority.Medium, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            enumerator = GameController.MoveCard(TurnTakerController, Card, chasm.UnderLocation, evenIfIndestructible: true, flipFaceDown: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(enumerator);
            }
            else
            {
                GameController.ExhaustCoroutine(enumerator);
            }
        }

    }
}