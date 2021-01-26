using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Cauldron.Pyre;

namespace CauldronTests
{
    [TestFixture()]
    public class PyreTests : CauldronBaseTest
    {
        #region PyreHelperFunctions
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(pyre.CharacterCard, 1);
            DealDamage(villain, pyre, 2, DamageType.Melee);
        }

        protected DamageType DTM => DamageType.Melee;

        protected Card MDP { get { return FindCardInPlay("MobileDefensePlatform"); } }

        protected bool IsIrradiated(Card card)
        {
            return card != null && card.NextToLocation.Cards.Any((Card c) => c.Identifier == "IrradiatedMarker");
        }
        protected void AssertIrradiated(Card card)
        {
            Assert.IsTrue(IsIrradiated(card), $"{card.Title} should have been irradiated, but it was not.");
        }
        protected void AssertNotIrradiated(Card card)
        {
            Assert.IsFalse(IsIrradiated(card), $"{card.Title} was irradiated, but it should not be.");
        }
        #endregion PyreHelperFunctions
        [Test]
        public void TestPyreLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(pyre);
            Assert.IsInstanceOf(typeof(PyreCharacterCardController), pyre.CharacterCardController);

            Assert.AreEqual(29, pyre.CharacterCard.HitPoints);
        }

        [Test]
        public void TestPyreInnatePowerDrawCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            QuickShuffleStorage(pyre);
            Card punch = PutOnDeck("AtomicPunch");
            UsePower(pyre);
            AssertInHand(punch);
            AssertIrradiated(punch);
            QuickShuffleCheck(0);

            PutInTrash(punch);
            AssertNotIrradiated(punch);

            Card drive = PutOnDeck("CherenkovDrive");
            UsePower(pyre);
            AssertIrradiated(drive);
            PlayCard(drive);
            AssertNotIrradiated(drive);
        }
        [Test]
        public void TestPyreInnatePowerShuffleCascadeIntoDeck()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card cascade = PutInTrash("RogueFissionCascade");
            Card punch = PutOnDeck("AtomicPunch");
            QuickShuffleStorage(pyre);
            UsePower(pyre);
            AssertInHand(punch);
            AssertIrradiated(punch);
            AssertInDeck(cascade);
            QuickShuffleCheck(1);
        }
        [Test]
        public void TestChromodynamicsDamageTrigger()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            QuickHPStorage(baron, pyre, legacy, bunker, scholar);
            PlayCard("Chromodynamics");
            Card drive = PutOnDeck("CherenkovDrive");
            UsePower(pyre);
            AssertIrradiated(drive);
            Card rod = PutInHand("FracturedControlRod");

            PlayCard(rod);
            QuickHPCheckZero();
            PlayCard(drive);
            QuickHPCheck(-1, 0, 0, 0, 0);
        }
        [Test]
        public void TestIrradiationOtherPlayer()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DecisionSelectTurnTaker = legacy.TurnTaker;
            Card ring = PutOnDeck("TheLegacyRing");
            UsePower(pyre);

            AssertIrradiated(ring);
            PlayCard(ring);
            AssertNotIrradiated(ring);
        }
        [Test]
        public void TestIrradiationRemainsAfterIncap()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DecisionSelectTurnTaker = legacy.TurnTaker;
            Card ring = PutOnDeck("TheLegacyRing");
            UsePower(pyre);

            AssertIrradiated(ring);
            DealDamage(baron, pyre, 50, DTM);

            AssertIrradiated(ring);
            PlayCard(ring);
            AssertNotIrradiated(ring);
        }
    }
}
