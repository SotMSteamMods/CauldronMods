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
        }

        public override IEnumerator Play()
        {
            //"Draw a card, or search your trash and deck for a constellation card, put it into play, then shuffle your deck."
            var actionList = new List<Function> { };
            bool hasConstellationInDeck = HeroTurnTakerController.HasCardsWhere((Card c) => c.IsInDeck && IsConstellation(c));
            bool hasConstellationInTrash = HeroTurnTakerController.HasCardsWhere((Card c) => c.IsInTrash && IsConstellation(c));
            string heroName = TurnTaker.Name;

            var constellationSearchAndPlay = SearchForConstellationAndShuffleDeck(HeroTurnTakerController, hasConstellationInDeck, hasConstellationInTrash);

            actionList.Add(new Function(HeroTurnTakerController, "Draw a card", SelectionType.DrawCard, () => DrawCard(HeroTurnTaker, false), HeroTurnTakerController != null && CanDrawCards(HeroTurnTakerController), heroName + " has no constellations in their deck or trash, so they must draw a card."));
            actionList.Add(new Function(HeroTurnTakerController, "Search for a constellation", SelectionType.SearchLocation, () => constellationSearchAndPlay, HeroTurnTakerController != null && (hasConstellationInDeck || hasConstellationInTrash), heroName + " cannot draw any cards, so they must search for a constellation."));
            SelectFunctionDecision selectFunction = new SelectFunctionDecision(GameController, HeroTurnTakerController, actionList, false, noSelectableFunctionMessage: heroName + " can neither draw nor search for a constellation, so nothing happens.", cardSource: GetCardSource());
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
            IEnumerator mayPlayCard = SelectAndPlayCardFromHand(HeroTurnTakerController);

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

        private IEnumerator SearchForConstellationAndShuffleDeck(HeroTurnTakerController heroTurnTakerController, bool hasConstellationInDeck, bool hasConstellationInTrash)
        {

            IEnumerator search = SearchForCards(heroTurnTakerController, searchDeck: hasConstellationInDeck, searchTrash: hasConstellationInTrash, 1, 1, new LinqCardCriteria((Card c) => IsConstellation(c)), putIntoPlay: true, putInHand: false, putOnDeck: false, shuffleAfterwards: false);
            IEnumerator shuffle = ShuffleDeck(heroTurnTakerController, heroTurnTakerController.TurnTaker.Deck);
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