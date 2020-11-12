using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheStranger
{
	public class TheStrangerCharacterCardController : HeroCharacterCardController
	{
		public TheStrangerCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
		{
			//Play a rune.
			IEnumerator coroutine = base.SelectAndPlayCardFromHand(base.HeroTurnTakerController, false, null, new LinqCardCriteria((Card c) => this.IsRune(c), "rune"), false, false, true, null);
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
		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						//One player may draw a card now.
						IEnumerator coroutine3 = base.GameController.SelectHeroToDrawCard(this.DecisionMaker, false, true, false, null, null, null, base.GetCardSource(null));
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine3);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine3);
						}
						break;
					}
				case 1:
					{
						//Reveal the top card of a deck, then replace it or discard it.
						List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
						IEnumerator coroutine3 = base.GameController.SelectADeck(this.DecisionMaker, SelectionType.RevealTopCardOfDeck, (Location l) => l.IsDeck && !l.OwnerTurnTaker.IsIncapacitatedOrOutOfGame, storedResults, false, null, base.GetCardSource(null));
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine3);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine3);
						}
						Location selectedLocation = base.GetSelectedLocation(storedResults);
						if (selectedLocation != null)
						{
							coroutine3 = this.RevealCard_PutItBackOrDiscardIt(base.TurnTakerController, selectedLocation,null,null,true, base.TurnTaker,true);
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine3);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine3);
							}
						}
						break;

					}
				case 2:
					{
						//Up to 2 ongoing hero cards may be played now.

						//choose up to 2 cards
						List<SelectCardsDecision> storedResults = new List<SelectCardsDecision>();
						IEnumerator coroutine3 = base.GameController.SelectCardsAndStoreResults(base.HeroTurnTakerController, SelectionType.PlayCard, (Card c) => c.IsInHand && c.Location.IsHero && c.IsOngoing,2, storedResults, true,new int?(0));
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine3);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine3);
						}
						if (storedResults != null && storedResults[0].NumberOfCards > 0)
						{
							//play each card selected
							foreach(SelectCardDecision d in storedResults[0].SelectCardDecisions)
							{
								Card selectedCard = d.SelectedCard;
								HeroTurnTakerController hero = (base.FindTurnTakerController(selectedCard.Location.OwnerTurnTaker)).ToHero();
								IEnumerator coroutine2 = this.SelectAndPlayCardFromHand(hero, false, null, new LinqCardCriteria( (Card c) => c == selectedCard), true, false, true, null);
								if (base.UseUnityCoroutines)
								{
									yield return base.GameController.StartCoroutine(coroutine2);
								}
								else
								{
									base.GameController.ExhaustCoroutine(coroutine2);
								}
							}
							
						}
						break;
					}
			}
			yield break;
		}

		private bool IsRune(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "rune", false, false);
		}

		protected IEnumerator RevealCard_PutItBackOrDiscardIt(TurnTakerController revealingTurnTaker, Location deck, LinqCardCriteria autoPlayCriteria = null, List<MoveCardAction> storedResults = null, bool showRevealedCards = true, TurnTaker responsibleTurnTaker = null, bool isDiscard = true)
		{
			RevealedCardDisplay revealedCardDisplay = RevealedCardDisplay.None;
			if (showRevealedCards)
			{
				revealedCardDisplay = RevealedCardDisplay.Message;
			}
			List<Card> revealedCards = new List<Card>();
			IEnumerator coroutine = this.GameController.RevealCards(revealingTurnTaker, deck, 1, revealedCards, false, revealedCardDisplay, null, this.GetCardSource(null));
			if (this.UseUnityCoroutines)
			{
				yield return this.GameController.StartCoroutine(coroutine);
			}
			else
			{
				this.GameController.ExhaustCoroutine(coroutine);
			}
			if (responsibleTurnTaker == null)
			{
				responsibleTurnTaker = this.TurnTaker;
			}
			if (revealedCards.Count<Card>() > 0)
			{
				Card card = revealedCards.First<Card>();
				CardController cardController = this.FindCardController(card);
				TurnTaker ownerTurnTaker = deck.OwnerTurnTaker;


				MoveCardDestination[] possibleDestinations = new MoveCardDestination[]
				{
				new MoveCardDestination(card.Owner.Deck, false, false, false),
				new MoveCardDestination(cardController.GetTrashDestination(), false, false, false)
				};
				IEnumerator coroutine4 = this.GameController.SelectLocationAndMoveCard(this.HeroTurnTakerController, card, possibleDestinations, false, true, null, null, storedResults, false, false, responsibleTurnTaker, isDiscard, this.GetCardSource(null));
				if (this.UseUnityCoroutines)
				{
					yield return this.GameController.StartCoroutine(coroutine4);
				}
				else
				{
					this.GameController.ExhaustCoroutine(coroutine4);
				}


				IEnumerator coroutine6 = this.CleanupCardsAtLocations(new List<Location>
			{
				deck.OwnerTurnTaker.Revealed
			}, deck, false, true, false, false, false, true, revealedCards);
				if (this.UseUnityCoroutines)
				{
					yield return this.GameController.StartCoroutine(coroutine6);
				}
				else
				{
					this.GameController.ExhaustCoroutine(coroutine6);
				}
				yield break;
			}
		}
	}
}
