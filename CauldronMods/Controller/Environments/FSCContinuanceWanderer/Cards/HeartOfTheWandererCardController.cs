﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.FSCContinuanceWanderer
{
    public class HeartOfTheWandererCardController : CardController
    {

        public HeartOfTheWandererCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //When this card enters play, reveal the top card of each deck in turn order and either discard it or replace it.
            IEnumerator coroutine = base.DoActionToEachTurnTakerInTurnOrder((TurnTakerController) => true, this.EachTurnTakerResponse, base.TurnTaker);
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
            //At the end of the environment turn, destroy this card.
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);
        }

        private IEnumerator EachTurnTakerResponse(TurnTakerController turnTakerController)
        {
            //...reveal the top card of each deck in turn order and either discard it or replace it.
            List<Card> revealedCards = new List<Card>();
            TurnTaker turnTaker = turnTakerController.TurnTaker;
            List<Location> decks = new List<Location>();
            decks.Add(turnTaker.Deck);
            decks = decks.Concat(turnTaker.SubDecks.Where(l => l.BattleZone == Card.BattleZone && l.IsRealDeck)).ToList();
            IEnumerator coroutine;
            foreach (Location deck in decks)
            {
                coroutine = base.GameController.RevealCards(turnTakerController, deck, 1, revealedCards, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                Card revealedCard = revealedCards.FirstOrDefault<Card>();
                if (revealedCard != null)
                {
                    coroutine = base.GameController.SelectLocationAndMoveCard(this.DecisionMaker, revealedCard, new MoveCardDestination[]
                    {
                        new MoveCardDestination(deck),
                        FindCardController(revealedCard).GetTrashDestination()
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
            }
            
            yield break;
        }
    }
}