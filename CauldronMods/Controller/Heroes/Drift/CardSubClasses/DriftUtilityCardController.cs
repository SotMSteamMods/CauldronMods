using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public abstract class DriftUtilityCardController : CardController
    {
        protected DriftUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected const string Past = "Past";
        protected const string Future = "Future";
        protected const string HasShifted = "HasShifted";

        public int CurrentShiftPosition()
        {
            return 0;
        }

        public Card GetActiveCharacterCard()
        {
            return base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.Location == base.TurnTaker.PlayArea && c.Owner == base.TurnTaker)).FirstOrDefault();
        }

        public Card GetShiftTrack()
        {
            return base.FindCard("ShiftTrack");
        }

        public bool IsFocus(Card c)
        {
            return c.DoKeywordsContain("focus");
        }

        public bool IsTimeMatching(string time)
        {
            if (this.CurrentShiftPosition() == 1 || this.CurrentShiftPosition() == 2)
            {
                return time == Past;
            }
            if (this.CurrentShiftPosition() == 3 || this.CurrentShiftPosition() == 4)
            {
                return time == Future;
            }
            return false;
        }

        public int MaxLeftShifts()
        {
            return this.CurrentShiftPosition() - 1;
        }

        public int MaxRightShifts()
        {
            return 4 - this.CurrentShiftPosition();
        }

        public IEnumerator ShiftL()
        {
            base.SetCardPropertyToTrueIfRealAction(HasShifted);
            yield break;
        }

        public IEnumerator ShiftLL()
        {
            IEnumerator coroutine = this.ShiftL();
            IEnumerator coroutine2 = this.ShiftL();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }

        public IEnumerator ShiftLLL()
        {
            IEnumerator coroutine = this.ShiftL();
            IEnumerator coroutine2 = this.ShiftL();
            IEnumerator coroutine3 = this.ShiftL();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
                yield return base.GameController.StartCoroutine(coroutine3);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
                base.GameController.ExhaustCoroutine(coroutine3);
            }
            yield break;
        }

        public IEnumerator ShiftR()
        {
            base.SetCardPropertyToTrueIfRealAction(HasShifted);
            yield break;
        }

        public IEnumerator ShiftRR()
        {
            IEnumerator coroutine = this.ShiftR();
            IEnumerator coroutine2 = this.ShiftR();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }

        public IEnumerator ShiftRRR()
        {
            IEnumerator coroutine = this.ShiftR();
            IEnumerator coroutine2 = this.ShiftR();
            IEnumerator coroutine3 = this.ShiftR();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
                yield return base.GameController.StartCoroutine(coroutine3);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
                base.GameController.ExhaustCoroutine(coroutine3);
            }
            yield break;
        }
    }
}
