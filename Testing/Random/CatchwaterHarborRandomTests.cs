﻿using NUnit.Framework;
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
    public class CatchwaterHarborRandomTests : RandomGameTest
    {
        [Test]
        public void TestCatchwaterHarbor_Random()
        {
            GameController gameController = SetupRandomGameController(false,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                useEnvironment: "Cauldron.CatchwaterHarbor");
            RunGame(gameController);
        }

        [Test]
        public void TestCatchwaterHarbor_Reasonable()
        {
            GameController gameController = SetupRandomGameController(true,
                availableHeroes: CauldronHeroes,
                availableVillains: CauldronVillains,
                useEnvironment: "Cauldron.CatchwaterHarbor");
            RunGame(gameController);
        }
    }
}