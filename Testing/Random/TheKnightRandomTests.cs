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
    public class TheKnightRandomTests : RandomGameTest
    {
        [Test]
        public void TestTheKnight_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheKnight" });
            RunGame(gameController);
        }

        [Test]
        public void TestFairTheKnight_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheKnight/FairTheKnightCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestBerserker_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheKnight/BerserkerTheKnightCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestWastelandRonin_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheKnight/WastelandRoninTheKnightCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestPastTheKnight_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheKnight/PastTheKnightCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestTheKnight_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheKnight" });
            RunGame(gameController);
        }

        [Test]
        public void TestFairTheKnight_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheKnight/FairTheKnightCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestBerserker_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheKnight/BerserkerTheKnightCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestWastelandRonin_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheKnight/WastelandRoninTheKnightCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestPastTheKnight_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheKnight/PastTheKnightCharacter" });
            RunGame(gameController);
        }

    }
}