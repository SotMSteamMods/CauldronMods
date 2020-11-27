using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Northspar
{
    public class WhatsLeftOfThemCardController : NorthsparCardController
    {

        public WhatsLeftOfThemCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals each hero target 1 psychic damage and is destroyed.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageAndDestroyResponse, new TriggerType[]
            {
                TriggerType.DealDamage,
                TriggerType.DestroySelf
            });
        }

        private IEnumerator DealDamageAndDestroyResponse(PhaseChangeAction pca)
        {
            //this card deals each hero target 1 psychic damage...
            IEnumerator coroutine = base.DealDamage(base.Card, (Card c) => c.IsHero && c.IsTarget, 1, DamageType.Psychic);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // ...and is destroyed.
            coroutine = base.DestroyThisCardResponse(pca);
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

        public override IEnumerator Play()
        {
            //When this card enters play, search the environment deck and trash for a First, Second, or Third Waypoint card and put it into play, then shuffle the deck.
            IEnumerator coroutine = SearchForWaypoints();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }

        private IEnumerator SearchForWaypoints()
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

            yield break;
        }
    }
}