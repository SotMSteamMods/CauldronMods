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
			base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource.IsSameCard(base.CharacterCard) && dd.DamageType == DamageType.Cold, new Func<DealDamageAction, IEnumerator>(this.DrawCardResponse), new TriggerType[]
			{
				TriggerType.DrawCard
			}, TriggerTiming.After, null, false, true, new bool?(false), false, null, null, false, false);
		}

		private IEnumerator DrawCardResponse(DealDamageAction dd)
		{
			//Whenever LadyOfTheWood deals cold damage to a target, draw a card.
			if (dd.DidDealDamage)
			{
				IEnumerator coroutine = base.DrawCard(null, false, null, true);
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
