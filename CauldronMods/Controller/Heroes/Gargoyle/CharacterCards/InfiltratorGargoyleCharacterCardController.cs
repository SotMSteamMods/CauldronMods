using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    public class InfiltratorGargoyleCharacterCardController : GargoyleUtilityCharacterCardController
    {
        private int TargetsAmount => GetPowerNumeral(0, 2);
        private int ToxicDamageAmount => GetPowerNumeral(1, 1);

        public InfiltratorGargoyleCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }


        /*
         * Power
         * {Gargoyle} deals 2 targets 1 toxic damage each. For each hero damaged this way, draw or play a card.
         */
        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<SelectCardDecision> storedResultsDecisions = new List<SelectCardDecision>();
            List<DealDamageAction> storedDamage = new List<DealDamageAction>();

            // {Gargoyle} deals 2 targets 1 toxic damage each.
            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), ToxicDamageAmount, DamageType.Toxic, TargetsAmount, false, TargetsAmount, storedResultsDamage: storedDamage, storedResultsDecisions: storedResultsDecisions, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var damagedHeroes = storedDamage.Where(dd => dd.DidDealDamage && IsHeroCharacterCard(dd.Target)).Select(dd => dd.Target).Distinct();
            if (damagedHeroes.Any())
            {
                // For each hero damaged this way, draw or play a card.
                foreach (var hero in damagedHeroes)
                {
                    coroutine = base.DrawACardOrPlayACard(DecisionMaker, false);
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

            yield break;
        }

        /* 
         * Incap
         * One player may play a card now.
         * Discard the top card of 1 deck.
         * 1 target deals itself 1 toxic damage. Another regains 1 HP.
        */
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine; 
            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            List<SelectCardDecision> storedDecisonResults = new List<SelectCardDecision>();
            Func<Card, bool> additionalCriteria = (card) => GameController.IsCardVisibleToCardSource(card, GetCardSource());

            switch (index)
            {
                case 0:
                    {
                        // One player may play a card now.
                        coroutine = base.GameController.SelectHeroToPlayCard(DecisionMaker, cardSource: base.GetCardSource());
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
                        // Discard the top card of 1 deck. 
                        coroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.DiscardFromDeck, (Location deck) => true, storedResults, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (storedResults != null && storedResults.Count() > 0)
                        {
                            coroutine = base.GameController.DiscardTopCardsOfLocations(DecisionMaker, new List<Location> { storedResults.FirstOrDefault().SelectedLocation.Location }, 1, cardSource: base.GetCardSource());
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
                        // 1 target deals itself 1 toxic damage. 
                        coroutine = base.GameController.SelectTargetsToDealDamageToSelf(DecisionMaker, 1, DamageType.Toxic, 1, false, 1, storedResultsDecisions: storedDecisonResults, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        // Another regains 1 HP.
                        if (storedDecisonResults != null && storedDecisonResults.Count() > 0)
                        {
                            additionalCriteria = (card) => card != storedDecisonResults.FirstOrDefault().SelectedCard && GameController.IsCardVisibleToCardSource(card, GetCardSource());
                        }

                        coroutine = base.GameController.SelectAndGainHP(DecisionMaker, 1, additionalCriteria: additionalCriteria, cardSource: GetCardSource());
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
