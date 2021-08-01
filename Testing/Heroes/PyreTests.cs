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
        protected void RemoveCascadeFromGame()
        {
            MoveCards(pyre, new string[] { "RogueFissionCascade", "RogueFissionCascade" }, pyre.TurnTaker.OutOfGame);
        }
        protected void StartGamePyre()
        {
            StartGame();
            ShuffleTrashIntoDeck(pyre);
            var cardsInHand = pyre.HeroTurnTaker.Hand.Cards.ToList();
            foreach (Card c in cardsInHand)
            {
                if (IsIrradiated(c))
                {
                    MoveCard(pyre, c, pyre.TurnTaker.Deck, true);
                }
            }
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
        public void TestPyreDeckList()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            AssertHasKeyword("cascade", new string[]
            {
                "RogueFissionCascade"
            });
            AssertHasKeyword("equipment", new string[]
            {
                "AtmosphereScrubbers",
                "CherenkovDrive",
                "FissionRegulator",
                "FracturedControlRod",
                "HullCladding",
                "NeutronForcefield",
                "ParticleCollider",
                "ThermonuclearCore"
            });
            AssertHasKeyword("limited", new string[]
            {
                "Chromodynamics",
                "ContainmentBreach",
                "FissionRegulator",
                "HullCladding",
                "NeutronForcefield",
                "ParticleCollider",
                "ThermonuclearCore"
            });
            AssertHasKeyword("one-shot", new string[]
            {
                "AtomicPunch",
                "CellularIrradiation",
                "GammaBurst",
                "HalfLife",
                "IonTrace",
                "RogueFissionCascade"
            });
            AssertHasKeyword("ongoing", new string[]
            {
                "Chromodynamics",
                "ContainmentBreach"
            });
        }
        [Test]
        public void TestPyreCascadeSpecialStrings()
        {
            //just to check them in the log, doesn't do any verification
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGamePyre();

            PrintSpecialStringsForCard(pyre.CharacterCard);

            Card cascade0 = GetCard("RogueFissionCascade", 0);
            Card cascade1 = GetCard("RogueFissionCascade", 1);

            MoveCard(pyre, cascade0, pyre.TurnTaker.Trash);
            PrintSpecialStringsForCard(pyre.CharacterCard);

            MoveCard(pyre, cascade1, pyre.TurnTaker.Trash);
            PrintSpecialStringsForCard(pyre.CharacterCard);

            MoveCard(pyre, cascade1, pyre.TurnTaker.Deck);
            PrintSpecialStringsForCard(pyre.CharacterCard);

            MoveCard(pyre, cascade0, pyre.TurnTaker.Deck, toBottom: true);
            PrintSpecialStringsForCard(pyre.CharacterCard);

            ShuffleLocation(pyre.TurnTaker.Deck);
            PrintSpecialStringsForCard(pyre.CharacterCard);
        }
        [Test]
        public void TestPyreInnatePowerDrawCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGamePyre();

            RemoveCascadeFromGame();
            PrintSpecialStringsForCard(pyre.CharacterCard);

            QuickShuffleStorage(pyre);
            Card punch = PutOnDeck("AtomicPunch");
            UsePower(pyre);
            AssertInHand(punch);
            AssertIrradiated(punch);
            PrintSpecialStringsForCard(pyre.CharacterCard);

            QuickShuffleCheck(0);

            PutInTrash(punch);
            AssertNotIrradiated(punch);

            DecisionSelectTurnTaker = legacy.TurnTaker;
            Card ring = PutOnDeck("TheLegacyRing");
            UsePower(pyre);
            AssertIrradiated(ring);
            PrintSpecialStringsForCard(pyre.CharacterCard);

            PlayCard(ring);
            AssertNotIrradiated(ring);

        }
        [Test]
        public void TestPyreInnatePowerShuffleCascadeIntoDeck()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGamePyre();

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
        public void TestPyreIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGamePyre();

            DecisionSelectTurnTaker = legacy.TurnTaker;
            Card ring = PutOnDeck("TheLegacyRing");
            UsePower(pyre);
            Card fort = PutInHand("Fortitude");
            Card thokk = PutOnDeck("Thokk");

            DealDamage(baron, pyre, 50, DTM);

            DecisionSelectCards = new Card[] { null, ring, fort };
            PrintSpecialStringsForCard(pyre.CharacterCard);
            //discard-to-do-stuff is optional
            UseIncapacitatedAbility(pyre, 0);
            AssertIrradiated(ring);
            AssertInHand(fort);
            AssertNumberOfUsablePowers(legacy, 1);
            AssertOnTopOfDeck(thokk);

            UseIncapacitatedAbility(pyre, 0);
            AssertInTrash(ring);
            AssertIsInPlay(fort);
            AssertInHand(thokk);
            AssertNumberOfUsablePowers(legacy, 0);
        }
        [Test]
        public void TestPyreIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            DealDamage(baron, pyre, 50, DTM);

            DecisionSelectCard = legacy.CharacterCard;
            Card traffic = PlayCard("TrafficPileup");
            QuickHPStorage(baron.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, traffic);
            UseIncapacitatedAbility(pyre, 1);
            QuickHPCheck(-2, -1, -1, -1, -1);


        }
        [Test]
        public void TestPyreIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGamePyre();
            DealDamage(baron, pyre, 50, DTM);

            DecisionSelectTurnTakers = new TurnTaker[] { legacy.TurnTaker, bunker.TurnTaker, scholar.TurnTaker };
            Card thokk = PutInHand("Thokk");
            Card surge = PutInHand("SurgeOfStrength");
            Card ring = PutOnDeck("TheLegacyRing");
            Card fort = PutOnDeck("Fortitude");

            DiscardAllCards(bunker);
            Card flak = PutOnDeck("FlakCannon");
            Card plating = PutOnDeck("HeavyPlating");

            Card iron = PutOnDeck("FleshToIron");
            DecisionsYesNo = new bool[] { true, true, false };
            DecisionSelectCards = new Card[] { thokk, surge };
            UseIncapacitatedAbility(pyre, 2);
            AssertInTrash(thokk, surge);
            AssertInHand(ring, fort);

            UseIncapacitatedAbility(pyre, 2);
            AssertInHand(flak, plating);

            UseIncapacitatedAbility(pyre, 2);
            AssertOnTopOfDeck(iron);
        }

        [Test]
        public void TestPyreIncaps_OblivaeonSoftlock()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Pyre", "Legacy", "Haka", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGamePyre();

            SetupIncap(oblivaeon);
            GoToAfterEndOfTurn(oblivaeon);
            RunActiveTurnPhase();
        }
        [Test]
        public void TestChromodynamicsDamageTrigger()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            RemoveCascadeFromGame();

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
            StartGamePyre();

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
            StartGamePyre();

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
            StartGamePyre();

            DiscardAllCards(pyre);
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
            StartGamePyre();

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
            StartGamePyre();


            RemoveCascadeFromGame();

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
            StartGamePyre();
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
            AssertNumberOfStatusEffectsInPlay(2);
        }
        [Test]
        public void TestAtomicPunchIrradiateOwnCards()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card chromo = PutInHand("Chromodynamics");
            Card drive = PutInHand("CherenkovDrive");
            Card ion = PutInHand("IonTrace");

            int numStartingEffects = GameController.StatusEffectControllers.Count();
            GoToPlayCardPhase(pyre);
            DecisionSelectTurnTaker = pyre.TurnTaker;
            DecisionSelectCards = new Card[] { chromo, drive, baron.CharacterCard };
            QuickHPStorage(baron, pyre, legacy, bunker, scholar);
            PlayCard("AtomicPunch");
            QuickHPCheck(-3, 0, 0, 0, 0);
            AssertIrradiated(chromo);
            AssertIrradiated(drive);
            AssertNotIrradiated(ion);
            AssertNumberOfStatusEffectsInPlay(numStartingEffects + 3);

            DealDamage(pyre, baron, 1, DamageType.Energy);
            QuickHPCheck(-2, 0, 0, 0, 0);
            DealDamage(pyre, baron, 1, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0, 0);

            GoToStartOfTurn(legacy);
            DealDamage(pyre, baron, 1, DamageType.Energy);
            QuickHPCheck(-1, 0, 0, 0, 0);
        }
        [Test]
        public void TestAtomicPunchNotIrradiateAlreadyIrradiated()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            DecisionSelectTurnTaker = pyre.TurnTaker;
            MoveAllCardsFromHandToDeck(pyre);
            Card ion = PutOnDeck("IonTrace");
            UsePower(pyre);
            Card drive = PutOnDeck("CherenkovDrive");
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
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            RemoveCascadeFromGame();

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
            StartGamePyre();
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
        [Test]
        public void TestCherenkovDriveIrradiateCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card fort = PutInHand("Fortitude");
            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionSelectCard = fort;

            PlayCard("CherenkovDrive");
            GoToEndOfTurn(pyre);
            AssertIrradiated(fort);
        }
        [Test]
        public void TestCherenkovDrivePowerOnCardInHand()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card flak = PutInHand("FlakCannon");
            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = flak;
            DecisionYesNo = true;

            QuickHPStorage(baron);
            PlayCard("CherenkovDrive");
            GoToEndOfTurn(pyre);
            AssertIrradiated(flak);
            QuickHPCheck(-3);
        }

        [Test]
        public void TestCherenkovDrive_GrandDjinn()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Cauldron.Malichae", "Bunker", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card grandBathiel = PutInHand("GrandBathiel");
            DecisionSelectTurnTaker = malichae.TurnTaker;
            DecisionSelectCard = grandBathiel;
            DecisionYesNo = true;
            DecisionSelectTarget = baron.CharacterCard;

            QuickHPStorage(baron);
            PlayCard("CherenkovDrive");
            GoToEndOfTurn(pyre);
            AssertIrradiated(grandBathiel);
            QuickHPCheck(-6);
        }
        [Test]
        public void TestCherenkovDrivePowerSelfDestruct()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card aux = PutInHand("AuxiliaryPowerSource");
            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = aux;
            DecisionYesNo = true;

            QuickHandStorage(bunker);
            QuickHPStorage(baron);
            PlayCard("CherenkovDrive");
            GoToEndOfTurn(pyre);

            Assert.Ignore("Issue with GameController.DrawCards prevents this from working right");
            //bad interaction with GameController.DrawCards - it is hardcoded to stop drawing after any draw
            //that ends with the card source out-of-play
            QuickHandCheck(2);
            AssertInTrash(aux);
        }
        [Test]
        public void TestCherenkovDrivePowerSelfDestructAccountForBug()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card aux = PutInHand("AuxiliaryPowerSource");
            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = aux;
            DecisionYesNo = true;

            QuickHandStorage(bunker);
            QuickHPStorage(baron);
            PlayCard("CherenkovDrive");
            GoToEndOfTurn(pyre);

            //bad interaction with GameController.DrawCards - it is hardcoded to stop drawing after any draw
            //that ends with the card source out-of-play
            QuickHandCheck(0);
            AssertInTrash(aux);
        }
        [Test]
        public void TestCherenkovDriveMultiplePowers()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card hurricane = PutInHand("LocalizedHurricane");
            DecisionSelectTurnTaker = tempest.TurnTaker;
            DecisionSelectCard = hurricane;
            DecisionYesNo = true;

            QuickHPStorage(baron);
            PlayCard("CherenkovDrive");
            GoToEndOfTurn(pyre);
            AssertIrradiated(hurricane);
            QuickHPCheck(-3);
        }
        [Test]
        public void TestCherenkovDriveCannotUsePowers()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card flak = PutInHand("FlakCannon");
            DecisionSelectTurnTaker = bunker.TurnTaker;
            DecisionSelectCard = flak;
            DecisionYesNo = true;

            PlayCard("PaparazziOnTheScene");
            PlayCard("CherenkovDrive");

            //turn taker, card to irradiate
            AssertMaxNumberOfDecisions(2);
            QuickHPStorage(baron);
            GoToEndOfTurn(pyre);
            AssertIrradiated(flak);
            QuickHPCheckZero();
        }
        [Test]
        public void TestChromodynamicsPower()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card chromo = PlayCard("Chromodynamics");
            Card punch = PutInHand("AtomicPunch");
            DecisionSelectCards = new Card[] { punch, baron.CharacterCard, pyre.CharacterCard, legacy.CharacterCard };

            QuickHPStorage(baron, pyre, legacy, tempest, scholar);
            QuickHandStorage(pyre, legacy, tempest, scholar);
            UsePower(chromo);
            QuickHPCheck(-2, -2, 0, 0, 0);
            QuickHandCheck(-1, 0, 0, 0);
            AssertInTrash(punch);
        }
        [Test]
        public void TestContainmentBreachCardPlayResponse()
        {
            SetupGameController(new string[] { "BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis" });
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            if(pyre.TurnTaker.Trash.HasCards)
            {
                ShuffleTrashIntoDeck(pyre);
            }

            Card breach = PlayCard("ContainmentBreach");
            QuickHPStorage(baron);
            Card cell = PutOnDeck("CellularIrradiation");
            UsePower(pyre);

            Card cascade = PutInTrash("RogueFissionCascade");
            QuickShuffleStorage(pyre);

            PlayCard(cell);
            DealDamage(pyre, baron, 1, DamageType.Energy);
            QuickHPCheck(-2);
            DealDamage(pyre, baron, 1, DTM);
            QuickHPCheck(-1);
            AssertNumberOfStatusEffectsInPlay(1);

            AssertInDeck(cascade);
            QuickShuffleCheck(1);

            GoToStartOfTurn(pyre);
            DecisionSelectTurnTaker = legacy.TurnTaker;
            Card ring = PutOnDeck("TheLegacyRing");
            UsePower(pyre);
            PlayCard(ring);
            AssertNumberOfStatusEffectsInPlay(1);
            DealDamage(pyre, baron, 1, DamageType.Energy);
            QuickHPCheck(-3);

            QuickShuffleCheck(0);

            PlayCard("SurgeOfStrength");
            AssertNumberOfStatusEffectsInPlay(1);

            Card fort = PutOnDeck("Fortitude");
            UsePower(pyre);
            PlayCard(fort, isPutIntoPlay: true);
            DealDamage(pyre, baron, 1, DamageType.Energy);
            QuickHPCheck(-3);

            GoToStartOfTurn(legacy);
            AssertNumberOfStatusEffectsInPlay(0);
        }
        [Test]
        public void TestContainmentBreachPower()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            QuickShuffleStorage(pyre);
            Card breach = PlayCard("ContainmentBreach");
            Card traffic = PlayCard("TrafficPileup");
            Card redist = PlayCard("ElementalRedistributor");

            QuickHPStorage(baron.CharacterCard, redist, pyre.CharacterCard, legacy.CharacterCard, tempest.CharacterCard, scholar.CharacterCard, traffic);
            UsePower(breach);
            QuickHPCheck(-1, -1, -1, 0, 0, 0, -1);
        }
        [Test]
        public void TestContainmentBreachStatusEffectTiming()
        {
            SetupGameController(new string[] { "BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis" });
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            if (pyre.TurnTaker.Trash.HasCards)
            {
                ShuffleTrashIntoDeck(pyre);
            }

            Card breach = PlayCard("ContainmentBreach");
            QuickHPStorage(baron);
            Card cell = PutOnDeck("CellularIrradiation");
            UsePower(pyre);

            Card cascade = PutInTrash("RogueFissionCascade");
            QuickShuffleStorage(pyre);

            PlayCard(cell);
            DealDamage(pyre, baron, 1, DamageType.Energy);
            QuickHPCheck(-2);
            DealDamage(pyre, baron, 1, DTM);
            QuickHPCheck(-1);
            AssertNumberOfStatusEffectsInPlay(1);

            GoToStartOfTurn(pyre);
            DecisionSelectTurnTaker = legacy.TurnTaker;
            Card ring = PutOnDeck("TheLegacyRing");
            UsePower(pyre);
            PlayCard(ring);
            AssertNumberOfStatusEffectsInPlay(1);
            DealDamage(pyre, baron, 1, DamageType.Energy);
            QuickHPCheck(-3);

            PlayCard("SurgeOfStrength");
            AssertNumberOfStatusEffectsInPlay(1);

            Card fort = PutOnDeck("Fortitude");
            UsePower(pyre);
            PlayCard(fort, isPutIntoPlay: true);
            DealDamage(pyre, baron, 1, DamageType.Energy);
            QuickHPCheck(-3);

            GoToEndOfTurn(pyre);
            DealDamage(pyre, baron, 1, DamageType.Energy);
        }
        [Test]
        public void TestFissionRegulatorCascadePrevention()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card punch = PutOnDeck("AtomicPunch");
            Card regulator = PlayCard("FissionRegulator");
            Card cascade = PlayCard("RogueFissionCascade");
            AssertOnTopOfDeck(cascade);
            AssertInHand(punch);
            AssertInTrash(regulator);
        }
        [Test]
        public void TestFissionRegulatorPower()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card punch = PutInHand("AtomicPunch");
            Card ring = PutInHand("TheLegacyRing");
            Card strat = PutInHand("IntoTheStratosphere");
            Card iron = PutInHand("FleshToIron");

            Card regulator = PlayCard("FissionRegulator");
            DecisionSelectCards = new Card[] { punch, ring, strat, iron };
            UsePower(regulator);
            AssertIrradiated(punch);
            AssertIrradiated(ring);
            AssertIrradiated(strat);
            AssertIrradiated(iron);
        }
        [Test]
        public void TestFracturedControlRodDamageWhenPlayed()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            QuickHPStorage(baron);
            Card rod = PutInHand("FracturedControlRod");
            PlayCard(rod);
            QuickHPCheckZero();
            PutOnDeck(pyre, rod);
            UsePower(pyre);
            PlayCard(rod);
            QuickHPCheck(-3);

        }
        [Test]
        public void TestFracturedControlRodDestroyToPlayDiscard()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card fort = PutOnDeck("Fortitude");
            DecisionSelectTurnTaker = legacy.TurnTaker;
            UsePower(pyre);
            Card surge = PutInHand("SurgeOfStrength");

            Card rod = PlayCard("FracturedControlRod");
            DecisionYesNo = true;

            GameController.ExhaustCoroutine(GameController.DiscardCard(legacy, surge, null));
            AssertInTrash(surge);
            AssertIsInPlay(rod);

            GameController.ExhaustCoroutine(GameController.DiscardCard(legacy, fort, null));
            AssertInTrash(rod);
            AssertIsInPlay(fort);
        }

        [Test]
        public void TestGammaBurst()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card traffic = PutIntoPlay("TrafficPileup");
            Card redist = PutIntoPlay("ElementalRedistributor");

            DecisionAutoDecideIfAble = true;
            MoveAllCardsFromHandToDeck(legacy);
            Card fort = PutInHand("Fortitude");
            Card ring = PutInHand("TheLegacyRing");
            DecisionSelectTurnTaker = legacy.TurnTaker;

            QuickHPStorage(baron.CharacterCard, redist, pyre.CharacterCard, legacy.CharacterCard, tempest.CharacterCard, scholar.CharacterCard, traffic);
            PlayCard("GammaBurst");
            QuickHPCheck(-2, -2, 0, -2, 0, 0, -2);
            AssertIrradiated(fort);
            AssertIrradiated(ring);
        }
        [Test]
        public void TestGammaBurstIrradiatesOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card traffic = PutIntoPlay("TrafficPileup");
            Card redist = PutIntoPlay("ElementalRedistributor");

            DecisionAutoDecideIfAble = true;
            MoveAllCardsFromHandToDeck(legacy);
            Card fort = PutInHand("Fortitude");
            Card ring = PutInHand("TheLegacyRing");
            DecisionSelectTurnTaker = legacy.TurnTaker;

            DecisionSelectCards = new Card[] { ring, null };
            QuickHPStorage(baron.CharacterCard, redist, pyre.CharacterCard, legacy.CharacterCard, tempest.CharacterCard, scholar.CharacterCard, traffic);
            PlayCard("GammaBurst");
            QuickHPCheck(-1, -1, 0, -1, 0, 0, -1);
            AssertNotIrradiated(fort);
            AssertIrradiated(ring);
        }
        [Test]
        public void TestHalfLife()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card traffic = PutIntoPlay("TrafficPileup");
            Card redist = PutIntoPlay("ElementalRedistributor");
            UsePower(legacy);

            Card rod = PutOnDeck("FracturedControlRod");

            DecisionSelectCards = new Card[] { rod, rod, baron.CharacterCard };
            DecisionAutoDecideIfAble = true;
            QuickHPStorage(baron.CharacterCard, redist, pyre.CharacterCard, legacy.CharacterCard, tempest.CharacterCard, scholar.CharacterCard, traffic);
            PlayCard("HalfLife");
            AssertIsInPlay(rod);
            QuickHPCheck(-5, -1, 0, 0, 0, 0, -1);
        }
        [Test]
        public void TestHullCladdingDamageReduction()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            PlayCard("HullCladding");
            QuickHPStorage(baron, pyre);
            DealDamage(baron, pyre, 2, DTM);
            DealDamage(pyre, baron, 2, DTM);
            QuickHPCheck(-1, -1);
            DealDamage(pyre, pyre, 4, DTM);
            DealDamage(baron, baron, 4, DTM);
            QuickHPCheck(-4, -2);
        }
        [Test]
        public void TestHullCladdingContainmentBreachDestruction()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card cladding = PlayCard("HullCladding");
            DecisionSelectCard = cladding;
            Card breach = PlayCard("ContainmentBreach");
            AssertInTrash(cladding);
            AssertIsInPlay(breach);

            PlayCard(cladding);
            AssertInTrash(cladding);

            DecisionSelectCard = breach;
            PlayCard(cladding);
            AssertInTrash(breach);
            AssertIsInPlay(cladding);
        }
        [Test]
        public void TestHullCladdingIndestructibleDrops()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "TimeCataclysm");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card point = PlayCard("FixedPoint");

            Card cladding = PlayCard("HullCladding");
            DecisionSelectCard = cladding;
            Card breach = PlayCard("ContainmentBreach");
            
            AssertIsInPlay(cladding, breach);

            DestroyCard(point);
            AssertInTrash(cladding);
            AssertIsInPlay(breach);

            PlayCard(point);
            DecisionSelectCard = breach;
            PlayCard(cladding);
            PlayCard("Fortitude");
            DestroyCard(point);
            AssertInTrash(breach);
            AssertIsInPlay(cladding);
        }
        [Test]
        public void TestHullCladdingPower()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();
            RemoveCascadeFromGame();

            Card hull = PlayCard("HullCladding");
            QuickHandStorage(pyre, legacy, tempest, scholar);
            UsePower(hull);
            AssertInTrash(hull);
            QuickHandCheck(2, 0, 0, 0);

        }
        [Test]
        public void TestIonTraceDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            DiscardAllCards(pyre, legacy, tempest, scholar);
            UsePower(legacy);
            Card traffic = PutIntoPlay("TrafficPileup");
            Card redist = PutIntoPlay("ElementalRedistributor");

            QuickHPStorage(baron.CharacterCard, redist, pyre.CharacterCard, legacy.CharacterCard, tempest.CharacterCard, scholar.CharacterCard, traffic);
            PlayCard("IonTrace");
            QuickHPCheck(-1, -1, 0, 0, 0, 0, -1);
        }
        [Test]
        public void TestIonTraceRecovery()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(legacy);
            MoveAllCardsFromHandToDeck(tempest);
            DecisionSelectTurnTakers = new TurnTaker[] { legacy.TurnTaker, legacy.TurnTaker, tempest.TurnTaker };
            Card thokk = PutOnDeck("Thokk");
            UsePower(pyre);
            Card ring = PutInHand("TheLegacyRing");
            Card surge = PutInTrash("SurgeOfStrength");
            Card danger = PutInTrash("DangerSense");
            Card fort = PutInTrash("Fortitude");

            Card ball = PutInHand("BallLightning");
            Card aqua = PutInTrash("AquaticCorrespondence");

            DecisionSelectCards = new Card[] { ring, fort, null };
            PlayCard("IonTrace");
            AssertInHand(fort);
            AssertInTrash(aqua, surge, danger);
            AssertIrradiated(ring);
            AssertIrradiated(fort);
            AssertNotIrradiated(ball);
        }
        [Test]
        public void TestNeutronForcefieldIndestructibility()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            GoToStartOfTurn(pyre);
            Card neutron = PutOnDeck("NeutronForcefield");
            UsePower(pyre);
            PlayCard(neutron);

            DestroyCard(neutron);
            AssertIsInPlay(neutron);
            GoToStartOfTurn(legacy);
            DestroyCard(neutron);
            AssertInTrash(neutron);
        }
        [Test]
        public void TestNeutronForcefieldIndestructibilityTiming()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            GoToStartOfTurn(pyre);
            Card neutron = PutOnDeck("NeutronForcefield");
            UsePower(pyre);
            PlayCard(neutron);

            DestroyCard(neutron);
            AssertIsInPlay(neutron);
            GoToEndOfTurn(pyre);
            DestroyCard(neutron);
            AssertInTrash(neutron);
        }
        [Test]
        public void TestNeutronForcefieldPower()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card neutron = PlayCard("NeutronForcefield");
            UsePower(neutron);
            AssertInTrash(neutron);

            QuickHPStorage(baron, pyre, legacy);
            DealDamage(baron, baron, 1, DTM);
            DealDamage(pyre, pyre, 1, DTM);
            DealDamage(legacy, legacy, 1, DTM);
            QuickHPCheck(-1, 0, -1);

            GoToStartOfTurn(pyre);
            DealDamage(pyre, pyre, 1, DTM);
            QuickHPCheck(0, -1, 0);
        }
        [Test]
        public void TestParticleColliderPowerPlayIrradiated()
        {
            SetupGameController(new string[] { "BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis" });
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            if(pyre.HeroTurnTaker.Hand.Cards.Any(c => IsIrradiated(c)))
            {
                MoveCards(pyre, (Card c) => IsIrradiated(c), pyre.TurnTaker.Trash);
            }

            Card fort = PutOnDeck("Fortitude");
            Card ring = PutOnDeck("TheLegacyRing");
            DecisionSelectTurnTaker = legacy.TurnTaker;
            UsePower(pyre);
            UsePower(pyre);

            Card collider = PlayCard("ParticleCollider");
            DecisionSelectCards = new Card[] { fort, null };
            UsePower(collider);
            AssertIsInPlay(fort);
            AssertIrradiated(ring);
            UsePower(collider);
            AssertNotInPlay(ring);

            DecisionSelectCards = null;
            PutInTrash(ring);
            QuickHandStorage(pyre, legacy, tempest, scholar);
            UsePower(collider);
            QuickHandCheckZero();
        }
        [Test]
        public void TestParticleColliderPowerDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            Card core = PlayCard("ThermonuclearCore");
            Card collider = PlayCard("ParticleCollider");

            DecisionYesNo = false;

            QuickHPStorage(baron);
            UsePower(collider);
            QuickHPCheck(-1);
            AssertIsInPlay(core);

            DecisionYesNo = true;
            UsePower(collider);
            QuickHPCheck(-4);
            AssertInTrash(core);

            UsePower(collider);
            QuickHPCheck(-1);
        }
        [Test]
        public void TestRogueFissionCascadeAutoplay()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            DecisionSelectFunction = 0;
            Card top = PutOnDeck("AtomicPunch");
            Card cascade = PutInHand("RogueFissionCascade");
            AssertInTrash(cascade);
            AssertInHand(top);

            DecisionSelectFunction = 1;
            top = PutOnDeck("Chromodynamics");
            PutInHand(cascade);
            AssertInTrash(top);
            AssertInTrash(cascade);
        }
        [Test]
        public void TestRogueFissionCascadeDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            //in case of Thermonuclear Core
            DiscardAllCards(pyre);

            DecisionSelectTurnTakers = new TurnTaker[] { legacy.TurnTaker, tempest.TurnTaker };
            UsePower(pyre);

            QuickHPStorage(pyre, legacy, tempest, scholar);
            PutOnDeck("AtomicPunch");
            PutInHand("RogueFissionCascade");
            QuickHPCheck(0, -1, 0, 0);

            UsePower(pyre);
            PutOnDeck("AtomicPunch");
            PutInHand("RogueFissionCascade");
            QuickHPCheck(0, -2, -2, 0);
        }
        [Test]
        public void TestRogueFissionCascadeWarningIfLocationKnown()
        {
            SetupGameController("Omnitron", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            DiscardAllCards(pyre);
            Card cascade = PutInHand("RogueFissionCascade");

            MoveCard(pyre, cascade, pyre.TurnTaker.Deck);

            DecisionYesNo = false;
            GameController.ExhaustCoroutine(GameController.DrawCard(pyre.HeroTurnTaker, true));
            AssertOnTopOfDeck(cascade);
        }
        [Test]
        public void TestRogueFissionCascadeWarningNotGeneratedIfLocationNotKnown()
        {
            SetupGameController("Omnitron", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();
            DestroyNonCharacterVillainCards();

            DiscardAllCards(pyre);
            Card cascade = PutInHand("RogueFissionCascade");

            MoveCard(pyre, cascade, pyre.TurnTaker.Deck);
            ShuffleDeck(pyre);
            MoveCard(pyre, cascade, pyre.TurnTaker.Deck);

            DecisionYesNo = false;
            GameController.ExhaustCoroutine(GameController.DrawCard(pyre.HeroTurnTaker, true));
            AssertInTrash(cascade);
        }
        [Test]
        public void TestThermonuclearCoreEnteringHandResponse()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();

            Card core = PutOnDeck("ThermonuclearCore");
            Card punch = PutInHand("AtomicPunch");
            Card hull = PutOnDeck("HullCladding");
            UsePower(pyre);

            DecisionSelectCard = punch;
            AssertNextDecisionChoices(new Card[] { core, punch }, new Card[] { hull });
            PutInHand(core);
            AssertIrradiated(hull);
            AssertIrradiated(punch);
            AssertNotIrradiated(core);
        }
        [Test]
        public void TestThermonuclearCoreEndOfTurn()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre", "Legacy", "Tempest", "TheScholar", "Megalopolis");
            StartGamePyre();

            DiscardAllCards(pyre);

            DecisionSelectTurnTakers = new TurnTaker[] { legacy.TurnTaker, tempest.TurnTaker, scholar.TurnTaker };
            UsePower(pyre);
            UsePower(pyre);

            GoToStartOfTurn(pyre);
            PlayCard("ThermonuclearCore");
            Card iron = PutOnDeck("FleshToIron");
            AssertNextDecisionChoices(new TurnTaker[] { pyre.TurnTaker, scholar.TurnTaker }, new TurnTaker[] { legacy.TurnTaker, tempest.TurnTaker });
            GoToEndOfTurn(pyre);
            AssertIrradiated(iron);
        }
    }
}
