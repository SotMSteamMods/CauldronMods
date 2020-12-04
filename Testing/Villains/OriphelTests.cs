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

using Cauldron.Oriphel;

namespace CauldronTests
{
    [TestFixture()]
    public class OriphelTests : BaseTest
    {
        #region OriphelHelperFunctions
        protected TurnTakerController oriphel { get { return FindVillain("Oriphel"); } }
        protected DamageType DTM
        {
            get { return DamageType.Melee; }
        }
        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                AssertCardHasKeyword(card, keyword, false);
            }
        }
        protected bool IsGuardian(Card c)
        {
            return GameController.DoesCardContainKeyword(c, "guardian");
        }

        private void CleanupStartingCards()
        {
            MoveCards(oriphel, (Card c) => c.IsInPlay && !c.IsCharacter, oriphel.TurnTaker.Deck, true);
        }
        #endregion

        [Test]
        public void TestOriphelLoads()
        {
            SetupGameController("Cauldron.Oriphel", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(oriphel);
            Assert.IsInstanceOf(typeof(OriphelCharacterCardController), oriphel.CharacterCardController);

            Assert.AreEqual(80, oriphel.CharacterCard.HitPoints);
            Assert.AreEqual("Jade", oriphel.CharacterCard.Title);
            FlipCard(oriphel.CharacterCard);
            Assert.AreEqual("Oriphel", oriphel.CharacterCard.Title);
        }
        [Test]
        public void TestOriphelDecklist()
        {
            SetupGameController("Cauldron.Oriphel", "Legacy", "Megalopolis");

            AssertHasKeyword("guardian", new[]
            {
                "HighAsriel",
                "HighDjaril",
                "HighPhaol",
                "HighTormul"
            });

            AssertHasKeyword("relic", new[]
            {
                "MoonShardkey",
                "SunShardkey",
                "VeilShardkey",
                "WorldShardkey"
            });

            AssertHasKeyword("goon", new[]
            {
                "MejiClanLeader",
                "MejiGuard",
                "MejiNomad",
                "ShardbearerNathaniel"
            });

            AssertHasKeyword("transformation", new[]
            {
                "GrandOriphel",
                "ShardwalkersAwakening"
            });

            AssertHasKeyword("ongoing", new[]
            {
                "GrandOriphel"
            });

            AssertHasKeyword("one-shot", new[]
            {
                "Mirage",
                "Sandstorm",
                "ScrollsOfZephaeren",
                "ShardwalkersAwakening",
                "UmbralJavelin"
            });

            AssertNumberOfCardsInGame((Card c) => c.IsVillain && !c.IsCharacter, 25);
        }
        [Test]
        public void TestShardkeyRevealTriggerPlaysTransformation([Values("Moon", "Sun", "Veil", "World")] string element)
        {
            SetupGameController("Cauldron.Oriphel", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            Card key = PlayCard(element + "Shardkey");
            Card wake = PutOnDeck("ShardwalkersAwakening");

            GoToStartOfTurn(oriphel);
            AssertInTrash(wake);
        }
        [Test]
        public void TestShardkeyRevealTriggerBottomsNonTransformation([Values("Moon", "Sun", "Veil", "World")] string element)
        {
            SetupGameController("Cauldron.Oriphel", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            Card key = PlayCard(element + "Shardkey");
            Card goon = PutOnDeck("MejiNomad");

            GoToStartOfTurn(oriphel);
            AssertOnBottomOfDeck(goon);
        }
        [Test]
        public void TestGuardianDestroyTrigger([Values("Asriel", "Djaril", "Phaol", "Tormul")] string name)
        {
            SetupGameController("Cauldron.Oriphel", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            QuickShuffleStorage(oriphel);
            Card guardian = PlayCard("High" + name);

            //stack deck so we know what Jade's extra play will be
            PutOnDeck("Sandstorm");
            PutOnDeck("WorldShardkey");
            PutOnDeck("Mirage");

            DestroyCard(guardian);
            QuickShuffleCheck(1);
            AssertNumberOfCardsInPlay((Card c) => c.IsRelic && c.IsVillain, 1);
        }
        [Test]
        public void TestGuardianDestroyTriggerNoRelicsLeft([Values("Asriel", "Djaril", "Phaol", "Tormul")] string name)
        {
            SetupGameController("Cauldron.Oriphel", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            PutInTrash(new string[] { "MoonShardkey", "SunShardkey", "VeilShardkey", "WorldShardkey" });
            QuickShuffleStorage(oriphel);
            Card guardian = PlayCard("High" + name);

            DestroyCard(guardian);
            QuickShuffleCheck(1);
            AssertNumberOfCardsInPlay((Card c) => c.IsRelic && c.IsVillain, 0);
        }
        [Test]
        public void TestOriphelSetup([Values(new string[] { }, new string[] { "Bunker" }, new string[] {"Bunker", "Fanatic"})] string[] extraHeroes)
        {
            var startStrings = new List<string> { "Cauldron.Oriphel", "Legacy", "Ra", "Tempest" };
            startStrings.AddRange(extraHeroes);
            startStrings.Add("Megalopolis");
            int totalHeroes = 3 + extraHeroes.Count();

            SetupGameController(startStrings);
            StartGame();

            AssertNumberOfCardsInPlay((Card c) => IsGuardian(c), totalHeroes - 2);
        }
        [Test]
        public void TestJadePlaysFromRelics()
        {
            SetupGameController("Cauldron.Oriphel", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            Card goon = PutOnDeck("MejiNomad");
            PlayCard("MoonShardkey");

            AssertIsInPlay(goon);
        }
        [Test]
        public void TestJadeTriggerHandlesOngoing()
        {
            SetupGameController("Cauldron.Oriphel", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            Card goon = PutOnDeck("MejiNomad");
            Card grand = PlayCard("GrandOriphel");

            AssertIsInPlay(goon);
            AssertInTrash(grand);
        }
        [Test]
        public void TestOriphelImmediateFlipResponse()
        {
            SetupGameController("Cauldron.Oriphel", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            PutInTrash(new string[] { "MoonShardkey", "SunShardkey", "VeilShardkey", "WorldShardkey" });
            QuickShuffleStorage(oriphel);

            FlipCard(oriphel);

            AssertNumberOfCardsInTrash(oriphel, 0);
            Assert.AreEqual(oriphel.CharacterCard.Title, "Oriphel");
            QuickShuffleCheck(1);
        }
        [Test]
        public void TestOriphelDamageReduction()
        {
            SetupGameController("Cauldron.Oriphel", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            QuickHPStorage(oriphel, legacy);
            FlipCard(oriphel);

            DealDamage(legacy, oriphel, 2, DTM);
            QuickHPCheck(-1, 0);
            DealDamage(oriphel, legacy, 2, DTM);
            QuickHPCheck(0, -2);
        }
        [Test]
        public void TestOriphelEndOfTurnDamageHIs3()
        {
            SetupGameController("Cauldron.Oriphel", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            FlipCard(oriphel);

            QuickHPStorage(oriphel, legacy, ra, tempest);
            GoToEndOfTurn();
            QuickHPCheck(0, -2, -2, 0);
        }
        [Test]
        public void TestOriphelEndOfTurnDamageHIs4()
        {
            SetupGameController("Cauldron.Oriphel", "Legacy", "Ra", "Tempest", "Haka", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            FlipCard(oriphel);

            QuickHPStorage(oriphel, legacy, ra, tempest, haka);
            GoToEndOfTurn();
            QuickHPCheck(0, -3, 0, 0, -3);
        }
        [Test]
        public void TestOriphelFlipConditionDestruction()
        {
            SetupGameController("Cauldron.Oriphel", "Legacy", "Ra", "Tempest", "Haka", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            FlipCard(oriphel);
            PutInTrash("MoonShardkey");
            Card sun = PlayCard("SunShardkey");
            DestroyCard(sun);
            AssertNotFlipped(oriphel);
        }
        [Test]
        public void TestOriphelFlipConditionDiscardFromDeck()
        {
            SetupGameController("Cauldron.Oriphel", "Legacy", "Ra", "Tempest", "Knyfe", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            FlipCard(oriphel);
            PutInTrash("MoonShardkey");
            PutOnDeck("SunShardkey");

            PlayCard("WreckingUppercut");
            AssertNotFlipped(oriphel);
        }
    }
}
