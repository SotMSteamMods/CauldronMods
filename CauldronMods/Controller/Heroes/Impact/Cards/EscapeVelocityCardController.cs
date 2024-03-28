using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.Impact
{
    public class EscapeVelocityCardController : CardController
    {
        public EscapeVelocityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"You may destroy 1 ongoing card.",
            IEnumerator coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && IsOngoing(c), "ongoing"), true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //"Select up to 3 non-character targets in play with 2 or fewer HP. 
            var storedTargets = new List<SelectCardsDecision> { };
            coroutine = GameController.SelectCardsAndStoreResults(DecisionMaker, SelectionType.MoveCardOnBottomOfDeck, (Card c) => c.IsInPlayAndHasGameText && c.IsTarget && !c.IsCharacter && c.HitPoints <= 2 && GameController.IsCardVisibleToCardSource(c, GetCardSource()),
                                                3, storedTargets, false, 0, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var targetsToMove = GetSelectedCards(storedTargets);
            if (targetsToMove.Any())
            {
                var numToMove = targetsToMove.Count();
                //Place those targets on the bottom of their associated decks in any order."
                var moveDecision = new SelectCardsDecision(GameController, DecisionMaker, (Card c) => targetsToMove.Contains(c), SelectionType.MoveCardOnBottomOfDeck, numToMove, requiredDecisions: numToMove, eliminateOptions: true, allowAutoDecide: true, cardSource: GetCardSource());
                coroutine = GameController.SelectCardsAndDoAction(moveDecision, MoveToBottomOfDeck, cardSource: GetCardSource());
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

        private IEnumerator MoveToBottomOfDeck(SelectCardDecision scd)
        {
            if (scd.SelectedCard != null)
            {
                var card = scd.SelectedCard;
                IEnumerator coroutine = GameController.MoveCard(DecisionMaker, card, GetNativeDeck(card), toBottom: true, cardSource: GetCardSource());
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