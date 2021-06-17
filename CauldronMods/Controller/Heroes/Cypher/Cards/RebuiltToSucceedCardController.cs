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
            ShowSpecialStringNumberOfAugmentsAtLocation(TurnTaker.Trash);
        }

        public override IEnumerator Play()
        {
            // Select two Augments in your trash. Put one into your hand and one into play.
            List<SelectCardsDecision> storedResults = new List<SelectCardsDecision>();
            IEnumerator routine = base.GameController.SelectCardsAndStoreResults(this.DecisionMaker, SelectionType.SearchTrash, 
                c => c.Location == TurnTaker.Trash && IsAugment(c), 
                2, storedResults, false, 2, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            List<Card> cards = GetSelectedCards(storedResults).ToList();
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
                new MoveCardDestination(base.TurnTaker.PlayArea),
                new MoveCardDestination(base.HeroTurnTaker.Hand)
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

            HeroTurnTakerController heroToPlayCard = null;
            if (moveResult.First().Destination.IsInPlay && card.Location.IsNextToCard)
            {
                heroToPlayCard = FindHeroTurnTakerController(card.Location.OwnerTurnTaker.ToHero());
                // The hero you augment this way may play a card now.
            }

            if (cards.Count <= 1)
            {
                if(heroToPlayCard != null)
                {
                    routine = base.SelectAndPlayCardFromHand(heroToPlayCard);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(routine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(routine);
                    }
                }

                yield break;
            }

            if (cards.Count == 2)
            {
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

                if (destination == base.TurnTaker.PlayArea)
                {
                    heroToPlayCard = FindHeroTurnTakerController(card.Location.OwnerTurnTaker.ToHero());
                    // The hero you augment this way may play a card now.
                }
            }

            if (heroToPlayCard != null)
            {
                routine = base.SelectAndPlayCardFromHand(heroToPlayCard);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }
            }
            yield break;
        }
    }
}