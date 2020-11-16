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
            bool hasConstellationInDeck = TurnTaker.HasCardsWhere((Card c) => c.IsInDeck && IsConstellation(c));
            bool hasConstellationInTrash = TurnTaker.HasCardsWhere((Card c) => c.IsInTrash && IsConstellation(c));
            string heroName = TurnTaker.Name;

            var constellationSearchAndPlay = SearchForConstellationAndShuffleDeck(HeroTurnTakerController, hasConstellationInDeck, hasConstellationInTrash);

            actionList.Add(new Function(HeroTurnTakerController, "Draw a card", SelectionType.DrawCard, () => DrawCard(HeroTurnTaker, false), HeroTurnTakerController != null && CanDrawCards(HeroTurnTakerController), heroName + " has no constellations in their deck or trash, so they must draw a card."));
            actionList.Add(new Function(HeroTurnTakerController, "Search for a constellation", SelectionType.SearchLocation, () => constellationSearchAndPlay, HeroTurnTakerController != null && (hasConstellationInDeck || hasConstellationInTrash), heroName + " cannot draw any cards, so they must search for a constellation."));
            SelectFunctionDecision selectFunction = new SelectFunctionDecision(GameController, base.HeroTurnTakerController, actionList, false, noSelectableFunctionMessage: heroName + " can neither draw nor search for a constellation, so nothing happens.", cardSource: GetCardSource());
            IEnumerator drawOrSearch = GameController.SelectAndPerformFunction(selectFunction);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawOrSearch);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawOrSearch);
            }

            //"You may play a card."
            IEnumerator mayPlayCard = SelectAndPlayCardFromHand(HeroTurnTakerController);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(mayPlayCard);
            }
            else
            {
                base.GameController.ExhaustCoroutine(mayPlayCard);
            }

            yield break;
        }

        private IEnumerator SearchForConstellationAndShuffleDeck(HeroTurnTakerController heroTurnTakerController, bool hasConstellationInDeck, bool hasConstellationInTrash)
        {

            IEnumerator search = SearchForCards(heroTurnTakerController, searchDeck: hasConstellationInDeck, searchTrash: hasConstellationInTrash, 1, 1, new LinqCardCriteria((Card c) => IsConstellation(c)), putIntoPlay: true, putInHand: false, putOnDeck: false, shuffleAfterwards: false);
            IEnumerator shuffle = ShuffleDeck(heroTurnTakerController, heroTurnTakerController.TurnTaker.Deck);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(search);
                yield return base.GameController.StartCoroutine(shuffle);
            }
            else
            {
                base.GameController.ExhaustCoroutine(search);
                base.GameController.ExhaustCoroutine(shuffle);
            }
            yield break;
        }
    }
}