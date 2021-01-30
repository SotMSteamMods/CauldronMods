using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public abstract class DualShiftTrackUtilityCardController : ShiftTrackUtilityCardController
    {
        protected DualShiftTrackUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowSpecialString(() => this.GetInactiveCharacterCard().AlternateTitleOrTitle + " is the inactive {Drift}, she is at position " + this.InactiveCharacterPosition());
            base.SpecialStringMaker.ShowIfElseSpecialString(() => this.HasTrackAbilityBeenActivated(), () => "Drift has changed the active character this turn", () => "Drift has not changed the active character this turn");
        }

        protected const string DriftPosition = "DriftPosition";
        protected const string OncePerTurn = "OncePerTurn";
        protected const string ShiftTrack = "ShiftTrack";
        protected const string Dual = "Dual";

        public override void AddTriggers()
        {
            //Once per turn you may do the following in order:
            //1. Place your active character on your current shift track space.
            //2. Place the shift token on your inactive character's shift track space.
            //3. Switch which character is active.
            base.AddTrigger<ActivateAbilityAction>((ActivateAbilityAction action) => !this.HasTrackAbilityBeenActivated(), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction action) => !this.HasTrackAbilityBeenActivated(), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => !this.HasTrackAbilityBeenActivated(), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<DiscardCardAction>((DiscardCardAction action) => !this.HasTrackAbilityBeenActivated(), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<DrawCardAction>((DrawCardAction action) => !this.HasTrackAbilityBeenActivated(), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<GainHPAction>((GainHPAction action) => !this.HasTrackAbilityBeenActivated(), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<GiveHighFiveAction>((GiveHighFiveAction action) => !this.HasTrackAbilityBeenActivated(), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<ModifyTokensAction>((ModifyTokensAction action) => !this.HasTrackAbilityBeenActivated(), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<MoveCardAction>((MoveCardAction action) => !this.HasTrackAbilityBeenActivated(), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<PhaseChangeAction>((PhaseChangeAction action) => !this.HasTrackAbilityBeenActivated(), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<RedirectDamageAction>((RedirectDamageAction action) => !this.HasTrackAbilityBeenActivated(), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<UsePowerAction>((UsePowerAction action) => !this.HasTrackAbilityBeenActivated(), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
        }

        private bool HasTrackAbilityBeenActivated()
        {
            IEnumerable<CardPropertiesJournalEntry> trackEntries = base.Journal.CardPropertiesEntries((CardPropertiesJournalEntry entry) => entry.Key == OncePerTurn && entry.Card.SharedIdentifier == ShiftTrack && entry.TurnIndex == base.Game.TurnIndex);
            return trackEntries.Any();
        }

        private IEnumerator TrackResponse(GameAction action)
        {
            List<YesNoCardDecision> switchDecision = new List<YesNoCardDecision>();
            IEnumerator coroutine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController, SelectionType.SwitchToHero, base.Card, storedResults: switchDecision, associatedCards: this.GetInactiveCharacterCard().ToEnumerable(), cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (base.DidPlayerAnswerYes(switchDecision.FirstOrDefault()))
            {
                //Once per turn you may do the following in order:
                base.SetCardPropertyToTrueIfRealAction(OncePerTurn);
                IEnumerable<CardPropertiesJournalEntry> trackEntries = base.Journal.CardPropertiesEntries((CardPropertiesJournalEntry entry) => entry.Key == OncePerTurn && entry.Card.SharedIdentifier == ShiftTrack);

                int inactivePosition = this.InactiveCharacterPosition();
                base.SetCardProperty(DriftPosition + inactivePosition, false);

                //1. Place your active character on your current shift track space.
                base.SetCardPropertyToTrueIfRealAction(DriftPosition + this.CurrentShiftPosition());

                //2. Place the shift token on your inactive character's shift track space.
                if (this.CurrentShiftPosition() < inactivePosition)
                {
                    coroutine = base.GameController.AddTokensToPool(this.GetShiftPool(), inactivePosition - this.CurrentShiftPosition(), cardSource: base.GetCardSource());
                }
                else
                {
                    coroutine = base.GameController.RemoveTokensFromPool(this.GetShiftPool(), this.CurrentShiftPosition() - inactivePosition, cardSource: base.GetCardSource());
                }
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                //Switch visual
                coroutine = base.GameController.SwitchCards(this.GetShiftTrack(), base.FindCard(Dual + ShiftTrack + this.CurrentShiftPosition(), false));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //3. Switch which character is active.
                coroutine = base.GameController.SwitchCards(this.GetActiveCharacterCard(), this.GetInactiveCharacterCard(), ignoreHitPoints: true, cardSource: base.GetCardSource());
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
        }

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

        public Card GetActiveCharacterCard()
        {
            return base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.Location == base.TurnTaker.PlayArea && c.Owner == base.TurnTaker)).FirstOrDefault();
        }

        public Card GetInactiveCharacterCard()
        {
            return base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.Location == base.TurnTaker.OffToTheSide && c.Owner == base.TurnTaker && c.HasPowers)).FirstOrDefault();
        }

        private int InactiveCharacterPosition()
        {
            int inactivePosition = 0;
            string[] inactivePositions = new string[] {
                DriftPosition + 1,
                DriftPosition + 2,
                DriftPosition + 3,
                DriftPosition + 4
            };
            //Get all entries of any of the positions being toggled
            IEnumerable<CardPropertiesJournalEntry> positionEntries = base.Journal.CardPropertiesEntries((CardPropertiesJournalEntry entry) => entry.Card.SharedIdentifier == ShiftTrack && inactivePositions.Contains(entry.Key));

            //If a position is set to true then there is an odd number of entries (true), if the position is set to false then there is an even number of entries (true + false)
            if (positionEntries.Where((CardPropertiesJournalEntry entry) => entry.Key == DriftPosition + 1).Count() % 2 == 1)
            {
                inactivePosition = 1;
            }
            if (positionEntries.Where((CardPropertiesJournalEntry entry) => entry.Key == DriftPosition + 2).Count() % 2 == 1)
            {
                inactivePosition = 2;
            }
            if (positionEntries.Where((CardPropertiesJournalEntry entry) => entry.Key == DriftPosition + 3).Count() % 2 == 1)
            {
                inactivePosition = 3;
            }
            if (positionEntries.Where((CardPropertiesJournalEntry entry) => entry.Key == DriftPosition + 4).Count() % 2 == 1)
            {
                inactivePosition = 4;
            }
            return inactivePosition;
        }
    }
}
