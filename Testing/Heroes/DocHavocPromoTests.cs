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
        public void TestInnatePowerFirstResponseIsFromHavocCard()
        {
            SetupGameController("BaronBlade", DeckNamespaceFirstResponse, "Ra", "Legacy", "Megalopolis");

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);
            QuickHandStorage(DocHavoc);
            DecisionYesNo = true;
            DecisionSelectTarget = mdp;
            UsePower(legacy);
            // Act
            GoToUsePowerPhase(DocHavoc);
            UsePower(DocHavoc);

            // Assert
            QuickHPCheck(-2);
            QuickHandCheck(1);
        }
        [Test]
        public void TestInnatePowerFirstResponseIsStandardMayDamage()
        {
            SetupGameController("BaronBlade", DeckNamespaceFirstResponse, "Ra", "Legacy", "Megalopolis");

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);
            QuickHandStorage(DocHavoc);
            DecisionYesNo = false;
            DecisionSelectTargets = new Card[] { mdp, mdp, null };
            // Act
            GoToUsePowerPhase(DocHavoc);
            UsePower(DocHavoc);
            UsePower(DocHavoc);
            UsePower(DocHavoc);

            // Assert
            QuickHPCheck(-2);
            QuickHandCheck(3);
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
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceFirstResponse, "Legacy", "Ra", "InsulaPrimalis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            SetHitPoints(mdp, 5);

            SetHitPoints(ra.CharacterCard, 5);
            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, mdp);

            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            // Act
            GoToUseIncapacitatedAbilityPhase(DocHavoc);
            UseIncapacitatedAbility(DocHavoc, 0);

            GoToStartOfTurn(legacy);
            DecisionSelectTarget = mdp;
            PlayCard(GetCard("Thokk"));

            DealDamage(baron, legacy, 2, DamageType.Cold);
            DealDamage(baron, ra, 2, DamageType.Cold);

            QuickHPCheck(-3, -1, -2); // No reduction applied to Legacy
            QuickHPStorage(ra.CharacterCard, mdp);

            // Loop back around to make sure the effect is gone
            GoToUseIncapacitatedAbilityPhase(DocHavoc);

            DealDamage(baron, mdp, 2, DamageType.Cold);
            DealDamage(baron, ra, 2, DamageType.Cold);
            QuickHPCheck(-2, -2); // Doc Havoc's incap expired, full damage done to Ra, MDP


            // Assert
            AssertIncapacitated(DocHavoc);
        }

        [Test]
        [Ignore("This does not check a dynamic HP amount, just those 6 or below at time of execution")]
        public void TestFirstResponseIncapacitateOption1DynamicHPCheck()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceFirstResponse, "Legacy", "Ra", "InsulaPrimalis");
            StartGame();

            SetHitPoints(ra, 7);

            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            // Act
            GoToUseIncapacitatedAbilityPhase(DocHavoc);
            UseIncapacitatedAbility(DocHavoc, 0);

            QuickHPStorage(ra);
            DealDamage(baron, ra, 2, DamageType.Cold);
            QuickHPCheck(-2);
            QuickHPStorage(ra);
            DealDamage(baron, ra, 2, DamageType.Cold);
            QuickHPCheck(-1);
        }

        [Test]
        public void TestFirstResponseIncapacitateOption2()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceFirstResponse, "Ra", "Legacy", "InsulaPrimalis");

            Card dangerSense = GetCard("DangerSense");
            Card thokk = GetCard("Thokk");
            Card bolsterAllies = GetCard("BolsterAllies");

            PutInTrash(legacy, dangerSense);
            PutInTrash(legacy, thokk);
            PutInTrash(legacy, bolsterAllies);

            Card fireBlast = GetCard("FireBlast");
            PutInTrash(ra, fireBlast);

            StartGame();
            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionSelectCard = thokk;

            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            // Act
            GoToUseIncapacitatedAbilityPhase(DocHavoc);
            UseIncapacitatedAbility(DocHavoc, 1);

            // Assert
            AssertIncapacitated(DocHavoc);
            AssertNotInTrash(legacy, thokk.Identifier);
            AssertOnTopOfDeck(legacy, thokk);
            AssertInTrash(fireBlast);
        }

        [Test]
        public void TestFirstResponseIncapacitateOption2_EmptyTrashes()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceFirstResponse, "Ra", "Legacy", "InsulaPrimalis");

            StartGame();
            QuickTopCardStorage(legacy, ra);

            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            // Act
            GoToUseIncapacitatedAbilityPhase(DocHavoc);
            UseIncapacitatedAbility(DocHavoc, 1);

            // Assert
            AssertIncapacitated(DocHavoc);
            QuickTopCardCheck(ttc => ttc.CharacterCard.Owner.Deck);
        }

        [Test]
        public void TestFirstResponseIncapacitateOption2_NoOneShots()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespaceFirstResponse, "Ra", "Legacy", "InsulaPrimalis");

            Card dangerSense = GetCard("DangerSense");
            Card fortitude = GetCard("Fortitude");

            PutInTrash(legacy, dangerSense);
            PutInTrash(legacy, fortitude);

            Card solarFlare = GetCard("SolarFlare");
            PutInTrash(ra, solarFlare);

            StartGame();
            QuickTopCardStorage(legacy, ra);
            DecisionSelectTurnTaker = legacy.TurnTaker;

            SetHitPoints(DocHavoc.CharacterCard, 1);
            DealDamage(baron, DocHavoc, 2, DamageType.Melee);

            // Act
            GoToUseIncapacitatedAbilityPhase(DocHavoc);
            UseIncapacitatedAbility(DocHavoc, 1);

            // Assert
            AssertIncapacitated(DocHavoc);
            AssertInTrash(dangerSense);
            AssertInTrash(fortitude);
            AssertInTrash(solarFlare);
            QuickTopCardCheck(ttc => ttc.CharacterCard.Owner.Deck);
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

            DecisionSelectCards = new[] { enragedTRex, obsidianField };

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
        [Test]
        public void TestFutureDocHavocLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc/FutureDocHavocCharacter", "Megalopolis");
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(DocHavoc);
            Assert.IsInstanceOf(typeof(FutureDocHavocCharacterCardController), DocHavoc.CharacterCardController);

            Assert.AreEqual(29, DocHavoc.CharacterCard.HitPoints);
        }
        [Test]
        public void TestFuturePowerNoHPGainers()
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc/FutureDocHavocCharacter", "Ra", "Legacy", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            QuickHPStorage(baron);
            AssertMaxNumberOfDecisions(1);
            UsePower(DocHavoc);
            QuickHPCheck(-2);
        }
        [Test]
        public void TestFuturePowerOneHPGainer()
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc/FutureDocHavocCharacter", "Ra", "Legacy", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            SetHitPoints(ra, 20);
            PlayCard("TheStaffOfRa");
            DecisionSelectFunction = 1;

            //damage or grant power, innate or staff, who to hit
            AssertMaxNumberOfDecisions(3);

            QuickHPStorage(baron);
            UsePower(DocHavoc);
            QuickHPCheck(-3);
        }
        [Test]
        public void TestFutureIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc/FutureDocHavocCharacter", "Ra", "Legacy", "Megalopolis");
            StartGame();
            DealDamage(baron, DocHavoc, 50, DamageType.Melee);

            AssertIncapLetsHeroDrawCard(DocHavoc, 0, ra, 1);
        }
        [Test]
        public void TestFutureIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc/FutureDocHavocCharacter", "Ra", "Legacy", "Megalopolis");
            StartGame();
            DealDamage(baron, DocHavoc, 50, DamageType.Melee);

            Card flesh = PlayCard("FleshOfTheSunGod");
            Card fort = PlayCard("Fortitude");
            Card ring = PlayCard("TheLegacyRing");
            Card lash = PlayCard("BacklashField");
            AssertNextDecisionChoices(new Card[] { flesh, fort, lash }, new Card[] { ring });

            DecisionSelectCard = lash;
            UseIncapacitatedAbility(DocHavoc, 1);
            AssertInTrash(lash);
        }
        [Test]
        public void TestFutureIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.DocHavoc/FutureDocHavocCharacter", "Ra", "Legacy", "Megalopolis");
            StartGame();
            DealDamage(baron, DocHavoc, 50, DamageType.Melee);

            SetHitPoints(ra, 20);
            QuickHPStorage(ra);
            UseIncapacitatedAbility(DocHavoc, 2);
            Card staff = PlayCard("TheStaffOfRa");
            QuickHPCheck(5);

            //only once
            DestroyCard(staff);
            PlayCard(staff);
            QuickHPCheck(3);
        }
    }
}
