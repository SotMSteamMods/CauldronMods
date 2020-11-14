using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheWanderingIsle
{
    public class ThroughTheHurricaneCardController : CardController
    {
        public ThroughTheHurricaneCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Whenever a target enters play, this card deals {H - 1} lightning damage to the target with the third highest HP.
            base.AddTargetEntersPlayTrigger((Card c) => true, (Card c) => base.DealDamageToHighestHP(base.Card, 3, (Card card) => true, (Card card) => new int?(base.H - 1), DamageType.Lightning), TriggerType.DealDamage, TriggerTiming.After);
            //At the start of the environment turn, you may play the top 2 cards of the environment deck. If you do, this card is destroyed.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.PlayCardsToDestroyResponse), new TriggerType[]
            {
                TriggerType.PlayCard,
                TriggerType.DestroySelf
            }, null, false);
        }

        private IEnumerator PlayCardsToDestroyResponse(PhaseChangeAction phaseChange)
        {

            //you may play the top 2 cards of the environment deck. If you do, this card is destroyed.
            YesNoDecision yesNo = new YesNoDecision(base.GameController, this.DecisionMaker, SelectionType.PlayTopCardOfEnvironmentDeck, true, null, null, base.GetCardSource(null));
            IEnumerator coroutine = base.GameController.MakeDecisionAction(yesNo, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (base.DidPlayerAnswerYes(yesNo))
            {
                //play the top 2 cards of the environment deck
                List<Card> playedCards = new List<Card>();
                coroutine = base.GameController.PlayTopCard(this.DecisionMaker, base.FindEnvironment(null), false, 2, false, playedCards, null, null, false, null, false, false, false, null, null, base.GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                //if you did destroy this card
                if (playedCards.Count<Card>() == 2)
                {
                    IEnumerator destroy = base.GameController.DestroyCard(this.DecisionMaker, base.Card, false, null, null, null, null, null, null, null, null, base.GetCardSource(null));
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(destroy);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(destroy);
                    }
                }
                else
                {
                    //send message to indicate no destruction
                    string message;
                    if (playedCards.Count<Card>() == 1)
                    {

                        message = "The environment only played 1 card, so " + base.Card.Title + " will not be destroyed.";
                    }
                    else
                    {
                        message = "The environment did not play a card, so " + base.Card.Title + " will not be destroyed.";
                    }
                    IEnumerator sendMessage = base.GameController.SendMessageAction(message, Priority.High, base.GetCardSource(null), null, true);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(sendMessage);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(sendMessage);
                    }
                }
            }
            yield break;
        }


    }
}
