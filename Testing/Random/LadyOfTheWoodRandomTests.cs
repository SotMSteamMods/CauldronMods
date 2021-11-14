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
    public class LadyOfTheWoodRandomTests : RandomGameTest
    {
        [Test]
        public void TestLadyOfTheWood_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.LadyOfTheWood" });
            RunGame(gameController);
        }

        [Test]
        public void TestMinistryOfStrategicScienceLadyOfTheWood_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.LadyOfTheWood/MinistryOfStrategicScienceLadyOfTheWoodCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestSeasonsOfChange_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.LadyOfTheWood/SeasonsOfChangeLadyOfTheWoodCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestFutureLadyOfTheWood_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.LadyOfTheWood/FutureLadyOfTheWoodCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestLadyOfTheWood_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.LadyOfTheWood" });
            RunGame(gameController);
        }

        [Test]
        public void TestMinistryOfStrategicScienceLadyOfTheWood_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.LadyOfTheWood/MinistryOfStrategicScienceLadyOfTheWoodCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestSeasonsOfChange_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.LadyOfTheWood/SeasonsOfChangeLadyOfTheWoodCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestFutureLadyOfTheWood_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.LadyOfTheWood/FutureLadyOfTheWoodCharacter" });
            RunGame(gameController);
        }

    }
}