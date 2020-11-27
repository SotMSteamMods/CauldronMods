using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class GlimpseOfThingsToComeCardController : CardController
    {
        public GlimpseOfThingsToComeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            var storedResults = new List<SelectLocationDecision> { };
            //"Reveal the top card of the Villain deck, then replace it.",
            IEnumerator coroutine = GameController.SelectADeck(DecisionMaker, SelectionType.RevealTopCardOfDeck, (Location deck) => deck.IsVillain, storedResults, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (DidSelectLocation(storedResults))
            {
                Location villainDeck = storedResults.FirstOrDefault().SelectedLocation.Location;
                coroutine = GameController.RevealCards(DecisionMaker, villainDeck, (Card c) => true, 1, null, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            //"You may draw a card.",
            coroutine = DrawCard();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            //"You may play a card."
            coroutine = SelectAndPlayCardFromHand(DecisionMaker);
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