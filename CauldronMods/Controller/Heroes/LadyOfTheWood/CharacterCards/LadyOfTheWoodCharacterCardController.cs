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
			IEnumerator coroutine = base.GameController.SelectDamageType(base.HeroTurnTakerController, storedResults,cardSource: base.GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			DamageType damageType = GetSelectedDamageType(storedResults).Value;

			//LadyOfTheWood deals 1 target 1 damage of the chosen type.
			int target = base.GetPowerNumeral(0, 1);
			int damage = base.GetPowerNumeral(1, 1); 
			IEnumerator coroutine2 = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard),damage, damageType, new int?(target), false, new int?(target), cardSource: base.GetCardSource());
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
						IEnumerator coroutine = base.GameController.SelectDamageType(base.HeroTurnTakerController, storedResults, cardSource: base.GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}

						DamageType damageType = GetSelectedDamageType(storedResults).Value;

						//Until the start of your next turn, increase damage of the chosen type by 1.
						IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
						increaseDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
						increaseDamageStatusEffect.DamageTypeCriteria.AddType(damageType);
						IEnumerator coroutine2 = base.AddStatusEffect(increaseDamageStatusEffect);
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
				case 1:
					{
						//1 Hero may discard a card
						List<DiscardCardAction> storedResults2 = new List<DiscardCardAction>();
						IEnumerator coroutine3 = base.GameController.SelectHeroToDiscardCard(this.DecisionMaker, storedResultsDiscard: storedResults2, cardSource: base.GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine3);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine3);
						}

						// If a card was discard, they regain 3 HP
						if (base.DidDiscardCards(storedResults2, new int?(1)))
						{
							HeroTurnTakerController httc = storedResults2[0].HeroTurnTakerController;
							Card hero = httc.CharacterCard;
							if(httc.HasMultipleCharacterCards)
                            {
								List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
								coroutine3 = GameController.SelectCardAndStoreResults(httc, SelectionType.GainHP, new LinqCardCriteria(c => c.Owner == httc.TurnTaker && c.IsHeroCharacterCard && !c.IsIncapacitatedOrOutOfGame && c.IsInPlayAndHasGameText), storedResults, false, cardSource: GetCardSource());
								if (base.UseUnityCoroutines)
								{
									yield return base.GameController.StartCoroutine(coroutine3);
								}
								else
								{
									base.GameController.ExhaustCoroutine(coroutine3);
								}
								if(!DidSelectCard(storedResults))
                                {
									yield break;
                                }
								hero = GetSelectedCard(storedResults);
							}
							coroutine3 = base.GameController.GainHP(hero, new int?(3), cardSource: GetCardSource());
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
						LinqCardCriteria criteria = new LinqCardCriteria((Card c) => c.IsEnvironment && c.IsInPlay, "environment");
						IEnumerator coroutine4 = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, criteria, false, cardSource: base.GetCardSource());
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine4);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine4);
						}
						yield break;
					}
			}
			yield break;
		}
	}
}
