using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.CatchwaterHarbor
{
    public abstract class CatchwaterHarborUtilityCardController : CardController
    {
        protected CatchwaterHarborUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public static readonly string TransportKeyword = "transport";
        public static readonly string GangsterKeyword = "gangster";

        protected static readonly string DestroyNextTurnKey = "DestroyNextTurn";

        protected bool IsTransport(Card card)
        {
            return card.DoKeywordsContain(TransportKeyword);
        }

        protected bool IsGangster(Card card)
        {
            return card.DoKeywordsContain(GangsterKeyword);
        }

        protected int GetNumberOfTransportsInPlay()
        {
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && IsTransport(c)).Count();
        }
    }
}
