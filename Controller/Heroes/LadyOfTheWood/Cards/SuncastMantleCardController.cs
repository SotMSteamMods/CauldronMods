using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Cauldron.LadyOfTheWood
{
	public class SuncastMantleCardController : CardController
    {
		public SuncastMantleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//Increase damage dealt by LadyOfTheWood by 3 as long as her HP is 5 or less.
			base.AddIncreaseDamageTrigger(delegate (DealDamageAction dealDamage)
			{
				if (dealDamage.DamageSource.IsSameCard(base.CharacterCard))
				{
					int? hitPoints = base.CharacterCard.HitPoints;
					int num = 5;
					return hitPoints.GetValueOrDefault() <= num & hitPoints != null;
				}
				return false;
			}, 3, null, null, false);
		}

		public override IEnumerator UsePower(int index = 0)
		{
			//LadyOfTheWood deals herself 1 fire damage
			IEnumerator coroutine = base.DealDamage(base.CharacterCard, base.CharacterCard, base.GetPowerNumeral(0, 1), DamageType.Fire, false, false, false, null, null, null, false, null);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			//LadyOfTheWood deals 1 target 4 fire damage
			int powerNumeral = base.GetPowerNumeral(1, 1);
			int powerNumeral2 = base.GetPowerNumeral(2, 4);
			coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), powerNumeral2, DamageType.Fire, new int?(powerNumeral), false, new int?(powerNumeral), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
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
	}
}
