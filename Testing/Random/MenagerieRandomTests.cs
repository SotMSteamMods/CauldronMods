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
    public class MenagerieRandomTests : RandomGameTest
    {
        [Test]
        public void TestMenagerie_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useVillain: "Cauldron.Menagerie");
            RunGame(gameController);
        }

        [Test]
        public void TestMenagerie_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableEnvironments: CauldronEnvironments,
                useVillain: "Cauldron.Menagerie");
            RunGame(gameController);
        }
    }
}