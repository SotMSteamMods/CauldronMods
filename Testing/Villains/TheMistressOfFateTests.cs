using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Reflection;
using Handelabra;
using System.Collections.Generic;
using Cauldron.TheMistressOfFate;

namespace CauldronTests
{
    class TheMistressOfFateTests : BaseTest
    {
        #region FateHelperFunctions
        protected TurnTakerController fate { get { return FindVillain("TheMistressOfFate"); } }
        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                AssertCardHasKeyword(card, keyword, false);
            }
        }
        private Card heroStorage(HeroTurnTakerController hero, string variety)
        {
            var cards = hero.TurnTaker.OffToTheSide.Cards;
            return cards.Where((Card c) => c.Identifier == variety + "Storage").FirstOrDefault();
        }
        #endregion
        [Test]
        public void TestMistressOfFateLoads()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(fate);
            Assert.IsInstanceOf(typeof(TheMistressOfFateCharacterCardController), fate.CharacterCardController);

            Assert.AreEqual(88, fate.CharacterCard.HitPoints);
        }
        [Test]
        public void TestMistressOfFateDecklist()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Megalopolis");

            AssertHasKeyword("day", new string[]
            {
                "DayOfSaints",
                "DayOfSinners",
                "DayOfSorrows",
                "DayOfSwords"
            });

            AssertHasKeyword("one-shot", new string[] {
                "CantFightFate",
                "FadingRealities",
                "TangledWeft",
                "ToDust"
            });
            AssertHasKeyword("ongoing", new string[] {
                "IllusionOfFreeWill",
                "MemoryOfTomorrow",
                "NecessaryCorrection",
                "SameTimeAndPlace",
                "SeeThePattern",
                "StolenFuture"
            });
            AssertHasKeyword("anomaly", new string[] {
                "HourDevourer",
                "ResidualMalus",
                "WarpedMalus"
            });
            AssertHasKeyword("creature", new string[] {
                "ChaosButterfly"
            });
        }

        [Test]
        public void TestMistressOfFateSetsUp()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            AssertNumberOfCardsAtLocation(legacy.TurnTaker.OffToTheSide, 3);
            AssertIsInPlay("TheTimeline");
            var days = new Card[] { GetCard("DayOfSaints"), GetCard("DayOfSinners"), GetCard("DayOfSorrows"), GetCard("DayOfSwords") };
            AssertIsInPlay(days);
            foreach (Card day in days)
            {
                AssertDoesNotHaveGameText(day);
            }
        }

        [Test]
        public void TestMistressOfFatePreservesIncappedHero()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card fortitude = PutInHand("Fortitude");
            Card surge = PutInTrash("SurgeOfStrength");
            Card ring = PutOnDeck("TheLegacyRing");
            Card presence = PutIntoPlay("InspiringPresence");

            DealDamage(fate, legacy, 50, DamageType.Melee);
            AssertUnderCard(heroStorage(legacy, "Hand"), fortitude);
            AssertUnderCard(heroStorage(legacy, "Deck"), ring);
            AssertUnderCard(heroStorage(legacy, "Trash"), surge);
            AssertUnderCard(heroStorage(legacy, "Trash"), presence);
        }

        [Test]
        public void TestMistressOfFateRestoresIncappedHero()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card fortitude = PutInHand("Fortitude");
            Card surge = PutIntoPlay("SurgeOfStrength");
            Card ring = PutOnDeck("TheLegacyRing");
            Card presence = PutInTrash("InspiringPresence");

            DealDamage(fate, legacy, 50, DamageType.Melee);

            var list = new List<UnincapacitateHeroAction>();
            var coroutine = GameController.UnincapacitateHero(legacy.CharacterCardController, legacy.CharacterCard.Definition.HitPoints ?? 10, null, list, cardSource: fate.CharacterCardController.GetCardSource());
            GameController.ExhaustCoroutine(coroutine);

            AssertInHand(fortitude);
            AssertInTrash(surge, presence);
            AssertOnTopOfDeck(ring);
        }

        [Test]
        public void TestMistressOfFateContinuesWithAllHeroesIncapped()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            DealDamage(fate, legacy, 50, DamageType.Melee);
            DealDamage(fate, ra, 50, DamageType.Melee);
            DealDamage(fate, haka, 50, DamageType.Melee);

            AssertNotGameOver();
            GoToStartOfTurn(fate);
        }
    }
}
