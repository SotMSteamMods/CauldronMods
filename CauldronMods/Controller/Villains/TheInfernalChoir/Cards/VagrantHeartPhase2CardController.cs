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
        public VagrantHeartPhase2CardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => "This card is indestructible.");

            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
        }

        public override bool CanBeDestroyed => false;

        public override bool AskIfCardIsIndestructible(Card card)
        {
            if (card == Card)
                return true;

            return base.AskIfCardIsIndestructible(card);
        }

        private IEnumerator RemoveDecisionsFromMakeDecisionsResponse(MakeDecisionsAction md)
        {
            //remove this card as an option to make decisions
            md.RemoveDecisions((IDecision d) => d.SelectedCard == base.Card);
            return base.DoNothing();
        }

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            if (card == base.Card && !cardSource.Card.IsVillain)
            {
                return false;
            }
            return base.AskIfCardIsVisibleToCardSource(card, cardSource);
        }

        public override bool AskIfActionCanBePerformed(GameAction action)
        {
            bool? effected = action.DoesFirstCardAffectSecondCard((Card c) => !c.IsVillain, (Card c) => c == base.Card);
            if (effected == true)
            {
                return false;
            }

            return base.AskIfActionCanBePerformed(action);
        }

        public override void AddTriggers()
        {
            base.AddTriggers();

            //Cancel card draws when deck is empty
            AddTrigger<DrawCardAction>(ga => ga.HeroTurnTaker.IsHero && !ga.HeroTurnTaker.Deck.HasCards, ga => HeartCancelResponse(ga, ga.HeroTurnTaker.Name, "drawing"), TriggerType.CancelAction, TriggerTiming.Before);

            //Cancel Shuffle Trash Into Deck when NecessaryToPlayCard to play card is set, this covers all calls to PlayTopCard
            AddTrigger<ShuffleTrashIntoDeckAction>(ga => ga.TurnTakerController.IsHero && !ga.TurnTakerController.TurnTaker.Deck.HasCards && ga.NecessaryToPlayCard, ga => HeartCancelResponse(ga, ga.TurnTakerController.Name, "playing"), TriggerType.CancelAction, TriggerTiming.Before);

            //Discards
            AddTrigger<MoveCardAction>(ga => ga.Origin.IsHero && ga.Origin.IsDeck && ga.Destination.IsHero && ga.Destination.IsTrash && ga.IsDiscard && ga.ShuffledTrashIntoDeck, ga => HeartDiscardCancelReponse(ga), TriggerType.CancelAction, TriggerTiming.Before);

            //visibility
            base.AddTrigger<MakeDecisionsAction>((MakeDecisionsAction md) => md.CardSource != null && !md.CardSource.Card.IsVillain, this.RemoveDecisionsFromMakeDecisionsResponse, TriggerType.RemoveDecision, TriggerTiming.Before);
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

            coroutine = GameController.BulkMoveCards(TurnTakerController, ga.Origin.Cards, ga.Destination, cardSource: GetCardSource());
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
