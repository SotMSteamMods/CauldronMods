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
    public class DriftRandomTests : RandomGameTest
    {
        [Test]
        public void TestDrift_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Drift" });
            RunGame(gameController);
        }

        [Test]
        public void TestDualDrift_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Drift/DualDriftCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestAllInGoodTimesDrift_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Drift/AllInGoodTimeDriftCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestDriftingShadowDrift_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Drift/DriftingShadowDriftCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestTestSubjectHalberdDrift_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Drift/TestSubjectHalberdDriftCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestThroughTheBreachDrift_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Drift/ThroughTheBreachDriftCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestDrift_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Drift" });
            RunGame(gameController);
        }

        [Test]
        public void TestDualDrift_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Drift/DualDriftCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestAllInGoodTimesDrift_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Drift/AllInGoodTimeDriftCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestDriftingShadowDrift_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Drift/DriftingShadowDriftCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestTestSubjectHalberdDrift_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Drift/TestSubjectHalberdDriftCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestThroughTheBreachDrift_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Drift/ThroughTheBreachDriftCharacter" });
            RunGame(gameController);
        }
    }
}