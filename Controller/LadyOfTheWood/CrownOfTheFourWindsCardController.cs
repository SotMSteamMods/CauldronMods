using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SotMWorkshop.Controller.LadyOfTheWood
{
	public class CrownOfTheFourWindsCardController : CardController
    {
		public CrownOfTheFourWindsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator UsePower(int index = 0)
		{
			//LadyOfTheWood deals 1 target 1 toxic damage, a second target 1 fire damage, a third target 1 lightning damage, and a fourth target 1 cold damage.
			List<DealDamageAction> targets = new List<DealDamageAction>();
			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Toxic, new int?(1), false, new int?(1), false, false, false, null, null, targets, null, null, false, null, null, false, null, base.GetCardSource(null));
			IEnumerator secondDamage = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Fire, new int?(1), false, new int?(1), false, false, false, (Card c) => !(from d in targets
																																																															 select d.Target).Contains(c), null, targets, null, null, false, null, null, false, null, base.GetCardSource(null));
			IEnumerator thirdDamage = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Lightning, new int?(1), false, new int?(1), false, false, false, (Card c) => !(from d in targets
																																																																 select d.Target).Contains(c), null, targets, null, null, false, null, null, false, null, base.GetCardSource(null));
			IEnumerator fourthDamage = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Cold, new int?(1), false, new int?(1), false, false, false, (Card c) => !(from d in targets
																																																															 select d.Target).Contains(c), null, targets, null, null, false, null, null, false, null, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
				yield return base.GameController.StartCoroutine(secondDamage);
				yield return base.GameController.StartCoroutine(thirdDamage);
				yield return base.GameController.StartCoroutine(fourthDamage);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
				base.GameController.ExhaustCoroutine(secondDamage);
				base.GameController.ExhaustCoroutine(thirdDamage);
				base.GameController.ExhaustCoroutine(fourthDamage);
			}
			yield break;
		}
	}
}
