using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SotMWorkshop.Controller.Anathema
{
	public class AnathemaRampageCardController : CardController
    {
		public AnathemaRampageCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override IEnumerator Play()
		{

			//Find the hero with the most cards in hand
			List<TurnTaker> storedResults = new List<TurnTaker>();
			IEnumerator coroutine = base.FindHeroWithMostCardsInHand(storedResults, 1, 1, null, null, false, false);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			TurnTaker turnTaker = storedResults.FirstOrDefault<TurnTaker>();
			if (turnTaker != null)
			{
				//Anathema deals the Hero target with the most cards in hand {H-1} melee damage.
				HeroTurnTakerController hero = base.FindHeroTurnTakerController(turnTaker as HeroTurnTaker);
				IEnumerator coroutine2 = base.DealDamage(base.CharacterCard, hero.CharacterCard, base.H - 1, DamageType.Melee, false, false, false, null, null, null, false, base.GetCardSource(null));
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}
			//Until the start of the next Villain Turn, increase damage dealt by Villain targets by 1 and reduce damage dealt to Villain targets by 1.
			ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(1);
			reduceDamageStatusEffect.TargetCriteria.IsVillain = true; ;
			reduceDamageStatusEffect.UntilStartOfNextTurn(this.TurnTaker);
			IEnumerator reduceCoroutine = this.AddStatusEffect(reduceDamageStatusEffect, true);

			IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
			increaseDamageStatusEffect.SourceCriteria.IsVillain = true; ;
			increaseDamageStatusEffect.UntilStartOfNextTurn(this.TurnTaker);
			IEnumerator increaseCoroutine = this.AddStatusEffect(increaseDamageStatusEffect, true);
			if (this.UseUnityCoroutines)
			{
				yield return this.GameController.StartCoroutine(reduceCoroutine);
				yield return this.GameController.StartCoroutine(increaseCoroutine);
			}
			else
			{
				this.GameController.ExhaustCoroutine(reduceCoroutine);
				this.GameController.ExhaustCoroutine(increaseCoroutine);
			}
			yield break;
		}

		

	}
}
