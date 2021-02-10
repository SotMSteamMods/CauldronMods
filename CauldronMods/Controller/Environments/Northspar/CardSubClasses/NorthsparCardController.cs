using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Northspar
{
    public abstract class NorthsparCardController : CardController
    {
        public static readonly string FrozenKeyword = "frozen";
        public static readonly string FirstWaypointKeyword = "first waypoint";
        public static readonly string SecondWaypointKeyword = "second waypoint";
        public static readonly string ThirdWaypointKeyword = "third waypoint";
        public static readonly string TakAhabIdentifier = "TakAhab";
        public static readonly string AethiumTriggerKey = "aethiumTriggers";

        protected NorthsparCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected bool IsFrozen(Card card)
        {
            return card.DoKeywordsContain(FrozenKeyword);
        }

        private IEnumerable<Card> FindTakAhab()
        {
            return base.FindCardsWhere(c => c.Identifier == TakAhabIdentifier);
        }

        protected bool IsTakAhabInPlay()
        {
            return FindTakAhab().Where(c => c.IsInPlayAndHasGameText).Any();
        }

        protected bool IsThirdWaypoint(Card card)
        {
            return card.DoKeywordsContain(ThirdWaypointKeyword);
        }

        protected Card FindTakAhabInPlay()
        {
            return FindTakAhab().Where(c => c.IsInPlayAndHasGameText).FirstOrDefault();
        }

        protected Card FindTakAhabAnywhere()
        {
            return FindTakAhab().First();
        }

        protected int GetNumberOfWaypointsInPlay()
        {
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && this.IsWaypoint(c)).Count();
        }

        protected bool IsWaypoint(Card card)
        {
            return card.DoKeywordsContain(ThirdWaypointKeyword) || card.DoKeywordsContain(SecondWaypointKeyword) || card.DoKeywordsContain(FirstWaypointKeyword);
        }

        protected IEnumerator SearchForWaypoints()
        {
            if (base.TurnTaker.Deck.Cards.Any((Card c) => IsWaypoint(c)) || base.TurnTaker.Trash.Cards.Any((Card c) => IsWaypoint(c)))
            {
                // Search the environment deck and trash for a First, Second, or Third Waypoint card and put it into play...
                IEnumerable<Card> choices = TurnTaker.Deck.Cards.Concat(TurnTaker.Trash.Cards);
                LinqCardCriteria criteria = new LinqCardCriteria((Card c) => IsWaypoint(c), "waypoint");
                List<PlayCardAction> storedResults = new List<PlayCardAction>();
                IEnumerator coroutine = GameController.SelectAndPlayCard(base.DecisionMaker, choices.Where((Card c) => IsWaypoint(c)), isPutIntoPlay: true, storedResults: storedResults, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                if (DidPlayCards(storedResults))
                {
                    Location origin = storedResults.First().Origin;
                    if (origin == TurnTaker.Deck)
                    {
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
                    }
                }

            }
            else
            {
                IEnumerator coroutine2 = base.GameController.SendMessageAction("There are no Waypoints in the deck or trash!", Priority.Medium, GetCardSource(), null, showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }

            yield break;
        }
    }
}