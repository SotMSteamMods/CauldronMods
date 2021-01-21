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
