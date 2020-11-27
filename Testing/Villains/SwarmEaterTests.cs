using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Linq;
using Cauldron.SwarmEater;

namespace CauldronTests
{
    [TestFixture()]
    class SwarmEaterTests : BaseTest
    {
        protected TurnTakerController swarm { get { return FindVillain("SwarmEater"); } }

        [Test()]
        public void TestSwarmEaterLoads()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(swarm);
            Assert.IsInstanceOf(typeof(SwarmEaterCharacterCardController), swarm.CharacterCardController);

            Assert.AreEqual(80, swarm.CharacterCard.HitPoints);
        }

        [Test()]
        public void Test()
        {
            SetupGameController("Cauldron.SwarmEater", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();
        }
    }
}
