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
    public class GargoyleRandomTests : RandomGameTest
    {
        [Test]
        public void TestGargoyle_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gargoyle" });
            RunGame(gameController);
        }

        [Test]
        public void TestDragonRangerGargoyle_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gargoyle/DragonRangerGargoyleCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestFutureGargoyle_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gargoyle/FutureGargoyleCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestInfiltratorGargoyle_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gargoyle/InfiltratorGargoyleCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestWastelandRoninGargoyle_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gargoyle/WastelandRoninGargoyleCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestGargoyle_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gargoyle" });
            RunGame(gameController);
        }

        [Test]
        public void TestDragonRangerGargoyle_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gargoyle/DragonRangerGargoyleCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestFutureGargoyle_Reasonable()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gargoyle/FutureGargoyleCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestInfiltratorGargoyle_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gargoyle/InfiltratorGargoyleCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestWastelandRoninGargoyle_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gargoyle/WastelandRoninGargoyleCharacter" });
            RunGame(gameController);
        }

    }
}