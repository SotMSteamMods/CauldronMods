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
    public class CypherRandomTests : RandomGameTest
    {
        [Test]
        public void TestCyper_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Cypher" });
            RunGame(gameController);
        }

        [Test]
        public void TestFirstResponseCypher_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Cypher/FirstResponseCypherCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestSwarmingProtocolCypher_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Cypher/SwarmingProtocolCypherCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestCyper_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Cypher" });
            RunGame(gameController);
        }

        [Test]
        public void TestFirstResponseCypher_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Cypher/FirstResponseCypherCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestSwarmingProtocolCypher_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Cypher/SwarmingProtocolCypherCharacter" });
            RunGame(gameController);
        }


    }
}