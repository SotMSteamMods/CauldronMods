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
        [Test]
        public void TestAtomicPunchIrradiateOtherCards()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card ring = PutInHand("TheLegacyRing");
            Card surge = PutInHand("SurgeOfStrength");
            Card fort = PutInHand("Fortitude");

            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionSelectCards = new Card[] { ring, surge, baron.CharacterCard };
            QuickHPStorage(baron, pyre, legacy, bunker, scholar);
            PlayCard("AtomicPunch");
            QuickHPCheck(-2, 0, 0, 0, 0);
            AssertIrradiated(ring);
            AssertIrradiated(surge);
            AssertNotIrradiated(fort);
            AssertNumberOfStatusEffectsInPlay(0);
        }
        [Test]
        public void TestAtomicPunchIrradiateOwnCards()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card chromo = PutInHand("Chromodynamics");
            Card drive = PutInHand("CherenkovDrive");
            Card ion = PutInHand("IonTrace");

            DecisionSelectTurnTaker = pyre.TurnTaker;
            DecisionSelectCards = new Card[] { chromo, drive, baron.CharacterCard };
            QuickHPStorage(baron, pyre, legacy, bunker, scholar);
            PlayCard("AtomicPunch");
            QuickHPCheck(-3, 0, 0, 0, 0);
            AssertIrradiated(chromo);
            AssertIrradiated(drive);
            AssertNotIrradiated(ion);
            AssertNumberOfStatusEffectsInPlay(3);
        }
        [Test]
        public void TestAtomicPunchNotIrradiateAlreadyIrradiated()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            DecisionSelectTurnTaker = pyre.TurnTaker;
            MoveAllCardsFromHandToDeck(pyre);
            Card ion = PutOnDeck("IonTrace");
            Card drive = PutOnDeck("CherenkovDrive");
            UsePower(pyre);
            UsePower(pyre);
            AssertIrradiated(ion);
            AssertIrradiated(drive);

            //which TurnTaker, and who to damage - no cards are possible to select
            AssertMaxNumberOfDecisions(2);
            PlayCard("AtomicPunch");
        }
        [Test]
        public void TestCellularIrradiationWithIrradiatedCards([Values(0, 1, 2, 3, 4)] int numIrradiated)
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            DecisionSelectTurnTaker = legacy.TurnTaker;
            Card thokk = PutOnDeck("Thokk");
            Card surge = PutOnDeck("SurgeOfStrength");
            Card ring = PutOnDeck("TheLegacyRing");
            Card charge = PutOnDeck("MotivationalCharge");
            Card fort = PutOnDeck("Fortitude");
            var drawOrder = new Card[] { charge, fort, ring, surge, thokk };

            for(int i = 0; i < numIrradiated; i++)
            {
                UsePower(pyre);
            }

            DecisionSelectCard = fort;

            Card cell = PutOnDeck("CellularIrradiation");
            QuickHandStorage(pyre, legacy, bunker, scholar);
            QuickHPStorage(baron, pyre, legacy, bunker, scholar);
            PlayCard(cell);

            int numCardsDrawn = 0;
            int numPowersLeft = 1;
            int numHPLoss = 0;
            Location fortLoc = legacy.HeroTurnTaker.Deck;

            if(numIrradiated >= 1)
            {
                fortLoc = legacy.HeroTurnTaker.Hand;
                numPowersLeft = 0;
            }
            if (numIrradiated >= 2)
            {
                numCardsDrawn = 1;
            }
            if(numIrradiated >= 3)
            {
                numHPLoss = -3;
            }
            if(numIrradiated >= 4)
            {
                numCardsDrawn--;
                fortLoc = legacy.TurnTaker.PlayArea;
            }

            AssertNumberOfUsablePowers(legacy, numPowersLeft);
            QuickHandCheck(0, numCardsDrawn, 0, 0);
            QuickHPCheck(0, 0, numHPLoss, 0, 0);
            Assert.AreEqual(fortLoc, fort.Location, $"Fortitude should have been in {fortLoc.GetFriendlyName()}, but it was in {fort.Location.GetFriendlyName()}");
        }
        [Test]
        public void TestCellularIrradiationOnlyCountSamePlayerCards()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card chromo = PutOnDeck("Chromodynamics");
            DecisionSelectTurnTaker = pyre.TurnTaker;
            UsePower(pyre);

            DecisionSelectTurnTaker = legacy.TurnTaker;
            Card fort = PutOnDeck("Fortitude");
            UsePower(pyre);

            QuickHandStorage(legacy);
            QuickHPStorage(legacy);
            PlayCard("CellularIrradiation");
            AssertNumberOfUsablePowers(legacy, 0);
            QuickHandCheckZero();
            QuickHPCheckZero();
        }
    }
}
