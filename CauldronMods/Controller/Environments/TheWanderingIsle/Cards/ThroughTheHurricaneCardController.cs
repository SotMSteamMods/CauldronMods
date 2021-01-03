using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheWanderingIsle
{
    public class ThroughTheHurricaneCardController : TheWanderingIsleCardController
    {
        public ThroughTheHurricaneCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHighestHP(ranking: 3);
        }

        public override void AddTriggers()
        {
            //Whenever a target enters play, this card deals {H - 1} lightning damage to the target with the third highest HP.
            base.AddTargetEntersPlayTrigger((Card c) => true, (Card c) => base.DealDamageToHighestHP(base.Card, 3, (Card card) => true, (Card card) => base.H - 1, DamageType.Lightning), TriggerType.DealDamage, TriggerTiming.After);
            //At the start of the environment turn, you may play the top 2 cards of the environment deck. If you do, this card is destroyed.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.PlayCardsToDestroyResponse, new TriggerType[]
            {
                TriggerType.PlayCard,
                TriggerType.DestroySelf
            });
        }

        private IEnumerator PlayCardsToDestroyResponse(PhaseChangeAction phaseChange)
        {
            //you may play the top 2 cards of the environment deck. If you do, this card is destroyed.
            YesNoDecision yesNo = new YesNoDecision(base.GameController, this.DecisionMaker, SelectionType.PlayTopCardOfEnvironmentDeck, requireUnanimous: true, cardSource: GetCardSource());
            IEnumerator coroutine = base.GameController.MakeDecisionAction(yesNo);
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
                coroutine = base.GameController.PlayTopCard(this.DecisionMaker, base.FindEnvironment(), numberOfCards: 2, playedCards: playedCards, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                //if you did destroy this card
                if (playedCards.Count() >= 2)
                {
                    coroutine = base.GameController.DestroyCard(this.DecisionMaker, base.Card, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
                else
                {
                    //send message to indicate no destruction
                    string message;
                    if (playedCards.Count<Card>() == 1)
                    {
                        message = $"The environment only played 1 card, so {base.Card.Title} will not be destroyed.";
                    }
                    else
                    {
                        message = $"The environment did not play a card, so {base.Card.Title} will not be destroyed.";
                    }
                    coroutine = base.GameController.SendMessageAction(message, Priority.High, base.GetCardSource(), showCardSource: true);
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
            yield break;
        }


    }
}
