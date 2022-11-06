using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Cauldron.SuperstormAkela
{
    public class RideTheCurrentsCardController : SuperstormAkelaCardController
    {

        public RideTheCurrentsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public readonly string HasTriggeredBeforeKey = "HasTriggeredBefore";

        public override void AddTriggers()
        {
            //At the start of the villain turn, you may move 1 environment card to a new location in the environment play area.
            AddStartOfTurnTrigger((TurnTaker tt) => FindVillainTurnTakerControllers(true).Contains(FindTurnTakerController(tt)) && !HasBeenSetToTrueThisTurn(HasTriggeredBeforeKey), MoveEnvironmentCardResponse, TriggerType.MoveCard);
        }

        private IEnumerator MoveEnvironmentCardResponse(PhaseChangeAction pca)
        {
            //Select a card and store it
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(base.DecisionMaker, SelectionType.MoveCard, new LinqCardCriteria((Card c) => c.IsEnvironment && c.IsInPlayAndNotUnderCard && TurnTaker.PlayArea.HasCard(c), "environment card in environment play area"), storedResults, true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(DidSelectCard(storedResults))
            {
                Card cardToMove = GetSelectedCard(storedResults);

                //Select a card to move next to
                List<SelectCardDecision> storedResults2 = new List<SelectCardDecision>();
                coroutine = GameController.SelectCardAndStoreResults(base.DecisionMaker, SelectionType.MoveCardNextToCard, new LinqCardCriteria((Card c) => c.IsEnvironment && c.IsInPlayAndNotUnderCard && TurnTaker.PlayArea.HasCard(c) && c != cardToMove, "environment card in environment play area"), storedResults2, false, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if(DidSelectCard(storedResults2))
                {
                    Card cardToMoveInRelationTo = GetSelectedCard(storedResults2);

                    //Select whether you want to move to the right or left of that card
                    //Option 1: Move to the left
                    var response1 = MoveToTheLeftOfCard(cardToMove, cardToMoveInRelationTo);
                    var op1 = new Function(this.DecisionMaker, $"Move {cardToMove.Title} to the left of {cardToMoveInRelationTo.Title}", SelectionType.MoveCard, () => response1);

                    //Option 2: Move to the right
                    var response2 = MoveToTheRightOfCard(cardToMove, cardToMoveInRelationTo);
                    var op2 = new Function(this.DecisionMaker, $"Move {cardToMove.Title} to the right of {cardToMoveInRelationTo.Title}", SelectionType.MoveCard, () => response2);

                    //Execute
                    var options = new Function[] { op1, op2 };
                    var selectFunctionDecision = new SelectFunctionDecision(base.GameController, this.DecisionMaker, options, false, cardSource: GetCardSource());
                    coroutine = base.GameController.SelectAndPerformFunction(selectFunctionDecision);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }

                SetCardPropertyToTrueIfRealAction(HasTriggeredBeforeKey);

            }

            yield break;
        }

        public override IEnumerator Play()
        {
            //When this card enters play, select the deck with the least number of non-character cards in play...
            List<TurnTaker> storedResults = new List<TurnTaker>() ;
            IEnumerator coroutine = GameController.DetermineTurnTakersWithMostOrFewest(false, 1, 1, (TurnTaker tt) => !tt.IsIncapacitatedOrOutOfGame, tt => GameController.FindCardsWhere((Card c) => c.IsInPlay && !c.IsCharacter && c.Owner == tt).Count(), SelectionType.PlayTopCard, storedResults, cardSource: GetCardSource(), battleZone: base.BattleZone);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(storedResults != null)
            {
                TurnTaker tt = storedResults.First();
                Location deck = tt.Decks.First();
                if (tt.Decks.Count > 1)
                {
                    List<SelectLocationDecision> storedDeck = new List<SelectLocationDecision>();
                    coroutine = GameController.SelectADeck(DecisionMaker, SelectionType.PlayTopCard, (Location l) => l.OwnerTurnTaker == tt, storedDeck, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    if(DidSelectLocation(storedDeck))
                    {
                        deck = GetSelectedLocation(storedDeck);
                    }
                }

                //... Put the top card of that deck into play.
                coroutine = GameController.PlayTopCardOfLocation(base.DecisionMaker, deck,isPutIntoPlay: true, cardSource: GetCardSource(), showMessage: true);
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