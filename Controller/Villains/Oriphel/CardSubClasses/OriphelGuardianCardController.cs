using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.Oriphel
{
    public class OriphelGuardianCardController : OriphelUtilityCardController
    {
        public OriphelGuardianCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria((Card c) => c.IsRelic, "relic"));
        }

        public override void AddTriggers()
        {
            //"When this card is destroyed, reveal cards from the top of the villain deck until a Relic is revealed and play it. 
            //Shuffle the remaining cards into the villain deck."
            AddWhenDestroyedTrigger(RevealRelicResponse, new TriggerType[] { TriggerType.RevealCard, TriggerType.PlayCard });
        }

        private IEnumerator RevealRelicResponse(DestroyCardAction dc)
        {
            //...reveal cards from the top of the villain deck until a Relic is revealed...
            var storedRelic = new List<RevealCardsAction> { };
            IEnumerator coroutine = GameController.RevealCards(TurnTakerController, TurnTaker.Deck, (Card c) => c.IsRelic, 1, storedRelic, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (storedRelic.FirstOrDefault() != null && storedRelic.FirstOrDefault().FoundMatchingCards)
            {
                //...and play it. 
                Card relic = storedRelic.FirstOrDefault().MatchingCards.FirstOrDefault();
                coroutine = GameController.PlayCard(TurnTakerController, relic, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            //Shuffle the remaining cards into the villain deck.
            coroutine = CleanupRevealedCards(TurnTaker.Revealed, TurnTaker.Deck, shuffleAfterwards: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}