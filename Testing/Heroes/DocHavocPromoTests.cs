using System;
using System.Collections.Generic;
using System.Linq;
using Cauldron.DocHavoc;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;

using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture]
    public class DocHavocPromoTests : BaseTest
    {
        protected HeroTurnTakerController DocHavoc => FindHero("DocHavoc");

        private const string DeckNamespaceFirstResponse = "Cauldron.DocHavoc/FirstResponseDocHavocCharacter";

        [Test]
        public void TestDocHavocFirstResponseLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", DeckNamespaceFirstResponse, "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(DocHavoc);
            Assert.IsInstanceOf(typeof(FirstResponseDocHavocCharacterCardController), DocHavoc.CharacterCardController);

            Assert.AreEqual(32, DocHavoc.CharacterCard.HitPoints);
        }

        [Test]
        public void TestInnatePowerFirstResponse()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceFirstResponse, "Ra", "Legacy", "Megalopolis");

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);
            QuickHandStorage(DocHavoc);
            DecisionYesNo = true;
            DecisionSelectTarget = mdp;

            // Act
            GoToUsePowerPhase(DocHavoc);
            UsePower(DocHavoc);

            // Assert
            QuickHPCheck(-1);
            QuickHandCheck(1);
        }

        [Test]
        public void TestInnatePowerFirstResponseNoDamage()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceFirstResponse, "Ra", "Legacy", "Megalopolis");

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);
            QuickHandStorage(DocHavoc);
            DecisionYesNo = false;

            // Act
            GoToUsePowerPhase(DocHavoc);
            UsePower(DocHavoc);

            // Assert
            QuickHPCheck(0);
            QuickHandCheck(1);
        }

        [Test]
        public void TestFirstResponseIncapacitateOption1()
        {
            Assert.True(false, "TODO");
        }

        [Test]
        public void TestFirstResponseIncapacitateOption2()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceFirstResponse, "Ra", "Legacy", "InsulaPrimalis");

            Card dangerSense = GetCard("DangerSense");
            Card thokk = GetCard("Thokk");

            PutInTrash(legacy, dangerSense);
            PutInTrash(legacy, thokk);

            StartGame();

            DecisionSelectCard = legacy.CharacterCard;

            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            // Act
            GoToUseIncapacitatedAbilityPhase(DocHavoc);
            UseIncapacitatedAbility(DocHavoc, 1);

            // Assert

            Assert.True(false, "TODO");
        }

        [Test]
        public void TestFirstResponseIncapacitateOption3_Destroy2Cards()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceFirstResponse, "Ra", "Legacy", "InsulaPrimalis");

            StartGame();

            PutIntoPlay("EnragedTRex");
            Card enragedTRex = GetCardInPlay("EnragedTRex");

            PutIntoPlay("ObsidianField");
            Card obsidianField = GetCardInPlay("ObsidianField");

            DecisionSelectCards = new[] {enragedTRex, obsidianField};

            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            // Act
            GoToUseIncapacitatedAbilityPhase(DocHavoc);
            UseIncapacitatedAbility(DocHavoc, 2);

            // Assert
            AssertIncapacitated(DocHavoc);
            AssertInTrash(enragedTRex);
            AssertInTrash(obsidianField);
        }

        [Test]
        public void TestFirstResponseIncapacitateOption3_Destroy1Card()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceFirstResponse, "Ra", "Legacy", "InsulaPrimalis");

            StartGame();

            PutIntoPlay("EnragedTRex");
            Card enragedTRex = GetCardInPlay("EnragedTRex");

            DecisionSelectCard = enragedTRex;

            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            // Act
            GoToUseIncapacitatedAbilityPhase(DocHavoc);
            UseIncapacitatedAbility(DocHavoc, 2);

            // Assert
            AssertIncapacitated(DocHavoc);
            AssertInTrash(enragedTRex);
        }

        [Test]
        public void TestFirstResponseIncapacitateOption3_NoEligibleCards()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceFirstResponse, "Ra", "Legacy", "InsulaPrimalis");

            StartGame();

            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            // Act
            GoToUseIncapacitatedAbilityPhase(DocHavoc);
            UseIncapacitatedAbility(DocHavoc, 2);

            // Assert
            AssertIncapacitated(DocHavoc);
            AssertNumberOfCardsInTrash(env, 0);
        }
    }
}
