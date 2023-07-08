using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace CauldronExtensionMethods
{
    public static class DriftExtensionMethods
    {
        private static readonly string DriftTurnTakerQualifiedIdentifier = "Cauldron.Drift";
        private static readonly string HasShifted = "HasShifted";
        private static readonly string ShiftTrack = "ShiftTrack";
        private static readonly string ShiftPoolIdentifier = "ShiftPool";
        public static int CurrentShiftPosition(this CardController cardController)
        {
            return cardController.GetShiftPool().CurrentValue;
        }

        public static TokenPool GetShiftPool(this CardController cardController)
        {
            Assembly assembly = ModHelper.GetAssemblyForTurnTaker(DriftTurnTakerQualifiedIdentifier);
            string fullTurnTakerName =  $"{DriftTurnTakerQualifiedIdentifier}TurnTakerController";
            Type driftTurnTakerControllerType = assembly.GetType(fullTurnTakerName);
            if (cardController.TurnTakerController.GetType().Equals(driftTurnTakerControllerType))
            {
                return cardController.GetShiftTrack().FindTokenPool(ShiftPoolIdentifier);
            }

            if (cardController.GameController.AllTurnTakers.Select(tt => cardController.FindTurnTakerController(tt)).Any(ttc => ttc.GetType().Equals(driftTurnTakerControllerType)))
            {
                var dttc = Convert.ChangeType(cardController.GameController.AllTurnTakers.Select(tt => cardController.FindTurnTakerController(tt)).First(ttc => ttc.GetType().Equals(driftTurnTakerControllerType)), driftTurnTakerControllerType);
                DriftSubCharacterCardController drift_cc = FindCardController(dttc.GetActiveCharacterCard()) as DriftSubCharacterCardController;
                return drift_cc.GetShiftPool();
            }

            if (!(GetShiftTrack() is null))
            {
                return this.GetShiftTrack().FindTokenPool(ShiftPoolIdentifier);
            }

            return null;
        }

        public static Card GetShiftTrack(this CardController cc)
        {
            return cc.FindCardsWhere((Card c) => c.SharedIdentifier == ShiftTrack && c.IsInPlayAndHasGameText, false).FirstOrDefault();
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

        public IEnumerator ShiftLAction()
        {
            if (this.GetShiftPool() is null)
            {
                IEnumerator coroutine = GameController.SendMessageAction("There is no shift track in play! No shifting will occur.", Priority.Medium, GetCardSource());
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

        public IEnumerator ShiftLLAction()
        {
            IEnumerator coroutine = this.ShiftLAction();
            IEnumerator coroutine2 = this.ShiftLAction();
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

        public IEnumerator ShiftLLLAction()
        {
            IEnumerator coroutine = this.ShiftLAction();
            IEnumerator coroutine2 = this.ShiftLAction();
            IEnumerator coroutine3 = this.ShiftLAction();
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

        public IEnumerator ShiftRAction()
        {
            if (this.GetShiftPool() is null)
            {
                IEnumerator coroutine = GameController.SendMessageAction("There is no shift track in play! No shifting will occur.", Priority.Medium, GetCardSource());
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

        public IEnumerator ShiftRRAction()
        {
            IEnumerator coroutine = this.ShiftRAction();
            IEnumerator coroutine2 = this.ShiftRAction();
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

        public IEnumerator ShiftRRRAction()
        {
            IEnumerator coroutine = this.ShiftRAction();
            IEnumerator coroutine2 = this.ShiftRAction();
            IEnumerator coroutine3 = this.ShiftRAction();
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
            IEnumerator coroutine = base.GameController.SwitchCards(this.GetShiftTrack(), base.FindCard(promoIdentifier + ShiftTrack + this.CurrentShiftPosition(), false), cardSource: GetCardSource());
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
            if (CharacterCardWithoutReplacements.PromoIdentifierOrIdentifier.Contains("Red"))
            {
                desiredIdentifier = CharacterCardWithoutReplacements.PromoIdentifierOrIdentifier.Replace("Red", "Blue");
            }
            else
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

            if (!_inTheMiddleOfPower)
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

        public override void PrepareToUsePower(Power power)
        {
            base.PrepareToUsePower(power);
            if (power.IsInnatePower)
            {
                var partnerCards = TurnTaker.GetCardsWhere((Card c) => c.SharedIdentifier == this.CardWithoutReplacements.SharedIdentifier && c != this.CardWithoutReplacements);
                HeroTurnTaker powerUser = null;
                if (power.TurnTakerController != null && power.TurnTakerController.TurnTaker.IsHero)
                {
                    powerUser = power.TurnTakerController.TurnTaker.ToHero();
                }
                foreach (Card partnerCard in partnerCards)
                {
                    //this adds an extra power-use record to the journal, of using the OTHER card's power
                    //WITHOUT actually ever 'using' a power, so it shouldn't cause extra triggers
                    //may cause problems with card that want to count how many powers a player has used in a turn, though
                    GameController.Game.Journal.RecordUsePower(partnerCard, power.Index, power.NumberOfUses, power.CardSource.Card, powerUser, false, power.CardController.CardWithoutReplacements.PlayIndex, power.CardSource.Card.PlayIndex, null, this.CardWithoutReplacements);
                }
            }
        }

        protected override IEnumerator RemoveCardsFromGame(IEnumerable<Card> cards)
        {
            if (!Card.IsInPlayAndHasGameText)
            {
                yield break;
            }
            IEnumerable<Card> enumerable = FindCardsWhere((Card c) => c != Card && c.SharedIdentifier != null && c.SharedIdentifier == Card.SharedIdentifier);
            foreach (Card item in enumerable)
            {
                if (!item.IsIncapacitated)
                {
                    IEnumerator coroutine = base.GameController.FlipCard(FindCardController(item), cardSource: GetCardSource());
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
            IEnumerator coroutine2 = base.RemoveCardsFromGame(cards);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
        }

        public IEnumerator ShiftL()
        {
            if (FindCardController(GetShiftTrack()) is DualShiftTrackUtilityCardController dualShiftTrack && !dualShiftTrack.HasTrackAbilityBeenActivated() && TurnTakerController is DriftTurnTakerController)
            {
                IEnumerator coroutine = DualDriftShifting(dualShiftTrack, ShiftLAction, "{ShiftL}");
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
                IEnumerator coroutine = ShiftLAction();
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
            if (FindCardController(GetShiftTrack()) is DualShiftTrackUtilityCardController dualShiftTrack && !dualShiftTrack.HasTrackAbilityBeenActivated() && TurnTakerController is DriftTurnTakerController)
            {
                IEnumerator coroutine = DualDriftShifting(dualShiftTrack, ShiftLLAction, "{ShiftLL}");
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
                IEnumerator coroutine = ShiftLLAction();
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

        public IEnumerator ShiftLLL()
        {
            if (FindCardController(GetShiftTrack()) is DualShiftTrackUtilityCardController dualShiftTrack && !dualShiftTrack.HasTrackAbilityBeenActivated() && TurnTakerController is DriftTurnTakerController)
            {
                IEnumerator coroutine = DualDriftShifting(dualShiftTrack, ShiftLLLAction, "{ShiftLLL}");
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
                IEnumerator coroutine = ShiftLLLAction();
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

        public IEnumerator ShiftR()
        {
            if (FindCardController(GetShiftTrack()) is DualShiftTrackUtilityCardController dualShiftTrack && !dualShiftTrack.HasTrackAbilityBeenActivated() && TurnTakerController is DriftTurnTakerController)
            {
                IEnumerator coroutine = DualDriftShifting(dualShiftTrack, ShiftRAction, "{ShiftR}");
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
                IEnumerator coroutine = ShiftRAction();
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
            if (FindCardController(GetShiftTrack()) is DualShiftTrackUtilityCardController dualShiftTrack && !dualShiftTrack.HasTrackAbilityBeenActivated() && TurnTakerController is DriftTurnTakerController)
            {
                IEnumerator coroutine = DualDriftShifting(dualShiftTrack, ShiftRRAction, "{ShiftRR}");
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
                IEnumerator coroutine = ShiftRRAction();
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

        public IEnumerator ShiftRRR()
        {
            if (FindCardController(GetShiftTrack()) is DualShiftTrackUtilityCardController dualShiftTrack && !dualShiftTrack.HasTrackAbilityBeenActivated() && TurnTakerController is DriftTurnTakerController)
            {
                IEnumerator coroutine = DualDriftShifting(dualShiftTrack, ShiftRRRAction, "{ShiftRRR}");
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
                IEnumerator coroutine = ShiftRRRAction();
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

        private IEnumerator DualDriftShifting(DualShiftTrackUtilityCardController dualShiftTrack, Func<IEnumerator> shiftAction, string shiftString)
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
                coroutine = shiftAction();
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

            coroutine = shiftAction();
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

        
    }
}
