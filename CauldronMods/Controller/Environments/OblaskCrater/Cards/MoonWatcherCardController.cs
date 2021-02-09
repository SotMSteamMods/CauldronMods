using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class MoonWatcherCardController : OblaskCraterUtilityCardController
    {
        /*
         * At the end of the environment turn, this card deals each non-environment target 1 sonic damage.
         * When this card is destroyed, if it has 0 or fewer HP, reveal the top card of each deck and 
         * replace or discard each one.
         */
        public MoonWatcherCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => c.IsNonEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.All, 1, DamageType.Sonic);
            base.AddWhenDestroyedTrigger(DestroyCardActionResponse, TriggerType.RevealCard);
        }

        private IEnumerator DestroyCardActionResponse(DestroyCardAction destroyCardAction)
        {
            // When this card is destroyed, if it has 0 or fewer HP, reveal the top card of each deck and
            // replace or discard each one.
            IEnumerator coroutine;

            if (base.Card.HitPoints <= 0)
            {
                //coroutine = base.GameController.SelectTurnTakersAndDoAction(base.DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => true), SelectionType.RevealTopCardOfDeck, (TurnTaker tt) => base.RevealCard_DiscardItOrPutItOnDeck(base.DecisionMaker, base.FindTurnTakerController(tt), tt.Deck, false), allowAutoDecide: true, cardSource: base.GetCardSource());
                coroutine = base.GameController.SelectLocationsAndDoAction(DecisionMaker, SelectionType.RevealTopCardOfDeck, (Location loc) => loc.IsDeck && !loc.OwnerTurnTaker.IsIncapacitatedOrOutOfGame && loc.HasCards, RevealAndReplaceOrDiscardResponse, cardSource: GetCardSource());

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

        private IEnumerator RevealAndReplaceOrDiscardResponse(Location location)
        {
            List<Card> cards = new List<Card>();           
            var coroutine = GameController.RevealCards(TurnTakerController, location, 1, cards, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            Card revealedCard = GetRevealedCard(cards);
            if (revealedCard != null)
            {
                YesNoDecision yesNo = new YesNoCardDecision(GameController, DecisionMaker, SelectionType.DiscardCard, revealedCard, cardSource: GetCardSource());
                List<IDecision> decisionSources = new List<IDecision>
                {
                    yesNo
                };
                IEnumerator coroutine2 = GameController.MakeDecisionAction(yesNo);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine2);
                }
                if (DidPlayerAnswerYes(yesNo))
                {
                    IEnumerator coroutine3 = GameController.MoveCard(TurnTakerController, revealedCard, FindCardController(revealedCard).GetTrashDestination().Location, decisionSources: decisionSources, cardSource: GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine3);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine3);
                    }
                }
                if (yesNo != null && yesNo.Completed && yesNo.Answer.HasValue)
                {
                    decisionSources.Add(yesNo);
                    if (!yesNo.Answer.Value)
                    {
                        IEnumerator coroutine4 = GameController.MoveCard(TurnTakerController, revealedCard, location, decisionSources: decisionSources, cardSource: GetCardSource());
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine4);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine4);
                        }
                    }
                }
            }

            if (location.OwnerTurnTaker.Revealed.HasCards)
            {
                List<Location> list = new List<Location>();
                list.Add(location.OwnerTurnTaker.Revealed);
                IEnumerator coroutine5 = CleanupCardsAtLocations(list, location, cardsInList: cards);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine5);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine5);
                }
            }

            yield break;
        }
    }
}
