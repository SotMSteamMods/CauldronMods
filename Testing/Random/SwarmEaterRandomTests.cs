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
    public class SwarmEaterRandomTests : RandomGameTest
    {
        [Test]
        public void TestSwarmEater_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useVillain: "Cauldron.SwarmEater");
            RunGame(gameController);
        }

        [Test]
        public void TestDistributedHivemindSwarmEater_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useVillain: "Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter");
            RunGame(gameController);
        }

        [Test]
        public void TestSwarmEater_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useVillain: "Cauldron.SwarmEater");
            RunGame(gameController);
        }

        [Test]
        public void TestDistributedHivemindSwarmEater_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useVillain: "Cauldron.SwarmEater/DistributedHivemindSwarmEaterCharacter");
            RunGame(gameController);
        }
    }
}