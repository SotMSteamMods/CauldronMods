using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Cypher
{
    public class RebuiltToSucceedCardController : CypherBaseCardController
    {
        //==============================================================
        // Select two Augments in your trash. Put one into your hand and one into play.
        // The hero you augment this way may play a card now.
        //==============================================================

        public static string Identifier = "RebuiltToSucceed";

        public RebuiltToSucceedCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            List<SelectCardsDecision> storedResults = new List<SelectCardsDecision>();
            IEnumerator routine = base.GameController.SelectCardsAndStoreResults(this.DecisionMaker, SelectionType.SearchTrash, 
                c => c.Location.Name == LocationName.Trash && IsAugment(c), 
                2, storedResults, false, 0, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            var cards = GetSelectedCards(storedResults).ToList();
            if (!cards.Any())
            {
                routine = base.GameController.SendMessageAction($"There were no augments {this.Card.Owner.CharacterCard.Title}'s trash to select", Priority.Medium, base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }

                yield break;
            }

            Card card = cards[0];

            MoveCardDestination[] destinations = 
            {
                new MoveCardDestination(base.HeroTurnTaker.Hand),
                new MoveCardDestination(base.TurnTaker.PlayArea)
            };

            List<MoveCardAction> moveResult = new List<MoveCardAction>();
            routine = base.GameController.SelectLocationAndMoveCard(this.DecisionMaker, card, destinations, 
                storedResults: moveResult, isPutIntoPlay: true, cardSource: base.GetCardSource());
                
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if (cards.Count <= 1)
            {
                if (moveResult.First().Destination.IsInPlay)
                {
                    // The hero you augment this way may play a card now.


                    base.GameController.PlayCard()

                }

                yield break;
            }

            card = cards[1];

            Location destination = moveResult.First().Destination.IsInPlay ? base.HeroTurnTaker.Hand : base.TurnTaker.PlayArea;

            routine = base.GameController.MoveCard(this.DecisionMaker, card, destination, isPutIntoPlay: true, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            if(moveResult.First().Destination.IsInPlay)


            /*
            var scsd = new SelectCardsDecision(GameController, this.HeroTurnTakerController, c => c.Location == TurnTaker.Trash, SelectionType.MoveCard,
                numberOfCards: 2,
                isOptional: false,
                requiredDecisions: 0,
                allowAutoDecide: true,
                cardSource: GetCardSource());


            IEnumerator coroutine = GameController.SelectCardsAndDoAction(scsd, scd => base.GameController.Moveca)
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            */

            /*
            List<MoveCardDestination> list = new List<MoveCardDestination>();
            list.Add(new MoveCardDestination(base.HeroTurnTaker.Hand, false, false, false));
            list.Add(new MoveCardDestination(base.TurnTaker.PlayArea, false, false, false));

            IEnumerator coroutine = base.RevealCardsFromDeckToMoveToOrderedDestinations(this.DecisionMaker, 
                base.TurnTaker.Trash, list, false, true, null);


            IEnumerable<MoveCardDestination> moveLocations = new[]
            {
                new MoveCardDestination(base.TurnTaker.PlayArea),
                new MoveCardDestination(base.HeroTurnTaker.Hand)
            };

            base.GameController.SelectCardsFromLocationAndMoveThem()


            IEnumerator routine = base.GameController.SelectCardsFromLocationAndMoveThem(this.HeroTurnTakerController,
                this.HeroTurnTaker.Trash, 0, 2, new LinqCardCriteria(IsAugment), moveLocations,
                cardSource: GetCardSource());


            //IEnumerator routine = base.GameController.SelectLocationAndMoveCard(base.HeroTurnTakerController, base.GetTitanform(), locations, true, cardSource: base.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(routine);
            }
            else
            {
                GameController.ExhaustCoroutine(routine);
            }
            */
        }
    }
}