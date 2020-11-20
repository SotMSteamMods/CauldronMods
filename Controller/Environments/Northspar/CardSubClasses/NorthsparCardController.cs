using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace Cauldron.Northspar
{
    public class NorthsparCardController : CardController
    {
        public static readonly string FrozenKeyword = "frozen";
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
    }
}