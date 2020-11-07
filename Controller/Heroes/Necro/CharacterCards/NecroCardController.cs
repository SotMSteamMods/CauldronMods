using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;
using System;

namespace Cauldron.Necro
{
    public abstract class NecroCardController : CardController
    {
        public static readonly string RitualKeyword = "ritual";
        public static readonly string UndeadKeyword = "undead";

        protected NecroCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected bool IsRitual(Card card)
        {
            return card.DoKeywordsContain(RitualKeyword);
        }

        protected int GetNumberOfRitualsInPlay()
        {
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && this.IsRitual(c)).Count();
        }

        protected bool IsUndead(Card card)
        {
            return card.DoKeywordsContain(UndeadKeyword);
        }
    }
}
