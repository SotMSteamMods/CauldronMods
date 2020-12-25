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
    }
}