using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class KnightsHeritageCardController : DriftUtilityCardController
    {
        public KnightsHeritageCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowIfElseSpecialString(() => base.HasBeenSetToTrueThisTurn(DamageTakenThisTurn), () => $"{GetActiveCharacterCard().AlternateTitleOrTitle} has been dealt damage this turn", () => $"{GetActiveCharacterCard().AlternateTitleOrTitle} has not been dealt damage this turn");
        }

        protected const string DamageTakenThisTurn = "DamageTakenThisTurn";

        public override IEnumerator Play()
        {
            //When this card enters play, destroy all other copies of Knight's Heritage...
            IEnumerator coroutine = base.GameController.DestroyCards(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.Identifier == this.Card.Identifier && c != this.Card), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //...and play up to 2 ongoing cards from your hand.
            coroutine = base.SelectAndPlayCardsFromHand(base.HeroTurnTakerController, 2, cardCriteria: new LinqCardCriteria((Card c) => IsOngoing(c), "ongoing"));
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
            //The first time {Drift} is dealt damage each turn, you may shift {DriftL} or {DriftR}.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => IsTargetSelf(action) && action.Amount > 0 && !base.HasBeenSetToTrueThisTurn(DamageTakenThisTurn), this.ShiftResponse, new TriggerType[] { TriggerType.ModifyTokens }, TriggerTiming.After);
        }

        private IEnumerator ShiftResponse(DealDamageAction action)
        {
            base.SetCardPropertyToTrueIfRealAction(DamageTakenThisTurn);

            //...you may shift {DriftL} or {DriftR}.
            IEnumerator coroutine = base.SelectAndPerformFunction(base.HeroTurnTakerController, new Function[] {
                    new Function(base.HeroTurnTakerController, "Shift {ShiftL}", SelectionType.AddTokens, () => base.ShiftL()),
                    new Function(base.HeroTurnTakerController, "Shift {ShiftR}", SelectionType.RemoveTokens, () => base.ShiftR())
            }, optional: true, associatedCards: GetShiftTrack().ToEnumerable());
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
