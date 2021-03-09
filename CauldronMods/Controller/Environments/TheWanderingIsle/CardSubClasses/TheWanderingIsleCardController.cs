using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheWanderingIsle
{
    public abstract class TheWanderingIsleCardController : CardController
    {
        public static readonly string TeryxIdentifier = "Teryx";

        protected TheWanderingIsleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public Card FindTeryx()
        {
            return base.TurnTaker.GetAllCards().FirstOrDefault(c => IsTeryx(c));
        }

        protected IEnumerator PlayTeryxFromDeckOrTrashThenShuffle()
        {
            var locations = new Location[]
            {
                base.TurnTaker.Deck,
                base.TurnTaker.Trash
            };

            //When this card enters play, search the environment deck and trash for Teryx and put it into play, then shuffle the deck.
            IEnumerator coroutine = base.PlayCardFromLocations(locations, TeryxIdentifier, isPutIntoPlay: true, showMessageIfFailed: false, shuffleAfterwardsIfDeck: false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.GameController.ShuffleLocation(base.TurnTaker.Deck, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }


        protected bool IsTeryx(Card card)
        {
            return card.IsInPlayAndHasGameText && card.Identifier == TeryxIdentifier;
        }

        protected bool IsCreature(Card card)
        {
            return card != null && card.DoKeywordsContain("creature");
        }

        private IEnumerable<Card> FindTeryxList()
        {
            return base.FindCardsWhere(c => c.Identifier == "Teryx");
        }
        protected bool IsTeryxInPlay()
        {
            return FindTeryxList().Where(c => c.IsInPlayAndHasGameText).Any();
        }
    }
}
