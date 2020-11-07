using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Necro
{
	public class DarkPactCardController : NecroCardController
	{
		public DarkPactCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
		{
			//Put an undead card from hand into play.
			IEnumerator coroutine = base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, false, null, new LinqCardCriteria((Card c) => this.IsUndead(c), "undead", true, false, null, null, false), true, false, false, null);
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

		public override void AddTriggers()
		{
			//Whenever an undead target is destroyed, draw a card.
			AddUndeadDestroyedTrigger(DrawCardResponse, TriggerType.DrawCard);
		}

		private IEnumerator DrawCardResponse(DestroyCardAction dca)
		{
			//draw a card
			IEnumerator coroutine = base.GameController.DrawCard(base.HeroTurnTaker, false, null, true, null, null);
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
	}
}
