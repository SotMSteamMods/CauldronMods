using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.TheCybersphere
{
    public class ReplicationCardController : TheCybersphereCardController
    {

        public ReplicationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, destroy this card.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);
        }

        public override IEnumerator Play()
        {
            //When this card enters play, play the top X cards of the environment deck, where X is 1 plus the number of Grid Virus cards currently in play.

            int? X = new int?(GetNumberOfGridVirusesInPlay() + 1);
            int cardsPlayed = 0;
            IEnumerator coroutine;
            IEnumerator coroutine2;
            while (cardsPlayed < X.Value)
            {
                cardsPlayed++;
                coroutine = GameController.SendMessageAction(base.Card.Title + " plays the top card of the environment deck...", Priority.High, GetCardSource(), showCardSource: true);
                coroutine2 = GameController.PlayTopCard(DecisionMaker, base.TurnTakerController, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                    yield return GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                    GameController.ExhaustCoroutine(coroutine2);
                }

                X = new int?(GetNumberOfGridVirusesInPlay() + 1);

            }
            yield break;
        }

    }
}