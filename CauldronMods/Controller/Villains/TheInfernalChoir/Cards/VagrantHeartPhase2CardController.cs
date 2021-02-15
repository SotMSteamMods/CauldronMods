using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class VagrantHeartPhase2CardController : TheInfernalChoirUtilityCardController
    {
        /*
         * "This card is indestructible.",
	     * "If a player would draw, play, or discard cards from an empty deck, nothing happens instead."
         */

        public VagrantHeartPhase2CardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => "This card is indestructible.");

            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            if (card == Card)
                return true;

            return base.AskIfCardIsIndestructible(card);
        }

       
        public override void AddTriggers()
        {
            base.AddTriggers();

            //Cancel card draws when deck is empty
            AddTrigger<DrawCardAction>(ga => ga.HeroTurnTaker.IsHero && !ga.HeroTurnTaker.Deck.HasCards, ga => HeartCancelResponse(ga, ga.HeroTurnTaker.Name, "drawing"), TriggerType.CancelAction, TriggerTiming.Before);

            //Cancel Shuffle Trash Into Deck when NecessaryToPlayCard to play card is set, this covers all calls to PlayTopCard
            AddTrigger<ShuffleTrashIntoDeckAction>(ga => ga.TurnTakerController.IsHero && !ga.TurnTakerController.TurnTaker.Deck.HasCards && ga.NecessaryToPlayCard, ga => HeartCancelResponse(ga, ga.TurnTakerController.Name, "playing"), TriggerType.CancelAction, TriggerTiming.Before);

            //Discards
            AddTrigger<ShuffleTrashIntoDeckAction>(ga => ga.TurnTakerController.IsHero && !ga.TurnTakerController.TurnTaker.Deck.HasCards && !ga.NecessaryToPlayCard, ga => StashCardsForPotentialDiscardAction(ga), TriggerType.Hidden, TriggerTiming.Before);
            AddTrigger<MoveCardAction>(ga => ga.Origin.IsHero && ga.Origin.IsDeck && ga.Destination.IsHero && ga.Destination.IsTrash && ga.IsDiscard && ga.ShuffledTrashIntoDeck, ga => HeartDiscardCancelReponse(ga), TriggerType.CancelAction, TriggerTiming.Before);

        }

        private Dictionary<TurnTaker, List<Card>> _stashedTrashOrder = new Dictionary<TurnTaker, List<Card>>();

        private IEnumerator StashCardsForPotentialDiscardAction(ShuffleTrashIntoDeckAction action)
        {
            //This can false trigger from cards that trigger a shuffle, but that's ok.
            //In the case we care about (DiscardTopCard) the shuffle will always be folloew by the MoveAction
            //So we don't need to worry about persistance.
            var tt = action.TurnTakerController.TurnTaker;
            var cards = tt.GetCardsAtLocation(tt.Trash).ToList();
            _stashedTrashOrder[tt] = cards;
            return DoNothing();
        }

        private IEnumerator HeartDiscardCancelReponse(MoveCardAction ga)
        {
            var coroutine = GameController.CancelAction(ga, showOutput: false, isPreventEffect: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //Un-shuffle the deck by moving the deck back to the trash.
            List<Card> cards;
            if (!_stashedTrashOrder.TryGetValue(ga.Origin.OwnerTurnTaker, out cards))
            {
                //fallback if my assumptions are wrong/bad.
                cards = ga.Origin.Cards.Reverse().ToList();
                //worst case we end up with a shuffled trash
            }

            coroutine = GameController.BulkMoveCards(TurnTakerController, cards, ga.Destination, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = HeartCancelMessage(ga.ResponsibleTurnTaker.Name, "discarding");
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator HeartCancelResponse(GameAction ga, string turnTaker, string reportedAction)
        {
            var coroutine = GameController.CancelAction(ga, showOutput: false, isPreventEffect: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = HeartCancelMessage(turnTaker, reportedAction);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator HeartCancelMessage(string turnTaker, string reportedAction)
        {
            //Message: Vagrant Heart: Soul Revealed prevented Legacy from drawing cards. [  ]
            var msg = $"{Card.Title} prevented {turnTaker} from {reportedAction} cards from an empty deck.";
            return GameController.SendMessageAction(msg, Priority.Medium, GetCardSource(), showCardSource: true);
        }

    }
}
