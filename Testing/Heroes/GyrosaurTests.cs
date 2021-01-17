using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Gyrosaur;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    class GyrosaurTests : CauldronBaseTest
    {
        #region GyrosaurHelperFunctions
        #endregion
        [Test]
        public void TestGyrosaurLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(gyrosaur);
            Assert.IsInstanceOf(typeof(GyrosaurCharacterCardController), gyrosaur.CharacterCardController);

            Assert.AreEqual(30, gyrosaur.CharacterCard.HitPoints);
        }
    }
}
