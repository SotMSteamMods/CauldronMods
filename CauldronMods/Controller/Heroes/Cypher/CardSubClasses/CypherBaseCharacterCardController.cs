using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cypher
{
    public abstract class CypherBaseCharacterCardController : HeroCharacterCardController
    {
        public static readonly string NanocloudKey = "Nanocloud";

        protected CypherBaseCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected SpecialString ShowSpecialStringAugmentedHeroes()
        {
            return SpecialStringMaker.ShowListOfCardsInPlay(new LinqCardCriteria(IsAugmentedHeroCharacterCard, "augmented heroes", false));
        }

        protected bool IsAugmentedHeroCharacterCard(Card hero)
        {
            return IsHeroCharacterCard(hero) && hero.IsInPlayAndHasGameText && !hero.IsIncapacitatedOrOutOfGame
                && hero.NextToLocation.HasCards && hero.GetAllNextToCards(false).Any(IsAugment);
        }

        protected List<Card> GetAugmentsInPlay()
        {
            return FindCardsWhere(IsInPlayAugment).ToList();
        }

        protected bool IsNanocloud(Card c)
        {
            return GameController.GetCardPropertyJournalEntryBoolean(c, CypherBaseCardController.NanocloudKey) == true;
        }

        protected bool IsAugment(Card card)
        {
            if (card != null)
            {
                if (card.HasGameText && base.GameController.DoesCardContainKeyword(card, "augment"))
                    return true;

                if (card.IsInPlay && card.IsFaceDownNonCharacter && IsNanocloud(card) == true)
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

                if (card.IsInPlay && card.IsFaceDownNonCharacter && IsNanocloud(card) == true)
                    return true;
            }
            return false;
        }

        protected List<TurnTaker> GetAugmentedHeroTurnTakers()
        {
            return FindTurnTakersWhere(tt =>
                IsHero(tt) && tt.CharacterCards.Any(IsAugmentedHeroCharacterCard)).ToList();
        }

        public override void AddStartOfGameTriggers()
        {
            AddTrigger((GameAction ga) => TurnTakerController is CypherTurnTakerController ttc && !ttc.ArePromosSetup, SetupPromos, TriggerType.Hidden, TriggerTiming.Before, priority: TriggerPriority.High);
        }

        public IEnumerator SetupPromos(GameAction ga)
        {
            if (TurnTakerController is CypherTurnTakerController ttc && !ttc.ArePromosSetup)
            {
                ttc.SetupPromos(ttc.availablePromos);
                ttc.ArePromosSetup = true;
            }

            return DoNothing();
        }

    }
}