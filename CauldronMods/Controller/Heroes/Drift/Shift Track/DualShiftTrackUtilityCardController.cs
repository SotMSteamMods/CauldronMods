using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift;

public abstract class DualShiftTrackUtilityCardController : ShiftTrackUtilityCardController
{
    protected DualShiftTrackUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
        base.SpecialStringMaker.ShowSpecialString(() => $"{this.GetInactiveCharacterCard().AlternateTitleOrTitle} is the inactive {{Drift}}; she is at position {this.InactiveCharacterPosition()}.").Condition = () => GetActiveCharacterCard() != null;
        base.SpecialStringMaker.ShowIfElseSpecialString(
            () => HasTrackAbilityBeenActivated, () => "Drift has changed the active character this turn.", 
            () => "Drift has not changed the active character this turn.", 
            showInEffectsList: () => true).Condition = () => GetActiveCharacterCard() != null;
    }

    protected enum CustomMode
    {
        AskToSwap,
        AskToSwapFromPlay
    }

    private Card customTextCardBeingPlayed { get; set; }

    private CustomMode customMode { get; set; }

    protected const string DriftPosition = "DriftPosition";
    protected const string SwapDriftOncePerTurn = "DualShiftAbilityOncePerTurn";
    public override bool AllowFastCoroutinesDuringPretend => false;

    public override void AddTriggers()
    {
        // Once per turn when Drift would shift, play a card, or be dealt damage, you may do the following in order
        //1. Place your active character on your current shift track space.
        //2. Place the shift token on your inactive character's shift track space.
        //3. Switch which character is active.

        //shift trigger happens in the actual shifting logic
        base.AddTrigger<DealDamageAction>((DealDamageAction action) => Game.HasGameStarted && !HasTrackAbilityBeenActivated && action.Target == GetActiveCharacterCard(), this.TrackResponse, TriggerType.Hidden, TriggerTiming.Before);

        string[] noResponseIdentifiers = new string[] { "ShiftTrack", "FutureDrift", "PastDrift" };
        base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction cpa) => cpa.CardEnteringPlay != null && !this.HasTrackAbilityBeenActivated && cpa.CardEnteringPlay.Owner == TurnTakerControllerWithoutReplacements.TurnTaker && noResponseIdentifiers.All(id => !cpa.CardEnteringPlay.Identifier.Contains(id)), this.TrackResponse, TriggerType.Hidden, TriggerTiming.Before);


        // Triggers from ShiftTrackUtilityCardController
        base.AddTriggers();
    }
    
    public bool HasTrackAbilityBeenActivated 
    {
        get
        {
            IEnumerable<CardPropertiesJournalEntry> source = from e in Journal.CardPropertiesEntriesThisTurn(entry => entry.Card.SharedIdentifier == this.ShiftTrackIdentifier)
                                                             where e.Key == SwapDriftOncePerTurn
                                                             select e;
            CardPropertiesJournalEntry cardPropertiesJournalEntry = source.LastOrDefault((CardPropertiesJournalEntry e) => e.BoolValue.HasValue);


            return cardPropertiesJournalEntry?.BoolValue ?? false;
        }
    }

    private IEnumerator TrackResponse(GameAction action)
    {
        List<YesNoCardDecision> switchDecision = [];
        if (action is CardEntersPlayAction cpa)
        {
            customMode = CustomMode.AskToSwapFromPlay;
            customTextCardBeingPlayed = cpa.CardEnteringPlay;
        }
        else
        {
            customMode = CustomMode.AskToSwap;
        }
        YesNoCardDecision decision = new(GameController, HeroTurnTakerController, SelectionType.Custom, Card, action: action is DealDamageAction ? action : null, associatedCards: GetInactiveCharacterCard().ToEnumerable(), cardSource: GetCardSource())
        {
            ExtraInfo = () => $"{GetInactiveCharacterCard().Title} is at position {InactiveCharacterPosition()}"
        };
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

        //put in a escape hatch if a swap has happened this turn after selecting to swap
        if (this.HasTrackAbilityBeenActivated)
        {
            coroutine = GameController.SendMessageAction("Drift has already swapped characters this turn...", Priority.Medium, GetCardSource(), showCardSource: true);
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

        //Once per turn you may do the following in order:
        base.SetCardPropertyToTrueIfRealAction(SwapDriftOncePerTurn);

        int inactivePosition = this.InactiveCharacterPosition();
        base.SetCardProperty(DriftPosition + inactivePosition, false);

        //1. Place your active character on your current shift track space.
        base.SetCardProperty(DriftPosition + this.CurrentShiftPosition, true);

        //2. Place the shift token on your inactive character's shift track space.
       
        int delta = inactivePosition - this.CurrentShiftPosition;
        coroutine = delta switch
        {
            -3 => this.ShiftLLL(),
            -2 => this.ShiftLL(),
            -1 => this.ShiftL(),
            0 => GameController.DoNothing(),
            1 => this.ShiftR(),
            2 => this.ShiftRR(),
            3 => this.ShiftRRR(),
            _ => GameController.DoNothing()
        };

        
        if (base.UseUnityCoroutines)
        {
            yield return base.GameController.StartCoroutine(coroutine);
        }
        else
        {
            base.GameController.ExhaustCoroutine(coroutine);
        }
        ////Switch visual
        //coroutine = base.GameController.SwitchCards(this.CurrentShiftTrack, base.FindCard(this.ShiftTrackIdentifierWithPrefixAndCurrentPosition, false), cardSource: GetCardSource());
        //if (base.UseUnityCoroutines)
        //{
        //    yield return base.GameController.StartCoroutine(coroutine);
        //}
        //else
        //{
        //    base.GameController.ExhaustCoroutine(coroutine);
        //}

 
        
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
        string[] inactivePositions = [
            DriftPosition + 1,
            DriftPosition + 2,
            DriftPosition + 3,
            DriftPosition + 4
        ];
        //Get all entries of any of the positions being toggled
        IEnumerable<CardPropertiesJournalEntry> positionEntries = base.Journal.CardPropertiesEntries((CardPropertiesJournalEntry entry) => entry.Card.SharedIdentifier == this.ShiftTrackIdentifier && inactivePositions.Contains(entry.Key));

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

        if(customMode == CustomMode.AskToSwapFromPlay)
        {
            return new CustomDecisionText($"Do you want to switch character cards before playing [b]{customTextCardBeingPlayed.Title}[/b]?", $"Should they switch character cards before playing {customTextCardBeingPlayed.Title}?", $"Vote for if they should switch character cardsbefore playing {customTextCardBeingPlayed.Title}?", $"switching character cards before playing {customTextCardBeingPlayed.Title}");
        }

        return base.GetCustomDecisionText(decision);
    }

    
}
