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
    public class TheWanderingIsleRandomTests : RandomGameTest
    {
        [Test]
        public void TestTheWanderingIsle_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                useEnvironment: "Cauldron.TheWanderingIsle");
            RunGame(gameController);
        }

        [Test]
        public void TestTheWanderingIsle_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                useEnvironment: "Cauldron.TheWanderingIsle");
            RunGame(gameController);
        }
    }
}