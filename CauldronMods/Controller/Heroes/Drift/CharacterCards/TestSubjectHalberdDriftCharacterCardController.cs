using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class TestSubjectHalberdDriftCharacterCardController : DriftSubCharacterCardController
    {
        public TestSubjectHalberdDriftCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Discard 1, 2, or 3 cards. For each card discarded this way, shift {DriftL} or {DriftR}. Draw 2 cards.
            int discard1Numeral = base.GetPowerNumeral(0, 1);
            int discard2Numeral = base.GetPowerNumeral(1, 2);
            int discard3Numeral = base.GetPowerNumeral(2, 3);
            int[] discardNumerals = new int[] { discard1Numeral, discard2Numeral, discard3Numeral };

            int drawNumeral = base.GetPowerNumeral(3, 2);

            //Discard 1, 2, or 3 cards.
            List<SelectNumberDecision> numberDecision = new List<SelectNumberDecision>();
            IEnumerator coroutine = PickDiscards(discardNumerals, numberDecision);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            List<DiscardCardAction> discardActions = new List<DiscardCardAction>();
            coroutine = base.SelectAndDiscardCards(base.HeroTurnTakerController, numberDecision.FirstOrDefault().SelectedNumber, storedResults: discardActions);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //For each card discarded this way...
            foreach (DiscardCardAction action in discardActions)
            {
                //...shift {DriftL}, {DriftR}. 
                coroutine = base.SelectAndPerformFunction(base.HeroTurnTakerController, new Function[] {
                    new Function(base.HeroTurnTakerController, "Shift Left", SelectionType.RemoveTokens, () => base.ShiftL()),
                    new Function(base.HeroTurnTakerController, "Shift Right", SelectionType.AddTokens, () => base.ShiftR())
            }, associatedCards: GetShiftTrack().ToEnumerable());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //Draw 2 cards.
            coroutine = base.GameController.DrawCards(base.HeroTurnTakerController, drawNumeral, cardSource: base.GetCardSource());
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

        private IEnumerator PickDiscards(int[] discardNumerals, List<SelectNumberDecision> numberDecision)
        {
            IEnumerator coroutine = base.GameController.SelectNumber(base.HeroTurnTakerController, SelectionType.DiscardCard, 0, 4, additionalCriteria: (int i) => discardNumerals.Contains(i), storedResults: numberDecision, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(!discardNumerals.Any(i => i ==numberDecision.FirstOrDefault().SelectedNumber))
            {
                numberDecision.Clear();
                coroutine = GameController.SendMessageAction("Please pick a valid position!", Priority.High, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = PickDiscards(discardNumerals, numberDecision);
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
                    {
                        //One player may draw a card now.
                        coroutine = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => tt.IsHero && !tt.IsIncapacitatedOrOutOfGame && !tt.IsIncapacitated), cardSource: base.GetCardSource());
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
                        //One hero may use a power now.
                        coroutine = base.GameController.SelectHeroToUsePower(base.HeroTurnTakerController, cardSource: base.GetCardSource());
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
                        //Select a target. Prevent the next damage dealt to it.
                        List<SelectCardDecision> cardDecisions = new List<SelectCardDecision>();
                        coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.PreventDamage, new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlayAndHasGameText), cardDecisions, false, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        Card selectedTarget = cardDecisions.FirstOrDefault().SelectedCard;
                        ImmuneToDamageStatusEffect statusEffect = new ImmuneToDamageStatusEffect();
                        statusEffect.TargetCriteria.IsSpecificCard = selectedTarget;
                        statusEffect.NumberOfUses = 1;
                        statusEffect.UntilTargetLeavesPlay(selectedTarget);

                        coroutine = base.AddStatusEffect(statusEffect);
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
            yield break;
        }
    }
}
