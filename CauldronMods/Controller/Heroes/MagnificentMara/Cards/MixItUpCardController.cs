using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class MixItUpCardController : CardController
    {
        public MixItUpCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Destroy 1 hero ongoing card, equipment card, or environment card.",
            var storedDestroy = new List<DestroyCardAction> { };
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => (IsOngoing(c) && IsHero(c)) || IsEquipment(c) || c.IsEnvironment), false, storedDestroy, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //"If you do, reveal the top 2 cards of the associated deck, put one into play and discard the other."
            if(DidDestroyCard(storedDestroy))
            {
                var associatedDeck = storedDestroy.FirstOrDefault().CardToDestroy.Card.NativeDeck;
                var destinations = new List<MoveCardDestination> {
                        new MoveCardDestination(associatedDeck.OwnerTurnTaker.PlayArea),
                        new MoveCardDestination(associatedDeck.OwnerTurnTaker.Trash)
                    };
                coroutine = RevealCardsFromDeckToMoveToOrderedDestinations(DecisionMaker, associatedDeck, destinations, numberOfCardsToReveal: 2, isPutIntoPlay: true);
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