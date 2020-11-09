using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron
{
    public class TemporalSlipstreamCardController : CardController
    {
        #region Constructors

        public TemporalSlipstreamCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //When this card enters play, each player discards their hand and draws that many cards.
            IEnumerator coroutine = base.DoActionToEachTurnTakerInTurnOrder((TurnTakerController turnTakerController) => turnTakerController.IsHero, DiscardAndDrawResponse);
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

        private IEnumerator DiscardAndDrawResponse(TurnTakerController turnTakerController)
        {
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            IEnumerator coroutine = base.GameController.DiscardHand(turnTakerController.ToHero(), false, storedResults, this.TurnTaker, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            int numberOfCardsDiscarded = base.GetNumberOfCardsDiscarded(storedResults);
            if (numberOfCardsDiscarded > 0)
            {
                coroutine = base.DrawCards(this.DecisionMaker, numberOfCardsDiscarded, false, false, null, true, null);
            }
            else
            {
                coroutine = base.GameController.SendMessageAction(base.TurnTaker.Name + " did not discard any cards, so no cards will be drawn.", Priority.High, base.GetCardSource(null), null, true);
            }
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
            //At the end of the environment turn, destroy this card.
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(base.DestroyThisCardResponse), TriggerType.DestroySelf);
        }

        #endregion Methods
    }
}