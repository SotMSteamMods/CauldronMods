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
    public class ImpactRandomTests : RandomGameTest
    {
        [Test]
        public void TestImpact_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Impact" });
            RunGame(gameController);
        }

        [Test]
        public void TestRenegadeImpact_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Impact/RenegadeImpactCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestWastelandRonin_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Impact/WastelandRoninImpactCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestImpact_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Impact" });
            RunGame(gameController);
        }

        [Test]
        public void TestRenegadeImpact_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Impact/RenegadeImpactCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestWastelandRonin_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Impact/WastelandRoninImpactCharacter" });
            RunGame(gameController);
        }

    }
}