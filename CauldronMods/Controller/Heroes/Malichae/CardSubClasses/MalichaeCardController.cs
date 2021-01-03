using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;
using System;

namespace Cauldron.Malichae
{
    public abstract class MalichaeCardController : CardController
    {
        public static readonly string DjinnKeyword = "djinn";

        protected MalichaeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }


        protected bool IsDjinn(Card card)
        {
            return card.DoKeywordsContain(DjinnKeyword);
        }
    }
}
