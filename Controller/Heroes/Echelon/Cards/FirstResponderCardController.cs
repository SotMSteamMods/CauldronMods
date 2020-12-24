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

        public FirstResponderCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria(IsTactic, "tactic"));
        }

        public override IEnumerator Play()
        {
            // You may discard a card
            List<DiscardCardAction> discardResults = new List<DiscardCardAction>();
            IEnumerator routine = base.GameController.SelectAndDiscardCard(base.HeroTurnTakerController, true, storedResults: discardResults, 
                selectionType: SelectionType.DiscardCard, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (DidDiscardCards(discardResults))
            {
                // If you do, put up to 2 Tactics from your trash into play.
                MoveCardDestination playArea = new MoveCardDestination(base.HeroTurnTaker.PlayArea);

                routine = base.GameController.SelectCardsFromLocationAndMoveThem(base.HeroTurnTakerController,
                    base.TurnTaker.Trash, 0, CardsFromTrash, new LinqCardCriteria(IsTactic, "tactic"),
                    playArea.ToEnumerable(), cardSource: GetCardSource());

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

                routine = base.GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.DiscardHand, this.Card, storedResults: storedYesNo, cardSource: GetCardSource());
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
    }
}