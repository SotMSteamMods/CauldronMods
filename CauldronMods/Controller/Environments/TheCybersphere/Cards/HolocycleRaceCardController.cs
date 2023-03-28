using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheCybersphere
{
    public class HolocycleRaceCardController : TheCybersphereCardController
    {

        public HolocycleRaceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Increase all damage dealt by 1.
            AddIncreaseDamageTrigger((DealDamageAction dd) => true, 1);

            //At the start of the environment turn, each player may discard up to 2 cards. Then, unless {H} cards were discarded this way, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, EachPlayerDiscardsCardsToDestroyCard, new TriggerType[]
            {
                TriggerType.DiscardCard,
                TriggerType.DestroySelf
            });
        }

        private IEnumerator EachPlayerDiscardsCardsToDestroyCard(PhaseChangeAction pca)
        {
            if (FindTurnTakersWhere((TurnTaker tt) => IsHero(tt)).Any())
            {
                //each player may discard up to 2 cards
                List<DiscardCardAction> storedDiscards = new List<DiscardCardAction>();
                IEnumerator coroutine = GameController.EachPlayerDiscardsCards(0, 2, storedResultsDiscard: storedDiscards, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //Then, unless {H} cards were discarded this way, destroy this card.
                //confirmed with tosx, this must be exact
                if (storedDiscards.Count() == base.H)
                {
                    coroutine = GameController.SendMessageAction($"Exactly {storedDiscards.Count()} cards were discarded for {Card.Title}, so it remains in play.", Priority.High, GetCardSource(), new[] { base.Card });
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }
                else
                {
                    coroutine = DestroyThisCardResponse(pca);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
            else
            {
                string message = $"There are no players in the {BattleZone.Name} to discard for {Card.Title}.";
                IEnumerator coroutine4 = GameController.SendMessageAction(message, Priority.Low, GetCardSource(), new Card[1]
                {
                    base.Card
                });
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine4);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine4);
                }
            }
        }
    }
}