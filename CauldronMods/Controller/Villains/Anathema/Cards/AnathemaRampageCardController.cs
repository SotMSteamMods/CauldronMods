using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Anathema
{
	public class AnathemaRampageCardController : CardController
    {
		public AnathemaRampageCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			SpecialStringMaker.ShowHeroWithMostCards(true);
		}

		public override IEnumerator Play()
		{

			//Find the hero with the most cards in hand
			List<TurnTaker> storedResults = new List<TurnTaker>();
			IEnumerator coroutine = base.FindHeroWithMostCardsInHand(storedResults);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			TurnTaker turnTaker = storedResults.FirstOrDefault();
			if (turnTaker != null)
			{
				List<Card> results = new List<Card>();
				coroutine = base.FindCharacterCardToTakeDamage(turnTaker, results, Card, base.H - 1, DamageType.Melee);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}

				//Anathema deals the Hero target with the most cards in hand {H-1} melee damage.
				IEnumerator coroutine2 = base.DealDamage(base.CharacterCard, results.First(), base.H - 1, DamageType.Melee, cardSource: base.GetCardSource());
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
			reduceDamageStatusEffect.TargetCriteria.IsVillain = true;
   			reduceDamageStatusEffect.TargetCriteria.IsTarget = true;
			reduceDamageStatusEffect.UntilStartOfNextTurn(this.TurnTaker);
			IEnumerator reduceCoroutine = this.AddStatusEffect(reduceDamageStatusEffect);

			IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
			increaseDamageStatusEffect.SourceCriteria.IsVillain = true;
   			increaseDamageStatusEffect.SourceCriteria.IsTarget = true;
			increaseDamageStatusEffect.UntilStartOfNextTurn(this.TurnTaker);
			IEnumerator increaseCoroutine = this.AddStatusEffect(increaseDamageStatusEffect);
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
