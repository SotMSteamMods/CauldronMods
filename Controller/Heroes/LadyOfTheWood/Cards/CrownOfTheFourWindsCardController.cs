using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.LadyOfTheWood
{
	public class CrownOfTheFourWindsCardController : CardController
    {
		public CrownOfTheFourWindsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator UsePower(int index = 0)
		{

			int damage1 = GetPowerNumeral(0, 1);
			int damage2 = GetPowerNumeral(1, 1);
			int damage3 = GetPowerNumeral(2, 1);
			int damage4 = GetPowerNumeral(3, 1);
			//LadyOfTheWood deals one target 1 toxic damage, a second target 1 fire damage, a third target 1 lightning damage, and a fourth target 1 cold damage.
			List<DealDamageAction> targets = new List<DealDamageAction>();
			IEnumerator firstDamage = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), damage1, DamageType.Toxic, new int?(1), false, new int?(1), storedResultsDamage: targets, cardSource: base.GetCardSource());
			IEnumerator secondDamage = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), damage2, DamageType.Fire, new int?(1), false, new int?(1), additionalCriteria: (Card c) => !(from d in targets
																																																															 select d.Target).Contains(c), storedResultsDamage: targets, cardSource: base.GetCardSource());
			IEnumerator thirdDamage = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), damage3, DamageType.Lightning, new int?(1), false, new int?(1), additionalCriteria: (Card c) => !(from d in targets
																																																																select d.Target).Contains(c), storedResultsDamage: targets, cardSource: base.GetCardSource());
			IEnumerator fourthDamage = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), damage4, DamageType.Cold, new int?(1), false, new int?(1), additionalCriteria: (Card c) => !(from d in targets
																																																															select d.Target).Contains(c), storedResultsDamage: targets, cardSource: base.GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(firstDamage);
				yield return base.GameController.StartCoroutine(secondDamage);
				yield return base.GameController.StartCoroutine(thirdDamage);
				yield return base.GameController.StartCoroutine(fourthDamage);
			}
			else
			{
				base.GameController.ExhaustCoroutine(firstDamage);
				base.GameController.ExhaustCoroutine(secondDamage);
				base.GameController.ExhaustCoroutine(thirdDamage);
				base.GameController.ExhaustCoroutine(fourthDamage);
			}
			yield break;
		}
	}
}
