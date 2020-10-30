using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SotMWorkshop.Controller.Necro
{
	public class NecroCharacterCardController : HeroCharacterCardController
	{
		public NecroCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
		{
			//Necro deals 1 target 1 toxic damage or 1 Undead target 2 toxic damage.
			List<Function> list = new List<Function>();
			list.Add(new Function(this.DecisionMaker, "Deal 1 target 1 toxic damage", SelectionType.DealDamage, () => base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Toxic, new int?(1), false, new int?(1), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null)), null, null, "Deal 1 target 1 toxic damage"));
			list.Add(new Function(this.DecisionMaker, "Deal 1 Undead target 2 toxic damage", SelectionType.DealDamage, () => base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Toxic, new int?(1), false, new int?(1), false, false, false, (Card c) => this.IsUndead(c), null, null, null, null, false, null, null, false, null, base.GetCardSource(null)), new bool?(true), null, "Deal 1 undead target 2 toxic damage"));
			SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, this.DecisionMaker, list, false, null, null, null, base.GetCardSource(null));
			IEnumerator coroutine = base.GameController.SelectAndPerformFunction(selectFunction, null, null);
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
		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{
						//One hero may deal himself 2 toxic damage to draw 2 cards now.

						//Select a Hero
						List<SelectCardDecision> storedHero;
						storedHero = new List<SelectCardDecision>();
						IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.DealDamageSelf, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsHero && c.IsCharacter && !c.IsIncapacitatedOrOutOfGame, "hero", false, false, null, null, false), storedHero, false, false, null, true, base.GetCardSource(null));
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						SelectCardDecision selectCardDecision = storedHero.FirstOrDefault<SelectCardDecision>();
						
						if (selectCardDecision != null && selectCardDecision.SelectedCard != null)
						{
							//Ask hero if they want to deal damage to themselves
							Card heroCard = selectCardDecision.SelectedCard;
							HeroTurnTakerController hero = base.FindHeroTurnTakerController(heroCard.Owner.ToHero());
							List<DealDamageAction> storedResults = new List<DealDamageAction>();
							Card thisCard = this.Card;

							IEnumerator coroutine2 = base.GameController.DealDamageToSelf(hero, (Card c) => c == heroCard, 2, DamageType.Toxic, false, storedResults, true, null, null, base.GetCardSource(null)); 
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine2);	
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine2);
							}
							DealDamageAction dealDamageAction = storedResults.FirstOrDefault<DealDamageAction>();
							if (dealDamageAction != null && dealDamageAction.CardSource != null )
							{
								//if they did deal damage to themselves, they draw 2 cards
								IEnumerator coroutine3 = base.DrawCards(hero, 2, false, false, null, true, null);
								if (base.UseUnityCoroutines)
								{
									yield return base.GameController.StartCoroutine(coroutine3);
								}
								else
								{
									base.GameController.ExhaustCoroutine(coroutine3);
								}
							}
							hero = null;
							storedResults = null;
						}
						yield break;
					}
				case 1:
					{
						//One hero may discard up to 3 cards, then regain 2 HP for each card discarded.

						//Select hero to discard cards
						List<DiscardCardAction> storedResult = new List<DiscardCardAction>();
						IEnumerator coroutine = base.GameController.SelectHeroToDiscardCards(this.DecisionMaker, 0, new int?(3), false, false, false, null, null, null, storedResult, null, false, base.GetCardSource(null));
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						int numCardsDiscarded = storedResult.Count<DiscardCardAction>();
						if (numCardsDiscarded > 0)
						{
							//Have selected hero gain HP equal to 2 times the number of cards discarded
							DiscardCardAction discardCardAction = storedResult.FirstOrDefault<DiscardCardAction>();
							IEnumerator coroutine2 = base.GameController.GainHP(discardCardAction.HeroTurnTakerController.CharacterCard, new int?(numCardsDiscarded * 2), null, null, base.GetCardSource(null));
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine2);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine2);
							}
							discardCardAction = null;
						}
						else
						{
							//no cards were discarded, send message to inform player of result
							IEnumerator coroutine3 = base.GameController.SendMessageAction("No cards were discarded, so no HP is gained.", Priority.Medium, base.GetCardSource(null), null, false);
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine3);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine3);
							}
						}
						break;

					}
				case 2:
					{
						//Select a hero target. Increase damage dealt by that target by 3 and increase damage dealt to that target by 2 until the start of your next turn.
						
						//Select a hero
						List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
						IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.IncreaseDamage, new LinqCardCriteria((Card c) => !c.IsIncapacitatedOrOutOfGame && c.IsHeroCharacterCard, "hero character", true, false, null, null, false), storedResults, false, false, null, true, base.GetCardSource(null));
						if (base.UseUnityCoroutines)
						{
							yield return base.GameController.StartCoroutine(coroutine);
						}
						else
						{
							base.GameController.ExhaustCoroutine(coroutine);
						}
						SelectCardDecision selectCardDecision = storedResults.FirstOrDefault<SelectCardDecision>();
						if (selectCardDecision != null && selectCardDecision.SelectedCard != null)
						{
							//Until start of turn, increase damage dealt by that selected card by 3
							IncreaseDamageStatusEffect increaseDamageDealtStatusEffect = new IncreaseDamageStatusEffect(3);
							increaseDamageDealtStatusEffect.SourceCriteria.IsSpecificCard = selectCardDecision.SelectedCard;
							increaseDamageDealtStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
							IEnumerator coroutine2 = base.AddStatusEffect(increaseDamageDealtStatusEffect, true);

							//Until start of turn, increase damage dealt to that selected card by 2
							IncreaseDamageStatusEffect increaseDamageTakenStatusEffect = new IncreaseDamageStatusEffect(2);
							increaseDamageTakenStatusEffect.TargetCriteria.IsSpecificCard = selectCardDecision.SelectedCard;
							increaseDamageTakenStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
							IEnumerator coroutine3 = base.AddStatusEffect(increaseDamageTakenStatusEffect, true);

							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine2);
								yield return base.GameController.StartCoroutine(coroutine3);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine2);
								base.GameController.ExhaustCoroutine(coroutine3);
							}
						}
						break;
					}
			}
			yield break;
		}

		private bool IsUndead(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "undead", false, false);
		}
	}
}
