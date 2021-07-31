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
    public class DocHavocRandomTests : RandomGameTest
    {
        [Test]
        public void TestDocHavoc_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.DocHavoc" });
            RunGame(gameController);
        }

        [Test]
        public void TestFirstResponseDocHavoc_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.DocHavoc/FirstResponseDocHavocCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestFutureDocHavoc_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.DocHavoc/FutureDocHavocCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestDocHavoc_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.DocHavoc" });
            RunGame(gameController);
        }

        [Test]
        public void TestFirstResponseDocHavoc_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.DocHavoc/FirstResponseDocHavocCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestFutureDocHavoc_Reasonable()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.DocHavoc/FutureDocHavocCharacter" });
            RunGame(gameController);
        }


    }
}