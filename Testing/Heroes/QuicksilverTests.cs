using Cauldron.Quicksilver;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    class QuicksilverTests : BaseTest
    {
        protected HeroTurnTakerController quicksilver { get { return FindHero("Quicksilver"); } }

        private void SetupIncap(TurnTakerController source, Card target)
        {
            SetHitPoints(target, 1);
            DealDamage(source, target, 2, DamageType.Melee);
        }

        [Test]
        public void TestQuicksilverLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver", "Ra", "TheWraith", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(quicksilver);
            Assert.IsInstanceOf(typeof(QuicksilverCharacterCardController), quicksilver.CharacterCardController);

            Assert.AreEqual(25, quicksilver.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestComboChainPreventDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver", "Legacy", "Ra", "RookCity");
            StartGame();
            PlayCard("ComboChain");
            PlayCard("AlloyStorm");
        }
    }
}
