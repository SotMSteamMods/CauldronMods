using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Drift
{
    public class DriftSubCharacterCardController : HeroCharacterCardController
    {
        public DriftSubCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected const string Base = "Base";
        protected const string Dual = "Dual";
        protected const string ThroughTheBreach = "ThroughTheBreach";

        protected const string HasShifted = "HasShifted";
        protected const string ShiftTrack = "ShiftTrack";

        private int totalShifts = 0;
        public int TotalShifts { get => totalShifts; set => totalShifts = value; }

        public int CurrentShiftPosition()
        {
            return this.GetShiftPool().CurrentValue;
        }

        public TokenPool GetShiftPool()
        {
            return this.GetShiftTrack().FindTokenPool("ShiftPool");
        }

        public Card GetShiftTrack()
        {
            return base.FindCardsWhere((Card c) => c.SharedIdentifier == ShiftTrack && c.IsInPlayAndHasGameText, false).FirstOrDefault();
        }

        public IEnumerator ShiftL()
        {
            //Ensures not shifting off track
            if (this.CurrentShiftPosition() > 1)
            {
                base.SetCardPropertyToTrueIfRealAction(HasShifted);
                IEnumerator coroutine = base.GameController.RemoveTokensFromPool(this.GetShiftPool(), 1, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //Switch to the new card
                coroutine = this.SwitchTrack();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                this.totalShifts++;
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

        public IEnumerator ShiftR()
        {
            //Ensures not shifting off track
            if (this.CurrentShiftPosition() < 4)
            {
                base.SetCardPropertyToTrueIfRealAction(HasShifted);
                IEnumerator coroutine = base.GameController.AddTokensToPool(this.GetShiftPool(), 1, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //Switch to the new card
                coroutine = this.SwitchTrack();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                this.totalShifts++;
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

        private IEnumerator SwitchTrack()
        {
            string promoIdentifier = Base;
            if (base.CharacterCardController is DualDriftCharacterCardController)
            {
                promoIdentifier = Dual;
            }
            else if (base.CharacterCardController is ThroughTheBreachDriftCharacterCardController)
            {
                promoIdentifier = ThroughTheBreach;
            }
            IEnumerator coroutine = base.GameController.SwitchCards(this.GetShiftTrack(), base.FindCard(promoIdentifier + ShiftTrack + this.CurrentShiftPosition(), false));
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