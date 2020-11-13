using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Gray;
using System.Linq;

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
            StartGame();
            GoToPlayCardPhase(gray);

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(gray);
            Assert.IsInstanceOf(typeof(GrayCharacterCardController), gray.CharacterCardController);

            Assert.AreEqual(75, gray.CharacterCard.HitPoints);
            AssertInPlayArea(gray, GetCardInPlay("ChainReaction"));
        }
    }
}
