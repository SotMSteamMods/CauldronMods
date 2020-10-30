using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SotMWorkshop.Controller.LadyOfTheWood
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
			IEnumerator coroutine = base.GameController.SelectDamageType(base.HeroTurnTakerController, storedResults, new DamageType[]
			{
				DamageType.Cold,
				DamageType.Energy,
				DamageType.Lightning,
				DamageType.Infernal,
				DamageType.Projectile,
				DamageType.Psychic,
				DamageType.Sonic,
				DamageType.Toxic,
				DamageType.Melee,
				DamageType.Fire,
				DamageType.Radiant
			}, null, SelectionType.DamageType, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			//LadyOfTheWood deals 1 target 1 damage of the chosen type.
			DamageType value = storedResults.First((SelectDamageTypeDecision d) => d.Completed).SelectedDamageType.Value;
			int powerNumeral = base.GetPowerNumeral(0, 1);
			IEnumerator coroutine2 = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), powerNumeral, value, new int?(powerNumeral), false, new int?(powerNumeral), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
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
						IEnumerator coroutine = base.GameController.SelectDamageType(base.HeroTurnTakerController, storedResults, new DamageType[]
						{
					DamageType.Cold,
					DamageType.Energy,
					DamageType.Lightning,
					DamageType.Infernal,
					DamageType.Projectile,
					DamageType.Psychic,
					DamageType.Sonic,
					DamageType.Toxic,
					DamageType.Melee,
					DamageType.Fire,
					DamageType.Radiant
						}, null, SelectionType.DamageType, base.GetCardSource(null));
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						//Until the start of your next turn, increase damage of the chosen type by 1.
						DamageType value = storedResults.First((SelectDamageTypeDecision d) => d.Completed).SelectedDamageType.Value;
						IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
						increaseDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
						increaseDamageStatusEffect.DamageTypeCriteria.AddType(value);
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
