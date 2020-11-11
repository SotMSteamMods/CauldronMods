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
        protected TurnTakerController Anathema => FindVillain("Anathema");

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
        public void TestCriticalHitDiscardCriticalCardSuccess()
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
            QuickHPCheck(-4);
        }

        [Test]
        public void TestCriticalHitDiscardCriticalCardFail()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();

            PutOnDeck(TangoOne, GetCard(FarsightCardController.Identifier));

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            DecisionYesNo = true;
            DecisionSelectTarget = mdp;


            // Act
            GoToPlayCardPhase(TangoOne);
            PlayCard(CriticalHitCardController.Identifier);
            UsePower(TangoOne);


            // Assert
            QuickHPCheck(-1);
        }

        [Test]
        public void TestCriticalHitDontDiscard()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();

            PutOnDeck(TangoOne, GetCard(FarsightCardController.Identifier));

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            DecisionYesNo = false;
            DecisionSelectTarget = mdp;


            // Act
            GoToPlayCardPhase(TangoOne);
            PlayCard(CriticalHitCardController.Identifier);
            UsePower(TangoOne);


            // Assert
            QuickHPCheck(-1);
        }

        /*
        [Test]
        public void TestCriticalHitWithNemesisDiscardCriticalCardSuccess()
        {
            // Arrange
            SetupGameController("Cauldron.Anathema", "Cauldron.TangoOne", "Ra", "Megalopolis");


            StartGame();
            DecisionSelectCard = ra.CharacterCard;

            PutOnDeck(TangoOne, GetCard(DamnGoodGroundCardController.Identifier));

            QuickHPStorage(Anathema);

            DecisionYesNo = true;
            DecisionSelectTarget = Anathema.CharacterCard;


            // Act
            GoToPlayCardPhase(TangoOne);
            PlayCard(CriticalHitCardController.Identifier);
            UsePower(TangoOne);

            // Assert
            //QuickHPCheck(-4);
        }
        */

        [Test]
        public void TestDamnGoodGround()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();

            DealDamage(baron, TangoOne, 5, DamageType.Cold);

            Card mdp = FindCardInPlay("MobileDefensePlatform");
            DecisionSelectTargets = new[] {baron.CharacterCard, mdp, null};
            QuickHPStorage(baron.CharacterCard, mdp, TangoOne.CharacterCard);

            // Act
            GoToStartOfTurn(TangoOne);
            PutInHand(DamnGoodGroundCardController.Identifier);
            PlayCard(DamnGoodGroundCardController.Identifier);

            // Assert
            QuickHPCheck(0, -1, 2);
        }

        [Test]
        public void TestDisablingShot()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();

            PlayCard(baron, "BacklashField");
            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectCard = FindCardInPlay("BacklashField");
            DecisionSelectTarget = mdp;
            //DecisionSelectTargets = new[] {mdp};
            QuickHPStorage(mdp);

            // Act
            GoToStartOfTurn(TangoOne);
            PutInHand(DisablingShotCardController.Identifier);
            PlayCard(DisablingShotCardController.Identifier);

            // Assert
            AssertNotInPlay(new []{ "BacklashField"});
            QuickHPCheck(-2);
        }

        [Test]
        public void TestFarsight()
        {
            // Arrange
            SetupGameController("GloomWeaver", "Cauldron.TangoOne", "Ra", "Megalopolis");
            StartGame();

            PlayCard("StrengthOfTheGrave"); // Reduce damage to zombies by 1
            
            Card zombie = FindCardInPlay("ZombieServant");
            DecisionSelectTarget = zombie;
            QuickHPStorage(zombie);

            // Act
            GoToStartOfTurn(TangoOne);
            PutInHand(FarsightCardController.Identifier);
            PlayCard(FarsightCardController.Identifier);

            UsePower(TangoOne);

            // Assert
            QuickHPCheck(-1); // Farsight allows Innate Power to deal 1 damage
        }

        [Test]
        public void TestGhostReactor()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                GhostReactorCardController.Identifier
            });

            StartGame();

            Card mdp = FindCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;
            QuickHPStorage(TangoOne.CharacterCard, mdp);
            QuickHandStorage(TangoOne);

            // Act
            GoToStartOfTurn(TangoOne);
            PlayCardFromHand(TangoOne, GhostReactorCardController.Identifier);

            UsePower(GhostReactorCardController.Identifier);

            DealDamage(baron, TangoOne, 10, DamageType.Psychic);
            DealDamage(TangoOne, mdp, 1, DamageType.Projectile); // With +2 increase from Ghost Reactor
            DealDamage(TangoOne, mdp, 1, DamageType.Projectile); // +1, Ghost Reactor effect expired

            // Assert
            QuickHPCheck(0, -4); // Tango One (0) due to Ghost Reactor immunity, MDP -4 
            QuickHandCheck(0); // Discard Ghost Reactor (-1), Draw a card (+1)
        }

        [Test]
        public void TestInfiltrate()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.TangoOne", "Ra", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                InfiltrateCardController.Identifier, GhostReactorCardController.Identifier
            });

            DecisionSelectLocation = new LocationChoice(TangoOne.HeroTurnTaker.Deck);
            DecisionSelectCardsIndex = 2;

            // Act
            GoToStartOfTurn(TangoOne);
            PlayCardFromHand(TangoOne, InfiltrateCardController.Identifier);


        }

    }
}
