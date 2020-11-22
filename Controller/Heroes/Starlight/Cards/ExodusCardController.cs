using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class ExodusCardController : StarlightCardController
    {
        public ExodusCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

            SpecialStringMaker.ShowNumberOfCardsAtLocations(ImportantLocations,
                                            new LinqCardCriteria((Card c) => IsConstellation(c), "constellation"));
        }

        private IEnumerable<Location> ImportantLocations()
        {
            yield return TurnTaker.Deck;
            yield return TurnTaker.Trash;
            yield break;
        }
        public override IEnumerator Play()
        {
            //"Draw a card, or search your trash and deck for a constellation card, put it into play, then shuffle your deck."
            var httc = HeroTurnTakerController;
            var actionList = new List<Function>()
            {
                new Function(httc, "Draw a card", SelectionType.DrawCard, () => DrawCard(HeroTurnTaker, false), httc != null && CanDrawCards(httc), $"{TurnTaker.Name} has no constellations in their deck or trash, so they must draw a card."),
                new Function(httc, "Search for a constellation", SelectionType.SearchLocation, () => SearchForConstellationAndShuffleDeck(httc), httc != null && HasFindableConstellations(httc), $"{TurnTaker.Name} cannot draw any cards, so they must search for a constellation.")
            };

            SelectFunctionDecision selectFunction = new SelectFunctionDecision(GameController, httc, actionList, false,
                noSelectableFunctionMessage: $"{TurnTaker.Name} can neither draw nor search for a constellation, so nothing happens.",
                cardSource: GetCardSource());
            IEnumerator drawOrSearch = GameController.SelectAndPerformFunction(selectFunction);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(drawOrSearch);
            }
            else
            {
                GameController.ExhaustCoroutine(drawOrSearch);
            }

            //"You may play a card."
            IEnumerator mayPlayCard = SelectAndPlayCardFromHand(httc);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(mayPlayCard);
            }
            else
            {
                GameController.ExhaustCoroutine(mayPlayCard);
            }

            yield break;
        }

        private bool HasFindableConstellations(HeroTurnTakerController httc)
        {
            return httc.HasCardsWhere(c => IsConstellation(c) && (c.IsInDeck || c.IsInTrash));
        }

        private IEnumerator SearchForConstellationAndShuffleDeck(HeroTurnTakerController httc)
        {
            //we pre search the decks to control the locations, otherwise SearchForCard will ask which you to search locations that don't have the card, and Exodus doesn't have the restriction.
            bool hasConstellationInTrash = httc.HasCardsWhere(c => IsConstellation(c) && c.IsInTrash);
            bool hasConstellationInDeck = httc.HasCardsWhere(c => IsConstellation(c) && c.IsInDeck);

            //if not anywhere, set deck to true so we get the nice no cards found message
            if (!hasConstellationInTrash && !hasConstellationInDeck)
                hasConstellationInDeck = true;

            IEnumerator search = SearchForCards(httc, hasConstellationInDeck, hasConstellationInTrash, 1, 1, new LinqCardCriteria((Card c) => IsConstellation(c)), putIntoPlay: true, putInHand: false, putOnDeck: false, shuffleAfterwards: false);
            IEnumerator shuffle = ShuffleDeck(httc, httc.TurnTaker.Deck);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(search);
                yield return GameController.StartCoroutine(shuffle);
            }
            else
            {
                GameController.ExhaustCoroutine(search);
                GameController.ExhaustCoroutine(shuffle);
            }
            yield break;
        }
    }
}