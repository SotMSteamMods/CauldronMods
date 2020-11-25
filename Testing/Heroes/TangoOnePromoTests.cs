using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cauldron.TangoOne;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture]
    public class TangoOnePromoTests : BaseTest
    {
        protected HeroTurnTakerController TangoOne => FindHero("TangoOne");

        private const string DeckNamespaceGhostOps = "Cauldron.TangoOne/GhostOpsTangoOneCharacter";
        private const string DeckNamespace1929 = "Cauldron.TangoOne/PastTangoOneCharacter";

        [Test]
        public void TestTangoOneGhostOpsLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", DeckNamespaceGhostOps, "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(TangoOne);
            Assert.IsInstanceOf(typeof(GhostOpsTangoOneCharacterCardController), TangoOne.CharacterCardController);

            Assert.AreEqual(28, TangoOne.CharacterCard.HitPoints);
        }

        [Test]
        public void TestInnatePowerGhostOps()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceGhostOps, "Ra", "Legacy", "Megalopolis");

            MakeCustomHeroHand(TangoOne, new List<string>()
            {
                FarsightCardController.Identifier, CriticalHitCardController.Identifier,
                GhostReactorCardController.Identifier, OneShotOneKillCardController.Identifier
            });

            StartGame();
            QuickHandStorage(TangoOne);

            Card ghostReactor = GetCardFromHand(GhostReactorCardController.Identifier);
            Card farsight = GetCardFromHand(FarsightCardController.Identifier);
            DecisionSelectCards = new[]
            {
                farsight,
                ghostReactor
            };

            // Act
            GoToStartOfTurn(TangoOne);
            UsePower(TangoOne);

            // Assert
            AssertOnTopOfDeck(farsight);
            QuickHandCheck(0);
        }

        [Test]
        public void TestGhostOpsIncapacitateOption1()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceGhostOps, "Ra", "Legacy", "Megalopolis");

            StartGame();
            QuickHandStorage(ra);

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);

            DecisionSelectTarget = ra.CharacterCard;

            // Act
            GoToUseIncapacitatedAbilityPhase(TangoOne);
            UseIncapacitatedAbility(TangoOne, 0);


            // Assert
            AssertIncapacitated(TangoOne);
            QuickHandCheck(1);

        }

        [Test]
        public void TestGhostOpsIncapacitateOption2()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceGhostOps, "Ra", "Legacy", "Megalopolis");

            StartGame();
            QuickHandStorage(ra);

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);

            DecisionSelectTarget = ra.CharacterCard;

            // Act
            GoToUseIncapacitatedAbilityPhase(TangoOne);
            UseIncapacitatedAbility(TangoOne, 1);

            Assert.NotNull(this.GameController.StatusEffectControllers
                .FirstOrDefault(s => s.StatusEffect.ToString().Equals("damage dealt is irreducible.")
                                     && s.StatusEffect.CardSource.Equals(TangoOne.CharacterCard)));

            GoToStartOfTurn(TangoOne);

            // Effect expired
            Assert.Null(this.GameController.StatusEffectControllers
                .FirstOrDefault(s => s.StatusEffect.ToString().Equals("damage dealt is irreducible.")
                                     && s.StatusEffect.CardSource.Equals(TangoOne.CharacterCard)));

            AssertIncapacitated(TangoOne);

        }

        [Test]
        public void TestGhostOpsIncapacitateOption3()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceGhostOps, "Ra", "Legacy", "Megalopolis");

            StartGame();
            QuickHandStorage(ra);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);

            DecisionSelectTarget = mdp;

            // Act
            GoToUseIncapacitatedAbilityPhase(TangoOne);
            UseIncapacitatedAbility(TangoOne, 2);

            GoToUsePowerPhase(ra);
            UsePower(ra);

            QuickHPCheck(-4); // Ra's Pyre 2 dmg + 2 from Tango's Incap #3

            QuickHPStorage(mdp);
            UsePower(ra);
            QuickHPCheck(-2); // Ra's Pyre 2 dmg, Incap expired
            AssertIncapacitated(TangoOne);

        }

        [Test]
        public void TestTangoOne1929Loads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", DeckNamespace1929, "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(TangoOne);
            Assert.IsInstanceOf(typeof(PastTangoOneCharacterCardController), TangoOne.CharacterCardController);

            Assert.AreEqual(24, TangoOne.CharacterCard.HitPoints);
        }

        [Test]
        public void TestInnatePower1929()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace1929, "Ra", "Legacy", "Megalopolis");

            GoToStartOfTurn(TangoOne);
            
            GoToUsePowerPhase(TangoOne);
            UsePower(TangoOne);

            GoToDrawCardPhase(TangoOne);
            

            GoToEndOfTurn(TangoOne);
            GoToStartOfTurn(TangoOne);



            Assert.True(false, "TODO");
        }

        [Test]
        public void Test1929IncapacitateOption1()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace1929, "Ra", "Legacy", "Megalopolis");

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);


            // Assert
            AssertIncapacitated(TangoOne);
            Assert.True(false, "TODO");
        }

        [Test]
        public void Test1929IncapacitateOption2()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace1929, "Ra", "Legacy", "Megalopolis");

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);


            // Assert
            AssertIncapacitated(TangoOne);
            Assert.True(false, "TODO");
        }

        [Test]
        public void Test1929IncapacitateOption3()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace1929, "Ra", "Legacy", "Megalopolis");

            SetHitPoints(TangoOne.CharacterCard, 1);
            DealDamage(baron, TangoOne, 2, DamageType.Melee);


            // Assert
            AssertIncapacitated(TangoOne);
            Assert.True(false, "TODO");
        }
    }
}
