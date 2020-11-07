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

        protected bool IsHeroConsidering1929(Card card)
        {
            return card.IsHero;
        }

        protected bool IsVillianConsidering1929(Card card)
        {
            return card.IsVillain;
        }

        protected string HeroStringConsidering1929
        {
            get
            {
                return "hero";
            }
        }

        protected string VillianStringConsidering1929
        {
            get
            {
                return "villian";
            }
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
