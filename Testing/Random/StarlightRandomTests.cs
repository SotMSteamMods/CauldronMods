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
    public class StarlightRandomTests : RandomGameTest
    {
        [Test]
        public void TestStarlight_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Starlight" });
            RunGame(gameController);
        }

        [Test]
        public void TestGenesisStarlight_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Starlight/GenesisStarlightCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestArea51_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Starlight/Area51StarlightCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestNightloreCouncilStarlight_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Starlight/NightloreCouncilStarlightCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestStarlight_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Starlight" });
            RunGame(gameController);
        }

        [Test]
        public void TestGenesisStarlight_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Starlight/GenesisStarlightCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestArea51_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Starlight/Area51StarlightCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestNightloreCouncilStarlight_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Starlight/NightloreCouncilStarlightCharacter" });
            RunGame(gameController);
        }

    }
}