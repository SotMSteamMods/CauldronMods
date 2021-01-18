using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class DualShiftTrackUtilityCardController : ShiftTrackUtilityCardController
    {
        public DualShiftTrackUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowSpecialString(() => this.GetInactiveCharacterCard().AlternateTitleOrTitle + " is the inactive {Drift}, she is at position " + this.InactiveCharacterPosition());
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
            base.AddTrigger<ActivateAbilityAction>((ActivateAbilityAction action) => !base.HasBeenSetToTrueThisTurn(OncePerTurn), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction action) => !base.HasBeenSetToTrueThisTurn(OncePerTurn), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => !base.HasBeenSetToTrueThisTurn(OncePerTurn), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<DiscardCardAction>((DiscardCardAction action) => !base.HasBeenSetToTrueThisTurn(OncePerTurn), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<DrawCardAction>((DrawCardAction action) => !base.HasBeenSetToTrueThisTurn(OncePerTurn), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<GainHPAction>((GainHPAction action) => !base.HasBeenSetToTrueThisTurn(OncePerTurn), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<GiveHighFiveAction>((GiveHighFiveAction action) => !base.HasBeenSetToTrueThisTurn(OncePerTurn), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<ModifyTokensAction>((ModifyTokensAction action) => !base.HasBeenSetToTrueThisTurn(OncePerTurn), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<MoveCardAction>((MoveCardAction action) => !base.HasBeenSetToTrueThisTurn(OncePerTurn), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<PhaseChangeAction>((PhaseChangeAction action) => !base.HasBeenSetToTrueThisTurn(OncePerTurn), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
            base.AddTrigger<UsePowerAction>((UsePowerAction action) => !base.HasBeenSetToTrueThisTurn(OncePerTurn), this.TrackResponse, TriggerType.ModifyTokens, TriggerTiming.Before);
        }

        private IEnumerator TrackResponse(GameAction action)
        {
            IEnumerator coroutine;
            //Once per turn you may do the following in order:
            base.SetCardPropertyToTrueIfRealAction(OncePerTurn);

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
            if (base.IsPropertyTrue(DriftPosition + 1))
            {
                inactivePosition = 1;
            }
            if (base.IsPropertyTrue(DriftPosition + 2))
            {
                inactivePosition = 2;
            }
            if (base.IsPropertyTrue(DriftPosition + 3))
            {
                inactivePosition = 3;
            }
            if (base.IsPropertyTrue(DriftPosition + 4))
            {
                inactivePosition = 4;
            }
            return inactivePosition;
        }
    }
}
