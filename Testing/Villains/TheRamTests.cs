using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Reflection;
using Handelabra;
using System.Collections.Generic;
using Cauldron.TheRam;

namespace CauldronTests
{
    [TestFixture()]
    public class TheRamTests : BaseTest
    {
        #region RamHelperFunctions
        protected TurnTakerController ram { get { return FindVillain("TheRam"); } }
        #endregion

        [Test]
        public void TestRamLoads()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(ram);
            Assert.IsInstanceOf(typeof(TheRamCharacterCardController), ram.CharacterCardController);

            Assert.AreEqual(80, ram.CharacterCard.HitPoints);
        }
        [Test]
        public void TestRamSetsUp()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Megalopolis");

            StartGame();

            AssertNumberOfCardsInPlay(ram, 2);
            AssertIsInPlay("GrapplingClaw");
            AssertNumberOfCardsInTrash(ram, 5, (Card c) => c.Identifier == "UpClose");
            AssertNotFlipped(ram.CharacterCard);
        }
    }
}