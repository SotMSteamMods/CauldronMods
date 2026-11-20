using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using static Cauldron.Drift.IDriftShifting;

namespace Cauldron.Drift;

public abstract class DriftBaseCardController : CardController, IDriftShifting
{
    protected DriftBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
    {
        var positionString = base.SpecialStringMaker.ShowIfElseSpecialString(
            () => this.IsTimeMatchingPast, () => $"{TurnTaker.NameRespectingVariant} is at position {this.CurrentShiftPosition}. {this.Past} effects are active.", 
            () => $"{TurnTaker.NameRespectingVariant} is at position {this.CurrentShiftPosition}. {this.Future} effects are active.");
        positionString.Condition = () => this.CurrentShiftTrack != null;
    }

    public DriftCustomTextMode CurrentCustomTextMode { get; set; }
    public GameController ShiftingGameController => GameController;
    public CardSource ShiftingCardSource => GetCardSource();
    public HeroTurnTakerController ShiftingDecisionMaker => DecisionMaker;
    public CharacterCardController ShiftingCharacterCardController => CharacterCardController;

    public Card GetActiveCharacterCard()
    {
        return base.FindCardsWhere(new LinqCardCriteria((Card c) =>  IsHeroCharacterCard(c) && c.Location == base.TurnTaker.PlayArea && c.Owner == base.TurnTaker)).FirstOrDefault();
    }

    public bool IsFocus(Card c)
    {
        return c.DoKeywordsContain("focus");
    }

    public bool IsTargetSelf(DealDamageAction dd)
    {
        if(dd.Target != null && dd.Target.SharedIdentifier != null && dd.Target.SharedIdentifier == GetActiveCharacterCard().SharedIdentifier)
        {
            return true;
        }
        else if (dd.Target != null && dd.Target == GetActiveCharacterCard())
        {
            return true;
        }
        return false;
    }

    public override CustomDecisionText GetCustomDecisionText(IDecision decision)
    {
        if (CurrentCustomTextMode == DriftCustomTextMode.DualDriftShift)
        {
            return this.GetDualDriftCustomDecisionText(decision);
        }

        return base.GetCustomDecisionText(decision);
    }
}
