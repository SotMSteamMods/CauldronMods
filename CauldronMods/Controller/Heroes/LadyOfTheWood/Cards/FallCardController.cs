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
			base.AddTrigger<DealDamageAction>(criteria,
				(DealDamageAction dd) => AddStatusEffectResponseToDamage(dd, (DealDamageAction damage) =>
					ReduceDamageDealtByThatTargetUntilTheStartOfYourNextTurnResponse(damage, 1), requireDamageToBeDealt: true
				), new TriggerType[]
			{
				TriggerType.ReduceDamage
			}, TriggerTiming.Before);
		}
		protected IEnumerator AddStatusEffectResponseToDamage(DealDamageAction dd, Func<DealDamageAction, IEnumerator> statusEffectResponse, bool requireDamageToBeDealt = false)
		{
			if (requireDamageToBeDealt)
			{
				Func<DealDamageAction, IEnumerator> statusEffectResponse2 = (DealDamageAction dd2) => dd2.DidDealDamage ? statusEffectResponse(dd2) : DoNothing();
				dd.AddStatusEffectResponse(statusEffectResponse2);
			}
			else
			{
				dd.AddStatusEffectResponse(statusEffectResponse);
			}
			yield return null;
		}
	}
}
