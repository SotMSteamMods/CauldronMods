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
    public class TerminusRandomTests : RandomGameTest
    {
        [Test]
        public void TestTerminus_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Terminus" });
            RunGame(gameController);
        }

        [Test]
        public void TestMinistryOfStrategicScienceTerminus_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Terminus/MinistryOfStrategicScienceTerminusCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestFutureTerminus_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Terminus/FutureTerminusCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestTerminus_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Terminus" });
            RunGame(gameController);
        }

        [Test]
        public void TestMinistryOfStrategicScienceTerminus_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Terminus/MinistryOfStrategicScienceTerminusCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestFutureTerminus_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Terminus/FutureTerminusCharacter" });
            RunGame(gameController);
        }

    }
}