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
    public class MalichaeRandomTests : RandomGameTest
    {
        [Test]
        public void TestMalichae_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Malichae" });
            RunGame(gameController);
        }

        [Test]
        public void TestMinistryOfStrategicScienceMalichae_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestShardmasterMalichae_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Malichae/ShardmasterMalichaeCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestMalichae_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Malichae" });
            RunGame(gameController);
        }

        [Test]
        public void TestMinistryOfStrategicScienceMalichae_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestShardmasterMalichae_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Malichae/ShardmasterMalichaeCharacter" });
            RunGame(gameController);
        }

    }
}