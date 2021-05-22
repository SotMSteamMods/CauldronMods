using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.LadyOfTheWood
{
	public class FallCardController : CardController
    {
		public FallCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			//Whenever LadyOfTheWood deals lightning damage to a target, reduce damage dealt by that target by 1 until the start of your next turn.
			Func<DealDamageAction,bool> criteria = (DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsSameCard(base.CharacterCard) && dd.DamageType == DamageType.Lightning;
			AddTrigger(criteria, AddReduceDamageResponse, TriggerType.AddStatusEffectToDamage, TriggerTiming.Before);
		}

		private IEnumerator AddReduceDamageResponse(DealDamageAction dd)
		{
			Func<DealDamageAction, IEnumerator> statusEffectResponse = delegate (DealDamageAction dd2)
			{
				if (dd2.DidDealDamage)
				{
					IEnumerator enumerator = ReduceDamageDealtByThatTargetUntilTheStartOfYourNextTurnResponse(dd2, 1);
					if (base.UseUnityCoroutines)
					{
						return enumerator;
					}
					base.GameController.ExhaustCoroutine(enumerator);
					return DoNothing();
				}
				return DoNothing();
			};
			dd.AddStatusEffectResponse(statusEffectResponse);
			yield return null;
		}
	}
}
