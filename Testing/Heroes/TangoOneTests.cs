using System;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;

using Cauldron.TangoOne;

using NUnit.Framework;


namespace CauldronTests
{
    [TestFixture()]
    public class TangoOneTests : BaseTest
    {
        protected HeroTurnTakerController TangoOne => FindHero("TangoOne");

        [Test]
        public void TestTangoOneLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(TangoOne);
            Assert.IsInstanceOf(typeof(TangoOneCharacterCardController), TangoOne.CharacterCardController);

            Assert.AreEqual(24, TangoOne.CharacterCard.HitPoints);
        }

        [Test]
        public void TestInnatePower()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            DecisionSelectTarget = mdp;

            // Act
            GoToStartOfTurn(TangoOne);
            UsePower(TangoOne);

            // Assert
            QuickHPCheck(-1);
        }

        [Test]
        public void TestChameleonArmorDiscardCriticalCardSuccess()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();
            PutOnDeck(TangoOne, GetCard(DamnGoodGroundCardController.Identifier));

            DecisionYesNo = true;
            QuickHPStorage(TangoOne);

            // Act
            GoToStartOfTurn(TangoOne);
            PutInHand(ChameleonArmorCardController.Identifier);
            PlayCard(ChameleonArmorCardController.Identifier);

            DealDamage(baron, TangoOne, 5, DamageType.Cold);

            // Assert
            QuickHPCheck(0);
        }

        [Test]
        public void TestChameleonArmorDiscardCriticalCardFail()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();
            PutOnDeck(TangoOne, GetCard(FarsightCardController.Identifier));

            DecisionYesNo = true;
            QuickHPStorage(TangoOne);

            // Act
            GoToStartOfTurn(TangoOne);
            PutInHand(ChameleonArmorCardController.Identifier);
            PlayCard(ChameleonArmorCardController.Identifier);

            DealDamage(baron, TangoOne, 5, DamageType.Cold);

            // Assert
            QuickHPCheck(-5);
        }

        [Test]
        public void TestChameleonArmorDontDiscard()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();

            DecisionYesNo = false;
            QuickHPStorage(TangoOne);

            // Act
            GoToStartOfTurn(TangoOne);
            PutInHand(ChameleonArmorCardController.Identifier);
            PlayCard(ChameleonArmorCardController.Identifier);

            DealDamage(baron, TangoOne, 5, DamageType.Cold);

            // Assert
            QuickHPCheck(-5);
        }

        [Test]
        public void TestCriticalHit()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();

            PutOnDeck(TangoOne, GetCard(DamnGoodGroundCardController.Identifier));

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            DecisionYesNo = true;
            DecisionSelectTarget = mdp;


            // Act
            GoToPlayCardPhase(TangoOne);
            PlayCard(CriticalHitCardController.Identifier);
            UsePower(TangoOne);


            // Assert
        }

    }
}
