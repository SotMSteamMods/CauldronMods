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
            var result = card != null && card.NextToLocation.Cards.Any((Card c) => c.Identifier == "IrradiatedMarker");
            if(result && !card.Location.IsHand)
            {
                Assert.Fail($"{card.Title} is irradiated, but is not in a hand!");
            }
            return result;
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

            DecisionSelectTurnTaker = legacy.TurnTaker;
            Card ring = PutOnDeck("TheLegacyRing");
            UsePower(pyre);
            AssertIrradiated(ring);
            PlayCard(ring);
            AssertNotIrradiated(ring);
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
        public void TestIrradiationRemainsAfterPyreIncap()
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
        [Test]
        public void TestIrradiationClearedWhenOwnerIncaps()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            AssertNumberOfCardsAtLocation(pyre.TurnTaker.OffToTheSide, 40);
            DecisionSelectTurnTaker = legacy.TurnTaker;
            Card ring = PutOnDeck("TheLegacyRing");
            UsePower(pyre);
            AssertNumberOfCardsAtLocation(pyre.TurnTaker.OffToTheSide, 39);

            AssertIrradiated(ring);
            DealDamage(baron, legacy, 50, DTM);
            AssertNotIrradiated(ring);
            AssertNumberOfCardsAtLocation(pyre.TurnTaker.OffToTheSide, 40);
        }
        [Test]
        public void TestAtmosphereScrubbersPlayGrantsPower()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card ring = PutOnDeck("TheLegacyRing");

            DecisionSelectPower = pyre.CharacterCard;
            DecisionSelectTurnTaker = legacy.TurnTaker;

            PlayCard("AtmosphereScrubbers");
            AssertInHand(ring);
            AssertIrradiated(ring);

            AssertNumberOfUsablePowers(pyre.CharacterCard, 0);
        }
        [Test]
        public void TestAtmosphereScrubbersPower()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card ring = PutOnDeck("TheLegacyRing");

            DecisionSelectPower = pyre.CharacterCard;
            DecisionSelectTurnTakers = new TurnTaker[] { legacy.TurnTaker , legacy.TurnTaker, bunker.TurnTaker};

            Card atmo = PlayCard("AtmosphereScrubbers");
            Card plating = PutInHand("HeavyPlating");
            Card surge = PutOnDeck("SurgeOfStrength");

            DecisionSelectCards = new Card[] { ring, plating };

            SetHitPoints(legacy, 20);
            SetHitPoints(pyre, 20);
            SetHitPoints(bunker, 20);
            QuickHPStorage(pyre, legacy, bunker);
            QuickHandStorage(pyre, legacy, bunker);

            UsePower(atmo);
            QuickHPCheck(0, 2, 2);
            QuickHandCheck(0, 0, -1);
            AssertInTrash(ring, plating);
            AssertInHand(surge);
        }
    }
}
