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
    public class TiamatRandomTests : RandomGameTest
    {
        [Test]
        public void TestTiamat_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useVillain: "Cauldron.Tiamat");
            RunGame(gameController);
        }

        [Test]
        public void TestHydraTiamat_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useVillain: "Cauldron.Tiamat/HydraWinterTiamatCharacter");
            RunGame(gameController);
        }

        [Test]
        public void TestFutureTiamat_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useVillain: "Cauldron.Tiamat/FutureTiamatCharacter");
            RunGame(gameController);
        }

        [Test]
        public void TestTiamat_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useVillain: "Cauldron.Tiamat");
            RunGame(gameController);
        }

        [Test]
        public void TestHydraTiamat_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useVillain: "Cauldron.Tiamat/HydraWinterTiamatCharacter");
            RunGame(gameController);
        }

        [Test]
        public void TestFutureTiamat_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useVillain: "Cauldron.Tiamat/FutureTiamatCharacter");
            RunGame(gameController);
        }
    }
}