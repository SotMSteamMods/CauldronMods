using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace Cauldron.Northspar
{
    public class NorthsparCardController : CardController
    {
        public static readonly string FrozenKeyword = "frozen";

        public NorthsparCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected bool IsFrozen(Card card)
        {
            return card.DoKeywordsContain(FrozenKeyword);
        }

        protected bool IsTakAhabInPlay()
        {
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && c.Identifier == "TakAhab").Count() > 0;
        }

        protected IEnumerable FindTakAhabInPlay()
        {
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && c.Identifier == "TakAhab");
        }
    }
}