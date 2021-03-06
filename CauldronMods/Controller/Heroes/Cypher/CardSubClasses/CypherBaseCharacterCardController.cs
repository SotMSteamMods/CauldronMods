﻿using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

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
            return hero.IsHeroCharacterCard && hero.IsInPlayAndHasGameText && !hero.IsIncapacitatedOrOutOfGame
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
                tt.IsHero && tt.CharacterCards.Any(IsAugmentedHeroCharacterCard)).ToList();
        }
    }
}