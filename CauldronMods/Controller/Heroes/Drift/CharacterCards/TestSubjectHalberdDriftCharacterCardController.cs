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
            string[] discardNumerals = new string[] { discard1Numeral.ToString(), discard2Numeral.ToString(), discard3Numeral.ToString() };

            int drawNumeral = base.GetPowerNumeral(3, 2);

            //Discard 1, 2, or 3 cards.
            List<SelectWordDecision> numberDecision = new List<SelectWordDecision>();
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
            coroutine = base.SelectAndDiscardCards(base.HeroTurnTakerController, Convert.ToInt32(numberDecision.FirstOrDefault().SelectedWord), storedResults: discardActions);
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
                    new Function(base.HeroTurnTakerController, "Shift {ShiftL}", SelectionType.RemoveTokens, () => base.ShiftL()),
                    new Function(base.HeroTurnTakerController, "Shift {ShiftR}", SelectionType.AddTokens, () => base.ShiftR())
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

        private IEnumerator PickDiscards(string[] discardNumerals, List<SelectWordDecision> numberDecision)
        {
            IEnumerator coroutine = GameController.SelectWord(HeroTurnTakerController, discardNumerals, SelectionType.Custom, storedResults: numberDecision, optional: false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //One player may draw a card now.
                        coroutine = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && !tt.IsIncapacitated), cardSource: base.GetCardSource());
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

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText("How many cards do you want to discard?", "How many cards should they discard?", "Vote for the number of cards they should discard?", "number of cards to discard");

        }
    }
}
