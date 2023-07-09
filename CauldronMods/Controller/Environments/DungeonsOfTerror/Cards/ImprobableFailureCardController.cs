using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DungeonsOfTerror
{
    public class ImprobableFailureCardController : DungeonsOfTerrorUtilityCardController
    {
        public ImprobableFailureCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //At the start of each player's draw phase, discard and check the top card of the environment deck.",
            //If it is a fate card, they skip the rest of their draw phase.",
            AddTrigger((PhaseChangeAction p) => IsHero(p.ToPhase.TurnTaker) && p.ToPhase.IsDrawCard, DiscardAndCheckResponse, TriggerType.MoveCard, TriggerTiming.After);

            //At the start of the environment turn, destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);
        }

        private IEnumerator DiscardAndCheckResponse(PhaseChangeAction pca)
        {
            List<MoveCardAction> storedDiscard = new List<MoveCardAction>();
            IEnumerator coroutine = DiscardCardsFromTopOfDeck(TurnTakerController, 1, storedResults: storedDiscard, showMessage: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card cardToCheck;
            if (DidMoveCard(storedDiscard))
            {
                cardToCheck = storedDiscard.FirstOrDefault().CardToMove;
            } else 
            {
                cardToCheck = TurnTaker.Deck.TopCard;
            }
            List<int> storedResults = new List<int>();
            coroutine = CheckForNumberOfFates(cardToCheck.ToEnumerable(), storedResults, TurnTaker.Deck);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            IEnumerator message = DoNothing();
            IEnumerator effect = DoNothing();
            if (storedResults.Any() && storedResults.First() == 1)
            {
                //If it is a fate card, they skip the rest of their draw phase.
                message = GameController.SendMessageAction($"{cardToCheck.Title} is a fate card!", Priority.High, GetCardSource(), associatedCards: cardToCheck.ToEnumerable(), showCardSource: true);
                effect = GameController.SkipToTurnPhase(GameController.FindNextTurnPhase(Game.ActiveTurnPhase), cardSource: GetCardSource());
            }
            else if (storedResults.Any() && storedResults.First() == 0)
            {
                message = GameController.SendMessageAction($"{cardToCheck.Title} is not a fate card!", Priority.High, GetCardSource(), associatedCards: cardToCheck.ToEnumerable(), showCardSource: true);
            }
            else
            {
                message = GameController.SendMessageAction("There are no cards in the environment deck!", Priority.High, GetCardSource(), showCardSource: true);
            }
        
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(message);
                yield return base.GameController.StartCoroutine(effect);

            }
            else
            {
                base.GameController.ExhaustCoroutine(message);
                base.GameController.ExhaustCoroutine(effect);
            }
            yield break;
        }

    }
}
