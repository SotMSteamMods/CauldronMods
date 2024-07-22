using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cauldron.Drift
{
    public abstract class DriftSubCharacterCardController : HeroCharacterCardController
    {
        protected DriftSubCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            var positionString = base.SpecialStringMaker.ShowIfElseSpecialString(() => this.IsTimeMatching(Past), () => $"{TurnTaker.NameRespectingVariant} is at position {this.CurrentShiftPosition()}. {Past} effects are active.", () => $"{TurnTaker.NameRespectingVariant} is at position {this.CurrentShiftPosition()}. {Future} effects are active.");
            positionString.Condition = () => GetShiftTrack() != null;

            if(shouldRunSetUp)
            {
                AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
            }
        }
        protected virtual bool shouldRunSetUp => true;
        protected const string Past = "{Past}";
        protected const string Future = "{Future}";

        protected const string Base = "Base";
        protected const string Dual = "Dual";
        protected const string ThroughTheBreach = "ThroughTheBreach";

        protected const string HasShifted = "HasShifted";
        protected const string ShiftTrack = "ShiftTrack";
        protected const string ShiftPoolIdentifier = "ShiftPool";

        protected bool _inTheMiddleOfPower = false;

        private bool _instantiatingShiftTracks = false;

        protected enum CustomMode
        {
            StartOfGameChooseDrift,
            DualDriftShift
        }
        protected CustomMode customMode { get; set; }

        private int totalShifts = 0;
        public int TotalShifts { get => totalShifts; set => totalShifts = value; }

        public override void AddStartOfGameTriggers()
        {
            AddTrigger((GameAction ga) => TurnTakerController is DriftTurnTakerController ttc && !ttc.ArePromosSetup, SetupPromos, TriggerType.Hidden, TriggerTiming.Before, priority: TriggerPriority.High);
        }

        public IEnumerator SetupPromos(GameAction ga)
        {
            if (TurnTakerController is DriftTurnTakerController ttc && !ttc.ArePromosSetup)
            {
                ttc.SetupPromos(ttc.availablePromos);
                foreach(string name in ttc.nonDriftPromos.Keys)
                {
                    ttc.SetupPromos(ttc.nonDriftPromos[name], name: name);
                }
                ttc.ArePromosSetup = true;
            }

            return DoNothing();
        }

        public override IEnumerator PerformEnteringGameResponse()
        {
            IEnumerator coroutine;
            if (TurnTakerController is DriftTurnTakerController)
            {
                DriftTurnTakerController dttc = ((DriftTurnTakerController)HeroTurnTakerController);
                coroutine = dttc.SetupDrift();
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

        public override void AddTriggers()
        {
            ////Whenever you shift from {DriftPast} to {DriftFuture}... 
            //base.AddTrigger<AddTokensToPoolAction>((AddTokensToPoolAction action) => action.IsSuccessful && action.TokenPool.Identifier == ShiftPoolIdentifier && action.TokenPool.CurrentValue == 3, ShiftRedBlue, TriggerType.Hidden, TriggerTiming.After);
            ////...or from {DriftFuture} to {DriftPast}...
            //base.AddTrigger<RemoveTokensFromPoolAction>((RemoveTokensFromPoolAction action) => action.IsSuccessful && action.TokenPool.Identifier == ShiftPoolIdentifier && action.TokenPool.CurrentValue == 2, ShiftRedBlue, TriggerType.Hidden, TriggerTiming.After);

            AddTrigger((GameAction ga) => !(TurnTakerController is DriftTurnTakerController) && GetShiftTrack() is null && !_instantiatingShiftTracks, ga => InstatiateShiftTrack(), TriggerType.Hidden, TriggerTiming.Before, priority: TriggerPriority.High);
        }

        public int CurrentShiftPosition()
        {
            return this.GetShiftPool().CurrentValue;
        }

        public TokenPool GetShiftPool()
        {
            if (TurnTakerController is DriftTurnTakerController)
            {
                return this.GetShiftTrack().FindTokenPool(ShiftPoolIdentifier);
            }
            
            if (GameController.AllTurnTakers.Select(tt => FindTurnTakerController(tt)).Any(ttc => ttc is DriftTurnTakerController))
            {
                DriftTurnTakerController dttc = (DriftTurnTakerController) GameController.AllTurnTakers.Select(tt => FindTurnTakerController(tt)).First(ttc => ttc is DriftTurnTakerController);
                DriftSubCharacterCardController drift_cc = FindCardController(dttc.GetActiveCharacterCard()) as DriftSubCharacterCardController;
                return drift_cc.GetShiftPool();
            }

            if (!(GetShiftTrack() is null))
            {
                return this.GetShiftTrack().FindTokenPool(ShiftPoolIdentifier);
            }

            return null;
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

        public IEnumerator ShiftLAction()
        {
            if(this.GetShiftPool() is null)
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
            return base.FindCardsWhere((Card c) => IsHeroCharacterCard(c) && c.Location == base.TurnTaker.PlayArea && c.Owner == this.TurnTaker && c.IsRealCard).FirstOrDefault();
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
        
        public override void PrepareToUsePower(Power power)
        {
            base.PrepareToUsePower(power);
            if (power.IsInnatePower)
            {
                var partnerCards = TurnTaker.GetCardsWhere((Card c) => c.SharedIdentifier == this.CardWithoutReplacements.SharedIdentifier && c != this.CardWithoutReplacements);
                HeroTurnTaker powerUser = null;
                if (power.TurnTakerController != null && IsHero(power.TurnTakerController.TurnTaker))
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

        private DeckDefinition DriftDeckDefinition => DeckDefinitionCache.GetDeckDefinition("Cauldron.Drift");
       
        private string ShiftTrackPrefix
        {
            get
            {
                if(this is DualDriftSubCharacterCardController)
                {
                    return "Dual";
                }

                if (this is ThroughTheBreachDriftCharacterCardController)
                {
                    return "ThroughTheBreach";
                }

                return "Base";
            }
        }
        protected IEnumerator InstatiateShiftTrack()
        {
            _instantiatingShiftTracks = true;
            Log.Debug("Instantiating Shift Track");
            IEnumerable<CardDefinition> shiftTrackDefinitions = DriftDeckDefinition.CardDefinitions.Where(cd => cd.Identifier.Contains(ShiftTrackPrefix + "ShiftTrack"));
            List<Card> shiftTracks = new List<Card>();
            Card modelCard;
            CardController cardController;
            Dictionary<Card, CardController> cardToControllerDict = new Dictionary<Card, CardController>();
            string overrideNamespace; 
            List<string> list;
            foreach (CardDefinition trackDefinition in shiftTrackDefinitions)
            {
                overrideNamespace = $"{trackDefinition.Namespace}.{DriftDeckDefinition.Identifier}";
                list = new List<string>();
                list.Add(overrideNamespace);
                list.Add(trackDefinition.QualifiedIdentifier);
                modelCard = new Card(trackDefinition, TurnTaker, 0);
                TurnTaker.OffToTheSide.AddCard(modelCard);
                shiftTracks.Add(modelCard);
                cardController = CardControllerFactory.CreateInstance(modelCard, TurnTakerController, overrideNamespace: "Cauldron.Drift");
                TurnTakerController.AddCardController(cardController);
                cardToControllerDict.Add(modelCard, cardController);
                GameController.AddCardPropertyJournalEntry(modelCard, "OverrideTurnTaker", list);
            }

            List<SelectCardDecision> cardDecisions = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.AddTokens, new LinqCardCriteria((Card c) => shiftTracks.Contains(c), "Shift Track Position"), cardDecisions, false, includeRealCardsOnly: false, cardSource: new CardSource(this));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (!DidSelectCard(cardDecisions))
                yield break;

            Card selectedTrack = GetSelectedCard(cardDecisions);
            PlayCardAction playShiftAction = new PlayCardAction(GameController, TurnTakerController, selectedTrack, isPutIntoPlay: true, responsibleTurnTaker: TurnTaker, null, null, null, false, canBeCancelled: false);
            playShiftAction.AllowTriggersToRespond = false;
            coroutine = GameController.DoAction(playShiftAction);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            CardController selectTrackController = cardToControllerDict[selectedTrack];
            int tokensToAdd = 0;
            if (selectTrackController is BaseShiftTrack1CardController || selectTrackController is DualShiftTrack1CardController || selectTrackController is ThroughTheBreachShiftTrack1CardController)
            {
                tokensToAdd = 1;
            }
            else if (selectTrackController is BaseShiftTrack2CardController || selectTrackController is DualShiftTrack2CardController || selectTrackController is ThroughTheBreachShiftTrack2CardController)
            {
                tokensToAdd = 2;
            }
            else if (selectTrackController is BaseShiftTrack3CardController || selectTrackController is DualShiftTrack3CardController || selectTrackController is ThroughTheBreachShiftTrack3CardController)
            {
                tokensToAdd = 3;
            }
            else if (selectTrackController is BaseShiftTrack4CardController || selectTrackController is DualShiftTrack4CardController || selectTrackController is ThroughTheBreachShiftTrack4CardController)
            {
                tokensToAdd = 4;
            }

            coroutine = base.GameController.AddTokensToPool(selectedTrack.FindTokenPool("ShiftPool"), tokensToAdd, new CardSource(this));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            _instantiatingShiftTracks = false;

            yield break;
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