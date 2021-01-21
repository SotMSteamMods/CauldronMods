using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class HiddenDetourCardController : GyrosaurUtilityCardController
    {
        private bool IsReplacingPlay = false;
        public HiddenDetourCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //Cards beneath this one are not considered in play.
            Card.UnderLocation.OverrideIsInPlay = false;
            SpecialStringMaker.ShowListOfCardsAtLocation(Card.UnderLocation, new LinqCardCriteria(c => true));
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, {Gyrosaur} regains 2HP. 
            IEnumerator coroutine = GameController.GainHP(CharacterCard, 2, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then, reveal the top card of the environment deck and place it beneath this card.",
            var revealStorage = new List<Card>();
            coroutine = GameController.RevealCards(DecisionMaker, FindEnvironment().TurnTaker.Deck, 1, revealStorage, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            var card = revealStorage.FirstOrDefault();
            if(card != null)
            {
                coroutine = GameController.MoveCard(DecisionMaker, card, this.Card.UnderLocation, cardSource: GetCardSource());
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

        public override void AddTriggers()
        {
            //"When an environment card would enter play, you may first switch it with the card beneath this one."
            AddTrigger((PlayCardAction pc) => Card.UnderLocation.HasCards && !IsReplacingPlay && pc.CardToPlay.IsEnvironment, AskToSwapCard, new TriggerType[] { TriggerType.CancelAction, TriggerType.PlayCard }, TriggerTiming.Before, isActionOptional: true);
            AddTrigger((MoveCardAction mc) => Card.UnderLocation.HasCards && !IsReplacingPlay && mc.CardToMove.IsEnvironment && !mc.Origin.IsInPlay && mc.Destination.IsInPlay && !mc.DoesNotEnterPlay, AskToSwapCard, new TriggerType[] { TriggerType.CancelAction, TriggerType.PutIntoPlay }, TriggerTiming.Before, isActionOptional: true);
        }

        private IEnumerator AskToSwapCard(GameAction ga)
        {
            IsReplacingPlay = true;

            Card cardEnteringPlay = null;
            if(ga is PlayCardAction pc)
            {
                cardEnteringPlay = pc.CardToPlay;
            }
            if(ga is MoveCardAction mc)
            {
                cardEnteringPlay = mc.CardToMove;
            }
            var storedYesNo = new List<YesNoCardDecision>();
            IEnumerator coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.MoveCardToUnderCard, cardEnteringPlay, storedResults: storedYesNo, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(DidPlayerAnswerYes(storedYesNo))
            {
                if(ga is PlayCardAction pc2)
                {
                    coroutine = ReplaceWithPlayCardUnder(pc2);
                }
                if(ga is MoveCardAction mc2)
                {
                    coroutine = ReplaceWithPutIntoPlayUnder(mc2);
                }

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            IsReplacingPlay = false;
            yield break;
        }
        private IEnumerator ReplaceWithPlayCardUnder(PlayCardAction pc)
        {

            var cardUnder = this.Card.UnderLocation.Cards.FirstOrDefault();
            //done to give the replacement the correct origin
            IEnumerator coroutine = GameController.MoveCard(DecisionMaker, cardUnder, pc.CardToPlay.Location, cardSource: GetCardSource(), doesNotEnterPlay: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            PlayCardAction swappedPlay = null;
            if (pc.CardSource != null)
            {
                swappedPlay = new PlayCardAction(pc.CardSource, pc.TurnTakerController, cardUnder, pc.IsPutIntoPlay, pc.ResponsibleTurnTaker, pc.OverridePlayLocation, pc.ReassignPlayIndex, pc.AssociateCardSource, pc.ActionSource, pc.FromBottom, pc.CanBeCancelled);
            }
            else
            {
                swappedPlay = new PlayCardAction(pc.GameController, pc.TurnTakerController, cardUnder, pc.IsPutIntoPlay, pc.ResponsibleTurnTaker, pc.OverridePlayLocation, pc.ReassignPlayIndex, pc.ActionSource, pc.FromBottom, pc.CanBeCancelled);
            }
            if(pc.IsPutIntoPlay)
            {
                pc.AllowPutIntoPlayCancel = true;
            }
            coroutine = CancelAction(pc, showOutput: false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.MoveCard(DecisionMaker, pc.CardToPlay, this.Card.UnderLocation);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SendMessageAction($"{Card.Title} replaces {pc.CardToPlay.Title} with {cardUnder.Title}.", Priority.Medium, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = DoAction(swappedPlay);
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
        private IEnumerator ReplaceWithPutIntoPlayUnder(MoveCardAction mc)
        {
            var cardUnder = this.Card.UnderLocation.Cards.FirstOrDefault();
            //done to give the replacement the correct origin
            IEnumerator coroutine = GameController.MoveCard(DecisionMaker, cardUnder, mc.CardToMove.Location, cardSource: GetCardSource(), doesNotEnterPlay: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            MoveCardAction swappedMove = null;
            if (mc.CardSource != null)
            {
                swappedMove = new MoveCardAction(mc.CardSource, cardUnder, mc.Destination, mc.ToBottom, mc.Offset, mc.DecisionSources, mc.ResponsibleTurnTaker, false, mc.ActionSource,
                                         mc.IsDiscard, mc.EvenIfPretendGameOver, mc.ShuffledTrashIntoDeck, mc.DoesNotEnterPlay);
            }
            else
            {
                swappedMove = new MoveCardAction(GameController, cardUnder, mc.Destination, mc.ToBottom, mc.Offset, mc.DecisionSources, mc.ResponsibleTurnTaker, false, mc.ActionSource,
                                         mc.IsDiscard, mc.EvenIfPretendGameOver, mc.ShuffledTrashIntoDeck, mc.DoesNotEnterPlay);
            }

            coroutine = CancelAction(mc, showOutput: false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.MoveCard(DecisionMaker, mc.CardToMove, this.Card.UnderLocation);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SendMessageAction($"{Card.Title} replaces {mc.CardToMove.Title} with {cardUnder.Title}.", Priority.Medium, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = DoAction(swappedMove);
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
    }
}
