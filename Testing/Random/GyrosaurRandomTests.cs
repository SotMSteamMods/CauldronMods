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
    public class GyrosaurRandomTests : RandomGameTest
    {
        [Test]
        public void TestGyrosaur_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gyrosaur" });
            RunGame(gameController);
        }

        [Test]
        public void TestRenegadeGyrosaur_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gyrosaur/RenegadeGyrosaurCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestSpeedDemon_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gyrosaur/SpeedDemonGyrosaurCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestCaptainGyrosaur_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gyrosaur/CaptainGyrosaurCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestGyrosaur_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gyrosaur" });
            RunGame(gameController);
        }

        [Test]
        public void TestRenegadeGyrosaur_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gyrosaur/RenegadeGyrosaurCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestSpeedDemon_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gyrosaur/SpeedDemonGyrosaurCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestCaptainGyrosaur_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Gyrosaur/CaptainGyrosaurCharacter" });
            RunGame(gameController);
        }

    }
}