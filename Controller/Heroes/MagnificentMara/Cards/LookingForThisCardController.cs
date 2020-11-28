using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Cauldron.MagnificentMara
{
    public class LookingForThisCardController : CardController
    {
        public LookingForThisCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"One player may return one of their non-character cards in play to their hand.",
            //"If they do, they may select a card in their trash that shares a keyword with that card and put it into play."
            var validHeroes = new SelectTurnTakerDecision(GameController, DecisionMaker, GameController.AllHeroControllers.Select((HeroTurnTakerController httc) => httc.TurnTaker), SelectionType.TurnTaker, isOptional: true, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectTurnTakerAndDoAction(validHeroes, ReturnCardAndRescueMatching);
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

        private IEnumerator ReturnCardAndRescueMatching(TurnTaker tt)
        {
            var hero = FindHeroTurnTakerController(tt.ToHero());
            var returnedStorage = new List<SelectCardDecision> { };
            IEnumerator coroutine = SelectAndMoveCardOptional(hero, (Card c) => c.IsInPlay && !c.IsCharacter && c.Owner == hero.TurnTaker && !GameController.IsCardIndestructible(c), hero.HeroTurnTaker.Hand, optional: true, storedResults: returnedStorage, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (DidSelectCard(returnedStorage))
            {
                List<string> handKeywords = returnedStorage.FirstOrDefault().SelectedCard.GetKeywords().ToList();
                coroutine = SelectAndMoveCardOptional(hero, (Card c) => c.Location == hero.TurnTaker.Trash && c.GetKeywords().Any((string trashKeyword) => handKeywords.Contains(trashKeyword)), hero.HeroTurnTaker.PlayArea, isPutIntoPlay: true, optional: true, storedResults: returnedStorage, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        private IEnumerator SelectAndMoveCardOptional(HeroTurnTakerController hero, Func<Card, bool> criteria, Location toLocation, bool toBottom = false, bool optional = true, bool isPutIntoPlay = false, bool playIfMovingToPlayArea = true, List<SelectCardDecision> storedResults = null, CardSource cardSource = null)
        {
            BattleZone battleZone = null;
            if (cardSource != null)
            {
                battleZone = cardSource.BattleZone;
            }
            SelectCardDecision selectCardDecision = new SelectCardDecision(GameController, hero, SelectionType.MoveCard, GameController.FindCardsWhere(criteria, realCardsOnly: true, null, battleZone), isOptional: optional, allowAutoDecide: false, null, null, null, null, null, maintainCardOrder: false, actionCanBeCancelled: true, cardSource);
            selectCardDecision.BattleZone = battleZone;
            storedResults?.Add(selectCardDecision);
            IEnumerator coroutine = GameController.SelectCardAndDoAction(selectCardDecision, (SelectCardDecision d) => GameController.MoveCard(hero, d.SelectedCard, toLocation, toBottom, isPutIntoPlay, playIfMovingToPlayArea, null, showMessage: false, null, null, null, evenIfIndestructible: false, flipFaceDown: false, null, isDiscard: false, evenIfPretendGameOver: false, shuffledTrashIntoDeck: false, doesNotEnterPlay: false, cardSource));
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
}