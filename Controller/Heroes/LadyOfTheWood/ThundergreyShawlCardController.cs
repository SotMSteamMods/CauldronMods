using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.LadyOfTheWood
{
	public class ThundergreyShawlCardController : CardController
	{
		public ThundergreyShawlCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			//Whenever LadyOfTheWood deals 2 or less damage to a target, that damage is irreducible.
			base.AddMakeDamageIrreducibleTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(base.CharacterCard) && dd.Amount <= 2);
		}

		public override IEnumerator UsePower(int index = 0)
		{
			//LadyOfTheWood deals up to 2 targets 1 lightning damage each.
			int powerNumeral = base.GetPowerNumeral(0, 2);
			int powerNumeral2 = base.GetPowerNumeral(1, 1);
			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), powerNumeral2, DamageType.Lightning, new int?(powerNumeral), false, new int?(0), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
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

		public override bool CanOrderAffectOutcome(GameAction action)
		{
			return action is DealDamageAction &&  (action as DealDamageAction).DamageSource.Card == base.CharacterCard;
		}
	}
}
