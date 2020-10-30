using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace SotMWorkshop.Controller.LadyOfTheWood
{
	public class FallCardController : CardController
    {
		public FallCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource.IsSameCard(base.CharacterCard) && dd.DamageType == DamageType.Lightning, new Func<DealDamageAction, IEnumerator>(this.ReduceDamageResponse), new TriggerType[]
			{
				TriggerType.ReduceDamage
			}, TriggerTiming.After, null, false, true, new bool?(false), false, null, null, false, false);
		}

		private IEnumerator ReduceDamageResponse(DealDamageAction dd)
		{
			if (dd.DidDealDamage)
			{
				ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(base.GetPowerNumeral(2, 1));
				reduceDamageStatusEffect.SourceCriteria.IsSpecificCard = dd.Target;
				reduceDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
				IEnumerator coroutine = base.AddStatusEffect(reduceDamageStatusEffect, true);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			yield break;
		}
	}
}
