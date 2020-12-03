using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.SuperstormAkela
{
    public class FracturedSkyCardController : SuperstormAkelaCardController
    {

        public FracturedSkyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SetCardProperty("PlayToTheLeft", false);
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, reveal the top 2 cards of the environment deck. Put one into play to the left of all cards in the environment play area, and the other into play to the right.",
            List<Card> storedCards = new List<Card>();
            IEnumerator coroutine = base.GameController.RevealCards(base.TurnTakerController, base.TurnTaker.Deck, 2, storedCards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (!storedCards.Any())
            {
                yield break;
            }
            SetCardProperty("PlayToTheLeft", true);
            coroutine = base.GameController.SelectCardsAndDoAction(DecisionMaker, new LinqCardCriteria((Card c) => storedCards.Contains(c)), SelectionType.PutIntoPlay, (Card c) => GameController.PlayCard(TurnTakerController, c, isPutIntoPlay: true, cardSource: GetCardSource()), numberOfCards: new int?(1), cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            SetCardProperty("PlayToTheLeft", false);
            Card otherCard = storedCards.Where((Card c) => c.Location.IsRevealed).FirstOrDefault();
            if (otherCard != null)
            {
                coroutine = PlayCardOnRight(otherCard);
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


        private IEnumerator PlayCardOnRight(Card card)
        {
            List<PlayCardAction> storedResults = new List<PlayCardAction>();
            IEnumerator coroutine = GameController.PlayCard(DecisionMaker, card, isPutIntoPlay: true, storedResults: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidPlayCards(storedResults))
            {
                coroutine = GameController.SendMessageAction("Played " + card.Title + " to the far right of the environment's play area.", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                Log.Debug(card.Title + " was played to the far right of the environment's play area.");

            }

            yield break;
        }

        public override void AddTriggers()
        {
            //When an environment target is destroyed, destroy this card.
            AddTrigger<DestroyCardAction>((DestroyCardAction destroy) => destroy.CardToDestroy != null && destroy.CardToDestroy.Card.IsEnvironmentTarget && destroy.WasCardDestroyed, DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);
        }
    }
}