using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Anathema
{
	public class BoneCleaverCardController : ArmCardController
    {
		public BoneCleaverCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			SpecialStringMaker.ShowHeroTargetWithLowestHP();
		}

		public override void AddTriggers()
		{
			//At the end of the Villain Turn, Anathema deals the Hero target with the lowest HP {H-2} melee damage. If that target took damage this way, it cannot deal damage until the start of the next Villain Turn.

			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
		}

		private IEnumerator DealDamageResponse(PhaseChangeAction phaseChange)
		{
			//Anathema deals the Hero target with the lowest HP {H-2} melee damage.
			//If that target took damage this way, it cannot deal damage until the start of the next Villain Turn.
			IEnumerator coroutine = base.DealDamageToLowestHP(base.CharacterCard, 1, (Card c) => IsHeroTarget(c), (Card c) => new int?(base.H - 2), DamageType.Melee,addStatusEffect: base.TargetsDealtDamageCannotDealDamageUntilTheStartOfNextTurnResponse);
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
