using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Gyrosaur;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    class GyrosaurTests : CauldronBaseTest
    {
        #region GyrosaurHelperFunctions
        protected DamageType DTM = DamageType.Melee;
        #endregion
        [Test]
        public void TestGyrosaurLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(gyrosaur);
            Assert.IsInstanceOf(typeof(GyrosaurCharacterCardController), gyrosaur.CharacterCardController);

            Assert.AreEqual(30, gyrosaur.CharacterCard.HitPoints);
        }

        public void TestGyrosaurDecklist()
        {
            //TODO
        }
        [Test]
        public void TestGyrosaurInnate2CrashInHand()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            PutInHand("SphereOfDevastation");
            PutInHand("Ricochet");

            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHPStorage(baron, gyrosaur, legacy, ra);
            QuickHandStorage(gyrosaur);
            UsePower(gyrosaur);
            QuickHPCheck(-1, -1, -1, 0);
            QuickHandCheckZero();

            //check that it's "up to"
            DecisionSelectTargets = new Card[] { baron.CharacterCard, null, gyrosaur.CharacterCard };
            DecisionSelectTargetsIndex = 0;
            UsePower(gyrosaur);
            QuickHPCheck(-1, 0, 0, 0);
        }
        [Test]
        public void TestGyrosaurInnate0CrashInHand()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            Card stabilizer = PutOnDeck("GyroStabilizer");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur);
            QuickHPStorage(baron, gyrosaur, legacy, ra);
            UsePower(gyrosaur);
            QuickHandCheck(1);
            QuickHPCheckZero();
            AssertInHand(stabilizer);
        }
        [Test]
        public void TestGyrosaurInnateStabilized0Crash()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PlayCard("GyroStabilizer");
            Card omni = PutOnDeck("Omnivore");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur);
            QuickHPStorage(baron, gyrosaur, legacy, ra);
            AssertNoDecision();

            //with 0 cards in hand Gyro Stabilizer cannot bump it up to the threshold.
            //Therefore it should not present a decision.
            UsePower(gyrosaur);
            QuickHandCheck(1);
            QuickHPCheckZero();
            AssertInHand(omni);
        }
        [Test]
        public void TestGyrosaurInnateStabilized1CrashUnchanged()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PlayCard("GyroStabilizer");
            PutInHand("SphereOfDevastation");
            Card omni = PutOnDeck("Omnivore");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur);
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            DecisionDoNotSelectFunction = true;

            //with 1 crash in hand Gyro Stabilizer can bump it up to 2.
            //We should get a decision, and be able to stand pat for a draw.
            UsePower(gyrosaur);
            QuickHandCheck(1);
            QuickHPCheckZero();
            AssertInHand(omni);
        }
        [Test]
        public void TestGyrosaurInnateStabilized1CrashIncreased()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PlayCard("GyroStabilizer");
            PutInHand("SphereOfDevastation");
            
            Card omni = PutOnDeck("Omnivore");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur);
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            DecisionSelectFunction = 2;

            //with 1 crash in hand Gyro Stabilizer can bump it up to 2.
            //We should get a decision, and be able to increase it for damage.
            UsePower(gyrosaur);
            QuickHandCheck(0);
            QuickHPCheck(-1, -1, -1, 0);
            AssertOnTopOfDeck(omni);
        }
        [Test]
        public void TestGyrosaurInnateStabilized2CrashUnchanged()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PlayCard("GyroStabilizer");
            PutInHand("SphereOfDevastation");
            PutInHand("Wipeout");

            Card omni = PutOnDeck("Omnivore");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur);
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            DecisionDoNotSelectFunction = true;

            //with 2 crash in hand, Gyro Stabilizer can bump it down to 1.
            //We should get a decision, and be able to leave it alone for damage.
            UsePower(gyrosaur);
            QuickHandCheck(0);
            QuickHPCheck(-1, -1, -1, 0);
            AssertOnTopOfDeck(omni);
        }
        [Test]
        public void TestGyrosaurInnateStabilized2CrashDecreased()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PlayCard("GyroStabilizer");
            PutInHand("SphereOfDevastation");
            PutInHand("Wipeout");

            Card omni = PutOnDeck("Omnivore");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur);
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            DecisionSelectFunction = 0;

            //with 2 crash in hand, Gyro Stabilizer can bump it down to 1.
            //We should get a decision, and be able to decrease it for a draw.
            UsePower(gyrosaur);
            QuickHandCheck(1);
            QuickHPCheckZero();
            AssertInHand(omni);
        }
        [Test]
        public void TestGyrosaurInnateStabilized3Crash()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PlayCard("GyroStabilizer");
            PutInHand("SphereOfDevastation");
            PutInHand("Wipeout");
            PutInHand("Ricochet");

            Card omni = PutOnDeck("Omnivore");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur);
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            AssertMaxNumberOfDecisions(3);

            //with 3 crash in hand, Gyro Stabilizer cannot push them past the threshold.
            //We should not get a decision for it.
            UsePower(gyrosaur);
            QuickHandCheck(0);
            QuickHPCheck(-1, -1, -1, 0);
            AssertOnTopOfDeck(omni);
        }
        [Test]
        public void TestGyrosaurIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DealDamage(baron, gyrosaur, 50, DTM);

            AssertIncapLetsHeroDrawCard(gyrosaur, 0, legacy, 1);
        }
        [Test]
        public void TestGyrosaurIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DealDamage(baron, gyrosaur, 50, DTM);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            AssertNextDecisionChoices(new Card[] { baron.CharacterCard, legacy.CharacterCard, ra.CharacterCard }, new Card[] { mdp });
            QuickHPStorage(baron, legacy, ra);
            //check it's self-damage
            PlayCard("TheStaffOfRa");
            DecisionSelectTarget = ra.CharacterCard;
            UseIncapacitatedAbility(gyrosaur, 1);
            QuickHPCheck(0, 0, -4);
        }
        [Test]
        public void TestGyrosaurIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DealDamage(baron, gyrosaur, 50, DTM);

            Card traffic = PlayCard("TrafficPileup");
            Card mdp = GetMobileDefensePlatform().Card;
            AssertNextDecisionChoices(new Card[] { mdp, traffic }, new Card[] { baron.CharacterCard, legacy.CharacterCard, ra.CharacterCard });
            UseIncapacitatedAbility(gyrosaur, 2);
            AssertNumberOfStatusEffectsInPlay(1);
            QuickHPStorage(mdp, traffic);
            DealDamage(ra, mdp, 1, DTM);
            DealDamage(ra, traffic, 1, DTM);
            QuickHPCheck(-2, -1);

            GoToStartOfTurn(gyrosaur);
            AssertNumberOfStatusEffectsInPlay(0);
        }
        [Test]
        public void TestAMerryChase()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            Card chase = PlayCard("AMerryChase");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(baron, gyrosaur, legacy, ra);
            DealDamage(baron, gyrosaur, 1, DTM);
            AssertNumberOfStatusEffectsInPlay(1);
            DealDamage(baron, legacy, 1, DTM);
            QuickHPCheck(0, -1, 0, 0);

            //doesn't prevent damage to Gyrosaur or stack extra effects
            DealDamage(baron, gyrosaur, 1, DTM);
            QuickHPCheck(0, -1, 0, 0);
            AssertNumberOfStatusEffectsInPlay(1);

            //doesn't stop damage from other targets
            DealDamage(mdp, legacy, 1, DTM);
            QuickHPCheck(0, 0, -1, 0);

            //prevent damage from multiple targets at once
            Card traffic = PlayCard("TrafficPileup");
            DealDamage(traffic, gyrosaur, 1, DTM);
            DealDamage(traffic, legacy, 1, DTM);
            QuickHPCheck(0, -1, 0, 0);
            AssertNumberOfStatusEffectsInPlay(2);

            PlayCard("HeroicInterception");
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            //requires damage to actually be dealt
            DealDamage(mdp, gyrosaur, 1, DTM);
            AssertNumberOfStatusEffectsInPlay(2);
            DealDamage(mdp, legacy, 1, DTM);
            QuickHPCheck(0, 0, -1, 0);

            //start-of-turn destruction and status effect end
            GoToStartOfTurn(gyrosaur);
            AssertInTrash(chase);
            DealDamage(baron, legacy, 1, DTM);
            QuickHPCheck(0, 0, -2, 0);
        }
    }
}
