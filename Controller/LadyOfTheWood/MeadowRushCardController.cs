using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace SotMWorkshop.Controller.LadyOfTheWood
{
	public class MeadowRushCardController : CardController
	{
		public MeadowRushCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator Play()
		{
			//Draw a card.
			IEnumerator coroutine = base.DrawCard(null, false, null, true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			//Search your deck for a Season card. Put it into your hand, then shuffle your deck.
			IEnumerator coroutine2 = base.SearchForCards(this.DecisionMaker, true, false, new int?(1), 1, new LinqCardCriteria((Card c) => this.IsSeason(c), "season", true, false, null, null, false), false, true, false, false, null, false, null, null);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}

			//You may play a card.
			IEnumerator coroutine3 = base.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, null, null, false, false, true, null);
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

		/// <summary>
		/// Check if a given card is a Season
		/// </summary>
		/// <param name="card">card to check</param>
		/// <returns></returns>
		private bool IsSeason(Card card)
		{
			return card.DoKeywordsContain("season", false, false);
		}
	}
}
