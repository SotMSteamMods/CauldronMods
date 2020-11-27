﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.FSCContinuanceWanderer
{
    public class TemporalReversalCardController : CardController
    {
        public TemporalReversalCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //When this card enters play, place 1 card in play from each other deck back on top of that deck.
            //SelectTurnTakersAndDoAction filters on BattleZone, no need to check for visibility here
            IEnumerator coroutine = base.GameController.SelectTurnTakersAndDoAction(base.DecisionMaker, new LinqTurnTakerCriteria((TurnTaker turnTaker) => !turnTaker.IsEnvironment && !turnTaker.IsIncapacitatedOrOutOfGame), SelectionType.ReturnToDeck, (TurnTaker turnTaker) => this.MoveCardToDeckResponse(turnTaker), cardSource: base.GetCardSource());
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

        private IEnumerator MoveCardToDeckResponse(TurnTaker turnTaker)
        {
            if (turnTaker is HeroTurnTaker htt)
            {
                IEnumerator coroutine = GameController.SelectAndReturnCards(FindHeroTurnTakerController(htt), 1, new LinqCardCriteria((Card c) => c.Owner == turnTaker && c.IsInPlay && !c.IsCharacter && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "card in play"), false, true, false, 1, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                IEnumerator coroutine = base.GameController.SelectAndMoveCard(DecisionMaker, (Card c) => c.Owner == turnTaker && c.IsInPlay && !c.IsCharacter && GameController.IsCardVisibleToCardSource(c, GetCardSource()), turnTaker.Deck, cardSource: GetCardSource());
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

        public override void AddTriggers()
        {
            //At the end of the environment turn, destroy this card.
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);
        }

    }
}