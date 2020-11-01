using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.LadyOfTheWood
{
	public class CalmBeforeTheStormCardController : CardController
    {
		public CalmBeforeTheStormCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator Play()
		{
			//Discard any number of cards.
			List<int> discardedCards = new List<int>();
			IEnumerator coroutine = this.DiscardOneOrMoreCards(discardedCards);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			//Increase the next damage dealt by {LadyOfTheWood} by X, where X is 2 plus the number of cards discarded this way.
			IEnumerator coroutine2 = base.AddStatusEffect(new IncreaseDamageStatusEffect(discardedCards.First<int>() + 2)
			{
				SourceCriteria =
				{
					IsSpecificCard = base.CharacterCard
				},
				NumberOfUses = new int?(1),
				CardDestroyedExpiryCriteria =
				{
					Card = base.CharacterCard
				}
			}, true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
			yield break;
		}

		/// <summary>
		/// Discard one or more cards
		/// </summary>
		/// <param name="storedResults">a list containing the number of cards discarded</param>
		/// <returns></returns>
		protected IEnumerator DiscardOneOrMoreCards(List<int> storedResults)
		{
			if (base.HeroTurnTakerController.HasCardsInHand)
			{
				List<DiscardCardAction> storedResultsDiscard = new List<DiscardCardAction>();
				IEnumerator coroutine = base.SelectAndDiscardCards(base.HeroTurnTakerController, null, false, new int?(0), storedResultsDiscard, false, null, null, null, SelectionType.DiscardCard, null);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				storedResults.Add(base.GetNumberOfCardsDiscarded(storedResultsDiscard));
			}
			else
			{
				IEnumerator coroutine2 = base.GameController.SendMessageAction("Lady of the Wood has no cards in his hand to discard.", Priority.High, base.GetCardSource(null), null, false);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}
			yield break;
		}
	}
}
