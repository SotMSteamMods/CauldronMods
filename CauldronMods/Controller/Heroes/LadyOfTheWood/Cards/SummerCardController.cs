using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.LadyOfTheWood
{
	public class SummerCardController : CardController
    {
		public SummerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			//Increase fire damage dealt by LadyOfTheWood by 2
			Func<DealDamageAction, bool> criteria = (DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsSameCard(base.CharacterCard) && dd.DamageType == DamageType.Fire;
			base.AddIncreaseDamageTrigger(criteria, (DealDamageAction dd) => 2);
		}
	}
}
