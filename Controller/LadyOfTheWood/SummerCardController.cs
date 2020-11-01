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
			Func<DealDamageAction, int> amountToIncrease = (DealDamageAction dd) => 2;
			base.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(base.CharacterCard) && dd.DamageType == DamageType.Fire, amountToIncrease, false);
		}
	}
}
