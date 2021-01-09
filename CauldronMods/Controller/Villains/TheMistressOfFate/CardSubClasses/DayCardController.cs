using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;

namespace Cauldron.TheMistressOfFate
{
    public abstract class DayCardController : TheMistressOfFateUtilityCardController
    {
        private string dayStoredKey = "MistressOfFateDayStoredCardKey";
        protected string[] soughtKeywords = null;
        private Card _storedCard;
        private bool? hasStoredCard = null;
        private Card storedCard
        {
            get 
            {
                if(_storedCard == null)
                {
                    if(hasStoredCard != false)
                    {
                        _storedCard = GetCardPropertyJournalEntryCard(dayStoredKey);
                    }
                    hasStoredCard = _storedCard != null;
                }
                return _storedCard;
            }
        }
        protected DayCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _isStoredCard = false;
            var storedCardString = SpecialStringMaker.ShowSpecialString(() => $"On this day, {storedCard.Title} recurs.", relatedCards: () => new Card[] { storedCard });
            storedCardString.Condition = () => storedCard != null;
        }

        public override void AddTriggers()
        {
            if (soughtKeywords != null)
            {
                //"When {TheMistressOfFate} flips or [the card tied to this one] leaves play, put it beneath this card."
                AddTrigger((MoveCardAction m) => m.Origin.IsInPlay && !m.Destination.IsInPlay && m.Destination != this.Card.UnderLocation && m.CardToMove == storedCard, ReclaimStoredCard, TriggerType.MoveCard, TriggerTiming.Before);

                AddTrigger((FlipCardAction fca) => fca.CardToFlip == CharacterCardController, ReclaimStoredCard, TriggerType.MoveCard, TriggerTiming.After);
            }
        }
        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            IEnumerator coroutine = base.AfterFlipCardImmediateResponse();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (Card.IsInPlayAndHasGameText)
            {
                coroutine = DayFlipFaceUpEffect();
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        protected abstract IEnumerator DayFlipFaceUpEffect();

        protected IEnumerator GetAndPlayStoredCard(string[] keywords)
        {
            IEnumerator coroutine;
            //"When this card flips face up, put any cards beneath it into play. 
            if(this.Card.UnderLocation.HasCards)
            {
                coroutine = PlayCardsFromLocation(this.Card.UnderLocation, new LinqCardCriteria(), showMessage: false);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            //If there are none, reveal cards from the top of the villain deck until a [soughtKeyword] is revealed, put it into play, and shuffle the other revealed cards into the villain deck.",
            else
            {
                var storedReveal = new List<RevealCardsAction>();
                coroutine = GameController.RevealCards(TurnTakerController, TurnTaker.Deck, (Card c) => keywords.Any((string word) => GameController.DoesCardContainKeyword(c, word)), 1, storedReveal, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                var reveal = storedReveal.FirstOrDefault();
                Card matchingCard = null;
                if(reveal == null)
                {
                    //message
                    yield break;
                }
                else if (reveal.FoundMatchingCards == false)
                {
                    //message about fail-to-find?
                    this.ClearStoredCard();
                }
                else
                {
                    matchingCard = reveal.MatchingCards.FirstOrDefault();
                    StoreCard(matchingCard);
                    //message about newly stored card?
                }

                coroutine = CleanupCardsAtLocations(new List<Location> { TurnTaker.Revealed }, TurnTaker.Deck, shuffleAfterwards: true, cardsInList: reveal.NonMatchingCards.ToList());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                if(matchingCard != null)
                {
                    coroutine = GameController.PlayCard(DecisionMaker, matchingCard, isPutIntoPlay: true, cardSource: GetCardSource());
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

            yield break;
        }

        protected void StoreCard(Card c)
        {
            ClearStoredCard();
            if(c != null && FindCardController(c) is TheMistressOfFateUtilityCardController cardCC)
            {
                cardCC.SetStoringDay(this.Card);
            }
            _storedCard = c;
            hasStoredCard = true;
        }

        protected void ClearStoredCard()
        {
            var card = storedCard;
            if(card != null && FindCardController(card) is TheMistressOfFateUtilityCardController cardCC)
            {
                cardCC.ClearStoringDay();
            }
            _storedCard = null;
            hasStoredCard = false;
        }

        private IEnumerator ReclaimStoredCard(GameAction ga)
        {
            if (ga is MoveCardAction mc)
            {
                mc.SetDestination(Card.UnderLocation);
                if (mc.Destination != this.Card.UnderLocation)
                {
                    ClearStoredCard();
                }
                yield return null;
            }
            else if (ga is FlipCardAction fc && storedCard != null && storedCard.IsInPlayAndHasGameText)
            {
                IEnumerator coroutine = GameController.MoveCard(DecisionMaker, storedCard, this.Card.UnderLocation, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                if(this.Card.UnderLocation.IsEmpty)
                {
                    ClearStoredCard();
                }
            }
            yield break;
        }
    }
}
