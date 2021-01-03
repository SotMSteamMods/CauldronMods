using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;

namespace Cauldron.Gray
{
    public class GrayCardController : CardController
    {
        public GrayCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public int? FindNumberOfRadiationCardsInPlay()
        {
            return new int?(base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("radiation"), false, null, false).Count<Card>());
        }

        public int? FindNumberOfHeroEquipmentInPlay()
        {
            return new int?(base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsHero && base.IsEquipment(c), false, null, false).Count<Card>());
        }
    }
}