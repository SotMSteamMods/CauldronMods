using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class WantonDestructionCardController : DynamoUtilityCardController
    {
        public WantonDestructionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, each player may destroy 1 of their non-character cards. If fewer than {H - 2} cards were destroyed this way, discard the top card of the villain deck.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DestroyCardsResponse, new TriggerType[] { TriggerType.DestroyCard, TriggerType.DiscardCard });
        }

        private IEnumerator DestroyCardsResponse(PhaseChangeAction action)
        {
            //...each player may destroy 1 of their non-character cards.
            List<DestroyCardAction> destroyCardActions = new List<DestroyCardAction>();
            IEnumerator coroutine = base.EachPlayerDestroysTheirCards(new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt)), new LinqCardCriteria((Card c) => !c.IsCharacter), requiredNumberOfCards: 0, storedResults: destroyCardActions);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //If fewer than {H - 2} cards were destroyed this way...
            if (destroyCardActions.Count() < base.Game.H - 2)
            {
                //...discard the top card of the villain deck.
                List<MoveCardAction> moveCardActions = new List<MoveCardAction>();
                coroutine = base.GameController.DiscardTopCard(base.TurnTaker.Deck, moveCardActions);
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
