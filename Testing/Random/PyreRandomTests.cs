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
    public class PyreRandomTests : RandomGameTest
    {
        [Test]
        public void TestPyre_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Pyre" });
            RunGame(gameController);
        }

        [Test]
        public void TestUnstablePyre_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Pyre/UnstablePyreCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestWastelandRonin_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Pyre/WastelandRoninPyreCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestExpeditionOblaskPyre_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Pyre/ExpeditionOblaskPyreCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestPyre_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Pyre" });
            RunGame(gameController);
        }

        [Test]
        public void TestUnstablePyre_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Pyre/UnstablePyreCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestWastelandRonin_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Pyre/WastelandRoninPyreCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestExpeditionOblaskPyre_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Pyre/ExpeditionOblaskPyreCharacter" });
            RunGame(gameController);
        }

    }
}