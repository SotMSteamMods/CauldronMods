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
			AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
		}

		public override bool AskIfCardIsIndestructible(Card card)
		{
			return card == base.Card || card.Location == base.Card.UnderLocation;
		}
		public override void AddTriggers()
        {
            //Whenever an environment target enters play, put a random card beneath this one into play next to that target.
            AddTrigger((CardEntersPlayAction cpa) => cpa.CardEnteringPlay != null && cpa.CardEnteringPlay.IsEnvironmentTarget && GameController.IsCardVisibleToCardSource(cpa.CardEnteringPlay, GetCardSource()), MoveNatureUnderResponse, TriggerType.MoveCard, TriggerTiming.After);
        }

        private IEnumerator MoveNatureUnderResponse(CardEntersPlayAction cpa)
        {
           
            //puts a random nature card from beneath this one into play
            IEnumerator coroutine2 = MoveRandomNatureUnderTarget(TurnTakerController, Card.UnderLocation, cpa.CardEnteringPlay.BelowLocation, new LinqCardCriteria((Card c) => IsNature(c), "nature"), new int?(1), shuffleBeforehand: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
        }
		public IEnumerator MoveRandomNatureUnderTarget(TurnTakerController revealingTurnTaker, Location source, Location destination, LinqCardCriteria cardCriteria, int? numberOfMatches, bool shuffleSourceAfterwards = true,  RevealedCardDisplay revealedCardDisplay = RevealedCardDisplay.None, bool shuffleReturnedCards = false, bool shuffleBeforehand = false, List<Card> storedMoveResults = null)
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
				if (matchingCards.Count() > 0)
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
				} else
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
            return card.DoKeywordsContain(NatureKeyword, evenIfUnderCard: true, evenIfFaceDown: true);
        }

        public static readonly string NatureKeyword = "nature";
    }
}
