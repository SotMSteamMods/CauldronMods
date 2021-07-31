using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class CrimeSpreeCardController : DynamoUtilityCardController
    {
        public CrimeSpreeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, the players may choose to play the top card of the environment deck. If they do not, discard the top card of the villain deck and {Dynamo} deals each hero target 1 energy damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.EndOfTurnResponse, new TriggerType[] { TriggerType.PlayCard, TriggerType.DiscardCard, TriggerType.DealDamage });
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction action)
        {
            //...the players may choose to play the top card of the environment deck.
            List<PlayCardAction> storedResults = new List<PlayCardAction>();
            IEnumerator coroutine = base.GameController.PlayTopCard(base.DecisionMaker, base.FindEnvironment(), true, storedResults: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //If they do not...
            if (!base.DidPlayCards(storedResults, 1))
            {
                //...discard the top card of the villain deck...
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

                //...and {Dynamo} deals each hero target 1 energy damage.
                coroutine = base.DealDamage(base.CharacterCard, (Card c) => c.IsHero && c.IsTarget, 1, DamageType.Energy);
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
