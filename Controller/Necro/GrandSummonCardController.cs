using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

namespace SotMWorkshop.Controller.Necro
{
	public class GrandSummonCardController : CardController
    {
		public GrandSummonCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		
		}
		public override IEnumerator Play()
		{
			//Reveal cards from the top of your deck until you reveal 2 Undead cards. 

			List<RevealCardsAction> revealedCardActions = new List<RevealCardsAction>();
			IEnumerator coroutine = base.GameController.RevealCards(base.HeroTurnTakerController, base.TurnTaker.Deck, (Card c) => this.IsUndead(c), 2, revealedCardActions, RevealedCardDisplay.None, this.GetCardSource(null));


			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			List<Card> revealedCards = new List<Card>();
			foreach(Card card in revealedCardActions.FirstOrDefault().RevealedCards)
			{
				if(this.IsUndead(card))
				{
					revealedCards.Add(card);
				}
			}

			if (revealedCards.Any<Card>())
			{
				//Put 1 into play 
				int powerNumeral = base.GetPowerNumeral(1, 1);
				coroutine = base.GameController.SelectCardsAndDoAction(this.DecisionMaker, new LinqCardCriteria( (Card c) => revealedCards.Contains(c), "", true, false, null, null, false), SelectionType.PutIntoPlay, (Card c) => this.GameController.PlayCard(this.TurnTakerController, c, true, null, false, null, null, false, null, null, null, false, false, true, this.GetCardSource(null)), new int?(powerNumeral), false, null, null, false, null, base.GetCardSource(null), true);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				Card otherCard = (from c in revealedCards
								  where c.Location.IsRevealed
								  select c).FirstOrDefault<Card>();
				if (otherCard != null)
				{
					//and put 1 into the trash.
					List<Card> list = new List<Card>();
					list.Add(otherCard);
					coroutine = base.GameController.SendMessageAction(base.Card.Title + " put " + otherCard.Title + " into the trash.", Priority.Low, base.GetCardSource(null), list, false);
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}
					coroutine = base.GameController.MoveCard(base.TurnTakerController, otherCard, base.FindCardController(otherCard).GetTrashDestination(), false, false, true, null, false, null, null, null, false, false, null, false, false, false, false, base.GetCardSource(null));
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}
				}
				otherCard = null;
			}
			yield break;
		}

		private bool IsUndead(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "undead", false, false);
		}

	}
}
