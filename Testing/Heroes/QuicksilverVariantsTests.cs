using Cauldron.Quicksilver;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    class QuicksilverVariantsTests : BaseTest
    {
        protected HeroTurnTakerController quicksilver { get { return FindHero("Quicksilver"); } }

        private void SetupIncap(TurnTakerController source, Card target)
        {
            SetHitPoints(target, 1);
            DealDamage(source, target, 2, DamageType.Melee);
        }

        [Test()]
        public void TestUncannyQuicksilverLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver/UncannyQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(quicksilver);
            Assert.IsInstanceOf(typeof(UncannyQuicksilverCharacterCardController), quicksilver.CharacterCardController);

            Assert.AreEqual(26, quicksilver.CharacterCard.HitPoints);
        }

        [Test]
        public void TestUncannyQuicksilverPower()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver/UncannyQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            QuickHPStorage(apostate);
            UsePower(quicksilver);
            //{Quicksilver} deals 1 target 2 melee damage.
            QuickHPCheck(-2);
        }

        [Test]
        public void TestQuicksilverIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver/UncannyQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            SetupIncap(baron, quicksilver.CharacterCard);
            DiscardAllCards(ra);

            //One player may draw until they have 4 cards in hand.
            QuickHandStorage(ra);
            UseIncapacitatedAbility(quicksilver, 0);
            QuickHandCheck(4);
        }

        [Test]
        public void TestQuicksilverIncap2()
        {
            SetupGameController("Apostate", "Cauldron.Quicksilver/UncannyQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            SetupIncap(apostate, quicksilver.CharacterCard);

            //One player may use a power now.
            AssertIncapLetsHeroUsePower(quicksilver, 1, ra);
        }

        [Test]
        public void TestQuicksilverIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Quicksilver/UncannyQuicksilverCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            SetupIncap(baron, quicksilver.CharacterCard);
            SetHitPoints(ra, 10);
            SetHitPoints(wraith, 10);
            Card knives = PutInHand("ThrowingKnives");
            Card comp = PutInHand("MicroTargetingComputer");

            Card staff = PutInHand("TheStaffOfRa");
            Card blaze = PutInHand("BlazingTornado");
            Card exc = PutInHand("Excavation");

            //One player may discard any number of cards to regain X HP, where X is the number of cards discarded.
            DecisionsYesNo = new bool[] { true, true };
            DecisionSelectTurnTakers = new TurnTaker[] { ra.TurnTaker, wraith.TurnTaker };
            DecisionSelectCards = new Card[] { staff, blaze, exc, null, knives, comp, null };

            QuickHPStorage(ra);
            UseIncapacitatedAbility(quicksilver, 2);
            QuickHPCheck(3);

            QuickHPStorage(wraith);
            UseIncapacitatedAbility(quicksilver, 2);
            QuickHPCheck(2);
        }
    }
}
