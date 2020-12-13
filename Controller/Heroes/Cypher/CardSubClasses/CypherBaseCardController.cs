using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Cauldron.Cypher
{
    public abstract class CypherBaseCardController : CardController
    {
        protected CypherBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected LinqCardCriteria AugmentCardCriteria(Func<Card, bool> additionalCriteria = null)
        {
            if (additionalCriteria != null)
            {
                return new LinqCardCriteria(c => IsAugment(c) && additionalCriteria(c), "augments", false, false, "augment", "augments");
            }
            else
            {
                return new LinqCardCriteria(IsAugment, "augments", false, false, "augment", "augments");
            }
        }

        protected LinqCardCriteria AugmentedHeroCharacterCardCriteria(Func<Card, bool> additionalCriteria = null)
        {
            if (additionalCriteria != null)
            {
                return new LinqCardCriteria(c => IsAugmentedHeroCharacterCard(c) && additionalCriteria(c), "augmented heroes", false);
            }
            else
            {
                return new LinqCardCriteria(IsAugmentedHeroCharacterCard, "augmented heroes", false);
            }
        }

        protected SpecialString ShowSpecialStringNumberOfAugmentsAtLocation(Location loc)
        {
            return SpecialStringMaker.ShowNumberOfCardsAtLocation(loc, AugmentCardCriteria());
        }

        protected SpecialString ShowSpecialStringNumberOfAugmentsInPlay()
        {
            return SpecialStringMaker.ShowNumberOfCardsInPlay(AugmentCardCriteria());
        }

        protected SpecialString ShowSpecialStringAugmentsInPlay()
        {
            return SpecialStringMaker.ShowListOfCardsInPlay(AugmentCardCriteria());
        }

        protected SpecialString ShowSpecialStringAugmentedHeroes()
        {
            return SpecialStringMaker.ShowListOfCardsInPlay(AugmentedHeroCharacterCardCriteria());
        }

        protected bool IsAugment(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "augment");
        }

        protected bool IsAugmentedHeroCharacterCard(Card hero)
        {
            return hero.IsHeroCharacterCard && hero.IsInPlayAndHasGameText && !hero.IsIncapacitatedOrOutOfGame
                && hero.NextToLocation.HasCards && hero.GetAllNextToCards(false).Any(IsAugment);
        }

        protected List<Card> GetAugmentsInPlay()
        {
            return FindCardsWhere(c => c.IsInPlayAndHasGameText && IsAugment(c)).ToList();
        }

        protected List<TurnTaker> GetAugmentedHeroTurnTakers()
        {
            return FindTurnTakersWhere(tt =>
                tt.IsHero && tt.CharacterCards.Any(card => card.NextToLocation.HasCards && card.NextToLocation.Cards.Any(IsAugment))).ToList();
        }

        protected List<Card> GetAugmentsForHero(Card hero)
        {
            return !IsAugmentedHeroCharacterCard(hero) ? new List<Card>() : hero.GetAllNextToCards(false).Where(IsAugment).ToList();
        }

        protected IEnumerator MoveAugment(SelectCardDecision scd)
        {
            if (scd.SelectedCard == null)
            {
                yield break;
            }

            var otherHeroLocations = FindCardsWhere(c => c != scd.SelectedCard.Location.OwnerCard && c.IsHeroCharacterCard &&
                                                         c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame)
                                    .Select(h => new MoveCardDestination(h.NextToLocation, showMessage: true)).ToList();

            IEnumerator routine = GameController.SelectLocationAndMoveCard(this.DecisionMaker, scd.SelectedCard,
                otherHeroLocations, cardSource: GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
    }
}