using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class WandOfBanishmentCardController : CardController
    {
        public WandOfBanishmentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"When a non-character card from another deck would be destroyed, you may put it on the top or bottom of its deck instead. If you do, destroy this card.",
            AddTrigger((DestroyCardAction dc) => !this.IsBeingDestroyed && !dc.CardToDestroy.Card.IsCharacter && dc.CardToDestroy.TurnTaker != this.TurnTaker && !GameController.IsCardIndestructible(dc.CardToDestroy.Card),
                                                MaybeMoveInsteadResponse,
                                                new TriggerType[] { TriggerType.MoveCard, TriggerType.DestroySelf },
                                                TriggerTiming.Before);

        }

        private IEnumerator MaybeMoveInsteadResponse(DestroyCardAction dc)
        {
            Card card = dc.CardToDestroy.Card;
            Location destination = card.NativeDeck is null || card.NativeDeck.OwnerTurnTaker != card.Owner ? card.Owner.Deck : card.NativeDeck;
            var functions = new List<Function>
            {
                new Function(DecisionMaker, $"Put {card.Title} on top of its deck", SelectionType.MoveCardOnDeck, () => CancelDestructionAndMoveCard(dc, destination)),
                new Function(DecisionMaker, $"Put {card.Title} on the bottom of its deck", SelectionType.MoveCardOnBottomOfDeck, () => CancelDestructionAndMoveCard(dc, destination, true))
            };
            var selectFunction = new SelectFunctionDecision(GameController, DecisionMaker, functions, optional: true, associatedCards: new Card[] { card }, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectAndPerformFunction(selectFunction);
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

        private IEnumerator CancelDestructionAndMoveCard(DestroyCardAction dc, Location deck, bool toBottom = false)
        {
            var moveCardStorage = new List<MoveCardAction> { };
            IEnumerator coroutine = GameController.CancelAction(dc, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.MoveCard(DecisionMaker, dc.CardToDestroy.Card, deck, toBottom, storedResults: moveCardStorage, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            if(DidMoveCard(moveCardStorage))
            {
                coroutine = DestroyThisCardResponse(dc);
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
    }
}