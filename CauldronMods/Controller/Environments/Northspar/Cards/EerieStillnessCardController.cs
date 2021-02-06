using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Northspar
{
    public class EerieStillnessCardController : NorthsparCardController
    {

        public EerieStillnessCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria(c => IsWaypoint(c), "waypoint"));
            SpecialStringMaker.ShowListOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria(c => IsWaypoint(c), "waypoint"));
        }


        public override void AddTriggers()
        {
            //All targets are immune to cold damage.
            base.AddImmuneToDamageTrigger((DealDamageAction dd) => dd.DamageType == DamageType.Cold);

            //At the start of the environment turn, search the environment deck and trash for a First, Second, or Third Waypoint card and put it into play, then shuffle the deck and destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.SearchForWaypoints, TriggerType.PutIntoPlay);
        }

        private IEnumerator SearchForWaypoints(PhaseChangeAction pca)
        {
            if (base.TurnTaker.Deck.Cards.Any((Card c) => base.IsWaypoint(c)) || base.TurnTaker.Trash.Cards.Any((Card c) => base.IsWaypoint(c)))
            {

                // Search the environment deck and trash for a First, Second, or Third Waypoint card and put it into play...
                List<MoveCardDestination> moveDestinations = new List<MoveCardDestination>();
                moveDestinations.Add(new MoveCardDestination(base.TurnTaker.PlayArea));

                List<SelectLocationDecision> storedLocation = new List<SelectLocationDecision>();
                IEnumerator coroutine = GameController.SelectLocation(this.DecisionMaker, new LocationChoice[2]
                {
                    new LocationChoice(base.TurnTaker.Deck),
                    new LocationChoice(base.TurnTaker.Trash)
                }, SelectionType.SearchLocation, storedLocation, optional: false, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                if (storedLocation.FirstOrDefault() != null)
                {
                    Location origin = storedLocation.FirstOrDefault().SelectedLocation.Location;

                    LinqCardCriteria criteria = new LinqCardCriteria((Card c) => base.IsWaypoint(c), "waypoint");
                    IEnumerator coroutine2 = GameController.SelectCardsFromLocationAndMoveThem(base.DecisionMaker, origin, 1, 1, criteria, moveDestinations, isPutIntoPlay: true, cardSource: base.GetCardSource());
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine2);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine2);
                    }
                }

            }
            else
            {
                IEnumerator coroutine2 = base.GameController.SendMessageAction(base.Card.Title + " has no deck or trash to search.", Priority.Medium, GetCardSource(), null, showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }

            // then shuffle the deck...
            IEnumerator coroutine3 = base.ShuffleDeck(base.DecisionMaker, base.TurnTaker.Deck);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine3);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine3);
            }
            // ...and destroy this card.
            coroutine3 = base.DestroyThisCardResponse(pca);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine3);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine3);
            }
        }
    }
}