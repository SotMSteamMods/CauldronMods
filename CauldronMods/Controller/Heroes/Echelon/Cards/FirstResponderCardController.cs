using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra;

namespace Cauldron.Echelon
{
    public class FirstResponderCardController : EchelonBaseCardController
    {
        //==============================================================
        // You may discard a card. If you do, put up to 2 Tactics from your trash into play.
        // If you did not discard a card, you may discard your hand and draw 4 cards.
        //==============================================================

        public static string Identifier = "FirstResponder";

        private const int CardsFromTrash = 2;
        private const int CardsToDraw = 4;

        private enum CustomMode
        {
            DiscardCardForTactics,
            DiscardHandForRedraw
        }
        private CustomMode CurrentMode;
        public FirstResponderCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria(IsTactic, "tactic"));
        }

        public override IEnumerator Play()
        {
            List<DiscardCardAction> discardResults = new List<DiscardCardAction>();
            IEnumerator routine;


            // You may discard a card
            var firstDiscardDecision = new YesNoDecision(GameController, DecisionMaker, SelectionType.Custom, cardSource: GetCardSource());
            if(DecisionMaker.HasCardsInHand)
            {
                CurrentMode = CustomMode.DiscardCardForTactics;
                routine = GameController.MakeDecisionAction(firstDiscardDecision);
            }
            else
            {
                routine = GameController.SendMessageAction($"{DecisionMaker.Name} cannot discard a card.", Priority.Medium, GetCardSource());
            }

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (DidPlayerAnswerYes(firstDiscardDecision))
            {
                routine = base.GameController.SelectAndDiscardCard(base.HeroTurnTakerController, false, storedResults: discardResults,
                    selectionType: SelectionType.DiscardCard, cardSource: GetCardSource());

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }
            }

            if (DidDiscardCards(discardResults))
            {
                // If you do, put up to 2 Tactics from your trash into play.
                MoveCardDestination playArea = new MoveCardDestination(base.HeroTurnTaker.PlayArea);

                routine = base.GameController.SelectCardsFromLocationAndMoveThem(base.HeroTurnTakerController,
                    base.TurnTaker.Trash, 0, CardsFromTrash, new LinqCardCriteria(IsTactic, "tactic"),
                    playArea.ToEnumerable(), isPutIntoPlay: true, cardSource: GetCardSource());

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }
            }
            else
            {
                // If you did not discard a card, you may discard your hand and draw 4 cards.
                var storedYesNo = new List<YesNoCardDecision>();

                CurrentMode = CustomMode.DiscardHandForRedraw;
                routine = base.GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.Custom, this.Card, storedResults: storedYesNo, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }

                if (DidPlayerAnswerYes(storedYesNo))
                {
                    routine = GameController.DiscardHand(DecisionMaker, false, discardResults, DecisionMaker.TurnTaker, GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }

                    routine = base.DrawCards(this.HeroTurnTakerController, CardsToDraw);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }
                }
            }

        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            if (CurrentMode == CustomMode.DiscardCardForTactics)
            {
                return new CustomDecisionText("Do you want to discard a card to return tactics from your trash?", $"{decision.DecisionMaker.Name} is deciding whether to discard a card...", $"Should {decision.DecisionMaker.Name} discard a card to return tactics from their trash?", "discard a card to return tactics");
            }
            else if (CurrentMode == CustomMode.DiscardHandForRedraw)
            {
                return new CustomDecisionText("Do you want to discard your hand to draw 4 cards?", $"{decision.DecisionMaker.Name} is deciding whether to discard their hand...", $"Should {decision.DecisionMaker.Name} discard their hand to draw 4 cards?", "discard their hand to draw 4 cards");
            }

            return base.GetCustomDecisionText(decision);

        }
    }
}