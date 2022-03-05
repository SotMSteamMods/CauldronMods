using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public abstract class DriftBaseCardController : CardController
    {
        protected DriftBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            var positionString = base.SpecialStringMaker.ShowIfElseSpecialString(() => this.IsTimeMatching(Past), () => $"{TurnTaker.NameRespectingVariant} is at position {this.CurrentShiftPosition()}. {Past} effects are active.", () => $"{TurnTaker.NameRespectingVariant} is at position {this.CurrentShiftPosition()}. {Future} effects are active.");
            positionString.Condition = () => GetShiftTrack() != null;
        }

        protected const string Base = "Base";
        protected const string Dual = "Dual";
        protected const string ThroughTheBreach = "ThroughTheBreach";

        protected const string Past = "{Past}";
        protected const string Future = "{Future}";

        protected const string HasShifted = "HasShifted";
        protected const string ShiftTrack = "ShiftTrack";
        protected const string ShiftPoolIdentifier = "ShiftPool";

        public int CurrentShiftPosition()
        {
            if (this.GetShiftPool() == null)
            {
                return 0;
            }
            return this.GetShiftPool().CurrentValue;
        }

        public Card GetActiveCharacterCard()
        {
            return base.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsHeroCharacterCard && c.Location == base.TurnTaker.PlayArea && c.Owner == base.TurnTaker)).FirstOrDefault();
        }

        public TokenPool GetShiftPool()
        {
            return this.GetShiftTrack()?.FindTokenPool("ShiftPool");
        }

        public Card GetShiftTrack()
        {
            return base.FindCardsWhere((Card c) => c.SharedIdentifier == ShiftTrack && c.IsInPlayAndHasGameText, false).FirstOrDefault();
        }

        public bool IsFocus(Card c)
        {
            return c.DoKeywordsContain("focus");
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
    }
}
