using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    public class WastelandRoninGargoyleCharacterCardController : GargoyleUtilityCharacterCardController
    {
        private int PowerTargetAmount => GetPowerNumeral(0, 1);
        private int PowerDamageAmount => GetPowerNumeral(1, 2);

        public WastelandRoninGargoyleCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        /*
         * Power:
         * {Gargoyle} deals 1 target 2 toxic damage.
         */
        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerable<Card> choices = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText));
            IEnumerator coroutine;

            // Select a target. 
            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), PowerDamageAmount, DamageType.Toxic, PowerTargetAmount, false, PowerTargetAmount, cardSource: base.GetCardSource());
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

        /* 
         * Incap
         * One player may play a card now.
         * Select a target. Prevent the next damage that would be dealt to that target and by that target.
         * One hero target deals itself 2 toxic damage. Another regains 2 HP.
        */
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            List<SelectTargetDecision> selectTargetResults = new List<SelectTargetDecision>();
            List<SelectCardDecision> selectCardResults = new List<SelectCardDecision>();
            CannotDealDamageStatusEffect cannotDealDamageStatusEffect;
            ImmuneToDamageStatusEffect immuneToDamageStatusEffect;
            IEnumerable<Card> choices = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource())));

            switch (index)
            {
                case 0:
                    {
                        // One player may play a card now.
                        coroutine = base.SelectHeroToPlayCard(DecisionMaker);
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
                case 1:
                    {
                        // Select a target. Prevent the next damage that would be dealt to that target and by that target.
                        coroutine = base.GameController.SelectTargetAndStoreResults(DecisionMaker, choices, selectTargetResults, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (selectTargetResults != null && selectTargetResults.Count() > 0)
                        {
                            // Prevent the next damage that would be dealt to that target...
                            immuneToDamageStatusEffect = new ImmuneToDamageStatusEffect();
                            immuneToDamageStatusEffect.TargetCriteria.IsSpecificCard = selectTargetResults.FirstOrDefault().SelectedCard;
                            immuneToDamageStatusEffect.NumberOfUses = 1;
                            coroutine = base.AddStatusEffect(immuneToDamageStatusEffect);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }

                            // ... and by that target
                            cannotDealDamageStatusEffect = new CannotDealDamageStatusEffect();
                            cannotDealDamageStatusEffect.SourceCriteria.IsSpecificCard = selectTargetResults.FirstOrDefault().SelectedCard;
                            cannotDealDamageStatusEffect.NumberOfUses = 1;
                            coroutine = base.AddStatusEffect(cannotDealDamageStatusEffect);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;
                    }
                case 2:
                    {
                        // One hero target deals itself 2 toxic damage. Another regains 2 HP.
                        coroutine = GameController.SelectTargetsToDealDamageToSelf(DecisionMaker, 2, DamageType.Toxic, 1, false, 1, additionalCriteria: (Card c) => IsHeroTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), storedResultsDecisions: selectCardResults, selectTargetsEvenIfCannotDealDamage: true, cardSource: GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        // Another regains 2 HP.
                        var selectedCard = GetSelectedCard(selectCardResults);
                        coroutine = base.GameController.SelectAndGainHP(DecisionMaker, 2, additionalCriteria: (card) => IsHeroTarget(card) && !(card == selectedCard) && GameController.IsCardVisibleToCardSource(card, GetCardSource()));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        
                    }
                    break;
            }
            yield break;
        }
    }
}
