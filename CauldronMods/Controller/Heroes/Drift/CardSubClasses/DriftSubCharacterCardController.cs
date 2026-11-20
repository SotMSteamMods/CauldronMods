using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using static Cauldron.Drift.IDriftShifting;

namespace Cauldron.Drift;

public abstract class DriftSubCharacterCardController : HeroCharacterCardController, IDriftShifting
{
    protected DriftSubCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
        var positionString = base.SpecialStringMaker.ShowIfElseSpecialString(
            () => this.IsTimeMatchingPast, () => $"{TurnTaker.NameRespectingVariant} is at position {this.CurrentShiftPosition}. {this.Past} effects are active.", 
            () => $"{TurnTaker.NameRespectingVariant} is at position {this.CurrentShiftPosition}. {this.Future} effects are active.");
        positionString.Condition = () => this.CurrentShiftTrack != null;

        if(shouldRunSetUp)
        {
            AddThisCardControllerToList(CardControllerListType.EnteringGameCheck);
        }
    }

    public DriftCustomTextMode CurrentCustomTextMode { get; set; }
    public GameController ShiftingGameController => GameController;
    public CardSource ShiftingCardSource => GetCardSource();
    public HeroTurnTakerController ShiftingDecisionMaker => DecisionMaker;
    public CharacterCardController ShiftingCharacterCardController => CharacterCardController;

    protected virtual bool shouldRunSetUp => true;

    protected bool _inTheMiddleOfPower = false;

    private bool _instantiatingShiftTracks = false;

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
        if (HeroTurnTakerController is DriftTurnTakerController dttc)
        {
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

        AddTrigger((GameAction ga) => TurnTakerController is not DriftTurnTakerController && this.CurrentShiftTrack is null && !_instantiatingShiftTracks, ga => InstatiateShiftTrack(), TriggerType.Hidden, TriggerTiming.Before, priority: TriggerPriority.High);
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
        if (this.IsTimeMatchingPast && GetActiveCharacterCard() == CharacterCardWithoutReplacements && CharacterCardWithoutReplacements.Identifier.Contains("Red"))
        {
            needsChange = true;
        }
        else
        {
            if (this.IsTimeMatchingFuture && GetActiveCharacterCard() == CharacterCardWithoutReplacements && CharacterCardWithoutReplacements.Identifier.Contains("Past"))
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

    private DeckDefinition DriftDeckDefinition => DeckDefinitionCache.GetDeckDefinition("Cauldron.Drift");
   
    
    protected IEnumerator InstatiateShiftTrack()
    {
        _instantiatingShiftTracks = true;
        Log.Debug("Instantiating Shift Track");
        IEnumerable<CardDefinition> shiftTrackDefinitions = DriftDeckDefinition.CardDefinitions.Where(cd => cd.Identifier.Contains(this.ShiftTrackIdentifierWithPrefix));
        List<Card> shiftTracks = [];
        Card modelCard;
        CardController cardController;
        Dictionary<Card, CardController> cardToControllerDict = [];
        string overrideNamespace; 
        List<string> list;
        foreach (CardDefinition trackDefinition in shiftTrackDefinitions)
        {
            overrideNamespace = $"{trackDefinition.Namespace}.{DriftDeckDefinition.Identifier}";
            list = [overrideNamespace, trackDefinition.QualifiedIdentifier];
            modelCard = new Card(trackDefinition, TurnTaker, 0);
            TurnTaker.OffToTheSide.AddCard(modelCard);
            shiftTracks.Add(modelCard);
            cardController = CardControllerFactory.CreateInstance(modelCard, TurnTakerController, overrideNamespace: "Cauldron.Drift");
            TurnTakerController.AddCardController(cardController);
            cardToControllerDict.Add(modelCard, cardController);
            GameController.AddCardPropertyJournalEntry(modelCard, "OverrideTurnTaker", list);
        }

        List<SelectCardDecision> cardDecisions = [];
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
        PlayCardAction playShiftAction = new(GameController, TurnTakerController, selectedTrack, isPutIntoPlay: true, responsibleTurnTaker: TurnTaker, null, null, null, false, canBeCancelled: false)
        {
            AllowTriggersToRespond = false
        };
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
        int tokensToAdd = selectTrackController switch
        {
            BaseShiftTrack1CardController or DualShiftTrack1CardController or ThroughTheBreachShiftTrack1CardController => 1,
            BaseShiftTrack2CardController or DualShiftTrack2CardController or ThroughTheBreachShiftTrack2CardController => 2,
            BaseShiftTrack3CardController or DualShiftTrack3CardController or ThroughTheBreachShiftTrack3CardController => 3,
            BaseShiftTrack4CardController or DualShiftTrack4CardController or ThroughTheBreachShiftTrack4CardController => 4,
            _ => 0
        };

        coroutine = base.GameController.AddTokensToPool(selectedTrack.FindTokenPool(this.ShiftPoolIdentifier), tokensToAdd, new CardSource(this));
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

    public override CustomDecisionText GetCustomDecisionText(IDecision decision) => CurrentCustomTextMode switch
    {
        DriftCustomTextMode.DualDriftShift => this.GetDualDriftCustomDecisionText(decision),
        DriftCustomTextMode.StartOfGameChooseShiftTrackPosition => new(
            "Where do you want to start on the Shift Track?",
            "Where should they start on the Shift Track?",
            "Vote for where they should start on the Shift Track?",
            "starting Shift Track position"),
        _ => base.GetCustomDecisionText(decision)
    };

    public override bool ShouldChangeCutout(CutoutInfo currentInfo, GameAction action, ActionTiming timing, out CutoutInfo changedInfo, out CutoutAnimation animation)
    {
        bool result = base.ShouldChangeCutout(currentInfo, action, timing, out changedInfo, out animation);
        if (action is not ModifyTokensAction modifyTokensAction) return result;
        if (modifyTokensAction.TokenPool.Identifier != this.ShiftPoolIdentifier) return result;

        if (timing == ActionTiming.WillPerform)
        {
            animation = CutoutAnimation.Fade;
            result = true;
            changedInfo.Identifier = currentInfo.Identifier;
            changedInfo.ExtraCutouts = [];

            return result;
        }

        if (timing == ActionTiming.DidPerform)
        {
            string updatedSuffix = this.IsTimeMatchingPast switch
            {
                true => "Blue",
                false => "Red"
            };

            if (currentInfo.HeroTurnSuffix == updatedSuffix) return result;

            result = true;
            changedInfo.Identifier = currentInfo.Identifier;
            changedInfo.HeroTurnSuffix = updatedSuffix;
            changedInfo.VillainTurnSuffix = updatedSuffix;
            animation = CutoutAnimation.Fade;
        }

        return result;
    }
}