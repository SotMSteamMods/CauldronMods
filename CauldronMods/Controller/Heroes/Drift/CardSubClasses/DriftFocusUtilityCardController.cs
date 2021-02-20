using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Drift
{
    public abstract class DriftFocusUtilityCardController : DriftUtilityCardController
    {
        protected DriftFocusUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowIfElseSpecialString(() => this.HasShiftedThisTurn(), () => $"{TurnTaker.Name} has shifted this turn", () => $"{TurnTaker.Name} has not shifted this turn");
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
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.Target == base.GetActiveCharacterCard() && action.Amount > 0 && !this.HasShiftedThisTurn(), this.ShiftResponse, TriggerType.ModifyTokens, TriggerTiming.After);
        }

        public abstract IEnumerator ShiftResponse(DealDamageAction action);

        private bool HasShiftedThisTurn()
        {
            return base.Journal.CardPropertiesEntriesThisTurn((CardPropertiesJournalEntry entry) => entry.Key == HasShifted).Any();
        }
    }
}