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
    class GyrosaurVariantTests : CauldronBaseTest
    {
        #region GyrosaurHelperFunctions
        protected DamageType DTM => DamageType.Melee;
        #endregion
        [Test]
        public void TestSpeedDemonGyrosaurLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/SpeedDemonGyrosaurCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(gyrosaur);
            Assert.IsInstanceOf(typeof(SpeedDemonGyrosaurCharacterCardController), gyrosaur.CharacterCardController);

            Assert.AreEqual(28, gyrosaur.CharacterCard.HitPoints);
        }
        [Test]
        public void TestSpeedDemonPowerLessThanHalfCrash()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/SpeedDemonGyrosaurCharacter", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card chase = PutInHand("AMerryChase");
            Card escort = PutOnDeck("ProtectiveEscort");

            UsePower(gyrosaur);
            AssertIsInPlay(chase);
            AssertOnTopOfDeck(escort);
        }
        [Test]
        public void TestSpeedDemonPowerMoreThanHalfCrash()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/SpeedDemonGyrosaurCharacter", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card wreck = PutInHand("WreckingBall");
            Card chase = PutOnDeck("AMerryChase");

            UsePower(gyrosaur);
            AssertInHand(wreck, chase);
        }
        [Test]
        public void TestSpeedDemonPowerStabilizedBelowThreshold()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/SpeedDemonGyrosaurCharacter", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            PlayCard("GyroStabilizer");
            Card wreck = PutInHand("WreckingBall");
            Card chase = PutOnDeck("AMerryChase");
            DecisionSelectFunction = 0;

            UsePower(gyrosaur);
            AssertIsInPlay(wreck);
            AssertOnTopOfDeck(chase);
        }
        [Test]
        public void TestSpeedDemonPowerStabilizedAboveThreshold()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/SpeedDemonGyrosaurCharacter", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            PlayCard("GyroStabilizer");
            Card chase = PutInHand("AMerryChase");
            Card escort = PutOnDeck("ProtectiveEscort");
            DecisionSelectFunction = 1;

            UsePower(gyrosaur);
            AssertInHand(chase, escort);
        }
        [Test]
        public void TestSpeedDemonPowerStabilizedCannotDecreasePastThreshold()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/SpeedDemonGyrosaurCharacter", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PlayCard("GyroStabilizer");

            Card wipeout = PutInHand("Wipeout");
            Card wreck = PutInHand("WreckingBall");
            Card sphere = PutOnDeck("SphereOfDevastation");

            AssertNoDecision();
            UsePower(gyrosaur);
            AssertInHand(wipeout, wreck, sphere);
        }
        [Test]
        public void TestSpeedDemonPowerStabilizedCannotIncreasePastThreshold()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/SpeedDemonGyrosaurCharacter", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PlayCard("GyroStabilizer");

            Card shell = PutInHand("RapturianShell");
            Card read = PutInHand("ReadTheTerrain");
            Card chase = PutInHand("AMerryChase");
            DecisionSelectCard = shell;
            Card wipeout = PutOnDeck("Wipeout");

            AssertMaxNumberOfDecisions(1);
            UsePower(gyrosaur);
            AssertInHand(read, chase);
            AssertIsInPlay(shell);
            AssertOnTopOfDeck(wipeout);
        }
        [Test]
        public void TestSpeedDemonIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/SpeedDemonGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            DealDamage(baron, gyrosaur, 50, DTM);

            AssertIncapLetsHeroDrawCard(gyrosaur, 0, legacy, 1);
        }
        [Test]
        public void TestSpeedDemonIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/SpeedDemonGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            DealDamage(baron, gyrosaur, 50, DTM);

            MoveAllCardsFromHandToDeck(legacy);
            PutInHand("Fortitude");
            PutInHand("SurgeOfStrength");
            PutInHand("DangerSense");

            DecisionYesNo = true;
            UseIncapacitatedAbility(gyrosaur, 1);
            AssertNumberOfCardsInHand(legacy, 1);
            AssertNumberOfCardsInPlay(legacy, 3);
        }
        [Test]
        public void TestSpeedDemonIncap2Optional()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/SpeedDemonGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            DealDamage(baron, gyrosaur, 50, DTM);

            MoveAllCardsFromHandToDeck(legacy);
            PutInHand("Fortitude");
            PutInHand("SurgeOfStrength");
            PutInHand("DangerSense");

            DecisionYesNo = false;
            UseIncapacitatedAbility(gyrosaur, 1);
            AssertNumberOfCardsInHand(legacy, 3);
            AssertNumberOfCardsInPlay(legacy, 1);
        }
        [Test]
        public void TestSpeedDemonIncap2OnlyOne()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/SpeedDemonGyrosaurCharacter", "Legacy", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            DealDamage(baron, gyrosaur, 50, DTM);

            MoveAllCardsFromHandToDeck(legacy);
            PutInHand("Fortitude");
            PutInHand("SurgeOfStrength");
            PutInHand("DangerSense");

            DiscardCard(wraith);
            DecisionYesNo = true;
            AssertNextDecisionChoices(new TurnTaker[] { legacy.TurnTaker, wraith.TurnTaker }, new TurnTaker[] { ra.TurnTaker });

            UseIncapacitatedAbility(gyrosaur, 1);
            AssertNumberOfCardsInHand(legacy, 1);
            AssertNumberOfCardsInPlay(legacy, 3);
            AssertNumberOfCardsInHand(wraith, 3);
            AssertNumberOfCardsInPlay(wraith, 1);
        }
        [Test]
        public void TestSpeedDemonIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/SpeedDemonGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            DealDamage(baron, gyrosaur, 50, DTM);

            QuickHPStorage(baron, legacy, ra);
            UseIncapacitatedAbility(gyrosaur, 2);

            AssertNumberOfStatusEffectsInPlay(1);
            DealDamage(ra, baron, 3, DTM);
            DealDamage(baron, ra, 3, DTM);
            QuickHPCheck(-3, 0, -1);
            AssertNumberOfStatusEffectsInPlay(0);
            DealDamage(baron, ra, 3, DTM);
            QuickHPCheck(0, 0, -3);
        }
        [Test]
        public void TestRenegadeGyrosaurLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/RenegadeGyrosaurCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(gyrosaur);
            Assert.IsInstanceOf(typeof(RenegadeGyrosaurCharacterCardController), gyrosaur.CharacterCardController);

            Assert.AreEqual(29, gyrosaur.CharacterCard.HitPoints);
        }
        [Test]
        public void TestRenegadePowerDiscardCrash()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/RenegadeGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            var ttcs = new TurnTakerController[] { baron, gyrosaur, legacy, ra };
            foreach(TurnTakerController ttc in ttcs)
            {
                SetHitPoints(ttc, 10);
            }
            QuickHPStorage(ttcs);

            Card wipeout = PutInHand("Wipeout");
            Card chase = PutOnDeck("AMerryChase");

            UsePower(gyrosaur);
            AssertInTrash(wipeout);
            AssertInHand(chase);
            QuickHPCheck(0, 2, 0, 0);
        }
        [Test]
        public void TestRenegadePowerDiscardNonCrash()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/RenegadeGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            var ttcs = new TurnTakerController[] { baron, gyrosaur, legacy, ra };
            foreach (TurnTakerController ttc in ttcs)
            {
                SetHitPoints(ttc, 10);
            }
            QuickHPStorage(ttcs);

            Card wipeout = PutOnDeck("Wipeout");
            Card chase = PutInHand("AMerryChase");

            UsePower(gyrosaur);
            AssertInTrash(chase);
            AssertInHand(wipeout);
            QuickHPCheck(-2, 0, 0, 0);
        }
        [Test]
        public void TestRenegadePowerDiscardImpossible()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/RenegadeGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            var ttcs = new TurnTakerController[] { baron, gyrosaur, legacy, ra };
            foreach (TurnTakerController ttc in ttcs)
            {
                SetHitPoints(ttc, 10);
            }
            QuickHPStorage(ttcs);

            Card wipeout = PutOnDeck("Wipeout");
            Card chase = PutOnDeck("AMerryChase");

            UsePower(gyrosaur);
            AssertOnTopOfDeck(wipeout);
            AssertInHand(chase);
            QuickHPCheck(-2, 0, 0, 0);
        }
        [Test]
        public void TestRenegadeIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/RenegadeGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            DealDamage(baron, gyrosaur, 50, DTM);

            AssertIncapLetsHeroDrawCard(gyrosaur, 0, ra, 1);
        }
        [Test]
        public void TestRenegadeIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/RenegadeGyrosaurCharacter", "Legacy", "Ra", "TheWraith", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            DealDamage(baron, gyrosaur, 50, DTM);

            PlayCard("StunBolt");
            PlayCard("ThrowingKnives");

            UseIncapacitatedAbility(gyrosaur, 1);

            AssertNumberOfUsablePowers(legacy, 0);
            AssertNumberOfUsablePowers(ra, 0);
            AssertNumberOfUsablePowers(wraith, 3);
        }
        [Test]
        public void TestRenegadeIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/RenegadeGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            DealDamage(baron, gyrosaur, 50, DTM);

            UseIncapacitatedAbility(gyrosaur, 2);
            AssertNumberOfStatusEffectsInPlay(1);

            SetHitPoints(baron, 10);
            SetHitPoints(legacy, 10);
            QuickHPStorage(baron, legacy);

            GainHP(baron, 10);
            GainHP(legacy, 10);
            QuickHPCheckZero();

            GoToStartOfTurn(gyrosaur);
            AssertNumberOfStatusEffectsInPlay(0);
            GainHP(baron, 10);
            GainHP(legacy, 10);
            QuickHPCheck(10, 10);
        }
        [Test]
        public void TestCaptainGyrosaurLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/CaptainGyrosaurCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(gyrosaur);
            Assert.IsInstanceOf(typeof(CaptainGyrosaurCharacterCardController), gyrosaur.CharacterCardController);

            Assert.AreEqual(29, gyrosaur.CharacterCard.HitPoints);
        }
    }
}
