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
    public class NecroRandomTests : RandomGameTest
    {
        [Test]
        public void TestNecro_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Necro" });
            RunGame(gameController);
        }

        [Test]
        public void TestWardenOfChaosNecro_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Necro/WardenOfChaosNecroCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestLastOfTheForgottenOrder_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Necro/LastOfTheForgottenOrderNecroCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestPastNecro_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Necro/PastNecroCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestNecro_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Necro" });
            RunGame(gameController);
        }

        [Test]
        public void TestWardenOfChaosNecro_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Necro/WardenOfChaosNecroCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestLastOfTheForgottenOrder_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Necro/LastOfTheForgottenOrderNecroCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestPastNecro_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Necro/PastNecroCharacter" });
            RunGame(gameController);
        }

    }
}