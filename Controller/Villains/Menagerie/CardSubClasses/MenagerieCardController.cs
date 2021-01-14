using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;

namespace Cauldron.Menagerie
{
    public class MenagerieCardController : CardController
    {
        public MenagerieCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public bool IsCaptured(TurnTaker tt)
        {
            Card prize = FindCard("PrizedCatch");
            return prize.Location.IsNextToCard && tt.GetAllCards().Contains(base.GetCardThisCardIsNextTo());
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