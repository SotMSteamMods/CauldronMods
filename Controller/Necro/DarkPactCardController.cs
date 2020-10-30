using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace SotMWorkshop.Controller.Necro
{
	public class DarkPactCardController : CardController
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
			base.AddTrigger<DestroyCardAction>((DestroyCardAction d) => this.IsUndead(d.CardToDestroy.Card) && d.WasCardDestroyed, new Func<DestroyCardAction, IEnumerator>(this.DrawCardResponse), TriggerType.DrawCard, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
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

		private bool IsUndead(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "undead", false, false);
		}
	}
}
