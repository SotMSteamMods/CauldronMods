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

        #region Speed Demon Gyrosaur
        [Test]
        public void TestSpeedDemonGyrosaurLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/SpeedDemonGyrosaurCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(gyrosaur);
            Assert.IsInstanceOf(typeof(SpeedDemonGyrosaurCharacterCardController), gyrosaur.CharacterCardController);

            Assert.AreEqual(28, gyrosaur.CharacterCard.HitPoints);
        }

        #region Test Innate Power
        /*
         * If at least half of the cards in your hand are crash cards, draw a card. If not, play a card.
         */
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

            Card wreck = PutInHand("WreckingBall"); // Crash
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
            Card wreck = PutInHand("WreckingBall"); // Crash
            Card chase = PutOnDeck("AMerryChase");
            DecisionSelectWord = "0 crash cards";

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
            DecisionSelectWord = "1 crash card";

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

            Card wipeout = PutInHand("Wipeout"); // Crash
            Card wreck = PutInHand("WreckingBall"); // Crash
            Card sphere = PutOnDeck("SphereOfDevastation"); // Crash

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
        #endregion Test Innate Power

        #region Test Incap Power
        /* 
         * One player may draw a card now. 
         * One player with fewer than 4 cards in their hand may play 2 cards now. 
         * Reduce the next damage dealt to a hero target by 2.
         */
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
            AssertNumberOfCardsInPlay(legacy, 3); // Includes Legacy Character Card
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
            AssertNumberOfCardsInPlay(legacy, 1); // Includes Legacy Character Card
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
            AssertNumberOfCardsInPlay(legacy, 3); // Includes Legacy Character Card
            AssertNumberOfCardsInHand(wraith, 3);
            AssertNumberOfCardsInPlay(wraith, 1); // Includes Wraith Character Card
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
        #endregion Test Incap Power

        #endregion Speed Demon Gyrosaur

        #region Renegade Gyrosaur
        [Test]
        public void TestRenegadeGyrosaurLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/RenegadeGyrosaurCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(gyrosaur);
            Assert.IsInstanceOf(typeof(RenegadeGyrosaurCharacterCardController), gyrosaur.CharacterCardController);

            Assert.AreEqual(29, gyrosaur.CharacterCard.HitPoints);
        }

        #region Test Innate Power
        /*
         * Discard a card. If it was a crash card, {Gyrosaur} regains 2 HP. Otherwise, she deals 1 target 2 melee damage. Draw a card.
         */
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

            Card wipeout = PutInHand("Wipeout"); // Crash
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
        #endregion Test Innate Power

        #region Test Incap Power
        /* 
         * One player may draw a card now. 
         * Each hero with fewer than 2 non-character cards in play may use a power now. 
         * Targets cannot regain HP until the start of your next turn.
         */
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
        #endregion Test Incap Power

        #endregion Renegade Gyrosaur

        #region Captain  Gyrosaur
        [Test]
        public void TestCaptainGyrosaurLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/CaptainGyrosaurCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(gyrosaur);
            Assert.IsInstanceOf(typeof(CaptainGyrosaurCharacterCardController), gyrosaur.CharacterCardController);

            Assert.AreEqual(29, gyrosaur.CharacterCard.HitPoints);
        }

        #region Test Innate Power
        /*
         * Put the top card of your deck beneath {Gyrosaur}, face up. When she deals damage, play or draw it.
         */
        [Test]
        public void TestCaptainPowerPlayStoredCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/CaptainGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();
            
            Card wreck = PutOnDeck("WreckingBall");
            UsePower(gyrosaur);
            AssertUnderCard(gyrosaur.CharacterCard, wreck);
            AssertNumberOfStatusEffectsInPlay(1);

            DecisionSelectFunction = 0;

            Card chase = PutOnDeck("AMerryChase");

            //not if damage prevented
            DealDamage(gyrosaur, baron, 1, DTM);
            AssertNumberOfStatusEffectsInPlay(1);
            AssertUnderCard(gyrosaur.CharacterCard, wreck);

            DealDamage(gyrosaur, ra, 1, DTM);
            AssertIsInPlay(wreck);
            AssertNumberOfStatusEffectsInPlay(0);

            AssertOnTopOfDeck(chase);
        }
        [Test]
        public void TestCaptainPowerDrawStoredCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/CaptainGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();

            Card wreck = PutOnDeck("WreckingBall");
            UsePower(gyrosaur);
            AssertUnderCard(gyrosaur.CharacterCard, wreck);
            AssertNumberOfStatusEffectsInPlay(1);

            DecisionSelectFunction = 1;

            Card chase = PutOnDeck("AMerryChase");

            //not if damage prevented
            DealDamage(gyrosaur, baron, 1, DTM);
            AssertNumberOfStatusEffectsInPlay(1);
            AssertUnderCard(gyrosaur.CharacterCard, wreck);

            DealDamage(gyrosaur, ra, 1, DTM);
            AssertInHand(wreck);
            AssertNumberOfStatusEffectsInPlay(0);

            AssertOnTopOfDeck(chase);
        }
        [Test]
        public void TestCaptainPowerForcedPlayIfCannotDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/CaptainGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();

            Card chase = PutOnDeck("AMerryChase");

            UsePower(gyrosaur);
            AssertUnderCard(gyrosaur.CharacterCard, chase);
            AssertNumberOfStatusEffectsInPlay(1);

            AssertNoDecision();
            Card wreck = PutOnDeck("WreckingBall");
            PlayCard("TrafficPileup"); // Prevent hero card play

            DealDamage(gyrosaur, ra, 1, DTM);
            AssertIsInPlay(chase);
            AssertNumberOfStatusEffectsInPlay(0);

            AssertOnTopOfDeck(wreck);
        }
        [Test]
        public void TestCaptainPowerForcedDrawIfCannotPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/CaptainGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();

            Card chase = PutOnDeck("AMerryChase");

            UsePower(gyrosaur);
            AssertUnderCard(gyrosaur.CharacterCard, chase);
            AssertNumberOfStatusEffectsInPlay(1);

            AssertNoDecision();
            Card wreck = PutOnDeck("WreckingBall");
            PlayCard("HostageSituation"); // Prevent hero card play

            DealDamage(gyrosaur, ra, 1, DTM);
            AssertInHand(chase);
            AssertNumberOfStatusEffectsInPlay(0);

            AssertOnTopOfDeck(wreck);
        }
        [Test]
        public void TestCaptainPowerWaitsIfCannotDrawOrPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/CaptainGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();

            Card chase = PutOnDeck("AMerryChase");

            UsePower(gyrosaur);
            AssertUnderCard(gyrosaur.CharacterCard, chase);
            AssertNumberOfStatusEffectsInPlay(1);

            AssertNoDecision();
            Card wreck = PutOnDeck("WreckingBall");
            PlayCard("HostageSituation"); // Prevent hero card play
            PlayCard("TrafficPileup"); // Prevent hero card draw

            DealDamage(gyrosaur, ra, 1, DTM);
            AssertUnderCard(gyrosaur.CharacterCard, chase);
            AssertNumberOfStatusEffectsInPlay(1);

            AssertOnTopOfDeck(wreck);
        }
        [Test]
        public void TestCaptainPowerStacks()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/CaptainGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();

            Card chase = PutOnDeck("AMerryChase");
            UsePower(gyrosaur);
            Card shell = PutOnDeck("RapturianShell");
            UsePower(gyrosaur);

            AssertUnderCard(gyrosaur.CharacterCard, shell);
            AssertUnderCard(gyrosaur.CharacterCard, chase);
            AssertNumberOfStatusEffectsInPlay(2);

            DecisionSelectFunction = 0;

            DealDamage(gyrosaur, ra, 1, DTM);

            AssertIsInPlay(chase, shell);
            AssertNumberOfStatusEffectsInPlay(0);
        }
        [Test]
        public void TestCaptainPowerDrawIsActualDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/CaptainGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();

            MoveAllCardsFromHandToDeck(gyrosaur);
            Card wreck = PutOnDeck("WreckingBall");
            UsePower(gyrosaur);

            DecisionSelectFunction = 1;
            Card spin = PlayCard("Hyperspin");

            DealDamage(gyrosaur, ra, 1, DTM);
            AssertIsInPlay(wreck);
            AssertInTrash(spin);
            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test]
        public void TestCaptainGyrosaurPowerUnderCardsWithEndOfDays()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/CaptainGyrosaurCharacter", "Legacy", "Fanatic", "Megalopolis");
            StartGame();

            MoveAllCardsFromHandToDeck(gyrosaur);
            Card ricochet = PutOnDeck("Ricochet");
            Card wreck = PutOnDeck("WreckingBall");
            UsePower(gyrosaur);
            AssertUnderCard(gyrosaur.CharacterCard, wreck);
            UsePower(gyrosaur);
            AssertUnderCard(gyrosaur.CharacterCard, ricochet);
            AssertNumberOfStatusEffectsInPlay(2);

            PutIntoPlay("LeadFromTheFront");
            PlayCard("EndOfDays");
            GoToStartOfTurn(base.env);
            AssertInTrash(gyrosaur, new List<Card> { wreck, ricochet });
            AssertNumberOfStatusEffectsInPlay(0);
        }
        #endregion Test Innate Power

        #region Test Incap Power
        /* 
         * One player may draw a card now.
         * One hero may use a power now. 
         * Select a target. Increase the next damage dealt to it by 2.
         */
        [Test]
        public void TestCaptainIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/CaptainGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            DealDamage(baron, gyrosaur, 50, DTM);

            AssertIncapLetsHeroDrawCard(gyrosaur, 0, ra, 1);
        }
        [Test]
        public void TestCaptainIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/CaptainGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            DealDamage(baron, gyrosaur, 50, DTM);

            AssertIncapLetsHeroUsePower(gyrosaur, 1, ra);
        }
        [Test]
        public void TestCaptainIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur/CaptainGyrosaurCharacter", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            DealDamage(baron, gyrosaur, 50, DTM);

            UseIncapacitatedAbility(gyrosaur, 2);
            Card redist = PlayCard("ElementalRedistributor");

            QuickHPStorage(baron, legacy, ra);
            AssertNumberOfStatusEffectsInPlay(1);
            DealDamage(ra, redist, 1, DTM);
            AssertHitPoints(redist, 9);

            DealDamage(baron, ra, 1, DTM);
            DealDamage(ra, baron, 1, DTM);
            QuickHPCheck(-3, 0, -1);

            AssertNumberOfStatusEffectsInPlay(0);
            DealDamage(ra, baron, 1, DTM);
            QuickHPCheck(-1, 0, 0);
        }
        #endregion Test Incap Power

        #endregion Captain Gyrosaur
    }
}
