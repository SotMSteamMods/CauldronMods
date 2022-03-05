using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;
using System;
using Handelabra;

namespace Cauldron.Cypher
{
    public abstract class CypherBaseCardController : CardController
    {
        public static readonly string NanocloudKey = "Nanocloud";

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
            return SpecialStringMaker.ShowSpecialString(() => GetAugListSpecial());
        }

        private string GetAugListSpecial()
        {
            var augList = GetAugmentsInPlay();
            string augListSpecial = "Augments in play: ";
            if (augList.Any())
            {
                augListSpecial += string.Join(", ", augList.Select(c => c.Title + (c.IsFlipped ? " (flipped)" : "")).ToArray());
            }
            else
            {
                augListSpecial += "None";
            }
            return augListSpecial;
        }

        protected SpecialString ShowSpecialStringAugmentedHeroes()
        {
            return SpecialStringMaker.ShowListOfCardsInPlay(AugmentedHeroCharacterCardCriteria());
        }

        protected bool IsAugment(Card card)
        {
            if (card != null)
            {
                if (card.HasGameText && base.GameController.DoesCardContainKeyword(card, "augment"))
                    return true;

                if (card.IsInPlay && card.IsFaceDownNonCharacter && GameController.GetCardPropertyJournalEntryBoolean(card, NanocloudKey) == true)
                    return true;
            }
            return false;
        }

        protected bool IsInPlayAugment(Card card)
        {
            if (card != null)
            {
                if (card.IsInPlayAndHasGameText && base.GameController.DoesCardContainKeyword(card, "augment"))
                    return true;

                if (card.IsInPlay && card.IsFaceDownNonCharacter && GameController.GetCardPropertyJournalEntryBoolean(card, NanocloudKey) == true)
                    return true;
            }
            return false;
        }

        protected bool IsAugmentedHeroCharacterCard(Card hero)
        {
            return hero.IsHeroCharacterCard && hero.IsInPlayAndHasGameText && !hero.IsIncapacitatedOrOutOfGame
                && hero.NextToLocation.HasCards && hero.GetAllNextToCards(false).Any(IsAugment);
        }

        protected List<Card> GetAugmentsInPlay()
        {
            return FindCardsWhere(c => IsInPlayAugment(c)).ToList();
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

        protected IEnumerator MoveInPlayAugment(SelectCardDecision scd)
        {
            if (scd.SelectedCard is null)
            {
                yield break;
            }

            var otherHeroLocations = FindCardsWhere(c => c != scd.SelectedCard.Location.OwnerCard && c.IsHeroCharacterCard &&
                                                         c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource()), realCardsOnly: true)
                                    .Select(h => new MoveCardDestination(h.NextToLocation, showMessage: true)).ToList();

            IEnumerator routine = null;
            if (otherHeroLocations.Count() > 0)
            {
                routine = GameController.SelectLocationAndMoveCard(this.DecisionMaker, scd.SelectedCard, otherHeroLocations,
                                        isPutIntoPlay: false,
                                        playIfMovingToPlayArea: false,
                                        cardSource: GetCardSource());
            }
            else
            {
                routine = GameController.SendMessageAction("There are no available heroes to move the augment to.", Priority.Medium, GetCardSource(), showCardSource: true);
            }
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