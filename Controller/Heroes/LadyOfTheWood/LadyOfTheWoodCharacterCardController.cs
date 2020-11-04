using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.LadyOfTheWood
{
	public class LadyOfTheWoodCharacterCardController : HeroCharacterCardController
	{
		public LadyOfTheWoodCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
		{
			//Select a damage type.
			List<SelectDamageTypeDecision> storedResults = new List<SelectDamageTypeDecision>();
			IEnumerator coroutine = base.GameController.SelectDamageType(base.HeroTurnTakerController, storedResults, null, null, SelectionType.DamageType, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			DamageType damageType = storedResults.First((SelectDamageTypeDecision d) => d.Completed).SelectedDamageType.Value;

			//LadyOfTheWood deals 1 target 1 damage of the chosen type.
			int powerNumeral = base.GetPowerNumeral(0, 1);
			IEnumerator coroutine2 = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), powerNumeral, damageType, new int?(powerNumeral), false, new int?(powerNumeral), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
			yield break;
		}
		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						//Select a damage type.
						List<SelectDamageTypeDecision> storedResults = new List<SelectDamageTypeDecision>();
						IEnumerator coroutine = base.GameController.SelectDamageType(base.HeroTurnTakerController, storedResults, null, null, SelectionType.DamageType, base.GetCardSource(null));
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						DamageType damageType = storedResults.First((SelectDamageTypeDecision d) => d.Completed).SelectedDamageType.Value;
						//Until the start of your next turn, increase damage of the chosen type by 1.
						IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
						increaseDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
						increaseDamageStatusEffect.DamageTypeCriteria.AddType(damageType);
						IEnumerator coroutine2 = base.AddStatusEffect(increaseDamageStatusEffect, true);
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine2);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine2);
						}
						break;
					}
				case 1:
					{
						//1 Hero may discard a card
						List<DiscardCardAction> storedResults2 = new List<DiscardCardAction>();
						IEnumerator coroutine3 = base.GameController.SelectHeroToDiscardCard(this.DecisionMaker, true, false, false, null, null, null, storedResults2, null, SelectionType.DiscardCard, base.GetCardSource(null));
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine3);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine3);
						}

						// If a card was discard, they regain 3 HP
						if (base.DidDiscardCards(storedResults2, new int?(1), false))
						{
							coroutine3 = base.GameController.GainHP(storedResults2[0].HeroTurnTakerController.CharacterCard, new int?(3), null, null, null);
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine3);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine3);
							}
						}
						yield break;
					}
				case 2:
					{
						//Destroy an environment card.
						IEnumerator coroutine4 = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsEnvironment, "environment", true, false, null, null, false), false, null, null, base.GetCardSource(null));
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine4);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine4);
						}
						break;
					}
			}
			yield break;
		}
	}
}
