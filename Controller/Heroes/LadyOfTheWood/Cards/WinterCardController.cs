using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.LadyOfTheWood
{
	public class WinterCardController : CardController
    {
		public WinterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			//Whenever LadyOfTheWood deals cold damage to a target, draw a card.
			Func<DealDamageAction, bool> criteria = (DealDamageAction dd) => dd.DamageSource != null &&  dd.DamageSource.IsSameCard(base.CharacterCard) && dd.DamageType == DamageType.Cold;
			base.AddTrigger<DealDamageAction>(criteria, new Func<DealDamageAction, IEnumerator>(this.DrawCardResponse), new TriggerType[]{ TriggerType.DrawCard	}, TriggerTiming.After);
		}

		private IEnumerator DrawCardResponse(DealDamageAction dd)
		{
			//Whenever LadyOfTheWood deals cold damage to a target, draw a card.
			if (dd.DidDealDamage)
			{
				IEnumerator coroutine = base.DrawCard();
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
			yield break;
		}
	}
}
