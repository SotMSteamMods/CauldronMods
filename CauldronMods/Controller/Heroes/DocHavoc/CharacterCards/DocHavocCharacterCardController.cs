using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class DocHavocCharacterCardController : HeroCharacterCardController
    {
        private const int PowerDamageAmount = 3;

        public DocHavocCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //==============================================================
            // Adrenaline: Deals 1 hero 3 toxic damage. If that hero took damage this way, they may play a card now.
            //==============================================================

            //Deals 1 hero 3 toxic damage.
            DamageSource damageSource = new DamageSource(base.GameController, base.CharacterCard);
            int numTargets = base.GetPowerNumeral(1, 1);
            int amount = base.GetPowerNumeral(1, PowerDamageAmount);

            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker,
                damageSource,
                amount,
                DamageType.Toxic,
                new int?(numTargets),
                false,
                new int?(numTargets),
                additionalCriteria: ((Func<Card, bool>)(c => c.IsHeroCharacterCard)),
                addStatusEffect: new Func<DealDamageAction, IEnumerator>(this.OnIntendedHeroDamageResponse),
                cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator OnIntendedHeroDamageResponse(DealDamageAction dd)
        {
            //If that hero took damage this way, they may play a card now.
            if (dd != null && dd.OriginalTarget == dd.Target && dd.DidDealDamage)
            {
                Card targetHero = dd.Target;
                HeroTurnTakerController heroController = null;
                if (targetHero.IsHero)
                {
                    heroController = base.FindHeroTurnTakerController(targetHero.Owner.ToHero());
                }
                IEnumerator coroutine = base.SelectAndPlayCardFromHand(heroController, overrideName: targetHero.Title);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:

                    coroutine = DoIncapacitateOption1();

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    break;

                case 1:

                    coroutine = DoIncapacitateOption2();

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    break;

                case 2:

                    coroutine = DoIncapacitateOption3();

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    break;
            }
        }

        private IEnumerator DoIncapacitateOption1()
        {
            //==============================================================
            // Up to 3 hero targets regain 1 HP each.
            //==============================================================

            return this.GameController.SelectAndGainHP(this.DecisionMaker, 1, additionalCriteria: ((Func<Card, bool>)(c => c.IsHero)),
                numberOfTargets: 3, requiredDecisions: new int?(0), cardSource: this.GetCardSource());
        }

        private IEnumerator DoIncapacitateOption2()
        {
            //==============================================================
            // One player may draw a card now.
            //==============================================================

            return this.GameController.SelectHeroToDrawCard(this.DecisionMaker, cardSource: GetCardSource());
        }

        private IEnumerator DoIncapacitateOption3()
        {
            //==============================================================
            // Environment cards cannot deal damage until the start of your next turn..
            //==============================================================

            CannotDealDamageStatusEffect cannotDealDamageStatusEffect = new CannotDealDamageStatusEffect();
            cannotDealDamageStatusEffect.IsPreventEffect = true;
            cannotDealDamageStatusEffect.SourceCriteria.IsEnvironment = true;
            cannotDealDamageStatusEffect.UntilStartOfNextTurn(this.TurnTaker);

            return this.GameController.AddStatusEffect(cannotDealDamageStatusEffect, true, this.GetCardSource());
        }


		public IEnumerator FauxDefensiveBlast()
        {
			List<DealDamageAction> damages = new List<DealDamageAction>()
			{
				new DealDamageAction(type: DamageType.Melee, cardSource: GetCardSource(), damageSource: new DamageSource(base.GameController, base.CharacterCard), target: null, amount: 1),
				new DealDamageAction(type: DamageType.Projectile, cardSource: GetCardSource(), damageSource: new DamageSource(base.GameController, base.CharacterCard), target: null, amount: 1),
				new DealDamageAction(type: DamageType.Cold, cardSource: GetCardSource(), damageSource: new DamageSource(base.GameController, base.CharacterCard), target: null, amount: 1)
			};
		
			var coroutine = MyDealMultipleInstancesOfDamage(damages, (Card c) => !c.IsHero);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}
		}

		protected IEnumerator MyDealMultipleInstancesOfDamage(List<DealDamageAction> damageInfo, Func<Card, bool> targetCriteria, TargetInfo exceptFor = null, int? numberOfTargets = null, List<DealDamageAction> storedResults = null, bool? includeReturnedTargets = null, bool ignoreBattleZone = false)
		{
			if (!includeReturnedTargets.HasValue)
			{
				includeReturnedTargets = !numberOfTargets.HasValue;
			}
			IEnumerator coroutine = MySelectTargetsAndDealMultipleInstancesOfDamage(damageInfo, targetCriteria, exceptFor, null, numberOfTargets, false, storedResults, null, includeReturnedTargets.Value, ignoreBattleZone);
			if (UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}
		}

		public IEnumerator MySelectTargetsAndDealMultipleInstancesOfDamage(List<DealDamageAction> damageInfo, Func<Card, bool> targetCriteria = null, TargetInfo exceptFor = null, int? minNumberOfTargets = null, int? maxNumberOfTargets = null, bool evenIfCannotDealDamage = false, List<DealDamageAction> storedResultsAction = null, Func<int> dynamicNumberOfTargets = null, bool includeReturnedTargets = true, bool ignoreBattleZone = false, Func<DealDamageAction, IEnumerator> addStatusEffect = null)
		{
			List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
			bool autoDecide = false;
			DealDamageAction initialDamage = damageInfo.FirstOrDefault();
			Func<Card, bool> fullCriteria = (Card c) => c.IsInPlayAndHasGameText && c.IsTarget;
			if (targetCriteria != null)
			{
				fullCriteria = (Card c) => c.IsInPlayAndHasGameText && c.IsTarget && targetCriteria(c);
			}
			DealDamageAction dealDamageAction = damageInfo.FirstOrDefault();
			if (dealDamageAction != null && dealDamageAction.DamageSource != null && dealDamageAction.DamageSource.IsCard && exceptFor != null)
			{
				List<Card> exceptions = new List<Card>();
				IEnumerator targets2 = exceptFor.GetTargets(GameController, exceptions, new DealDamageAction(GetCardSource(), dealDamageAction.DamageSource, null, dealDamageAction.Amount, dealDamageAction.DamageType, dealDamageAction.IsIrreducible), evenIfCannotDealDamage, SelectionType.CharacterCard, GetCardSource());
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(targets2);
				}
				else
				{
					GameController.ExhaustCoroutine(targets2);
				}
				fullCriteria = (Card c) => c.IsInPlayAndHasGameText && c.IsTarget && targetCriteria(c) && !exceptions.Contains(c);
			}
			bool? cardSourceFlipped = false;
			if (initialDamage.CardSource != null && initialDamage.CardSource.Card != null)
			{
				cardSourceFlipped = initialDamage.CardSource.Card.IsFlipped;
			}
			bool? isInPlay = true;
			if (initialDamage.DamageSource != null && initialDamage.DamageSource.IsCard)
			{
				isInPlay = initialDamage.DamageSource.Card.IsInPlay;
			}
			int? damagesPerTarget = null;
			if (damageInfo.Count() > 1)
			{
				damagesPerTarget = damageInfo.Count();
			}
			if (initialDamage == null)
			{
				yield break;
			}
			int? sequenceIndex = null;
			int value = FindCardsWhere(fullCriteria, false, null, ignoreBattleZone).Count();
			if (maxNumberOfTargets.HasValue)
			{
				value = maxNumberOfTargets.Value;
			}
			if (!maxNumberOfTargets.HasValue && dynamicNumberOfTargets == null)
			{
				sequenceIndex = 1;
			}
			if (!GameController.MultipleInstancesOfDamageOutput.ContainsKey(Card))
			{
				GameController.MultipleInstancesOfDamageOutput.Add(Card, value);
			}
			bool skipped = false;
			for (int i = 0; ShouldContinueDealingMultipleInstancesOfDamage(initialDamage, fullCriteria, storedResults, i, maxNumberOfTargets, skipped, isInPlay, cardSourceFlipped, dynamicNumberOfTargets, damagesPerTarget, includeReturnedTargets, ignoreBattleZone); i++)
			{
				List<DealDamageAction> followUp = null;
				if (damageInfo.Count() > 1)
				{
					followUp = new List<DealDamageAction>();
					for (int j = 1; j < damageInfo.Count(); j++)
					{
						followUp.Add(damageInfo.ElementAt(j));
					}
				}
				bool allowAutoDecide = true;
				bool flag = false;
				List<DealDamageAction> storedDamages = new List<DealDamageAction>();
				/*
				 * Predicate bug, calling FindCardsWhere without the CardSource here means Visibility isn't correctly calculated
				 *                                                                                vvvv
				 * IEnumerable<Card> targets = FindCardsWhere((Card c) => fullCriteria(c), false, null, ignoreBattleZone);
				 */

				//IEnumerable<Card> targets = FindCardsWhere((Card c) => fullCriteria(c), false, null, ignoreBattleZone);
				IEnumerable<Card> targets = FindCardsWhere((Card c) => fullCriteria(c), false, GetCardSource(), ignoreBattleZone);
				targets = targets.Where((Card c) => !storedResults.Any((SelectCardDecision d) => d.SelectedCard == c && (!d.PlayIndexOfSelectedCard.HasValue || !c.PlayIndex.HasValue || d.PlayIndexOfSelectedCard.Value == c.PlayIndex.Value) && (c.IsCharacter || d.IsSelectedCardFlipped == c.IsFlipped)));
				int num = targets.Count();
				if (minNumberOfTargets.HasValue && minNumberOfTargets.Value <= num)
				{
					allowAutoDecide = false;
				}
				else if (dynamicNumberOfTargets != null && dynamicNumberOfTargets() <= num)
				{
					allowAutoDecide = false;
				}
				if (minNumberOfTargets.HasValue && minNumberOfTargets.Value <= i)
				{
					flag = true;
				}
				IEnumerator enumerator = null;
				if (initialDamage.DamageSource == null)
				{
					int value2 = ((!flag) ? 1 : 0);
					enumerator = GameController.SelectTargetsToDealDamageToSelf(DecisionMaker, initialDamage.Amount, initialDamage.DamageType, 1, false, value2, initialDamage.IsIrreducible, storedResultsDecisions: storedResults, allowAutoDecide: allowAutoDecide, autoDecide: autoDecide, additionalCriteria: (Card c) => targets.Contains(c), storedResultsDamage: storedDamages, followUpDamageInformation: followUp, addStatusEffect: addStatusEffect, selectTargetsEvenIfCannotDealDamage: false, overrideSequenceIndex: sequenceIndex, stopDealingDamage: null, cardSource: GetCardSource());
				}
				else if (initialDamage.DamageSource.IsCard)
				{
					int value3 = ((!flag) ? 1 : 0);
					enumerator = GameController.SelectTargetsAndDealDamage(DecisionMaker, initialDamage.DamageSource, initialDamage.Amount, initialDamage.DamageType, 1, false, value3, initialDamage.IsIrreducible, storedResultsDecisions: storedResults, allowAutoDecide: allowAutoDecide, autoDecide: autoDecide, additionalCriteria: (Card c) => targets.Contains(c), storedResultsDamage: storedDamages, followUpDamageInformation: followUp, addStatusEffect: addStatusEffect, selectTargetsEvenIfCannotDealDamage: false, overrideSequenceIndex: sequenceIndex, stopDealingDamage: null, ignoreBattleZone: ignoreBattleZone, damageSourceInfo: null, cardSource: GetCardSource());
				}
				else if (initialDamage.DamageSource.IsTurnTaker)
				{
					enumerator = GameController.SelectTargetsAndDealDamage(DecisionMaker, initialDamage.DamageSource, initialDamage.Amount, initialDamage.DamageType, 1, flag, (!flag) ? 1 : 0, initialDamage.IsIrreducible, allowAutoDecide, autoDecide, (Card c) => targets.Contains(c), storedResults, storedDamages, followUp, addStatusEffect, false, sequenceIndex, null, ignoreBattleZone, null, GetCardSource());
				}
				if (enumerator != null)
				{
					if (UseUnityCoroutines)
					{
						yield return GameController.StartCoroutine(enumerator);
					}
					else
					{
						GameController.ExhaustCoroutine(enumerator);
					}
				}
				if (storedResultsAction != null && storedDamages.Count() > 0)
				{
					storedResultsAction.AddRange(storedDamages);
				}

				DealDamageAction initialDealt = storedDamages.FirstOrDefault();
				SelectCardDecision mostRecent = storedResults.LastOrDefault();
				/*
				 * Should be checking initialDealt != null here.
				 */
				if (mostRecent != null)
				{
					if (mostRecent.SelectedCard != null)
					{
						/*
						 * Exception here, storedDamages is empty, so initialDealt is null so initialDealt.Target throws.
					     */
						int? playIndexWhenInitiated = null;
						if (mostRecent.SelectedCard == initialDealt.Target)
						{
							playIndexWhenInitiated = mostRecent.PlayIndexOfSelectedCard;
						}
						foreach (DealDamageAction item in followUp)
						{
							Card target = mostRecent.SelectedCard;
							if (target.SharedIdentifier != null && target.IsOffToTheSide)
							{
								/*
								 * Missing CardSource: 
								 * Card card = FindCardsWhere((Card c) => c.SharedIdentifier == target.SharedIdentifier && c.IsInPlay).FirstOrDefault();
								 */
								//Card card = FindCardsWhere((Card c) => c.SharedIdentifier == target.SharedIdentifier && c.IsInPlay).FirstOrDefault();
								Card card = FindCardsWhere((Card c) => c.SharedIdentifier == target.SharedIdentifier && c.IsInPlay, visibleToCard: GetCardSource()).FirstOrDefault();
								if (card != null)
								{
									target = card;
								}
							}
							if (ShouldDealFollowUpDamage(initialDealt, mostRecent, playIndexWhenInitiated, target) && fullCriteria(target))
							{
								AddAssociatedCardSources(item.CardSource.AssociatedCardSources);
								enumerator = null;
								if (item.DamageSource == null)
								{
									enumerator = DealDamage(target, target, item.Amount, item.DamageType, item.IsIrreducible, false, false, addStatusEffect, storedResultsAction, null, ignoreBattleZone, item.CardSource);
								}
								else if (item.DamageSource.IsCard)
								{
									enumerator = DealDamage(item.DamageSource.Card, target, item.Amount, item.DamageType, item.IsIrreducible, false, false, addStatusEffect, storedResultsAction, null, ignoreBattleZone, item.CardSource);
								}
								else if (item.DamageSource.IsTurnTaker)
								{
									enumerator = GameController.DealDamageToTarget(item.DamageSource, target, item.Amount, item.DamageType, item.IsIrreducible, false, storedResultsAction, addStatusEffect, null, null, false, false, item.CardSource);
								}
								if (UseUnityCoroutines)
								{
									yield return GameController.StartCoroutine(enumerator);
								}
								else
								{
									GameController.ExhaustCoroutine(enumerator);
								}
								RemoveAssociatedCardSources();
							}
							if (mostRecent.AutoDecided)
							{
								autoDecide = true;
							}
						}
					}
					else
					{
						skipped = true;
					}
				}
				if (sequenceIndex.HasValue)
				{
					sequenceIndex++;
				}
			}
			GameController.MultipleInstancesOfDamageOutput.Remove(Card);
			if (initialDamage.DamageSource == null || !initialDamage.DamageSource.IsCard)
			{
				yield break;
			}
			CardSource cardSource = GameController.CanDealDamage(initialDamage.DamageSource.Card, true, GetCardSource());
			if (cardSource != null)
			{
				string message = $"{cardSource.Card.Title} prevented {initialDamage.DamageSource.Card.Title} from dealing damage.";
				IEnumerator coroutine = GameController.SendMessageAction(message, Priority.High, GetCardSource(), new Card[1]
				{
					cardSource.Card
				});
				if (UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
				}
			}
		}

		private bool ShouldDealFollowUpDamage(DealDamageAction initialDamage, SelectCardDecision mostRecent, int? playIndexWhenInitiated, Card overrideTarget = null)
		{
			bool flag = true;
			DamageSource damageSource = initialDamage.DamageSource;
			Card card = overrideTarget;
			if (card == null)
			{
				card = mostRecent.SelectedCard;
			}
			if (initialDamage.DamageSource == null && mostRecent != null && card != null)
			{
				damageSource = new DamageSource(GameController, card);
			}
			if (damageSource.IsCard)
			{
				flag = GameController.CanDealDamage(damageSource.Card, true, GetCardSource()) == null;
			}
			bool flag2 = mostRecent != null && card != null && card.IsInPlayAndHasGameText && !GameController.IsGameOver && flag && !GameController.IsInhibited(initialDamage.CardSource.CardController);
			if (flag2 && card.PlayIndex.HasValue && playIndexWhenInitiated.HasValue)
			{
				flag2 &= card.PlayIndex.Value == playIndexWhenInitiated.Value;
			}
			if (flag2 && !card.IsCharacter && initialDamage.OriginalTarget == initialDamage.Target)
			{
				flag2 &= card.IsFlipped == initialDamage.TargetWasFlipped;
			}
			return flag2;
		}

		private bool ShouldContinueDealingMultipleInstancesOfDamage(DealDamageAction initialDamage, Func<Card, bool> cardCriteria, List<SelectCardDecision> storedResults, int currentTargetIndex, int? maxNumberOfTargets, bool skipped, bool? isInPlay, bool? cardSourceFlipped, Func<int> dynamicNumberOfTargets = null, int? damagesPerTarget = null, bool includeReturnedTargets = false, bool ignoreBattleZone = false)
		{
			/*
			 * Missing CardSource
			 */

			IEnumerable<Card> source = FindCardsWhere((Card c) => cardCriteria(c), false, GetCardSource(), ignoreBattleZone).Where((Card c) => !storedResults.Any((SelectCardDecision d) => d.SelectedCard == c && (!includeReturnedTargets || !d.PlayIndexOfSelectedCard.HasValue || !c.PlayIndex.HasValue || d.PlayIndexOfSelectedCard.Value == c.PlayIndex.Value) && (c.IsCharacter || d.IsSelectedCardFlipped == c.IsFlipped)));
			int num = 0;
			if (maxNumberOfTargets.HasValue)
			{
				int num2 = ((!damagesPerTarget.HasValue) ? 1 : damagesPerTarget.Value);
				num = maxNumberOfTargets.Value * num2;
			}
			return (!GameController.IsGameOver && !skipped) & ((!maxNumberOfTargets.HasValue || currentTargetIndex < maxNumberOfTargets.Value) && (dynamicNumberOfTargets == null || currentTargetIndex < dynamicNumberOfTargets())) & !GameController.IsInhibited(initialDamage.CardSource.CardController) & (source.Count() > 0) & (initialDamage.DamageSource == null || !initialDamage.DamageSource.IsCard || !isInPlay.HasValue || isInPlay.Value == initialDamage.DamageSource.Card.IsInPlay) & (initialDamage.DamageSource == null || (GameController.HowManyTimesIsDamagePrevented(initialDamage.DamageSource).HasValue && (!maxNumberOfTargets.HasValue || GameController.HowManyTimesIsDamagePrevented(initialDamage.DamageSource).Value < num))) & (initialDamage.CardSource == null || initialDamage.CardSource.Card == null || !cardSourceFlipped.HasValue || cardSourceFlipped.Value == initialDamage.CardSource.Card.IsFlipped);
		}
	}
}
