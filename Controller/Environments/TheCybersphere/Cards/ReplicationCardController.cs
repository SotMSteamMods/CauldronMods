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

            int X = GetNumberOfGridVirusesInPlay() + 1;
            IEnumerator coroutine;
            IEnumerator coroutine2;
            string cardPlural = X == 1 ? "card" : "cards";

            coroutine = GameController.SendMessageAction(base.Card.Title + $" plays the top {X} {cardPlural} of the environment deck.", Priority.High, GetCardSource(), showCardSource: true);
            coroutine2 = GameController.PlayTopCard(DecisionMaker, base.TurnTakerController, numberOfCards: X, cardSource: GetCardSource());
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
            yield break;
        }
    }
}