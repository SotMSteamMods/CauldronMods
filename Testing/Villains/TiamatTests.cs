using Cauldron.Tiamat;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    class TiamatTests : BaseTest
    {
        protected TurnTakerController tiamat { get { return FindVillain("Tiamat"); } }
        protected Card inferno { get { return GetCardInPlay("InfernoTiamatCharacter"); } }
        protected Card storm { get { return GetCardInPlay("StormTiamatCharacter"); } }
        protected Card winter { get { return GetCardInPlay("WinterTiamatCharacter"); } }

        private void SetupIncap(TurnTakerController source, Card target)
        {
            SetHitPoints(target, 1);
            DealDamage(source, target, 2, DamageType.Melee);
        }

        [Test()]
        public void TestTiamatLoad()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(tiamat);
            Assert.IsInstanceOf(typeof(WinterTiamatCharacterCardController), tiamat.CharacterCardController);
            AssertNumberOfCardsInPlay(tiamat, 3);
            Assert.AreEqual(40, inferno.HitPoints);
            Assert.AreEqual(40, storm.HitPoints);
            Assert.AreEqual(40, winter.HitPoints);
        }

        [Test()]
        public void TestInfernoFlipOnDestroyIncap()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Megalopolis");
            SetupIncap(legacy, inferno);
            Assert.IsTrue(inferno.IsFlipped);
        }

        [Test()]
        public void TestDidNotWinOnInfernoIncap()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Megalopolis");
            SetupIncap(legacy, inferno);
            AssertNotGameOver();
        }

        [Test()]
        public void TestStormFlipOnDestroyIncap()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Megalopolis");
            SetupIncap(legacy, storm);
            Assert.IsTrue(storm.IsFlipped);
        }

        [Test()]
        public void TestDidNotWinOnStormIncap()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Megalopolis");
            SetupIncap(legacy, storm);
            AssertNotGameOver();
        }

        [Test()]
        public void TestWinterFlipOnDestroyIncap()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Megalopolis");
            SetupIncap(legacy, winter);
            Assert.IsTrue(winter.IsFlipped);
        }

        [Test()]
        public void TestDidNotWinOnWinterIncap()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Megalopolis");
            SetupIncap(legacy, winter);
            AssertNotGameOver();
        }

        [Test()]
        public void TestDecapitatedHeadCannotDealDamage()
        {
            SetupGameController("Cauldron.Tiamat", "Legacy", "Megalopolis");
            SetupIncap(legacy, winter);
            QuickHPStorage(legacy);
            DealDamage(winter, legacy, 2, DamageType.Cold);
            QuickHPCheck(0);
        }
    }
}
