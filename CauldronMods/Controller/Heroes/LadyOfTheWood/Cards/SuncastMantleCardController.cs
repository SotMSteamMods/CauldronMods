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
				if (dealDamage.DamageSource != null && dealDamage.DamageSource.IsSameCard(base.CharacterCard))
				{
					int? hitPoints = base.CharacterCard.HitPoints;
					int num = 5;
					return hitPoints != null && hitPoints.GetValueOrDefault() <= num;
				}
				return false;
			}, 3);
		}

		public override IEnumerator UsePower(int index = 0)
		{
			//LadyOfTheWood deals herself 1 fire damage
			int selfDamage = base.GetPowerNumeral(0, 1);
			IEnumerator coroutine = base.DealDamage(base.CharacterCard, base.CharacterCard, selfDamage, DamageType.Fire, cardSource: GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			//LadyOfTheWood deals 1 target 4 fire damage
			int targets = base.GetPowerNumeral(1, 1);
			int damage = base.GetPowerNumeral(2, 4);
			coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), damage, DamageType.Fire, new int?(targets), false, new int?(targets), cardSource: base.GetCardSource());
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
