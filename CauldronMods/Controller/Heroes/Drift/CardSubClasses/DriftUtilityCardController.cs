using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public abstract class DriftUtilityCardController : DriftBaseCardController
    {
        protected DriftUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private readonly static string ShiftTrackPositionKeyword = "ShiftTrackPosition";

        protected enum CustomMode
        {
            AskToShift,
            DualDriftShift
        }

        protected CustomMode customMode { get; set; }

        public IEnumerator ShiftL(List<bool> shiftCounter = null)
        {
            if (FindCardController(GetShiftTrack()) is DualShiftTrackUtilityCardController dualShiftTrack && !dualShiftTrack.HasTrackAbilityBeenActivated())
            {
                IEnumerator coroutine = DualDriftShifting(shiftCounter, dualShiftTrack, ShiftLAction, "{ShiftL}");
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

            }
            else
            {
                IEnumerator coroutine = ShiftLAction(shiftCounter);
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

        public IEnumerator ShiftLL(List<bool> shiftCounter = null)
        {
            if (FindCardController(GetShiftTrack()) is DualShiftTrackUtilityCardController dualShiftTrack && !dualShiftTrack.HasTrackAbilityBeenActivated())
            {
                IEnumerator coroutine = DualDriftShifting(shiftCounter, dualShiftTrack, ShiftLLAction, "{ShiftLL}");
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

            }
            else
            {
                IEnumerator coroutine = ShiftLLAction(shiftCounter);
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

        public IEnumerator ShiftLLL(List<bool> shiftCounter = null)
        {
            if (FindCardController(GetShiftTrack()) is DualShiftTrackUtilityCardController dualShiftTrack && !dualShiftTrack.HasTrackAbilityBeenActivated())
            {
                IEnumerator coroutine = DualDriftShifting(shiftCounter, dualShiftTrack, ShiftLLLAction, "{ShiftLLL}");
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

            }
            else
            {
                IEnumerator coroutine = ShiftLLLAction(shiftCounter);
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

        public IEnumerator ShiftLAction(List<bool> shiftCounter = null)
        {

            if (shiftCounter == null)
            {
                shiftCounter = new List<bool>();
            }
            //Ensures not shifting off track
            if (this.CurrentShiftPosition() > 1)
            {
                base.SetCardPropertyToTrueIfRealAction(HasShifted);

                //Switch to the new card
                IEnumerator coroutine = this.SwitchTrack(-1);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = base.GameController.RemoveTokensFromPool(this.GetShiftPool(), 1, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (shiftCounter != null)
                {
                    shiftCounter.Add(true);
                }
            }
            else
            {
                IEnumerator coroutine = base.GameController.SendMessageAction("Drift has reached the end of the Shift Track", Priority.Medium, base.GetCardSource());
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


        public IEnumerator ShiftLLAction(List<bool> shiftCounter = null)
        {
            if (shiftCounter == null)
            {
                shiftCounter = new List<bool>();
            }
            for (int i = 0; i < 2; i++)
            {
                IEnumerator coroutine = this.ShiftLAction(shiftCounter);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (shiftCounter.Count() <= i)
                {
                    //if we have already failed to shift we don't need to try again
                    break;
                }
            }
            yield break;
        }

        public IEnumerator ShiftLLLAction(List<bool> shiftCounter = null)
        {
            if (shiftCounter == null)
            {
                shiftCounter = new List<bool>();
            }
            for (int i = 0; i < 3; i++)
            {
                IEnumerator coroutine = this.ShiftLAction(shiftCounter);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (shiftCounter.Count() <= i)
                {
                    //if we have already failed to shift we don't need to try again
                    break;
                }
            }
            yield break;
        }

        public IEnumerator ShiftR(List<bool> shiftCounter = null)
        {
            if (FindCardController(GetShiftTrack()) is DualShiftTrackUtilityCardController dualShiftTrack && !dualShiftTrack.HasTrackAbilityBeenActivated())
            {
                IEnumerator coroutine = DualDriftShifting(shiftCounter, dualShiftTrack, ShiftRAction, "{ShiftR}");
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                IEnumerator coroutine = ShiftRAction(shiftCounter);
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

        public IEnumerator ShiftRR(List<bool> shiftCounter = null)
        {
            if (FindCardController(GetShiftTrack()) is DualShiftTrackUtilityCardController dualShiftTrack && !dualShiftTrack.HasTrackAbilityBeenActivated())
            {
                IEnumerator coroutine = DualDriftShifting(shiftCounter, dualShiftTrack, ShiftRRAction, "{ShiftRR}");
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                IEnumerator coroutine = ShiftRRAction(shiftCounter);
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

        public IEnumerator ShiftRRR(List<bool> shiftCounter = null)
        {
            if (FindCardController(GetShiftTrack()) is DualShiftTrackUtilityCardController dualShiftTrack && !dualShiftTrack.HasTrackAbilityBeenActivated())
            {
                IEnumerator coroutine = DualDriftShifting(shiftCounter, dualShiftTrack, ShiftRRRAction, "{ShiftRRR}");
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                IEnumerator coroutine = ShiftRRRAction(shiftCounter);
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

        private IEnumerator DualDriftShifting(List<bool> shiftCounter, DualShiftTrackUtilityCardController dualShiftTrack, Func<List<bool>, IEnumerator> shiftAction, string shiftString)
        {
            string option1 = "Shift " + shiftString;
            string option2 = "Swap the active Drift, then Shift " + shiftString;
            string option3 = "Shift " + shiftString + ", then swap the active Drift";
            string[] options = new string[] { option1, option2, option3 };

            List<SelectWordDecision> storedResults = new List<SelectWordDecision>();
            SelectWordDecision decision = new SelectWordDecision(GameController, DecisionMaker, SelectionType.Custom, options, cardSource: GetCardSource(), associatedCards: GetShiftTrack().ToEnumerable());
            decision.ExtraInfo = () => $"{dualShiftTrack.GetInactiveCharacterCard().Title} is at position {dualShiftTrack.InactiveCharacterPosition()}";
            storedResults.Add(decision);
            customMode = CustomMode.DualDriftShift;
            IEnumerator coroutine = GameController.MakeDecisionAction(decision);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (!DidSelectWord(storedResults))
            {
                coroutine = shiftAction(shiftCounter);
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

            string selectedWord = GetSelectedWord(storedResults);
            if (selectedWord == option2)
            {
                coroutine = dualShiftTrack.SwapActiveDrift();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            coroutine = shiftAction(shiftCounter);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (selectedWord == option3)
            {
                coroutine = dualShiftTrack.SwapActiveDrift();
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

        public IEnumerator ShiftRAction(List<bool> shiftCounter = null)
        {
            if (shiftCounter == null)
            {
                shiftCounter = new List<bool>();
            }
            //Ensures not shifting off track
            if (this.CurrentShiftPosition() < 4)
            {
                base.SetCardPropertyToTrueIfRealAction(HasShifted);

                //Switch to the new card
                IEnumerator coroutine = this.SwitchTrack(1);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = base.GameController.AddTokensToPool(this.GetShiftPool(), 1, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (shiftCounter != null)
                {
                    shiftCounter.Add(true);
                }
            }
            else
            {
                IEnumerator coroutine = base.GameController.SendMessageAction("Drift has reached the end of the Shift Track", Priority.Medium, base.GetCardSource());
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

        public IEnumerator ShiftRRAction(List<bool> shiftCounter = null)
        {
            if (shiftCounter == null)
            {
                shiftCounter = new List<bool>();
            }
            for (int i = 0; i < 2; i++)
            {
                IEnumerator coroutine = this.ShiftRAction(shiftCounter);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (shiftCounter.Count() <= i)
                {
                    //if we have already failed to shift we don't need to try again
                    break;
                }
            }
            yield break;
        }

        public IEnumerator ShiftRRRAction(List<bool> shiftCounter = null)
        {
            if (shiftCounter == null)
            {
                shiftCounter = new List<bool>();
            }
            for (int i = 0; i < 3; i++)
            {
                IEnumerator coroutine = this.ShiftRAction(shiftCounter);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (shiftCounter.Count() <= i)
                {
                    //if we have already failed to shift we don't need to try again
                    break;
                }
            }
            yield break;
        }

        private IEnumerator SwitchTrack(int newPositionModifier = 0)
        {
            string promoIdentifier = Base;
            if (base.CharacterCardController is DualDriftSubCharacterCardController)
            {
                promoIdentifier = Dual;
            }
            else if (base.CharacterCardController is ThroughTheBreachDriftCharacterCardController)
            {
                promoIdentifier = ThroughTheBreach;
            }

            Card currentShiftTrack = this.GetShiftTrack();
            Card newShiftTrack = base.FindCard(promoIdentifier + ShiftTrack + (this.CurrentShiftPosition() + newPositionModifier), false);
            CardSource cardSource = GetCardSource();
            IEnumerator coroutine = base.GameController.SwitchCards(currentShiftTrack, newShiftTrack, cardSource: cardSource);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            if (customMode == CustomMode.DualDriftShift)
            {
                return new CustomDecisionText($"What do you want to do?", $"What should they do?", $"Vote for what they should do.", $"what to do");
            }

            return base.GetCustomDecisionText(decision);
        }
    }
}
