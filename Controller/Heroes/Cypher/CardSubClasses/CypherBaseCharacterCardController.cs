using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class CypherBaseCharacterCardController : HeroCharacterCardController
    {
        public CypherBaseCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected bool IsAugment(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "augment");
        }

        protected List<Card> GetAugmentsInPlay()
        {
            return FindCardsWhere(c => c.IsInPlayAndHasGameText && IsAugment(c)).ToList();
        }

        protected List<TurnTaker> GetAugmentedHeroTurnTakers()
        {
            return FindTurnTakersWhere(tt =>
                tt.IsHero && tt.CharacterCard.NextToLocation.HasCards &&
                tt.CharacterCard.NextToLocation.Cards.Any(IsAugment)).ToList();
        }
    }
}