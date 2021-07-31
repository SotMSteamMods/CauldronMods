using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using Cauldron;
using System.Collections.Generic;

namespace CauldronTests
{
    [TestFixture()]
    public class CricketRandomTests : RandomGameTest
    {
        [Test]
        public void TestCricket_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Cricket" });
            RunGame(gameController);
        }

        [Test]
        public void TestFirstResponseCricket_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Cricket/FirstResponseCricketCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestRenegadeCricket_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Cricket/RenegadeCricketCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestWastelandRoninCricket_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Cricket/WastelandRoninCricketCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestCricket_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Cricket" });
            RunGame(gameController);
        }

        [Test]
        public void TestFirstResponseCricket_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Cricket/FirstResponseCricketCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestRenegadeCricket_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Cricket/RenegadeCricketCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestWastelandRoninCricket_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Cricket/WastelandRoninCricketCharacter" });
            RunGame(gameController);
        }


    }
}