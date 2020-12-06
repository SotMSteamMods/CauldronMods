using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheCybersphere
{
    public class TheCybersphereCardController : CardController
    {

        public TheCybersphereCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public static readonly string GridVirusKeyword = "grid virus";

        protected bool IsGridVirus(Card card)
        {
            return card.DoKeywordsContain(GridVirusKeyword);
        }

        protected int GetNumberOfGridVirusesInPlay()
        {
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && this.IsGridVirus(c)).Count();
        }

    }
}