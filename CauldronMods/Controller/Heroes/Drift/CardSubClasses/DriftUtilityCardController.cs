using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public abstract class DriftUtilityCardController : DriftBaseCardController
    {
        protected DriftUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public IEnumerator ShiftL(List<bool> shiftCounter = null)
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

        public IEnumerator ShiftLL(List<bool> shiftCounter = null)
        {
            if (shiftCounter == null)
            {
                shiftCounter = new List<bool>();
            }
            for (int i = 0; i < 2; i++)
            {
                IEnumerator coroutine = this.ShiftL(shiftCounter);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if(shiftCounter.Count() <= i)
                {
                    //if we have already failed to shift we don't need to try again
                    break;
                }
            }
            yield break;
        }

        public IEnumerator ShiftLLL(List<bool> shiftCounter = null)
        {
            if(shiftCounter == null)
            {
                shiftCounter = new List<bool>();
            }
            for (int i = 0; i < 3; i++)
            {
                IEnumerator coroutine = this.ShiftL(shiftCounter);
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

        public IEnumerator ShiftRR(List<bool> shiftCounter = null)
        {
            if (shiftCounter == null)
            {
                shiftCounter = new List<bool>();
            }
            for (int i = 0; i < 2; i++)
            {
                IEnumerator coroutine = this.ShiftR(shiftCounter);
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

        public IEnumerator ShiftRRR(List<bool> shiftCounter = null)
        {
            if (shiftCounter == null)
            {
                shiftCounter = new List<bool>();
            }
            for (int i = 0; i < 3; i++)
            {
                IEnumerator coroutine = this.ShiftR(shiftCounter);
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
            IEnumerator coroutine = base.GameController.SwitchCards(this.GetShiftTrack(), base.FindCard(promoIdentifier + ShiftTrack + (this.CurrentShiftPosition() + newPositionModifier), false), cardSource: GetCardSource());
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
}
