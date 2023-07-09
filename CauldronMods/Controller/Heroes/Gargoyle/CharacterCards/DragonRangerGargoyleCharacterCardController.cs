using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    public class DragonRangerGargoyleCharacterCardController : GargoyleUtilityCharacterCardController
    {
        private int TargetsAmount => GetPowerNumeral(0, 3);
        private int ToxicDamageAmount => GetPowerNumeral(1, 1);

        public DragonRangerGargoyleCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        /*
         * Power
         * {Gargoyle} deals 3 targets 1 toxic damage each. Increase the next damage he deals by the number of heroes damaged this way.
         */
        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<DealDamageAction> storedResultsDamage = new List<DealDamageAction>();
            IncreaseDamageStatusEffect increaseDamageStatusEffect;
            int totalHeroesDamaged = 0;

            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), ToxicDamageAmount, DamageType.Toxic, TargetsAmount, false, TargetsAmount, storedResultsDamage: storedResultsDamage, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResultsDamage != null && storedResultsDamage.Any((DealDamageAction dd) => dd.DidDealDamage && IsHeroCharacterCard(dd.Target)))
            {
                totalHeroesDamaged = storedResultsDamage.Where((dd) => dd.DidDealDamage && IsHeroCharacterCard(dd.Target)).Select(dd => dd.Target).Distinct().Count();

                increaseDamageStatusEffect = new IncreaseDamageStatusEffect(totalHeroesDamaged);
                increaseDamageStatusEffect.SourceCriteria.IsSpecificCard = base.CharacterCard;
                increaseDamageStatusEffect.NumberOfUses = 1;
                increaseDamageStatusEffect.UntilTargetLeavesPlay(base.CharacterCard);

                coroutine = base.AddStatusEffect(increaseDamageStatusEffect);
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

        /* 
         * Incap
         * One player may draw a card now.
         * One hero may use a power now.
         * Select a target. Prevent the next damage it would deal.
        */
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            IEnumerable<Card> choices = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource())));
            List<SelectTargetDecision> selectTargetResults = new List<SelectTargetDecision>();
            CannotDealDamageStatusEffect cannotDealDamageStatusEffect;

            switch (index)
            {
                case 0:
                    {
                        // One player may draw a card now.
                        coroutine = base.GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: base.GetCardSource());
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
                        // One hero may use a power now.
                        coroutine = base.GameController.SelectHeroToUsePower(DecisionMaker, cardSource: base.GetCardSource());
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
                case 2:
                    {
                        // Select a target. Prevent the next damage it would deal.
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
                    }
                    break;
            }

            yield break;
        }
    }
}
