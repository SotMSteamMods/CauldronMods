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
    public class TangoOneRandomTests : RandomGameTest
    {
        [Test]
        public void TestTangoOne_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TangoOne" });
            RunGame(gameController);
        }

        [Test]
        public void TestGhostOpsTangoOne_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TangoOne/GhostOpsTangoOneCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestCreedOfTheSniper_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TangoOne/CreedOfTheSniperTangoOneCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestPastTangoOne_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TangoOne/PastTangoOneCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestTangoOne_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TangoOne" });
            RunGame(gameController);
        }

        [Test]
        public void TestGhostOpsTangoOne_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TangoOne/GhostOpsTangoOneCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestCreedOfTheSniper_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TangoOne/CreedOfTheSniperTangoOneCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestPastTangoOne_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TangoOne/PastTangoOneCharacter" });
            RunGame(gameController);
        }

    }
}