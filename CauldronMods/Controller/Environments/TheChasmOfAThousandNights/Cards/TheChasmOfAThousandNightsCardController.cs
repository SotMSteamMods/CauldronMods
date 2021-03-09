using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class TheChasmOfAThousandNightsCardController : TheChasmOfAThousandNightsUtilityCardController
    {
        public static readonly string Identifier = "TheChasmOfAThousandNights";

        public TheChasmOfAThousandNightsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            base.Card.UnderLocation.OverrideIsInPlay = false;
        }

        public override void AddTriggers()
        {
            //Whenever an environment target enters play, put a random card beneath this one into play next to that target.
            AddTrigger((CardEntersPlayAction cpa) => cpa.CardEnteringPlay != null && cpa.CardEnteringPlay.IsEnvironmentTarget && GameController.IsCardVisibleToCardSource(cpa.CardEnteringPlay, GetCardSource()), MoveNatureUnderResponse, TriggerType.MoveCard, TriggerTiming.After);

            base.AddTrigger<MakeDecisionsAction>((MakeDecisionsAction md) => md.CardSource != null && !md.CardSource.Card.IsEnvironment, this.RemoveDecisionsFromMakeDecisionsResponse, TriggerType.RemoveDecision, TriggerTiming.Before);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            if (card == Card)
                return true;

            if (IsNature(card))
                return true;

            return base.AskIfCardIsIndestructible(card);
        }

        private IEnumerator RemoveDecisionsFromMakeDecisionsResponse(MakeDecisionsAction md)
        {
            //remove this card as an option to make decisions
            md.RemoveDecisions((IDecision d) => d.SelectedCard == base.Card || Card.UnderLocation.HasCard(d.SelectedCard));
            return base.DoNothing();
        }

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            if (card == base.Card || Card.UnderLocation.HasCard(card) && !cardSource.Card.IsEnvironment)
            {
                return false;
            }
            return true;
        }

        public override bool AskIfActionCanBePerformed(GameAction g)
        {
            bool? flag = g.DoesFirstCardAffectSecondCard((Card c) => !c.IsEnvironment, (Card c) => c == base.Card || Card.UnderLocation.HasCard(c));

            if (flag.HasValue && flag.Value)
            {
                return false;
            }

            return true;
        }

        private IEnumerator MoveNatureUnderResponse(CardEntersPlayAction cpa)
        {
            //puts a random nature card from beneath this one into play
            IEnumerator coroutine = MoveRandomNatureUnderTarget(TurnTakerController, Card.UnderLocation, cpa.CardEnteringPlay.BelowLocation, new LinqCardCriteria((Card c) => IsNature(c), "nature"), 1, shuffleBeforehand: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public IEnumerator MoveRandomNatureUnderTarget(TurnTakerController revealingTurnTaker, Location source, Location destination, LinqCardCriteria cardCriteria, int? numberOfMatches, bool shuffleSourceAfterwards = true, RevealedCardDisplay revealedCardDisplay = RevealedCardDisplay.None, bool shuffleReturnedCards = false, bool shuffleBeforehand = false, List<Card> storedMoveResults = null)
        {
            IEnumerator e3 = null;
            if (shuffleBeforehand)
            {
                e3 = GameController.ShuffleLocation(source, null, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e3);
                }
                else
                {
                    GameController.ExhaustCoroutine(e3);
                }
            }
            List<Card> matchingCards = new List<Card>();
            List<Card> nonMatchingCards = new List<Card>();
            List<Card> storedResultsCard = new List<Card>();
            List<RevealCardsAction> storedResultsAction = new List<RevealCardsAction>();
            if (numberOfMatches.HasValue)
            {
                e3 = GameController.RevealCards(revealingTurnTaker, source, cardCriteria.Criteria, numberOfMatches.Value, storedResultsAction, revealedCardDisplay, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(e3);
                }
                else
                {
                    GameController.ExhaustCoroutine(e3);
                }
                if (storedResultsAction.FirstOrDefault() != null)
                {
                    matchingCards.AddRange(storedResultsAction.FirstOrDefault().MatchingCards);
                    nonMatchingCards.AddRange(storedResultsAction.FirstOrDefault().NonMatchingCards);
                }
            }
            if (e3 != null)
            {
                if (matchingCards.Any())
                {
                    foreach (Card item in matchingCards)
                    {
                        storedMoveResults?.Add(item);
                        IEnumerator coroutine2 = GameController.MoveCard(TurnTakerController, item, destination, isPutIntoPlay: true, cardSource: GetCardSource());

                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine2);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine2);
                        }
                        FindCardController(item).RemoveAllTriggers();
                        GameController.RemoveInhibitor(FindCardController(item));
                        FindCardController(item).AddAllTriggers();
                    }
                }
                else
                {
                    e3 = GameController.SendMessageAction("No " + cardCriteria.GetDescription() + " were found in " + source.GetFriendlyName() + ".", Priority.Medium, GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(e3);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(e3);
                    }
                }
                if (shuffleReturnedCards)
                {
                    nonMatchingCards.Shuffle(GameController.Game.RNG);
                }
                IEnumerator coroutine7 = GameController.BulkMoveCards(TurnTakerController, nonMatchingCards, source, toBottom: false, performBeforeDestroyActions: true, null, isDiscard: false, GetCardSource());
                IEnumerator e6 = GameController.ShuffleLocation(source, null, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine7);
                    if (shuffleSourceAfterwards)
                    {
                        yield return GameController.StartCoroutine(e6);
                    }
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine7);
                    if (shuffleSourceAfterwards)
                    {
                        GameController.ExhaustCoroutine(e6);
                    }
                }
            }
            else
            {
                Log.Warning("RevealCards_MoveMatching_ReturnNonMatchingCards was called, but with no specified number of cards to reveal.");
            }
            List<Location> list2 = new List<Location>();
            list2.Add(source.OwnerTurnTaker.Revealed);
            e3 = CleanupCardsAtLocations(list2, source, toBottom: false, addInhibitorException: true, shuffleAfterwards: false, sendMessage: false, isDiscard: false, isReturnedToOriginalLocation: true, storedResultsCard);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(e3);
            }
            else
            {
                GameController.ExhaustCoroutine(e3);
            }
        }

        protected bool IsNature(Card card)
        {
            return card != null && card.DoKeywordsContain(NatureKeyword, evenIfUnderCard: true, evenIfFaceDown: true);
        }

        public static readonly string NatureKeyword = "nature";
    }
}
