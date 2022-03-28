using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.LadyOfTheWood
{
	public class SpringCardController : CardController
    {
		public SpringCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			//Whenever LadyOfTheWood deals toxic damage to a target, she regains that much HP.
			Func<DealDamageAction, bool> criteria = (DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsSameCard(base.CharacterCard) && dd.DamageType == DamageType.Toxic && dd.Amount > 0;
			base.AddTrigger<DealDamageAction>(criteria, (DealDamageAction dd) => base.GameController.GainHP(base.CharacterCard, new int?(dd.Amount), cardSource: base.GetCardSource()), new TriggerType[]{ TriggerType.GainHP }, TriggerTiming.After);
		}
	}
}
