using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace SotMWorkshop.Controller.Necro
{
	public class HellfireCardController : CardController
	{
		public HellfireCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}


		public override void AddTriggers()
		{
			//When an Undead target is destroyed, Necro deals 1 non-hero target 3 infernal damage.
			base.AddTrigger<DestroyCardAction>((DestroyCardAction d) => this.IsUndead(d.CardToDestroy.Card) && d.WasCardDestroyed, new Func<DestroyCardAction, IEnumerator>(this.DealDamageResponse), TriggerType.DrawCard, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
		}

		private IEnumerator DealDamageResponse(DestroyCardAction dca)
		{
			//Necro deals 1 non - hero target 3 infernal damage.
			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 3, DamageType.Melee, new int?(1), false, new int?(1), false, false, false, (Card c) => !c.IsHero, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
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
