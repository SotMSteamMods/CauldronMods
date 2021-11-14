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
    public class VanishRandomTests : RandomGameTest
    {
        [Test]
        public void TestVanish_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Vanish" });
            RunGame(gameController);
        }

        [Test]
        public void TestFirstResponseVanish_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Vanish/FirstResponseVanishCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestTombOfThieves_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Vanish/TombOfThievesVanishCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestPastVanish_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Vanish/PastVanishCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestVanish_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Vanish" });
            RunGame(gameController);
        }

        [Test]
        public void TestFirstResponseVanish_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Vanish/FirstResponseVanishCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestTombOfThieves_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Vanish/TombOfThievesVanishCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestPastVanish_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Vanish/PastVanishCharacter" });
            RunGame(gameController);
        }

    }
}