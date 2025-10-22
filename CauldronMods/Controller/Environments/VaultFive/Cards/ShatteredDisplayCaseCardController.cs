using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.VaultFive
{
    public class ShatteredDisplayCaseCardController : VaultFiveUtilityCardController
    {
        public ShatteredDisplayCaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Whenever a target is destroyed, discard the top card of the environment deck. If an Artifact card is discarded this way, put it into play and destroy this card.
            AddTrigger<DestroyCardAction>((DestroyCardAction dca) => dca.WasCardDestroyed && dca.CardToDestroy != null && dca.CardToDestroy.Card.IsTarget, DiscardTopCardResponse, new TriggerType[]
            {
                TriggerType.DiscardCard,
                TriggerType.PutIntoPlay,
                TriggerType.DestroySelf
            }, TriggerTiming.After);

            //At the end of the environment turn, place all Artifact cards in all trash piles on top of their respective decks.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, ReturnArtifactsToDeckResponse, TriggerType.MoveCard);
        }

        private IEnumerator ReturnArtifactsToDeckResponse(PhaseChangeAction arg)
        {
            IEnumerator coroutine = DoActionToEachTurnTakerInTurnOrder((TurnTakerController ttc) => IsHero(ttc.TurnTaker), (TurnTakerController ttc) => RevealArtifactsInTrash_MoveInAnyOrder(DecisionMaker,ttc, ttc.TurnTaker), TurnTaker);
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

        private IEnumerator DiscardTopCardResponse(DestroyCardAction dca)
        {
            //discard the top card of the environment deck.
            List<MoveCardAction> storedResults = new List<MoveCardAction>();
            IEnumerator coroutine = base.GameController.DiscardTopCard(base.TurnTaker.Deck, storedResults, (Card c) => true, base.TurnTaker, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            MoveCardAction moveCardAction = storedResults.FirstOrDefault();
            //If an Artifact card is discarded this way,
            if (moveCardAction != null && moveCardAction.CardToMove != null && IsArtifact(moveCardAction.CardToMove))
            {
                //put it into play
                coroutine = base.GameController.PlayCard(base.TurnTakerController, moveCardAction.CardToMove, isPutIntoPlay: true, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //and destroy this card.
                coroutine = DestroyThisCardResponse(dca);
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

        protected IEnumerator RevealArtifactsInTrash_MoveInAnyOrder(HeroTurnTakerController decisionMaker, TurnTakerController revealingTurnTaker, TurnTaker turnTaker,  List<MoveCardDestination> moveDestination = null,  Location cleanUpDestination = null)
        {
            IEnumerable<Card> cardsToMove = FindCardsWhere(c => turnTaker.Trash.HasCard(c) && IsArtifact(c));
            IEnumerator coroutine = GameController.MoveCards(decisionMaker, cardsToMove, turnTaker.Deck, showIndividualMessages: true, cardSource: GetCardSource());
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
