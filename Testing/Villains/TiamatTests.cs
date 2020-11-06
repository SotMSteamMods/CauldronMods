using Cauldron;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CauldronTests
{
    [TestFixture()]
    class TiamatTests : BaseTest
    {
        protected TurnTakerController tiamat { get { return FindVillain("InfernoTiamatCharacter"); } }

        [Test()]
        public void TestTiamatLoad()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(tiamat);
            Assert.IsInstanceOf(typeof(InfernoTiamatCharacterCardController), tiamat.CharacterCardController);

            Assert.AreEqual(40, tiamat.CharacterCard.HitPoints);
        }
    }
}
