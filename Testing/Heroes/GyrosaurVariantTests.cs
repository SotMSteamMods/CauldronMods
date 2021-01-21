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
