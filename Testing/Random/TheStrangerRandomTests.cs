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
    public class TheStrangerRandomTests : RandomGameTest
    {
        [Test]
        public void TestTheStranger_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheStranger" });
            RunGame(gameController);
        }

        [Test]
        public void TestRunecarvedTheStranger_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheStranger/RunecarvedTheStrangerCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestWastelandRonin_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheStranger/WastelandRoninTheStrangerCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestPastTheStranger_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheStranger/PastTheStrangerCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestCornTheStranger_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheStranger/CornTheStrangerCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestTheStranger_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheStranger" });
            RunGame(gameController);
        }

        [Test]
        public void TestRunecarvedTheStranger_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheStranger/RunecarvedTheStrangerCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestWastelandRonin_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheStranger/WastelandRoninTheStrangerCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestPastTheStranger_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheStranger/PastTheStrangerCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestCornTheStranger_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.TheStranger/CornTheStrangerCharacter" });
            RunGame(gameController);
        }

    }
}