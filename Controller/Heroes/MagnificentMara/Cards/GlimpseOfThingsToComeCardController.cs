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
            IEnumerator coroutine = GameController.SelectADeck(DecisionMaker, SelectionType.RevealTopCardOfDeck, (Location deck) => IsVillain(deck.OwnerTurnTaker), storedResults, cardSource: GetCardSource());
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
                var revealStorage = new List<RevealCardsAction> { };
                coroutine = GameController.RevealCards(DecisionMaker, villainDeck, (Card c) => true, 1, revealStorage, revealedCardDisplay: RevealedCardDisplay.None, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                if (revealStorage.Any() && revealStorage.FirstOrDefault().RevealedCards.Any())
                {
                    Card c = revealStorage.FirstOrDefault().RevealedCards.First();
                    coroutine = GameController.SendMessageAction($"{this.Card.Title} reveals {c.Title}", Priority.Medium, GetCardSource(), new Card[] { c });
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                        else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                    coroutine = GameController.MoveCard(DecisionMaker, c, villainDeck, cardSource: GetCardSource());
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
                    coroutine = GameController.SendMessageAction($"There were no cards to reveal!", Priority.Medium, GetCardSource());

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
            //"You may draw a card.",
            coroutine = DrawCard(this.HeroTurnTaker, optional: true);
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