using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class BankHeistCardController : DynamoUtilityCardController
    {
        public BankHeistCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, each player may discard 1 card. If fewer than {H - 1} cards were discarded this way, discard the top card of the villain deck.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DiscardCardsResponse, TriggerType.DiscardCard);
        }

        private IEnumerator DiscardCardsResponse(PhaseChangeAction action)
        {
            //...each player may discard 1 card. 
            List<DiscardCardAction> storedResultsDiscard = new List<DiscardCardAction>();
            IEnumerator coroutine = base.GameController.EachPlayerDiscardsCards(0, 1, storedResultsDiscard, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //If fewer than {H - 1} cards were discarded this way...
            if (base.GetNumberOfCardsDiscarded(storedResultsDiscard) < base.Game.H - 1)
            {
                //...discard the top card of the villain deck.
                List<MoveCardAction> storedResult = new List<MoveCardAction>();
                coroutine = base.GameController.DiscardTopCard(base.TurnTaker.Deck, storedResult, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}
