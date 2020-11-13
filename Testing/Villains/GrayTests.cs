using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Gray;

namespace CauldronTests
{
    [TestFixture()]
    class GrayTests : BaseTest
    {
        protected TurnTakerController gray { get { return FindVillain("Gray"); } }

        [Test()]
        public void TestGrayLoads()
        {
            SetupGameController("Cauldron.Gray", "Legacy", "Haka", "Ra", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(gray);
            Assert.IsInstanceOf(typeof(GrayCharacterCardController), gray.CharacterCardController);

            Assert.AreEqual(45, anathema.CharacterCard.HitPoints);
        }
    }
}
