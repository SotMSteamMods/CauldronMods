using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    public class FutureGargoyleCharacterCardController : GargoyleUtilityCharacterCardController
    {
        private int HeroOngoingOrEquipmentAmount => GetPowerNumeral(0, 1);
        private int PlayerAmount => GetPowerNumeral(1, 1);

        public FutureGargoyleCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        /*
         * Power:
         * You may destroy 1 hero ongoing or equipment card. If you do, 1 player plays a card. Draw a card.
         */
        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<DestroyCardAction> storedDestroyCardActions = new List<DestroyCardAction>();

            // You may destroy 1 hero ongoing or equipment card. 
            coroutine = base.GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((card) => IsHero(card) && (IsOngoing(card) || base.IsEquipment(card)), "hero ongoing or equipment"), HeroOngoingOrEquipmentAmount, false, 0, storedResultsAction: storedDestroyCardActions, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(GetNumberOfCardsDestroyed(storedDestroyCardActions) >= HeroOngoingOrEquipmentAmount)
            {
                // If you do, 1 player plays a card
                coroutine = GameController.SelectTurnTakersAndDoAction(DecisionMaker,
                                                        new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt) && CanPlayCardsFromHand(GameController.FindHeroTurnTakerController(tt.ToHero()))),
                                                        SelectionType.PlayCard,
                                                        (TurnTaker tt) => SelectAndPlayCardFromHand(GameController.FindHeroTurnTakerController(tt.ToHero()), optional: false),
                                                        numberOfTurnTakers: PlayerAmount,
                                                        optional: false,
                                                        requiredDecisions: PlayerAmount,
                                                        cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            // Draw a card.
            coroutine = base.GameController.DrawCard(DecisionMaker.HeroTurnTaker);
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
         * Increase the next damage dealt to a non-hero target by 2.
         * Reduce the next damage dealt by a non-hero target by 2.
         * Shuffle 1 non-villain trash into its deck.
        */
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            IncreaseDamageStatusEffect increaseDamageStatusEffect;
            ReduceDamageStatusEffect reduceDamageStatusEffect;
            List<SelectLocationDecision> storedSelectLocationDecisions = new List<SelectLocationDecision>();

            switch (index)
            {
                case 0:
                    {
                        // Increase the next damage dealt to a non-hero target by 2.
                        increaseDamageStatusEffect = new IncreaseDamageStatusEffect(2);
                        increaseDamageStatusEffect.TargetCriteria.IsHero = false;
                        increaseDamageStatusEffect.TargetCriteria.IsTarget = true;
                        increaseDamageStatusEffect.NumberOfUses = 1;
                        coroutine = base.AddStatusEffect(increaseDamageStatusEffect);
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
                        // Reduce the next damage dealt by a non-hero target by 2.
                        reduceDamageStatusEffect = new ReduceDamageStatusEffect(2);
                        reduceDamageStatusEffect.SourceCriteria.IsHero = false;
                        reduceDamageStatusEffect.SourceCriteria.IsTarget = true;
                        reduceDamageStatusEffect.NumberOfUses = 1;
                        coroutine = base.AddStatusEffect(reduceDamageStatusEffect);
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
                        // Shuffle 1 non-villain trash into its deck.
                        coroutine = base.GameController.SelectATrash(DecisionMaker, SelectionType.ShuffleTrashIntoDeck, (location) => !location.IsVillain, storedSelectLocationDecisions, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (storedSelectLocationDecisions != null && storedSelectLocationDecisions.Count() > 0)
                        {
                            coroutine = base.GameController.ShuffleTrashIntoDeck(base.FindTurnTakerController(storedSelectLocationDecisions.FirstOrDefault().SelectedLocation.Location.OwnerTurnTaker));
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
