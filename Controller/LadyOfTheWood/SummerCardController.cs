using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace SotMWorkshop.Controller.LadyOfTheWood
{
	public class SummerCardController : CardController
    {
		public SummerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			Func<DealDamageAction, int> amountToIncrease = (DealDamageAction dd) => 2;
			base.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(base.CharacterCard) && dd.DamageType == DamageType.Fire, amountToIncrease, false);
		}
	}
}
