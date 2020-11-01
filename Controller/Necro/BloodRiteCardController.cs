using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Necro
{
	public class BloodRiteCardController : CardController
	{
		public BloodRiteCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}


		public override void AddTriggers()
		{
			//When an Undead target is destroyed, all non-undead hero targets regain 2 HP.
			base.AddTrigger<DestroyCardAction>((DestroyCardAction d) => this.IsUndead(d.CardToDestroy.Card) && d.WasCardDestroyed, new Func<DestroyCardAction, IEnumerator>(this.GainHPResponse), TriggerType.DrawCard, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
		}

		private IEnumerator GainHPResponse(DestroyCardAction dca)
		{
			//all non-undead hero targets regain 2 HP.
			int powerNumeral = base.GetPowerNumeral(0, 2);
			IEnumerator coroutine = base.GameController.GainHP(base.HeroTurnTakerController, (Card c) => c.IsHero && !this.IsUndead(c), powerNumeral, null, false, null, null, null, base.GetCardSource(null)); if (base.UseUnityCoroutines)
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
