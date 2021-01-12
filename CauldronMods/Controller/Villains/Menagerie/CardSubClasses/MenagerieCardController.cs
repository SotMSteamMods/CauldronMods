using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;

namespace Cauldron.Menagerie
{
    public abstract class MenagerieCardController : CardController
    {
        protected MenagerieCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public bool IsCaptured(TurnTaker tt)
        {
            Location prize = FindCard("PrizedCatch")?.Location;
            return prize != null && prize.IsNextToCard && prize.OwnerTurnTaker == tt;
        }

        public bool IsEnclosure(Card c)
        {
            return c.DoKeywordsContain("enclosure");
        }

        public bool IsInsect(Card c)
        {
            return c.DoKeywordsContain("insect");
        }

        public bool IsMercenary(Card c)
        {
            return c.DoKeywordsContain("mercenary");
        }

        public bool IsSpecimen(Card c)
        {
            return c.DoKeywordsContain("specimen");
        }
    }
}