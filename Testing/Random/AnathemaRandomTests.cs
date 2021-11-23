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
    public class AnathemaRandomTests : RandomGameTest
    {
        [Test]
        public void TestAnathema_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useVillain: "Cauldron.Anathema");
            RunGame(gameController);
        }

        [Test]
        public void TestAcceleratedEvolutionAnathema_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useVillain: "Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter");
            RunGame(gameController);
        }

        [Test]
        public void TestAnathema_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useVillain: "Cauldron.Anathema");
            RunGame(gameController);
        }

        [Test]
        public void TestAcceleratedEvolutionAnathema_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useVillain: "Cauldron.Anathema/AcceleratedEvolutionAnathemaCharacter");
            RunGame(gameController);
        }
    }
}