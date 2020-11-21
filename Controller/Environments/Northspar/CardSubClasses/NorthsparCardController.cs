using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Northspar
{
    public class NorthsparCardController : CardController
    {
        public static readonly string FrozenKeyword = "frozen";
        public static readonly string FirstWaypointKeyword = "first waypoint";
        public static readonly string SecondWaypointKeyword = "second waypoint";
        public static readonly string ThirdWaypointKeyword = "third waypoint";
        public static readonly string TakAhabIdentifier = "TakAhab";

        public NorthsparCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected bool IsFrozen(Card card)
        {
            return card.DoKeywordsContain(FrozenKeyword);
        }

        protected bool IsTakAhabInPlay()
        {
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && c.Identifier == TakAhabIdentifier).Count() > 0;
        }

        protected bool IsThirdWaypoint(Card card)
        {
            return card.DoKeywordsContain(ThirdWaypointKeyword);
        }

        protected Card FindTakAhabInPlay()
        {
            if(!IsTakAhabInPlay())
            {
                return null;
            }
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && c.Identifier == "TakAhab").First();
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