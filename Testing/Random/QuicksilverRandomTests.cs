using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using Cauldron;
using System.Collections.Generic;

namespace CauldronTests.Random
{
    [TestFixture()]
    public class QuicksilverRandomTests : RandomGameTest
    {
        [Test]
        public void TestQuicksilver_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Quicksilver" });
            RunGame(gameController);
        }

        [Test]
        public void TestUncannyQuicksilver_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Quicksilver/UncannyQuicksilverCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestRenegade_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Quicksilver/RenegadeQuicksilverCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestHarbingerQuicksilver_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Quicksilver/HarbingerQuicksilverCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestQuicksilver_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Quicksilver" });
            RunGame(gameController);
        }

        [Test]
        public void TestUncannyQuicksilver_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Quicksilver/UncannyQuicksilverCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestRenegade_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Quicksilver/RenegadeQuicksilverCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestHarbingerQuicksilver_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Quicksilver/HarbingerQuicksilverCharacter" });
            RunGame(gameController);
        }

    }
}