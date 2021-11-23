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
    public class MagnificentMaraRandomTests : RandomGameTest
    {
        [Test]
        public void TestMagnificentMara_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.MagnificentMara" });
            RunGame(gameController);
        }

        [Test]
        public void TestMinistryOfStrategicScienceMagnificentMara_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.MagnificentMara/MinistryOfStrategicScienceMagnificentMaraCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestPastMagnificentMara_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.MagnificentMara/PastMagnificentMaraCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestMagnificentMara_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.MagnificentMara" });
            RunGame(gameController);
        }

        [Test]
        public void TestMinistryOfStrategicScienceMagnificentMara_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.MagnificentMara/MinistryOfStrategicScienceMagnificentMaraCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestPastMagnificentMara_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.MagnificentMara/PastMagnificentMaraCharacter" });
            RunGame(gameController);
        }

    }
}