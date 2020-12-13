using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public abstract class CypherBaseCharacterCardController : HeroCharacterCardController
    {
        public CypherBaseCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected SpecialString ShowSpecialStringAugmentedHeroes()
        {
            return SpecialStringMaker.ShowListOfCardsInPlay(new LinqCardCriteria(IsAugmentedHeroCharacterCard, "augmented heroes", false));
        }

        protected bool IsAugmentedHeroCharacterCard(Card hero)
        {
            return hero.IsHeroCharacterCard && hero.IsInPlayAndHasGameText && !hero.IsIncapacitatedOrOutOfGame
                && hero.NextToLocation.HasCards && hero.GetAllNextToCards(false).Any(IsAugment);
        }

        protected bool IsAugment(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "augment");
        }

        protected List<TurnTaker> GetAugmentedHeroTurnTakers()
        {
            return FindTurnTakersWhere(tt =>
                tt.IsHero && tt.CharacterCards.Any(IsAugmentedHeroCharacterCard)).ToList();
        }
    }
}