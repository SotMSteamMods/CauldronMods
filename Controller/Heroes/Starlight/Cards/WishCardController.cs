using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class WishCardController : StarlightCardController
    {
        public WishCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
			//"1 player may..." 
			List<SelectTurnTakerDecision> storedResults = new List<SelectTurnTakerDecision>();
			IEnumerator coroutine = base.GameController.SelectHeroTurnTaker(DecisionMaker, SelectionType.RevealCardsFromDeck, optional: true, allowAutoDecide: false, storedResults, new LinqTurnTakerCriteria((TurnTaker tt) => GameController.IsTurnTakerVisibleToCardSource(tt, GetCardSource()) && !tt.IsIncapacitatedOrOutOfGame, "active heroes"), cardSource:GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			TurnTaker hero = GetSelectedTurnTaker(storedResults);
			if (hero == null || !hero.IsHero)
			{
				yield break;
			}
			HeroTurnTakerController heroTTC = FindHeroTurnTakerController(hero.ToHero());

			if(heroTTC != null)
            {
				//"...look at the top 5 cards of their deck, put 1 of them into play, then put the rest on the bottom of their deck in any order."

				IEnumerator coroutine3 = RevealCardsWithCustomLogic(heroTTC);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine3);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine3);
				}
			}

			yield break;
        }

		private IEnumerator RevealCardsWithCustomLogic(HeroTurnTakerController heroTTC)
        {
			List<Card> revealedCards = new List<Card> { };
			IEnumerator reveal = GameController.RevealCards(heroTTC, heroTTC.TurnTaker.Deck, 5, revealedCards, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(reveal);
			}
			else
			{
				base.GameController.ExhaustCoroutine(reveal);
			}

			string message = null;
			switch (revealedCards.Count())
            {
				case 5:
                    {
						break;
                    }
				case 0:
					{
						message = "No cards were revealed! There is no further effect.";
						yield break;
					}
				case 1:
                    {
						message = "Only one card was revealed! It will automatically be put into play.";
						break;
                    }
				default:
                    {
						message = String.Format("Only {0} cards were revealed!", revealedCards.Count());
						break;
                    }
            }

			if (message != null)
            {
				IEnumerator warnNotEnoughCards = GameController.SendMessageAction(message, Priority.High, this.GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(warnNotEnoughCards);
				}
				else
				{
					base.GameController.ExhaustCoroutine(warnNotEnoughCards);
				}
			}

			var playCardStorage = new List<PlayCardAction> { };
			IEnumerator playCard = GameController.SelectAndPlayCard(heroTTC, revealedCards, false, isPutIntoPlay: true, playCardStorage, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(playCard);
			}
			else
			{
				base.GameController.ExhaustCoroutine(playCard);
			}

			Card playedCard = null;
			if (DidPlayCards(playCardStorage))
            {
				playedCard = playCardStorage.FirstOrDefault().CardToPlay;
				revealedCards.Remove(playedCard);
            }

			int numCardsLeft = revealedCards.Count();
			if (numCardsLeft > 0)
            {
				Location heroRevealedLocation = heroTTC.TurnTaker.Revealed;
				List<MoveCardDestination> bottomOfDeck = new List<MoveCardDestination> { new MoveCardDestination(heroTTC.TurnTaker.Deck, toBottom: true) };
				var moveRest = GameController.SelectCardsFromLocationAndMoveThem(heroTTC,
																	heroRevealedLocation,
																	numCardsLeft,
																	numCardsLeft,
																	new LinqCardCriteria((Card c) => revealedCards.Contains(c), "remaining"),
																	bottomOfDeck,
																	allowAutoDecide: true,
																	cardSource: this.GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(moveRest);
				}
				else
				{
					base.GameController.ExhaustCoroutine(moveRest);
				}
			}

			//clean up RevealedCards if we have to
			revealedCards.Add(playedCard);
			List<Location> cleanup = new List<Location> { heroTTC.TurnTaker.Revealed };
			IEnumerator cleanupRoutine = CleanupCardsAtLocations(cleanup, heroTTC.TurnTaker.Deck, toBottom: false, addInhibitorException: true, shuffleAfterwards: false, sendMessage: false, isDiscard: false, isReturnedToOriginalLocation: true, revealedCards);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(cleanupRoutine);
			}
			else
			{
				GameController.ExhaustCoroutine(cleanupRoutine);
			}


			yield break;
        }

		private IEnumerator RevealCardsWithArgentAdept(HeroTurnTakerController heroTTC)
        {
			//"...look at the top 5 cards of their deck, put 1 of them into play, then put the rest on the bottom of their deck in any order."
			List<MoveCardDestination> list = new List<MoveCardDestination>();
			list.Add(new MoveCardDestination(heroTTC.TurnTaker.PlayArea));
			list.Add(new MoveCardDestination(heroTTC.TurnTaker.Deck, toBottom: true));
			list.Add(new MoveCardDestination(heroTTC.TurnTaker.Deck, toBottom: true));
			list.Add(new MoveCardDestination(heroTTC.TurnTaker.Deck, toBottom: true));
			list.Add(new MoveCardDestination(heroTTC.TurnTaker.Deck, toBottom: true));
			IEnumerator coroutine3 = RevealCardsFromDeckToMoveToOrderedDestinations(heroTTC, heroTTC.TurnTaker.Deck, list, numberOfCardsToReveal: 5);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine3);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine3);
			}
			yield break;
		}
    }
}