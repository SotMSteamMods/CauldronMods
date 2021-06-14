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
    public class EchelonRandomTests : RandomGameTest
    {
        [Test]
        public void TestEchelon_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Echelon" });
            RunGame(gameController);
        }

        [Test]
        public void TestFirstResponseEchelon_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Echelon/FirstResponseEchelonCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestFutureEchelon_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Echelon/FutureEchelonCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestEchelon_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Echelon" });
            RunGame(gameController);
        }

        [Test]
        public void TestFirstResponseEchelon_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Echelon/FirstResponseEchelonCharacter" });
            RunGame(gameController);
        }

        [Test]
        public void TestFutureEchelon_Reasonable()
        {
            GameController gameController = SetupRandomGameController(false,
                availableVillains: CauldronVillains,
                availableEnvironments: CauldronEnvironments,
                useHeroes: new List<string> { "Cauldron.Echelon/FutureEchelonCharacter" });
            RunGame(gameController);
        }


    }
}