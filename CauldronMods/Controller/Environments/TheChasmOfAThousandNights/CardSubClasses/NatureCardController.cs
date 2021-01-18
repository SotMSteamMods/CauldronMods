﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
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
        }

        public override void AddTriggers()
        {
            //If a nature ever has no target next to it, put that nature face down beneath this card.
            AddIfTheTargetThatThisCardIsBelowLeavesPlayMoveBackUnderTrigger();
        }

        public override bool CanBeDestroyed => false;
        
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
            coroutine = GameController.MoveCard(TurnTakerController, Card, chasm.UnderLocation, evenIfIndestructible: true, flipFaceDown: true, cardSource: GetCardSource());
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