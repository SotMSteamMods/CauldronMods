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

			//X is 2 plus the number of cards discarded
			int X;
			if(discardedCards.Count > 0)
			{
				X = discardedCards.First<int>() + 2;
			}
			else
			{
				X = 2;
			}
			//Increase the next damage dealt by {LadyOfTheWood} by X
			IEnumerator coroutine2 = base.AddStatusEffect(new IncreaseDamageStatusEffect(X)
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
			});
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
				IEnumerator coroutine = base.SelectAndDiscardCards(base.HeroTurnTakerController, null, false, new int?(0), storedResultsDiscard);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}

				if(storedResultsDiscard != null)
				{
					storedResults.Add(base.GetNumberOfCardsDiscarded(storedResultsDiscard));

				}
				
			}
			else
			{
				IEnumerator coroutine2 = base.GameController.SendMessageAction("Lady of the Wood has no cards in her hand to discard.", Priority.High, base.GetCardSource());
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
