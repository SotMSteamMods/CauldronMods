using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;
using static Cauldron.Drift.IDriftShifting;

namespace Cauldron.Drift
{
    public abstract class DriftFocusUtilityCardController : DriftBaseCardController
    {
        private string CustomTextDirection { get; set; }

        protected DriftFocusUtilityCardController(Card card, TurnTakerController turnTakerController, string customTextDirection) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowIfElseSpecialString(() => this.HasShiftedThisTurn(), () => $"{TurnTaker.Name} has shifted this turn", () => $"{TurnTaker.Name} has not shifted this turn");
            CustomTextDirection = customTextDirection;
        }

        public override IEnumerator Play()
        {
            //When this card enters play, return all other focus cards to your hand.
            IEnumerator coroutine = base.GameController.MoveCards(base.TurnTakerController, base.FindCardsWhere(new LinqCardCriteria((Card c) => base.IsFocus(c) && c.IsInPlayAndHasGameText && c != this.Card)), (Card c) => new MoveCardDestination(base.HeroTurnTaker.Hand), cardSource: base.GetCardSource());
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

        public override void AddTriggers()
        {
            //When {Drift} is dealt damage, if you have not shifted this turn...
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => IsTargetSelf(action) && action.Amount > 0 && !this.HasShiftedThisTurn(), this.ShiftResponse, TriggerType.ModifyTokens, TriggerTiming.After);
        }

        public abstract IEnumerator ShiftResponse(DealDamageAction action);

        private bool HasShiftedThisTurn()
        {
            return base.Journal.CardPropertiesEntriesThisTurn((CardPropertiesJournalEntry entry) => entry.Key == this.HasShiftedPropertyKey()).Any();
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            if (CurrentCustomTextMode == DriftCustomTextMode.AskToShift)
            {
                return new CustomDecisionText($"Do you want to shift {CustomTextDirection}?", $"Should they shift {CustomTextDirection}?", $"Vote for if they should shift {CustomTextDirection}", $"shifting {CustomTextDirection}");
            }

            return base.GetCustomDecisionText(decision);
        }
    }
}