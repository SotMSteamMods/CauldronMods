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
    public class TitanRandomTests : RandomGameTest
    {
        [Test]
        public void TestTitan_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Titan" });
            RunGame(gameController);
        }

        [Test]
        public void TestMinistryOfStrategicScienceTitan_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Titan/MinistryOfStrategicScienceTitanCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestOni_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Titan/OniTitanCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestFutureTitan_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Titan/FutureTitanCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestTitan_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Titan" });
            RunGame(gameController);
        }

        [Test]
        public void TestMinistryOfStrategicScienceTitan_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Titan/MinistryOfStrategicScienceTitanCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestOni_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Titan/OniTitanCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestFutureTitan_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Titan/FutureTitanCharacter" });
            RunGame(gameController);
        }

    }
}