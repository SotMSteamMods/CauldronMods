using System;
using System.Linq;

using Cauldron.DocHavoc;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture()]
    public class DocHavocTests : BaseTest
    {
        protected HeroTurnTakerController DocHavoc => FindHero("DocHavoc");


        [Test]
        public void TestDocHavocLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(DocHavoc);
            Assert.IsInstanceOf(typeof(DocHavocCharacterCardController), DocHavoc.CharacterCardController);

            Assert.AreEqual(30, DocHavoc.CharacterCard.HitPoints);
        }


        [Test]
        public void TestDocHavocInnatePowerSuccess()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Ra", "Megalopolis");
            StartGame();

            GoToUsePowerPhase(DocHavoc);
            DecisionSelectTarget = ra.CharacterCard;
            DecisionSelectTurnTaker = ra.HeroTurnTaker;

            QuickHandStorage(ra);
            QuickHPStorage(ra);

            // Act
            UsePower(DocHavoc.CharacterCard);

            // Assert
            QuickHPCheck(-3);
            QuickHandCheck(1);

        }

        [Test]
        public void TestDocHavocInnatePowerFail()
        {
            // Arrange
            SetupGameController("BaronBlade", "Legacy", "Cauldron.DocHavoc", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(legacy);
            PutInHand("NextEvolution");
            Card nextEvolution = GetCardFromHand("NextEvolution");

            PlayCard(nextEvolution);
            GoToUsePowerPhase(legacy);
            DecisionSelectDamageType = DamageType.Toxic;
            UsePower(nextEvolution);

            GoToUsePowerPhase(DocHavoc);
            DecisionSelectTarget = legacy.CharacterCard;
            DecisionSelectTurnTaker = legacy.HeroTurnTaker;

            QuickHandStorage(legacy);
            QuickHPStorage(legacy);

            // Act
            UsePower(DocHavoc.CharacterCard);

            // Assert
            QuickHPCheck(0); // Legacy immune to toxic, no change to HP
            QuickHandCheck(0); // Hand size should still be 4 as Legacy could only draw a card if he sustained damage
        }

        [Test]
        public void TestIncapacitateOption1()
        {
            // Arrange
            SetupGameController("BaronBlade", "Legacy", "Ra", "Cauldron.DocHavoc", "Megalopolis");
            StartGame();

            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            DealDamage(baron, legacy, 2, DamageType.Melee);
            DealDamage(baron, ra, 2, DamageType.Melee);

            Card[] healTargets = new Card[] { legacy.CharacterCard, ra.CharacterCard };
            QuickHPStorage(healTargets);
            QuickHPUpdate();

            DecisionSelectCards = healTargets;
            GoToUseIncapacitatedAbilityPhase(DocHavoc);

            // Act
            UseIncapacitatedAbility(DocHavoc, 0);


            // Assert
            AssertIncapacitated(DocHavoc);
            QuickHPCheck(1, 1);

        }

        [Test]
        public void TestIncapacitateOption2()
        {
            // Arrange
            SetupGameController("BaronBlade", "Legacy", "Ra", "Cauldron.DocHavoc", "Megalopolis");
            StartGame();

            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            DecisionSelectTarget = legacy.CharacterCard;

            QuickHandStorage(legacy);

            // Act
            UseIncapacitatedAbility(DocHavoc, 1);


            // Assert
            AssertIncapacitated(DocHavoc);
            QuickHandCheck(1);

        }

        [Test]
        public void TestIncapacitateOption3()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Legacy", "Ra", "MobileDefensePlatform");
            StartGame();

            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            QuickHPStorage(new[] { legacy.CharacterCard, ra.CharacterCard });

            // Act
            GoToUseIncapacitatedAbilityPhase(DocHavoc);
            UseIncapacitatedAbility(DocHavoc, 2);
            
            GoToStartOfTurn(env);

            PutIntoPlay("BattalionGunner"); // End of env. each hero target takes 1 damage

            GoToEndOfTurn(env);

            // Assert
            AssertIncapacitated(DocHavoc);
            QuickHPCheck(0, 0);
        }

        [Test]
        public void TestDocsFlask()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Ra", "Megalopolis");
            StartGame();

            // Reduce Ra's, Havoc's  HP by 2
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);
            DealDamage(baron, ra, 2, DamageType.Melee);
            QuickHPStorage(ra, DocHavoc);

            // Act
            PutInHand(DocsFlaskCardController.Identifier);
            Card docsFlask = GetCardFromHand(DocsFlaskCardController.Identifier);
            //GoToPlayCardPhase(DocHavoc);
            PlayCard(docsFlask);

            DecisionSelectCard = ra.CharacterCard;

            // Procs flask to heal Ra +1 HP
            GoToStartOfTurn(DocHavoc);

            // Now use power portion of Doc's Flask
            GoToUsePowerPhase(DocHavoc);
            UsePower(docsFlask);

            // Assert
            AssertTriggersWhere((Func<ITrigger, bool>)(t => t.Types.Contains(TriggerType.GainHP)));
            QuickHPCheck(2, 1);
        }

    }
}
