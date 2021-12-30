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
            base.SpecialStringMaker.ShowIfElseSpecialString(() => this.HasTrackAbilityBeenActivated(), () => "Drift has changed the active character this turn", () => "Drift has not changed the active character this turn", showInEffectsList: () => true);
        }

        protected enum CustomMode
        {
            AskToSwap
        }

        private CustomMode customMode { get; set; }

        protected const string DriftPosition = "DriftPosition";
        protected const string OncePerTurn = "DualShiftAbilityOncePerTurn";
        public override bool AllowFastCoroutinesDuringPretend => false;

        public override void AddTriggers()
        {
            base.AddTriggers();
            // Once per turn when Drift would shift, play a card, or be dealt damage, you may do the following in order
            //1. Place your active character on your current shift track space.
            //2. Place the shift token on your inactive character's shift track space.
            //3. Switch which character is active.

            //shift trigger happens in the actual shifting logic
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => Game.HasGameStarted && !this.HasTrackAbilityBeenActivated() && action.Target == GetActiveCharacterCard(), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);

            string[] noResponseIdentifiers = new string[] { "ShiftTrack", "FutureDrift", "PastDrift" };
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cpa) => cpa.CardEnteringPlay != null && !this.HasTrackAbilityBeenActivated() && cpa.CardEnteringPlay.Owner == TurnTakerControllerWithoutReplacements.TurnTaker && noResponseIdentifiers.All(id => !cpa.CardEnteringPlay.Identifier.Contains(id)), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
        }

        public bool HasTrackAbilityBeenActivated()
        {
            IEnumerable<CardPropertiesJournalEntry> trackEntries = base.Journal.CardPropertiesEntries((CardPropertiesJournalEntry entry) => entry.Key == OncePerTurn && entry.Card.SharedIdentifier == ShiftTrack && entry.TurnIndex == base.Game.TurnIndex);
            return trackEntries.Any();
        }

        private IEnumerator TrackResponse(GameAction action)
        {
            List<YesNoCardDecision> switchDecision = new List<YesNoCardDecision>();
            customMode = CustomMode.AskToSwap;
            YesNoCardDecision decision = new YesNoCardDecision(GameController, HeroTurnTakerController, SelectionType.Custom, Card, action: action is DealDamageAction ? action : null, associatedCards: GetInactiveCharacterCard().ToEnumerable(), cardSource: GetCardSource());
            decision.ExtraInfo = () => $"{GetInactiveCharacterCard().Title} is at position {InactiveCharacterPosition()}";
            switchDecision.Add(decision);
            IEnumerator coroutine = GameController.MakeDecisionAction(decision);
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
                if(action.IsPretend)
                { 
                    yield break;
                }
                coroutine = SwapActiveDrift();
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (action is DealDamageAction dealDamageAction)
                {
                    coroutine = RedirectDamage(dealDamageAction, TargetType.SelectTarget, c => c == GetActiveCharacterCard());
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

        public IEnumerator SwapActiveDrift()
        {
            IEnumerator coroutine = null;
            //Once per turn you may do the following in order:
            base.SetCardPropertyToTrueIfRealAction(OncePerTurn);
            IEnumerable<CardPropertiesJournalEntry> trackEntries = base.Journal.CardPropertiesEntries((CardPropertiesJournalEntry entry) => entry.Key == OncePerTurn && entry.Card.SharedIdentifier == ShiftTrack);

            int inactivePosition = this.InactiveCharacterPosition();
            base.SetCardProperty(DriftPosition + inactivePosition, false);

            //1. Place your active character on your current shift track space.
            base.SetCardProperty(DriftPosition + this.CurrentShiftPosition(), true);

            //2. Place the shift token on your inactive character's shift track space.
            if (this.CurrentShiftPosition() < inactivePosition)
            {
                AddTokensToPoolAction addTokensAction = new AddTokensToPoolAction(GetCardSource(), GetShiftPool(), inactivePosition - CurrentShiftPosition());
                addTokensAction.AllowTriggersToRespond = false;
                addTokensAction.CanBeCancelled = false;
                coroutine = DoAction(addTokensAction);
            }
            else if (this.CurrentShiftPosition() > inactivePosition)
            {
                RemoveTokensFromPoolAction removeTokensAction = new RemoveTokensFromPoolAction(GetCardSource(), GetShiftPool(), CurrentShiftPosition() - inactivePosition);
                removeTokensAction.AllowTriggersToRespond = false;
                removeTokensAction.CanBeCancelled = false;
                coroutine = DoAction(removeTokensAction);
            }

            if (coroutine != null)
            {
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                //Switch visual
                coroutine = base.GameController.SwitchCards(this.GetShiftTrack(), base.FindCard(Dual + ShiftTrack + this.CurrentShiftPosition(), false), cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

            }
            
            Card switchFromDrift = GetActiveCharacterCard();
            Card switchToDrift = GetInactiveCharacterCard(inactivePosition);
            //3. Switch which character is active.
            coroutine = GameController.SwitchCards(switchFromDrift, switchToDrift, ignoreHitPoints: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            yield return DoNothing();
        }

        public Card GetInactiveCharacterCard(int inactivePosition = 0)
        {
            inactivePosition = inactivePosition == 0 ? InactiveCharacterPosition() : inactivePosition;
            string activeBaseIdentifier = GetActiveCharacterCard().Identifier.Replace("Red", "").Replace("Blue", "");
            string desiredIdentifier;
            if(activeBaseIdentifier.Contains("Past"))
            {
                desiredIdentifier = activeBaseIdentifier.Replace("Past", "Future");
            } else
            {
                desiredIdentifier = activeBaseIdentifier.Replace("Future", "Past");
            }
            return base.FindCardsWhere(new LinqCardCriteria((Card c) => c.Location == base.TurnTaker.OffToTheSide && c.Owner == base.TurnTaker && c.Identifier == desiredIdentifier)).FirstOrDefault();
        }

        public int InactiveCharacterPosition()
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

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            if (customMode == CustomMode.AskToSwap)
            {
                return new CustomDecisionText("Do you want to switch character cards?", "Should they switch character cards?", "Vote for if they should switch character cards?", "switching character cards");
            }

            return base.GetCustomDecisionText(decision);
        }

        
    }
}
