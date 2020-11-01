using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Cauldron.Necro
{
	public class BookOfTheDeadCardController : CardController
    {
		public BookOfTheDeadCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			base.SpecialStringMaker.ShowNumberOfCardsAtLocations(() => new Location[]
			{
				base.TurnTaker.Trash,
				base.TurnTaker.Deck
			}, new LinqCardCriteria((Card c) => this.IsRitual(c), "ritual", true, false, null, null, false), null);
		}
		public override IEnumerator Play()
		{
			//Search your deck or trash for a ritual and put it into play or into your hand. If you searched your deck, shuffle your deck.
			if (base.TurnTaker.IsHero)
			{

				IEnumerator coroutine = base.SearchForCards(base.HeroTurnTakerController, true, true, new int?(1), 1, new LinqCardCriteria((Card c) => this.IsRitual(c), "ritual", true, false, null, null, false), true, true, false, false, null, false, null, null);
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
				IEnumerator coroutine2 = base.GameController.SendMessageAction(base.Card.Title + " has no deck or trash to search.", Priority.Medium, base.GetCardSource(null), null, true);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}

			//You may draw a card.
			IEnumerator coroutine3 = base.DrawCards(this.DecisionMaker, 1, false, true, null, false, null);
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

		private bool IsRitual(Card card)
		{
			return card != null && this.GameController.DoesCardContainKeyword(card, "ritual", false, false);
		}
	}
}
