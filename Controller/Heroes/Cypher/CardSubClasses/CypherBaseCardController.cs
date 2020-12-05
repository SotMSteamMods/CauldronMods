using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cypher
{
    public class CypherBaseCardController : CardController
    {
        public CypherBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected bool IsAugment(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "augment");
        }

        protected bool IsAugmented(Card hero)
        {
            return hero.NextToLocation.HasCards && hero.GetAllNextToCards(false).Any(IsAugment);
        }

        protected List<Card> GetAugmentsInPlay()
        {
            return FindCardsWhere(c => c.IsInPlay && IsAugment(c)).ToList();
        }

        protected List<Card> GetAugmentedHeroCards()
        {
            return FindCardsWhere(c => c.IsHero && c.NextToLocation.HasCards && c.NextToLocation.Cards.Any(IsAugment)).ToList();
        }

        protected List<TurnTaker> GetAugmentedHeroTurnTakers()
        {
            return FindTurnTakersWhere(tt =>
                tt.IsHero && tt.CharacterCard.NextToLocation.HasCards &&
                tt.CharacterCard.NextToLocation.Cards.Any(IsAugment)).ToList();
        }

        protected List<Card> GetAugmentsForHero(Card hero)
        {
            return !IsAugmented(hero) ? new List<Card>() : hero.GetAllNextToCards(false).Where(IsAugment).ToList();
        }

        protected List<Card> GetValidAugmentMoveHeroes(Card sourceHero)
        {
            return FindCardsWhere(c => c != sourceHero && c.IsHero).ToList();
        }

        protected List<TurnTaker> GetValidAugmentMoveHeroes(TurnTaker sourceHero)
        {
            return FindTurnTakersWhere(tt => tt != sourceHero && tt.IsHero).ToList();
        }
    }
}