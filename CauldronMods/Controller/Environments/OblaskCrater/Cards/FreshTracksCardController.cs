﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class FreshTracksCardController : OblaskCraterUtilityCardController
    {
        /*
         * When this card enters play, reveal cards from the top of the environment deck 
         * until a target is revealed, place it beneath this card, and discard the rest.
         * 
         * Cards beneath this one are not considered in play. When a target enters play, 
         * the players may destroy this card to play the card beneath it
        */
        private const int NumberOfCardMatches = 1;

        public FreshTracksCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.Card.UnderLocation.OverrideIsInPlay = false;
            SpecialStringMaker.ShowListOfCards(new LinqCardCriteria(c => Card.UnderLocation.HasCard(c), "target under this card", useCardsSuffix: false)).Condition = () => Card.IsInPlayAndHasGameText;
        }

        public override void AddTriggers()
        {
            base.AddTrigger<CardEntersPlayAction>((cep) => cep.CardEnteringPlay.IsTarget, PlayCardActionResponse, new TriggerType[]
                { 
                    TriggerType.DestroyCard,
                    TriggerType.PlayCard
                }, TriggerTiming.Before);
        }

        private IEnumerator PlayCardActionResponse(CardEntersPlayAction playCardAction)
        {
            IEnumerator coroutine;
            List<bool> storedResults = new List<bool>();
            List<DestroyCardAction> destroyCardActions = new List<DestroyCardAction>();
            Card underCard = base.Card.UnderLocation.BottomCard;

            List<YesNoCardDecision> storedYesNo = new List<YesNoCardDecision>();
            coroutine = base.GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.DestroyCard, Card, storedResults: storedYesNo, associatedCards: underCard.ToEnumerable(), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

           if(DidPlayerAnswerYes(storedYesNo))
            {
                // Destroy this card and put the target beneath it into play
                coroutine = base.GameController.DestroyCard(base.DecisionMaker, base.Card, postDestroyAction: () =>
                {
                    return base.GameController.PlayCard(base.TurnTakerController, underCard, cardSource: GetCardSource());
                }, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        public override IEnumerator Play()
        {
            Location environmentDeck = FindLocationsWhere(location => location.IsRealDeck && location.IsEnvironment && GameController.IsLocationVisibleToSource(location, GetCardSource())).First();
            Location environmentTrash = FindLocationsWhere(location => location.IsRealTrash && location.IsEnvironment && GameController.IsLocationVisibleToSource(location, GetCardSource())).First();
            List<RevealCardsAction> revealedCards = new List<RevealCardsAction>();
            IEnumerator revealCardsRoutine;
            IEnumerator returnCardsRoutine;
            IEnumerator coroutine;
            Card matchedCard;
            List<Card> otherCards;

            // Reveal cards until target is revealed (if any)      
            revealCardsRoutine = base.GameController.RevealCards(TurnTakerController, environmentDeck, card => card.IsTarget, NumberOfCardMatches, revealedCards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(revealCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(revealCardsRoutine);
            }

            matchedCard = GetRevealedCards(revealedCards).FirstOrDefault(c => c.IsTarget);
            otherCards = GetRevealedCards(revealedCards).Where(c => !c.IsTarget).ToList();
            if (otherCards.Any())
            {
                // Put non matching revealed cards back in the trash
                returnCardsRoutine = GameController.MoveCards(DecisionMaker, otherCards, environmentTrash, cardSource: GetCardSource());
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(returnCardsRoutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(returnCardsRoutine);
                }
            }

            if (matchedCard != null)
            {                
                coroutine = base.GameController.MoveCard(base.TurnTakerController, matchedCard, base.Card.UnderLocation,
                            showMessage: true,
                            flipFaceDown: false,
                            cardSource: base.GetCardSource()); 
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }
    }
}
