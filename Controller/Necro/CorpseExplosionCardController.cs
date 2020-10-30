using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace SotMWorkshop.Controller.Necro
{
	public class CorpseExplosionCardController : CardController
	{
		public CorpseExplosionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}


		public override void AddTriggers()
		{
			//When an Undead target is destroyed, Necro deals 2 toxic damage to all villain targets.
			base.AddTrigger<DestroyCardAction>((DestroyCardAction d) => this.IsUndead(d.CardToDestroy.Card) && d.WasCardDestroyed, new Func<DestroyCardAction, IEnumerator>(this.DealDamageResponse), TriggerType.DrawCard, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
		}

		private IEnumerator DealDamageResponse(DestroyCardAction dca)
		{
			//Necro deals 2 toxic damage to all villain targets.
			IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card card) => card.IsVillainTarget, 2, DamageType.Toxic, false, false, null, null, null, false, null, null, false, false);
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
