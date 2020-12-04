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
    }
}
