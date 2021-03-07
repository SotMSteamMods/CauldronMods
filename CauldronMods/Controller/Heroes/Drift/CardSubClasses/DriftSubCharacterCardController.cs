using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Drift
{
    public abstract class DriftSubCharacterCardController : HeroCharacterCardController
    {
        protected DriftSubCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            var positionString = base.SpecialStringMaker.ShowIfElseSpecialString(() => this.IsTimeMatching(Past), () => $"{TurnTaker.NameRespectingVariant} is at position {this.CurrentShiftPosition()}, this is in the {Past}", () => $"{TurnTaker.NameRespectingVariant} is at position {this.CurrentShiftPosition()}, this is in the {Future}");
            positionString.Condition = () => GetShiftTrack() != null;
        }

        protected const string Past = "Past";
        protected const string Future = "Future";

        protected const string Base = "Base";
        protected const string Dual = "Dual";
        protected const string ThroughTheBreach = "ThroughTheBreach";

        protected const string HasShifted = "HasShifted";
        protected const string ShiftTrack = "ShiftTrack";
        protected const string ShiftPoolIdentifier = "ShiftPool";

        protected bool _inTheMiddleOfPower = false;


        private int totalShifts = 0;
        public int TotalShifts { get => totalShifts; set => totalShifts = value; }


        public override void AddTriggers()
        {
            //Whenever you shift from {DriftPast} to {DriftFuture}... 
            base.AddTrigger<AddTokensToPoolAction>((AddTokensToPoolAction action) => action.IsSuccessful && action.TokenPool.Identifier == ShiftPoolIdentifier && action.TokenPool.CurrentValue == 3, ShiftRedBlue, TriggerType.Hidden, TriggerTiming.After);
            //...or from {DriftFuture} to {DriftPast}...
            base.AddTrigger<RemoveTokensFromPoolAction>((RemoveTokensFromPoolAction action) => action.IsSuccessful && action.TokenPool.Identifier == ShiftPoolIdentifier && action.TokenPool.CurrentValue == 2, ShiftRedBlue, TriggerType.Hidden, TriggerTiming.After);
        }

        public int CurrentShiftPosition()
        {
            return this.GetShiftPool().CurrentValue;
        }

        public TokenPool GetShiftPool()
        {
            return this.GetShiftTrack().FindTokenPool(ShiftPoolIdentifier);
        }

        public Card GetShiftTrack()
        {
            return base.FindCardsWhere((Card c) => c.SharedIdentifier == ShiftTrack && c.IsInPlayAndHasGameText, false).FirstOrDefault();
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

        public Card GetPositionalShiftTrack(int position)
        {
            string promoIdentifier = Base;
            if (base.CharacterCardController is DualDriftSubCharacterCardController)
            {
                promoIdentifier = Dual;
            }
            else if (base.CharacterCardController is ThroughTheBreachSubCharacterCardController)
            {
                promoIdentifier = ThroughTheBreach;
            }
            return base.FindCard(promoIdentifier + ShiftTrack + position, false);
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
            if (base.CharacterCardController is DualDriftSubCharacterCardController)
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

        public Card GetActiveCharacterCard()
        {
            return base.FindCardsWhere((Card c) => c.IsHeroCharacterCard && c.Location == base.TurnTaker.PlayArea && c.Owner == this.TurnTaker && c.IsRealCard).FirstOrDefault();
        }

        public Card FindRedBlueDriftCharacterCard()
        {
           
            var characters = base.TurnTaker.GetAllCards().Where(c => c.IsCharacter && c.SharedIdentifier == CharacterCardWithoutReplacements.SharedIdentifier).ToList();
            string desiredIdentifier;
            if(CharacterCardWithoutReplacements.PromoIdentifierOrIdentifier.Contains("Red"))
            {
                desiredIdentifier = CharacterCardWithoutReplacements.PromoIdentifierOrIdentifier.Replace("Red", "Blue");
            } else
            {
                desiredIdentifier = CharacterCardWithoutReplacements.PromoIdentifierOrIdentifier.Replace("Blue", "Red");
            }

            var driftCharacter = characters.First(c => c.Identifier == desiredIdentifier);
            return driftCharacter;
        }

        private IEnumerator ShiftRedBlue(ModifyTokensAction tpa)
        {
            if (GetActiveCharacterCard() == CharacterCardWithoutReplacements && (CharacterCardWithoutReplacements.Identifier.Contains("Blue") || CharacterCardWithoutReplacements.Identifier.Contains("Red")) && !_inTheMiddleOfPower)
            {
                var driftCharacter = FindRedBlueDriftCharacterCard();
                driftCharacter.SetHitPoints(CharacterCardWithoutReplacements.HitPoints.Value);

                Log.Debug($"Switching to {driftCharacter.Identifier}");
                Log.Debug($"What should happen is \"SwitchCutoutCard: from {CharacterCardWithoutReplacements.PromoIdentifierOrIdentifier} to {driftCharacter.PromoIdentifierOrIdentifier}\"");

                var coroutine = GameController.SwitchCards(CharacterCardWithoutReplacements, driftCharacter, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
        }

        protected IEnumerator RetroactiveShiftIfNeeded()
        {
            IEnumerator coroutine;
            bool needsChange = false;

            if(!_inTheMiddleOfPower)
            {
                yield break;
            }
            _inTheMiddleOfPower = false;

            // If needed
            if (IsTimeMatching(Past) && GetActiveCharacterCard() == CharacterCardWithoutReplacements && CharacterCardWithoutReplacements.Identifier.Contains("Red"))
            {
                needsChange = true;
            }
            else
            {
                if (IsTimeMatching(Future) && GetActiveCharacterCard() == CharacterCardWithoutReplacements && CharacterCardWithoutReplacements.Identifier.Contains("Past"))
                {
                    needsChange = true;
                }
            }

            if (needsChange)
            {
                coroutine = ShiftRedBlue(null);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                yield break;
            }
        }

    }
}