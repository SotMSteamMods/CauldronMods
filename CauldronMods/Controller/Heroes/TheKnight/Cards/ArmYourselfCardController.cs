using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class ArmYourselfCardController : TheKnightCardController
    {
        public ArmYourselfCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria(c => IsEquipment(c), "equipment"));
        }

        public override IEnumerator Play()
        {
            //"Select up to 2 Equipment cards from your trash. Put one into play and one into your hand.",
            bool criteria(Card c) => c.Owner == this.TurnTaker && IsEquipment(c) && c.Location.OwnerTurnTaker == this.TurnTaker && c.Location.Name == LocationName.Trash;
            List<SelectCardsDecision> storedResults = new List<SelectCardsDecision>();
            var coroutine = base.GameController.SelectCardsAndStoreResults(this.DecisionMaker, SelectionType.SearchTrash, criteria, 2, storedResults, false, 0, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var cards = GetSelectedCards(storedResults).ToList();
            if (cards.Any())
            {
                var card = cards[0];

                var destinations = new[] {
                    new MoveCardDestination(base.TurnTaker.PlayArea),
                    new MoveCardDestination(base.HeroTurnTaker.Hand)
                };

                List<MoveCardAction> moveResult = new List<MoveCardAction>();
                coroutine = base.GameController.SelectLocationAndMoveCard(this.DecisionMaker, card, destinations, storedResults: moveResult, isPutIntoPlay: true, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (cards.Count > 1)
                {
                    card = cards[1];
                    Location destination;
                    if (moveResult.First().Destination.IsInPlay)
                    {
                        destination = base.HeroTurnTaker.Hand;
                    }
                    else
                    {
                        destination = base.TurnTaker.PlayArea;
                    }

                    coroutine = base.GameController.MoveCard(this.DecisionMaker, card, destination, isPutIntoPlay: true, cardSource: base.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
            yield break;
        }
    }
}
