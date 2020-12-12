using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Menagerie
{
    public class MenagerieCardController : CardController
    {
        public MenagerieCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

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