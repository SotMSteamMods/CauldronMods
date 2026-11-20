using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using static Cauldron.Drift.IDriftShifting;

namespace Cauldron.Drift;

public class DriftTurnTakerController(TurnTaker turnTaker, GameController gameController) : HeroTurnTakerController(turnTaker, gameController), IDriftShifting
{
    public DriftCustomTextMode CurrentCustomTextMode { get; set; }
    public GameController ShiftingGameController => GameController;
    public CardSource ShiftingCardSource => CharacterCardController.GetCardSource();
    public HeroTurnTakerController ShiftingDecisionMaker => this;
    public CharacterCardController ShiftingCharacterCardController => CharacterCardController;

    //public override IEnumerator StartGame()
    //{
    //    IEnumerator coroutine = SetupDrift();
    //    if (base.UseUnityCoroutines)
    //    {
    //        yield return base.GameController.StartCoroutine(coroutine);
    //    }
    //    else
    //    {
    //        base.GameController.ExhaustCoroutine(coroutine);
    //    }
    //}

    public string[] availablePromos = [];
    public Dictionary<string, string[]> nonDriftPromos = new()
    {
        { "Necro", ["LastOfTheForgottenOrderNecro"] }
    };

    public bool ArePromosSetup { get; set; } = false;

    public IEnumerator SetupDrift()
    {
        string[] tracks = [
            this.ShiftTrackIdentifierWithPrefix + 1,
            this.ShiftTrackIdentifierWithPrefix + 2,
            this.ShiftTrackIdentifierWithPrefix + 3,
            this.ShiftTrackIdentifierWithPrefix + 4,
        ];

        //At the start of the game, after drawing your cards, place a token on 1 of the 4 spaces of the shift track.
        List<SelectCardDecision> cardDecisions = [];
        (ShiftingCharacterCardController as DriftSubCharacterCardController).CurrentCustomTextMode = DriftCustomTextMode.StartOfGameChooseShiftTrackPosition;
        IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(this, SelectionType.Custom, new LinqCardCriteria((Card c) => c.SharedIdentifier == this.ShiftTrackIdentifier && tracks.Contains(c.Identifier), "Shift Track Position"), cardDecisions, false, includeRealCardsOnly: false, cardSource: ShiftingCardSource);
        if (base.UseUnityCoroutines)
        {
            yield return base.GameController.StartCoroutine(coroutine);
        }
        else
        {
            base.GameController.ExhaustCoroutine(coroutine);
        }

        if(cardDecisions.Any())
        {
            Card selectedTrack = cardDecisions.FirstOrDefault().SelectedCard;
            PlayCardAction playShiftAction = new(GameController, this, selectedTrack, isPutIntoPlay: true, responsibleTurnTaker: TurnTaker, null, null, null, false, canBeCancelled: false)
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

            CardController selectTrackController = base.FindCardController(selectedTrack);
            int tokensToAdd = selectTrackController switch
            {
                BaseShiftTrack1CardController or DualShiftTrack1CardController or ThroughTheBreachShiftTrack1CardController => 1,
                BaseShiftTrack2CardController or DualShiftTrack2CardController or ThroughTheBreachShiftTrack2CardController => 2,
                BaseShiftTrack3CardController or DualShiftTrack3CardController or ThroughTheBreachShiftTrack3CardController => 3,
                BaseShiftTrack4CardController or DualShiftTrack4CardController or ThroughTheBreachShiftTrack4CardController => 4,
                _ => 0
            };

            coroutine = base.GameController.AddTokensToPool(selectedTrack.FindTokenPool(this.ShiftPoolIdentifier), tokensToAdd, new CardSource(base.CharacterCardController));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
        
        if(base.CharacterCardController is not DualDriftSubCharacterCardController)
        {
            coroutine = GameController.BulkMoveCards(this, FindCardsWhere((Card c) => FindCardController(c) is DualDriftSubCharacterCardController), TurnTaker.InTheBox);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        yield break;
    }

    public override bool IsIncapacitated
    {
        get
        {
            if (base.TurnTaker.Identifier == "Drift")
            {
                if (this.GetActiveCharacterCard().IsFlipped)
                {
                    return true;
                }
            }
            return base.IsIncapacitated;
        }
    }

    public override bool IsIncapacitatedOrOutOfGame
    {
        get
        {
            if (!IsIncapacitated)
            {
                return IncapacitationCardController.Card.IsOutOfGame;
            }
            return true;
        }
    }

    public Card GetActiveCharacterCard()
    {
        return base.FindCardsWhere((Card c) => c.IsHeroCharacterCard && c.Location == base.TurnTaker.PlayArea && c.Owner == this.TurnTaker).FirstOrDefault();
    }
}
