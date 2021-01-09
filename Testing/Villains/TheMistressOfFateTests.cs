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

        private DamageType DTM = DamageType.Melee;
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
        }
        [Test]
        public void TestTheTimelineDayCardsNotAffectedByHeroCards()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();

            PlayCard("IntoTheStratosphere");
            AssertNumberOfCardsInPlay(fate, 6);
        }
        [Test]
        public void TestTheTimelineEndOfEnvironmentFlipAllHeroesIncapped()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            GoToStartOfTurn(FindEnvironment());

            DealDamage(fate, legacy, 50, DamageType.Melee);
            DealDamage(fate, ra, 50, DamageType.Melee);
            DealDamage(fate, haka, 50, DamageType.Melee);

            DecisionYesNo = false;
            GoToStartOfTurn(fate);
            AssertFlipped(fate);
        }
        [Test]
        public void TestTheTimelineEndOfEnvironmentFlipAllDaysFaceUp()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();

            GoToStartOfTurn(FindEnvironment());
            var days = FindCardsWhere((Card c) => c.IsInPlay && c.IsFaceDownNonCharacter);
            foreach (Card day in days)
            {
                FlipCard(day);
            }
            DecisionYesNo = false;
            GoToStartOfTurn(fate);
            AssertFlipped(fate);
        }
        [Test]
        public void TestTheTimelineEndOfEnvironmentFlipMakeDecision()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();

            DecisionYesNo = true;
            GoToStartOfTurn(fate);
            AssertFlipped(fate);
        }
        [Test]
        public void TestTheTimelineEndOfEnvironmentFlipMakeDecisionDecline()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();

            DecisionYesNo = false;
            GoToStartOfTurn(fate);
            AssertNotFlipped(fate);
        }
        [Test]
        public void TestDayOfSaintsFlipFaceUp()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();

            QuickHPStorage(fate, legacy, ra, tempest);
            FlipCard(GetCard("DayOfSaints"));
            //3 * 3 for the raw damage, +2 for the boost, total 11 damage
            QuickHPCheck(0, 0, 0, -11);

            DealDamage(fate, legacy, 1, DamageType.Melee);
            QuickHPCheck(0, -3, 0, 0);

            DealDamage(legacy, fate, 1, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0);
        }
        [Test]
        public void TestDayOfSinnersFlipFaceUp()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();

            Card day = GetCard("DayOfSinners");
            Card oneshot = PutOnDeck("ToDust");
            Card anomaly = PutOnDeck("WarpedMalus");
            Card ongoing = PutOnDeck("SameTimeAndPlace");
            Card creature = PutOnDeck("ChaosButterfly");

            FlipCard(day);
            AssertUnderCard(day, oneshot);
            AssertInDeck(anomaly);
            AssertInDeck(ongoing);
            AssertInDeck(creature);

        }
        [Test]
        public void TestDayOfSinnersSpecialStrings()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();

            Card day = GetCard("DayOfSinners");
            Card oneshot = PutOnDeck("ToDust");

            FlipCard(day);
            AssertCardSpecialString(day, 0, "On this day, To Dust recurs.");
            AssertCardSpecialString(oneshot, 1, "This card recurs on the Day of Sinners.");
        }
        [Test]
        public void TestDayOfSorrowsFlipFaceUp()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();

            Card day = GetCard("DayOfSorrows");
            Card anomaly = PutOnDeck("WarpedMalus");
            Card oneshot = PutOnDeck("ToDust");
            Card ongoing = PutOnDeck("SameTimeAndPlace");
            Card creature = PutOnDeck("ChaosButterfly");

            FlipCard(day);
            AssertIsInPlay(anomaly);
            AssertInDeck(oneshot);
            AssertInDeck(ongoing);
            AssertInDeck(creature);

            DestroyCard("WarpedMalus");
            AssertUnderCard(day, anomaly);
        }
        [Test]
        public void TestDayOfSorrowsSpecialStrings()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();

            Card day = GetCard("DayOfSorrows");
            Card anomaly = PutOnDeck("WarpedMalus");

            FlipCard(day);
            AssertCardSpecialString(anomaly, 0, "This card recurs on the Day of Sorrows.");
            AssertCardSpecialString(day, 0, "On this day, Warped Malus recurs.");

            DestroyCard(anomaly);
            AssertCardSpecialString(anomaly, 1, "This card recurs on the Day of Sorrows.");
        }
        [Test]
        public void TestDayOfSwordsFlipFaceUpGrabOneShot()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();

            Card day = GetCard("DayOfSwords");
            Card anomaly = PutOnDeck("WarpedMalus");
            Card oneshot = PutOnDeck("ToDust");
            Card ongoing = PutOnDeck("SameTimeAndPlace");
            Card creature = PutOnDeck("ChaosButterfly");

            FlipCard(day);
            AssertInDeck(anomaly);
            AssertInDeck(ongoing);
            AssertInDeck(creature);

            AssertUnderCard(day, oneshot);
        }
        [Test]
        public void TestDayOfSwordsFlipFaceUpGrabAnomaly()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();

            Card day = GetCard("DayOfSwords");
            Card oneshot = PutOnDeck("ToDust");
            Card anomaly = PutOnDeck("WarpedMalus");
            Card ongoing = PutOnDeck("SameTimeAndPlace");
            Card creature = PutOnDeck("ChaosButterfly");

            FlipCard(day);
            AssertIsInPlay(anomaly);
            AssertInDeck(oneshot);
            AssertInDeck(ongoing);
            AssertInDeck(creature);

            DestroyCard(anomaly);
            AssertUnderCard(day, anomaly);
        }
        [Test]
        public void TestDayOfSwordsSpecialStrings()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();

            Card day = GetCard("DayOfSwords");
            Card anomaly = PutOnDeck("WarpedMalus");

            FlipCard(day);
            AssertCardSpecialString(anomaly, 0, "This card recurs on the Day of Swords.");
            AssertCardSpecialString(day, 0, "On this day, Warped Malus recurs.");

            DestroyCard(anomaly);
            AssertCardSpecialString(anomaly, 1, "This card recurs on the Day of Swords.");
        }
    }
}
