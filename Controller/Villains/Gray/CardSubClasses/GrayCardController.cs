using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;

namespace Cauldron
{
    public class GrayCardController : CardController
    {
        public GrayCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCards(new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("radiation"), "radiation"));
        }

        public int? FindNumberOfRadiationCardsInPlay()
        {
            return new int?(base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("radiation"), false, null, false).Count<Card>());
        }
    }
}