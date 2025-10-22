using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Cypher;

namespace CauldronTests
{
    [TestFixture]
    public class CypherTests : CauldronBaseTest
    {

        private const string DeckNamespace = "Cauldron.Cypher";

        [Test]
        public void TestCypherLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", DeckNamespace, "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(cypher);
            Assert.IsInstanceOf(typeof(CypherCharacterCardController), cypher.CharacterCardController);

            Assert.AreEqual(26, cypher.CharacterCard.HitPoints);
        }

        [Test]
        public void TestCypherDecklist()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            AssertHasKeyword("augment", new[]
            {
                "DermalAug",
                "FusionAug",
                "MuscleAug",
                "RetinalAug",
                "VascularAug"
            });

            AssertHasKeyword("ongoing", new[]
            {
                "BackupPlan",
                "ElectroOpticalCloak",
                "HackingProgram",
                "NeuralInterface"
            });

            AssertHasKeyword("equipment", new[]
            {
                "DermalAug",
                "FusionAug",
                "MuscleAug",
                "RetinalAug",
                "VascularAug"
            });

            AssertHasKeyword("limited", new[]
            {
                "BackupPlan",
                "ElectroOpticalCloak",
                "HackingProgram",
                "NeuralInterface"
            });

            AssertHasKeyword("one-shot", new[]
            {
                "Cyberdefense",
                "Cyberintegration",
                "CyborgBlaster",
                "CyborgPunch",
                "HeuristicAlgorithm",
                "InitiatedUpgrade",
                "NaniteSurge",
                "NetworkedAttack",
                "RapidPrototyping",
                "RebuiltToSucceed"
            });
        }

        [Test]
        public void TestInnatePower()
        {
            // One augmented hero may draw a card now.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card muscleAug = GetCard(MuscleAugCardController.Identifier);
            Card dermalAug = GetCard(DermalAugCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { muscleAug }},
                { tachyon.CharacterCard, new List<Card>() { dermalAug }},
            }); ;

            QuickHandStorage(tachyon);
            DecisionSelectTurnTaker = tachyon.TurnTaker;

            GoToUsePowerPhase(cypher);
            UsePower(cypher);

            // Assert
            QuickHandCheck(1);
            Assert.AreEqual(2, GetAugmentsInPlay().Count);
            Assert.True(AreAugmented(new List<Card>() { ra.CharacterCard, tachyon.CharacterCard }));
        }

        [Test]
        public void TestIncapacitateOption1()
        {
            //One player may destroy 1 of their ongoing cards to draw 3 cards.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card fleshOfTheSunGod = GetCard("FleshOfTheSunGod");
            PlayCard(fleshOfTheSunGod);

            SetHitPoints(cypher.CharacterCard, 1);
            DealDamage(baron, cypher, 2, DamageType.Melee);

            QuickHandStorage(ra);

            // Act
            GoToUseIncapacitatedAbilityPhase(cypher);
            UseIncapacitatedAbility(cypher, 0);

            // Assert
            AssertIncapacitated(cypher);
            AssertInTrash(fleshOfTheSunGod);
            QuickHandCheck(3);
        }
        [Test]
        public void TestIncapacitateOption1Optional()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card fleshOfTheSunGod = GetCard("FleshOfTheSunGod");
            PlayCard(fleshOfTheSunGod);

            SetHitPoints(cypher.CharacterCard, 1);
            DealDamage(baron, cypher, 2, DamageType.Melee);

            QuickHandStorage(ra);

            DecisionSelectCards = new Card[] { null, GetCard("TheStaffOfRa") };
            // Act
            GoToUseIncapacitatedAbilityPhase(cypher);
            UseIncapacitatedAbility(cypher, 0);

            // Assert
            AssertIncapacitated(cypher);
            AssertIsInPlay(fleshOfTheSunGod);
            QuickHandCheck(0);
        }

        [Test]
        public void TestIncapacitateOption2()
        {
            // One player may destroy 1 of their equipment cards to play 3 cards.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card hudGoogles = GetCard("HUDGoggles");
            PlayCard(hudGoogles);

            Card syn = PutInHand("SynapticInterruption");
            Card grant = PutInHand("ResearchGrant");
            Card push = PutInHand("PushingTheLimits");

            SetHitPoints(cypher.CharacterCard, 1);
            DealDamage(baron, cypher, 2, DamageType.Melee);

            DecisionSelectTurnTaker = tachyon.TurnTaker;
            QuickHandStorage(tachyon);

            DecisionSelectCards = new Card[] { hudGoogles, syn, grant, push };
            // Act
            GoToUseIncapacitatedAbilityPhase(cypher);
            UseIncapacitatedAbility(cypher, 1);

            // Assert
            AssertIncapacitated(cypher);
            AssertInTrash(hudGoogles);
            QuickHandCheck(-3);
            AssertIsInPlay(syn, grant, push);
        }
        [Test]
        public void TestIncapacitateOption2Optional()
        {
            // One player may destroy 1 of their equipment cards to play 3 cards.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card hudGoogles = GetCard("HUDGoggles");
            PlayCard(hudGoogles);

            Card syn = PutInHand("SynapticInterruption");
            Card grant = PutInHand("ResearchGrant");
            Card push = PutInHand("PushingTheLimits");

            SetHitPoints(cypher.CharacterCard, 1);
            DealDamage(baron, cypher, 2, DamageType.Melee);

            DecisionSelectTurnTaker = tachyon.TurnTaker;
            QuickHandStorage(tachyon);

            DecisionSelectCards = new Card[] { null, syn, grant, push };
            // Act
            GoToUseIncapacitatedAbilityPhase(cypher);
            UseIncapacitatedAbility(cypher, 1);

            // Assert
            AssertIncapacitated(cypher);
            AssertIsInPlay(hudGoogles);
            QuickHandCheck(0);
            AssertInHand(syn, grant, push);
        }

        [Test]
        public void TestIncapacitateOption3()
        {
            // One target regains 1 HP.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            SetHitPoints(tachyon.CharacterCard, 20);
            SetHitPoints(cypher.CharacterCard, 1);
            DealDamage(baron, cypher, 2, DamageType.Melee);

            DecisionSelectCard = tachyon.CharacterCard;
            QuickHPStorage(tachyon);

            // Act
            GoToUseIncapacitatedAbilityPhase(cypher);
            UseIncapacitatedAbility(cypher, 2);

            // Assert
            AssertIncapacitated(cypher);
            QuickHPCheck(1);
        }

        [Test]
        public void TestBackupPlan_NoDestroy()
        {
            // When a non-hero card enters play, you may destroy this card.
            // If you do, select any number of Augments in play and move each one next to a new hero.
            // Then, each augmented hero regains 2HP.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            SetHitPoints(new[] { cypher.CharacterCard, ra.CharacterCard, tachyon.CharacterCard }, 18);

            Card BladeBattalion = GetCard("BladeBattalion");

            Card muscleAug = GetCard(MuscleAugCardController.Identifier);
            Card dermalAug = GetCard(DermalAugCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { muscleAug}},
                { tachyon.CharacterCard, new List<Card>() { dermalAug}}
            });

            Card backupPlan = GetCard(BackupPlanCardController.Identifier);

            DecisionYesNo = false;
            QuickHPStorage(cypher, ra, tachyon);

            // Act
            GoToPlayCardPhase(cypher);

            PlayCard(backupPlan);
            PlayCard(BladeBattalion); // Triggers Backup plan

            // Assert
            AssertNotInTrash(backupPlan);
            Assert.True(AreAugmented(new List<Card>() { ra.CharacterCard, tachyon.CharacterCard }));
            Assert.True(HasAugment(ra.CharacterCard, muscleAug));
            Assert.True(HasAugment(tachyon.CharacterCard, dermalAug));
            Assert.True(AreNotAugmented(new List<Card>() { cypher.CharacterCard }));
            QuickHPCheck(0, 0, 0); // Backup plan wasn't destroyed so no HP was gained
        }

        [Test]
        public void TestBackupPlan_Destroy()
        {
            // When a non-hero card enters play, you may destroy this card.
            // If you do, select any number of Augments in play and move each one next to a new hero.
            // Then, each augmented hero regains 2HP.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            SetHitPoints(new[] { cypher.CharacterCard, ra.CharacterCard, tachyon.CharacterCard }, 18);

            Card BladeBattalion = GetCard("BladeBattalion");

            Card muscleAug = GetCard(MuscleAugCardController.Identifier);
            Card dermalAug = GetCard(DermalAugCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { muscleAug}},
                { tachyon.CharacterCard, new List<Card>() { dermalAug}}
            });

            Card backupPlan = GetCard(BackupPlanCardController.Identifier);

            DecisionYesNo = true;
            QuickHPStorage(cypher, ra, tachyon);

            // Act
            GoToPlayCardPhase(cypher);
            Assert.True(AreAugmented(new List<Card>() { ra.CharacterCard, tachyon.CharacterCard }));
            Assert.True(HasAugment(ra.CharacterCard, muscleAug));
            Assert.True(HasAugment(tachyon.CharacterCard, dermalAug));

            PlayCard(backupPlan);
            DecisionSelectCards = new[] { muscleAug, cypher.CharacterCard, dermalAug, cypher.CharacterCard };
            PlayCard(BladeBattalion); // Triggers Backup plan

            // Assert
            AssertInTrash(backupPlan);
            Assert.True(AreAugmented(new List<Card>() { cypher.CharacterCard }));
            Assert.True(HasAugments(cypher.CharacterCard, new List<Card>() { muscleAug, dermalAug }));
            Assert.True(AreNotAugmented(new List<Card>() { ra.CharacterCard, tachyon.CharacterCard }));
            QuickHPCheck(2, 0, 0); // Only Cypher is augmented now
        }
        [Test]
        public void TestCyberdefenseSimple()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card batt = PlayCard("BladeBattalion");
            Card redist = PlayCard("ElementalRedistributor");

            Card dermal = PlayCard("DermalAug");
            Card retinal = PlayCard("RetinalAug");

            DecisionYesNo = true;
            DecisionSelectCards = new Card[] { dermal, retinal };
            DecisionAutoDecideIfAble = true;
            QuickHPStorage(mdp, batt, redist);
            PlayCard("Cyberdefense");
            QuickHPCheck(-2, -2, -2);
            AssertInTrash(dermal, retinal);
        }

        [Test]
        public void TestCyberdefenseDestroyLessThanMax()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card batt = PlayCard("BladeBattalion");
            Card redist = PlayCard("ElementalRedistributor");

            Card dermal = PlayCard("DermalAug");
            Card retinal = PlayCard("RetinalAug");

            DecisionYesNo = true;
            DecisionAutoDecideIfAble = true;
            DecisionSelectCards = new Card[] { dermal, null };
            QuickHPStorage(mdp, batt, redist);
            PlayCard("Cyberdefense");
            QuickHPCheck(-1, -1, -1);
            AssertInTrash(dermal);
            AssertIsInPlay(retinal);
        }


        [Test]
        public void TestCyberdefenseDestroyNone()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Legacy", "Tachyon", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card batt = PlayCard("BladeBattalion");
            Card redist = PlayCard("ElementalRedistributor");

            Card dermal = PlayCard("DermalAug");
            Card retinal = PlayCard("RetinalAug");
            PlayCard("InspiringPresence");

            DecisionYesNo = true;
            DecisionAutoDecideIfAble = true;
            DecisionSelectCards = new Card[] { null };
            QuickHPStorage(mdp, batt, redist);
            PlayCard("Cyberdefense");
            QuickHPCheck(-1, -1, -1);
            AssertIsInPlay(dermal);
            AssertIsInPlay(retinal);
        }

        [Test]
        public void TestCyberintegration()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            SetHitPoints(new[] { cypher.CharacterCard, ra.CharacterCard, tachyon.CharacterCard }, 18);
            Card muscleAug = GetCard(MuscleAugCardController.Identifier);
            Card dermalAug = GetCard(DermalAugCardController.Identifier);
            Card retinalAug = GetCard(RetinalAugCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { muscleAug, retinalAug}},
                { tachyon.CharacterCard, new List<Card>() { dermalAug}}
            });

            QuickHPStorage(cypher, ra, tachyon);

            DecisionSelectCards = new Card[] { muscleAug, retinalAug, null };
            PlayCard("Cyberintegration");
            QuickHPCheck(0, 6, 0);
        }

        [Test]
        public void TestCyborgBlaster()
        {
            // You may move 1 Augment in play next to a new hero.
            // One augmented hero deals 1 target 2 lightning damage.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card muscleAug = GetCard(MuscleAugCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { muscleAug}}
            });

            Card cyborgBlaster = GetCard(CyborgBlasterCardController.Identifier);
            QuickHPStorage(mdp);

            DecisionSelectCards = new[] { muscleAug, tachyon.CharacterCard, mdp };

            PlayCard(cyborgBlaster);

            Assert.True(AreAugmented(new List<Card>() { tachyon.CharacterCard }));
            Assert.True(AreNotAugmented(new List<Card>() { ra.CharacterCard }));
            Assert.True(HasAugment(tachyon.CharacterCard, muscleAug));
            QuickHPCheck(-3); // +2 from Cyborg Blaster, +1 from Muscle Aug
        }

        [Test]
        public void TestCyborgBlasterMultipleAugmentsOnOneHero()
        {
            // You may move 1 Augment in play next to a new hero.
            // One augmented hero deals 1 target 2 lightning damage.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card muscleAug = GetCard(MuscleAugCardController.Identifier);
            Card dermalAug = GetCard(DermalAugCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { muscleAug, dermalAug}}
            });

            Card cyborgBlaster = GetCard(CyborgBlasterCardController.Identifier);
            QuickHPStorage(mdp);

            DecisionSelectCards = new[] { muscleAug, tachyon.CharacterCard, tachyon.CharacterCard, mdp };

            PlayCard(cyborgBlaster);

            Assert.True(AreAugmented(new List<Card>() { tachyon.CharacterCard }));
            Assert.True(HasAugment(tachyon.CharacterCard, muscleAug));
            Assert.True(AreAugmented(new List<Card>() { ra.CharacterCard }));
            Assert.True(HasAugment(ra.CharacterCard, dermalAug));
            QuickHPCheck(-3); // +2 from Cyborg Blaster, +1 from Muscle Aug
        }

        [Test]
        public void TestCyborgBlasterAugmentsOnMultipleHeroes()
        {
            // You may move 1 Augment in play next to a new hero.
            // One augmented hero deals 1 target 2 lightning damage.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card muscleAug = GetCard(MuscleAugCardController.Identifier);
            Card dermalAug = GetCard(DermalAugCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { muscleAug}},
                { tachyon.CharacterCard, new List<Card>() { dermalAug}}
            });

            Card cyborgBlaster = GetCard(CyborgBlasterCardController.Identifier);
            QuickHPStorage(mdp);

            DecisionSelectCards = new[] { muscleAug, tachyon.CharacterCard, mdp };
            DecisionSelectTurnTaker = tachyon.TurnTaker;

            PlayCard(cyborgBlaster);

            Assert.True(AreAugmented(new List<Card>() { tachyon.CharacterCard }));
            Assert.True(AreNotAugmented(new List<Card>() { ra.CharacterCard }));
            Assert.True(HasAugment(tachyon.CharacterCard, muscleAug));
            Assert.True(HasAugment(tachyon.CharacterCard, dermalAug));
            QuickHPCheck(-3); // +2 from Cyborg Blaster, +1 from Muscle Aug
        }

        [Test]
        public void TestCyborgBlasterWhenIsolated()
        {
            // You may move 1 Augment in play next to a new hero.
            // One augmented hero deals 1 target 2 lightning damage.

            // Arrange
            SetupGameController("MissInformation", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            DestroyNonCharacterVillainCards();

            Card neuralInterface = PlayCard("NeuralInterface");

            Card muscleAug = GetCard("MuscleAug");

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { cypher.CharacterCard, new List<Card>() { muscleAug}}
            });

            string isolatedHeroMessage = "Isolated Hero will prevent Cypher from affecting other heroes, and other heroes cannot affect Cypher.";
            string cyborgBlasterMessage = "There are no available heroes to move the augment to.";
            string moveMuscleAugMessage = "Muscle Aug was moved next to Cypher.";
            AssertNextMessages(new string[] { isolatedHeroMessage, cyborgBlasterMessage, isolatedHeroMessage, moveMuscleAugMessage });

            Card isolatedHero = PlayCard("IsolatedHero");

            DecisionSelectCard =  muscleAug;

            // Muscle Aug is on Cypher
            // Since Cypher is isolated, he cannot see any of the other hero cards
            // Should see message indicating that nothing will happen
            Card cyborgBlaster = PlayCard("CyborgBlaster");

            DestroyCard(muscleAug);
            DestroyCard(isolatedHero);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { muscleAug}}
            });

            DecisionSelectCard = muscleAug;


            PlayCard(isolatedHero);

            // Muscle Aug is on Ra
            // Even though Cypher is isolated, he should still see his own cards
            //   * this is true even though the aug is in a different play area
            // However, no other hero characters can be seen, so he moves it to himself
            // Should see message indicating that the move happened
            PlayCard(cyborgBlaster);

        }


        [Test]
        public void TestCyborgPunch()
        {
            // You may move 1 Augment in play next to a new hero.
            // One augmented hero deals 1 target 1 melee damage and draws a card now.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card muscleAug = GetCard(MuscleAugCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { muscleAug}}
            });

            Card cyborgPunch = GetCard(CyborgPunchCardController.Identifier);

            DecisionSelectCards = new[] { muscleAug, tachyon.CharacterCard, mdp };

            QuickHandStorage(tachyon);
            QuickHPStorage(mdp);

            PlayCard(cyborgPunch);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { tachyon.CharacterCard }));
            Assert.True(AreNotAugmented(new List<Card>() { ra.CharacterCard }));
            Assert.True(HasAugment(tachyon.CharacterCard, muscleAug));
            QuickHPCheck(-2); // +1 from Cyborg Blaster, +1 from Muscle Aug
            QuickHandCheck(1);
        }

        [Test]
        public void TestCyborgPunchMultipleAugmentsOnOneHero()
        {
            // You may move 1 Augment in play next to a new hero.
            // One augmented hero deals 1 target 1 melee damage and draws a card now.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card muscleAug = GetCard(MuscleAugCardController.Identifier);
            Card dermalAug = GetCard(DermalAugCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { muscleAug, dermalAug}}
            });

            Card cyborgPunch = GetCard(CyborgPunchCardController.Identifier);

            DecisionSelectCards = new[] { muscleAug, tachyon.CharacterCard, tachyon.CharacterCard, mdp };

            QuickHandStorage(tachyon);
            QuickHPStorage(mdp);

            PlayCard(cyborgPunch);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { tachyon.CharacterCard }));
            Assert.True(HasAugment(tachyon.CharacterCard, muscleAug));
            Assert.True(AreAugmented(new List<Card>() { ra.CharacterCard }));
            Assert.True(HasAugment(ra.CharacterCard, dermalAug));
            QuickHPCheck(-2); // +1 from Cyborg Blaster, +1 from Muscle Aug
            QuickHandCheck(1);
        }

        [Test]
        public void TestCyborgPunchAugmentsOnMultipleHeroes()
        {
            // You may move 1 Augment in play next to a new hero.
            // One augmented hero deals 1 target 1 melee damage and draws a card now.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card muscleAug = GetCard(MuscleAugCardController.Identifier);
            Card dermalAug = GetCard(DermalAugCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { muscleAug}},
                { tachyon.CharacterCard, new List<Card>() { dermalAug}}
            });

            Card cyborgPunch = GetCard(CyborgPunchCardController.Identifier);

            DecisionSelectCards = new[] { muscleAug, tachyon.CharacterCard, mdp };
            DecisionSelectTurnTaker = tachyon.TurnTaker;

            QuickHandStorage(tachyon);
            QuickHPStorage(mdp);

            PlayCard(cyborgPunch);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { tachyon.CharacterCard }));
            Assert.True(AreNotAugmented(new List<Card>() { ra.CharacterCard }));
            Assert.True(HasAugment(tachyon.CharacterCard, muscleAug));
            QuickHPCheck(-2); // +1 from Cyborg Blaster, +1 from Muscle Aug
            QuickHandCheck(1);
        }

        [Test]
        public void TestDermalAug()
        {
            // Play this card next to a hero. The hero next to this card is augmented.
            // Reduce damage dealt to that hero by 1.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");

            StartGame();

            Card dermalAug = GetCard(DermalAugCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { tachyon.CharacterCard, new List<Card>() { dermalAug}}
            });

            QuickHPStorage(tachyon);

            DealDamage(baron, tachyon, 2, DamageType.Energy);

            QuickHPCheck(-1);
            Assert.True(AreAugmented(new List<Card>() { tachyon.CharacterCard }));
            Assert.True(HasAugment(tachyon.CharacterCard, dermalAug));
        }

        [Test]
        public void TestElectroOpticalCloak()
        {
            // Augmented heroes are immune to damage.
            // At the start of your turn, destroy this card.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card muscleAug = GetCard(MuscleAugCardController.Identifier);
            Card dermalAug = GetCard(DermalAugCardController.Identifier);
            Card electroCloak = GetCard(ElectroOpticalCloakCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { muscleAug}},
                { tachyon.CharacterCard, new List<Card>() { dermalAug}}
            });

            GoToPlayCardPhase(cypher);

            QuickHPStorage(cypher, ra, tachyon);

            PlayCard(electroCloak);

            DealDamage(baron, cypher.CharacterCard, 4, DamageType.Energy);
            DealDamage(baron, ra.CharacterCard, 4, DamageType.Energy);
            DealDamage(baron, tachyon.CharacterCard, 4, DamageType.Energy);

            GoToStartOfTurn(cypher);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { ra.CharacterCard, tachyon.CharacterCard }));
            Assert.True(AreNotAugmented(new List<Card>() { cypher.CharacterCard }));
            Assert.True(HasAugment(ra.CharacterCard, muscleAug));
            Assert.True(HasAugment(tachyon.CharacterCard, dermalAug));
            QuickHPCheck(-4, 0, 0);
            AssertInTrash(electroCloak);

        }

        [Test]
        public void TestFusionAug()
        {
            // Play this card next to a hero. The hero next to this card is augmented.
            // That hero may use an additional power during their power phase.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card fusionAug = GetCard(FusionAugCardController.Identifier);
            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { fusionAug}}
            });

            GoToUsePowerPhase(ra);
            AssertPhaseActionCount(2);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { ra.CharacterCard }));
            Assert.True(HasAugment(ra.CharacterCard, fusionAug));
        }

        [Test]
        public void TestHackingProgram()
        {
            // POWER: {Cypher} deals himself 2 irreducible energy damage.
            // If he takes damage this way, destroy 1 ongoing or environment card.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card policeBackup = GetCard("PoliceBackup");
            PlayCard(policeBackup);
            DecisionSelectCard = policeBackup;

            Card hackingProgram = GetCard(HackingProgramCardController.Identifier);

            QuickHPStorage(cypher);

            // Act
            GoToPlayCardPhase(cypher);
            PlayCard(hackingProgram);
            GoToUsePowerPhase(cypher);
            UsePower(hackingProgram);

            // Assert
            AssertInTrash(policeBackup);
            QuickHPCheck(-2);
        }
        [Test]
        public void TestHackingProgramDamageRedirected()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "TheScholar", "Megalopolis");
            StartGame();

            Card policeBackup = GetCard("PoliceBackup");
            PlayCard(policeBackup);
            DecisionSelectCard = policeBackup;

            PlayCard("AlchemicalRedirection");

            Card hackingProgram = GetCard(HackingProgramCardController.Identifier);
            PlayCard(hackingProgram);
            UsePower(hackingProgram);
            AssertIsInPlay(policeBackup);
        }
        [Test]
        public void TestHackingProgramNotOptional()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card hackingProgram = PlayCard("HackingProgram");

            DecisionYesNo = false;
            AssertNoDecision();
            UsePower(hackingProgram);
            AssertInTrash(hackingProgram);
        }

        [Test]
        public void TestHeuristicAlgorithm_PutAugInPlay()
        {
            // Reveal cards from the top of your deck until you reveal an Augment.
            // Put it into play or into your trash. Shuffle the rest of the revealed cards into your deck.
            // If you did not put an Augment into play this way, draw 2 cards.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");

            Card cyborgPunch = GetCard(CyborgPunchCardController.Identifier);
            PutOnDeck(cypher, cyborgPunch); // Put a non augment on top so we guarantee a deck shuffle after revealing

            StartGame();


            Card heuristicAlg = GetCard(HeuristicAlgorithmCardController.Identifier);

            DecisionSelectFunction = 0;
            DecisionSelectCard = cypher.CharacterCard;


            GoToPlayCardPhase(cypher);

            QuickShuffleStorage(cypher);

            PlayCard(heuristicAlg);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { cypher.CharacterCard }));
            QuickShuffleCheck(1);
        }

        [Test]
        public void TestHeuristicAlgorithm_PutAugInTrash()
        {
            // Reveal cards from the top of your deck until you reveal an Augment.
            // Put it into play or into your trash. Shuffle the rest of the revealed cards into your deck.
            // If you did not put an Augment into play this way, draw 2 cards.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");

            Card cyborgPunch = GetCard(CyborgPunchCardController.Identifier);
            PutOnDeck(cypher, cyborgPunch); // Put a non augment on top so we guarantee a deck shuffle after revealing

            StartGame();

            Card heuristicAlg = GetCard(HeuristicAlgorithmCardController.Identifier);

            DecisionSelectFunction = 1;

            GoToPlayCardPhase(cypher);
            QuickHandStorage(cypher);
            QuickShuffleStorage(cypher);

            PlayCard(heuristicAlg);

            // Assert
            Assert.True(AreNotAugmented(new List<Card>() { cypher.CharacterCard }));
            QuickShuffleCheck(1);
            QuickHandCheck(2); // No Augment was put into play so 2 cards were drawn
        }
        [Test]
        public void TestHeuristicAlgorithm_NoAugsInDeck()
        {
            // Reveal cards from the top of your deck until you reveal an Augment.
            // Put it into play or into your trash. Shuffle the rest of the revealed cards into your deck.
            // If you did not put an Augment into play this way, draw 2 cards.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");

            PutInTrash(cypher, (Card c) => IsAugment(c));

            Card cyborgPunch = GetCard(CyborgPunchCardController.Identifier);
            PutOnDeck(cypher, cyborgPunch); // Put a non augment on top so we guarantee a deck shuffle after revealing


            StartGame();

            Card heuristicAlg = PutInTrash(HeuristicAlgorithmCardController.Identifier);

            DecisionSelectFunction = 1;

            GoToPlayCardPhase(cypher);
            QuickHandStorage(cypher);
            QuickShuffleStorage(cypher);

            AssertNoDecision();
            PlayCard(heuristicAlg);

            // Assert
            Assert.True(AreNotAugmented(new List<Card>() { cypher.CharacterCard }));
            QuickShuffleCheck(1);
            QuickHandCheck(2); // No Augment was put into play so 2 cards were drawn
        }
        [Test]
        public void TestInitiatedUpgrade_SearchDeck_DrawCard()
        {
            // Search your deck or trash for an Augment card and put it into play. If you searched your deck, shuffle it.
            // You may draw a card.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");

            StartGame();

            // Act
            Card initiatedUpgrade = GetCard(InitiatedUpgradeCardController.Identifier);
            PutOnDeck(cypher, initiatedUpgrade); //avoid bad seeds putting all copies in hand
            Card dermalAug = GetCard(DermalAugCardController.Identifier);
            PutInDeck(dermalAug);

            DecisionSelectLocation = new LocationChoice(cypher.TurnTaker.Deck);
            DecisionSelectCards = new[] { dermalAug, cypher.CharacterCard };
            QuickShuffleStorage(cypher);
            QuickHandStorage(cypher);
            DecisionYesNo = true;

            PlayCard(initiatedUpgrade);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { cypher.CharacterCard }));
            Assert.True(HasAugment(cypher.CharacterCard, dermalAug));
            AssertIsInPlay(dermalAug);
            QuickShuffleCheck(1);
            QuickHandCheck(1);
        }

        [Test]
        public void TestInitiatedUpgrade_SearchDeck_DontDrawCard()
        {
            // Search your deck or trash for an Augment card and put it into play. If you searched your deck, shuffle it.
            // You may draw a card.

            // Arrange
            SetupGameController("Omnitron", DeckNamespace, "Ra", "Tachyon", "Megalopolis");

            StartGame();

            // Act
            Card initiatedUpgrade = GetCard(InitiatedUpgradeCardController.Identifier);
            PutOnDeck(cypher, initiatedUpgrade); //avoid bad seeds putting all copies in hand
            Card dermalAug = GetCard(DermalAugCardController.Identifier);
            PutInDeck(dermalAug);

            PlayCard("InterpolationBeam");
            //allows us to expose optional draw

            DecisionSelectLocation = new LocationChoice(cypher.TurnTaker.Deck);
            DecisionSelectCards = new[] { dermalAug, cypher.CharacterCard };
            QuickShuffleStorage(cypher);
            QuickHandStorage(cypher);
            DecisionYesNo = false;

            PlayCard(initiatedUpgrade);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { cypher.CharacterCard }));
            Assert.True(HasAugment(cypher.CharacterCard, dermalAug));
            AssertIsInPlay(dermalAug);
            QuickShuffleCheck(1);
            QuickHandCheck(0);
        }

        [Test]
        public void TestInitiatedUpgrade_SearchTrash_AugmentsInTrash()
        {
            // Search your deck or trash for an Augment card and put it into play. If you searched your deck, shuffle it.
            // You may draw a card.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            // Act
            Card initiatedUpgrade = GetCard(InitiatedUpgradeCardController.Identifier);

            Card dermalAug = GetCard(DermalAugCardController.Identifier);
            Card muscleAug = GetCard(MuscleAugCardController.Identifier);
            PutInTrash(cypher, dermalAug);
            PutInTrash(cypher, muscleAug);


            DecisionSelectLocation = new LocationChoice(cypher.TurnTaker.Trash);
            DecisionSelectCards = new[] { dermalAug, cypher.CharacterCard };
            QuickShuffleStorage(cypher);
            QuickHandStorage(cypher);
            DecisionYesNo = true;

            PlayCard(initiatedUpgrade);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { cypher.CharacterCard }));
            Assert.True(HasAugment(cypher.CharacterCard, dermalAug));
            AssertIsInPlay(dermalAug);
            QuickShuffleCheck(0);
            QuickHandCheck(1);
        }

        [Test]
        public void TestInitiatedUpgrade_SearchTrash_NoAugmentsInTrash()
        {
            // Search your deck or trash for an Augment card and put it into play. If you searched your deck, shuffle it.
            // You may draw a card.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            // Act
            Card initiatedUpgrade = GetCard(InitiatedUpgradeCardController.Identifier);


            DecisionSelectLocation = new LocationChoice(cypher.TurnTaker.Trash);
            QuickShuffleStorage(cypher);
            QuickHandStorage(cypher);
            DecisionYesNo = true;

            PlayCard(initiatedUpgrade);

            // Assert
            Assert.True(AreNotAugmented(new List<Card>() { cypher.CharacterCard }));
            QuickShuffleCheck(0);
            QuickHandCheck(1);
        }


        [Test]
        public void TestMuscleAug()
        {
            // Play this card next to a hero. The hero next to this card is augmented.
            // Increase damage dealt by that hero by 1.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card muscleAug = GetCard(MuscleAugCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { muscleAug}}
            });

            DecisionSelectTarget = mdp;
            QuickHPStorage(mdp);

            GoToUsePowerPhase(ra);
            UsePower(ra);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { ra.CharacterCard }));
            Assert.True(HasAugment(ra.CharacterCard, muscleAug));
            QuickHPCheck(-3); // +2 from Pyre, +1 from Muscle Aug
        }

        [Test]
        public void TestNaniteSurge()
        {
            // You may draw a card.
            // You may play a card.
            // Each augmented hero regains X HP, where X is the number of Augments next to them.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            SetHitPoints(new[] { ra, tachyon }, 20);

            Card muscleAug = GetCard(MuscleAugCardController.Identifier);
            Card dermalAug = GetCard(DermalAugCardController.Identifier);
            Card retinalAug = GetCard(RetinalAugCardController.Identifier);
            Card electroCloak = GetCard(ElectroOpticalCloakCardController.Identifier);
            PutInHand(cypher, electroCloak);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { muscleAug, retinalAug }},
                { tachyon.CharacterCard, new List<Card>() { dermalAug }},
            }); ;

            // Act
            Card naniteSurge = GetCard(NaniteSurgeCardController.Identifier);

            DecisionYesNo = false;
            DecisionSelectCard = electroCloak;

            GoToPlayCardPhase(cypher);


            QuickHPStorage(ra, tachyon);

            PlayCard(naniteSurge);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { ra.CharacterCard, tachyon.CharacterCard }));
            Assert.True(HasAugments(ra.CharacterCard, new List<Card>() { muscleAug, retinalAug }));
            Assert.True(HasAugment(tachyon.CharacterCard, dermalAug));
            QuickHPCheck(2, 1); // Ra has 2 augs, Tachyon has 1
        }

        [Test]
        public void TestNetworkedAttack()
        {
            // Each augmented hero may use a power now.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card muscleAug = GetCard(MuscleAugCardController.Identifier);
            Card dermalAug = GetCard(DermalAugCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { muscleAug }},
                { tachyon.CharacterCard, new List<Card>() { dermalAug }},
            }); ;

            // Act
            Card networkedAttack = GetCard(NetworkedAttackCardController.Identifier);

            GoToPlayCardPhase(cypher);

            DecisionYesNo = true;
            int tachyonTrashCount = GetNumberOfCardsInTrash(tachyon);

            QuickHandStorage(tachyon);
            DecisionSelectTarget = mdp;
            QuickHPStorage(mdp);

            PlayCard(networkedAttack);

            // Assert
            QuickHPCheck(-3); // Ra's power usage
            Assert.AreEqual(tachyonTrashCount + 1, GetNumberOfCardsInTrash(tachyon)); // Tachyon's power usage
        }

        [Test]
        public void TestNeuralInterface()
        {
            // You may move 1 Augment in play next to a new hero. Draw 2 cards. Discard a card

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card muscleAug = GetCard(MuscleAugCardController.Identifier);
            Card neuralInterface = GetCard(NeuralInterfaceCardController.Identifier);

            //DecisionYesNo = true;
            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { muscleAug }}
            }); ;

            // Act
            GoToPlayCardPhase(cypher);

            DecisionSelectCards = new[] { muscleAug, tachyon.CharacterCard, GetCardFromHand(cypher, 0) };

            PlayCard(neuralInterface);
            GoToUsePowerPhase(cypher);

            QuickHandStorage(cypher);

            UsePower(neuralInterface);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { tachyon.CharacterCard }));
            Assert.True(HasAugment(tachyon.CharacterCard, muscleAug));
            Assert.True(AreNotAugmented(new List<Card>() { ra.CharacterCard }));
            QuickHandCheck(1);
        }

        [Test]
        public void TestRapidPrototyping()
        {
            // Draw 2 cards.
            // Play any number of Augments from your hand.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card rapidProto = GetCard(RapidPrototypingCardController.Identifier);

            Card muscleAug = GetCard(MuscleAugCardController.Identifier);
            Card dermalAug = GetCard(DermalAugCardController.Identifier);
            Card fusionAug = GetCard(FusionAugCardController.Identifier);
            Card retinalAug = GetCard(RetinalAugCardController.Identifier);
            Card vascularAug = GetCard(VascularAugCardController.Identifier);

            PutInHand(cypher, new[] { muscleAug, dermalAug, fusionAug, retinalAug, vascularAug });

            // Act
            GoToPlayCardPhase(cypher);

            QuickHandStorage(cypher);
            DecisionSelectCards = new[] { muscleAug, ra.CharacterCard, dermalAug, tachyon.CharacterCard, null };

            PlayCard(rapidProto);

            // Assert
            QuickHandCheck(0); // +2 cards drawn from RapidProto, -2 augs played
            Assert.True(AreAugmented(new List<Card>() { ra.CharacterCard, tachyon.CharacterCard }));
            Assert.True(AreNotAugmented(new List<Card>() { cypher.CharacterCard }));
            Assert.True(HasAugment(ra.CharacterCard, muscleAug));
            Assert.True(HasAugment(tachyon.CharacterCard, dermalAug));
            AssertInHand(cypher, new[] { fusionAug.Identifier, retinalAug.Identifier, vascularAug.Identifier });
        }

        [Test]
        public void TestRebuiltToSucceedNoAugmentsInTrash()
        {
            // Select two Augments in your trash. Put one into your hand and one into play.
            // The hero you augment this way may play a card now.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card rebuilt = GetCard(RebuiltToSucceedCardController.Identifier);

            // Act
            GoToPlayCardPhase(cypher);

            QuickHandStorage(cypher);
            int cypherCardsInPlay = GetNumberOfCardsInPlay(cypher); ;

            PlayCard(rebuilt);

            // Assert
            QuickHandCheck(0);
            Assert.AreEqual(cypherCardsInPlay, GetNumberOfCardsInPlay(cypher));
        }

        [Test]
        public void TestRebuiltToSucceedAugmentsInTrash()
        {
            // Select two Augments in your trash. Put one into your hand and one into play.
            // The hero you augment this way may play a card now.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");

            Card dermal = GetCard(DermalAugCardController.Identifier);
            Card muscle = GetCard(MuscleAugCardController.Identifier);
            Card retinal = GetCard(RetinalAugCardController.Identifier);
            PutInTrash(cypher, dermal);
            PutInTrash(cypher, muscle);
            PutInTrash(cypher, retinal);

            Card fleshOfTheSunGod = GetCard("FleshOfTheSunGod");
            PutInHand(ra, fleshOfTheSunGod);

            StartGame();

            Card rebuilt = GetCard(RebuiltToSucceedCardController.Identifier);

            DecisionSelectCards = new[] { dermal, muscle, ra.CharacterCard, GetCardFromHand(ra, fleshOfTheSunGod.Identifier) };
            DecisionMoveCardDestination = new MoveCardDestination(cypher.HeroTurnTaker.Hand);

            // Act

            int raCardsInPlay = GetNumberOfCardsInPlay(ra);

            GoToPlayCardPhase(cypher);
            PlayCard(rebuilt);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { ra.CharacterCard }));
            Assert.True(HasAugment(ra.CharacterCard, muscle));
            AssertInHand(dermal);
            AssertInTrash(retinal);
            Assert.AreEqual(raCardsInPlay + 1, GetNumberOfCardsInPlay(ra)); // Ra was augmented by Rebuilt and was able to play a card
        }
        [Test]
        public void TestRebuiltToSucceedInPlayFirst()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Legacy", "Haka", "Megalopolis");
            StartGame();

            Card dermal = PutInTrash("DermalAug");
            Card muscle = PutInTrash("MuscleAug");
            PutInTrash("VascularAug");
            Card fort = PutInHand("Fortitude");

            DecisionSelectCards = new Card[] { dermal, muscle, legacy.CharacterCard, fort };
            DecisionMoveCardDestination = new MoveCardDestination(cypher.HeroTurnTaker.PlayArea);
            PlayCard("RebuiltToSucceed");
            AssertNextToCard(dermal, legacy.CharacterCard);
            AssertIsInPlay(fort);
        }
        [Test]
        public void TestRetinalAug()
        {
            // Play this card next to a hero. The hero next to this card is augmented.
            // During their play phase, that hero may play an additional card.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card retinalAug = GetCard(RetinalAugCardController.Identifier);

            GoToPlayCardPhase(cypher);
            AssertPhaseActionCount(1);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { retinalAug }}
            });

            AssertPhaseActionCount(0);

            // Act
            GoToPlayCardPhase(ra);
            AssertPhaseActionCount(2);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { ra.CharacterCard }));
            Assert.True(HasAugment(ra.CharacterCard, retinalAug));
        }


        [Test]
        public void TestRetinalAugSameTurnBuff()
        {
            // Play this card next to a hero. The hero next to this card is augmented.
            // During their play phase, that hero may play an additional card.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card retinalAug = GetCard(RetinalAugCardController.Identifier);

            GoToPlayCardPhase(cypher);
            AssertPhaseActionCount(1);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { cypher.CharacterCard, new List<Card>() { retinalAug }}
            }); ;

            AssertPhaseActionCount(1);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { cypher.CharacterCard }));
            Assert.True(HasAugment(cypher.CharacterCard, retinalAug));
        }


        [Test]
        public void TestRetinalAugBuffMovesWithAug()
        {
            // Play this card next to a hero. The hero next to this card is augmented.
            // During their play phase, that hero may play an additional card.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card retinalAug = GetCard(RetinalAugCardController.Identifier);

            GoToPlayCardPhase(cypher);
            AssertPhaseActionCount(1);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { cypher.CharacterCard, new List<Card>() { retinalAug }}
            }); ;

            AssertPhaseActionCount(1);

            GoToPlayCardPhase(ra);

            AssertPhaseActionCount(1);

            MoveCard(cypher, retinalAug, ra.CharacterCard.NextToLocation, false, true, false, cypher.CharacterCardController.GetCardSource());

            AssertPhaseActionCount(2);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { ra.CharacterCard }));
            Assert.True(HasAugment(ra.CharacterCard, retinalAug));
        }

        [Test]
        public void TestVascularAug()
        {
            // Play this card next to a hero. The hero next to this card is augmented.
            // That hero regains 1HP at the end of their turn.


            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");

            StartGame();

            SetHitPoints(cypher, 16);

            Card vascularAug = GetCard(VascularAugCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { cypher.CharacterCard, new List<Card>() { vascularAug }}
            });

            // Act
            QuickHPStorage(cypher);
            GoToEndOfTurn(cypher);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { cypher.CharacterCard }));
            Assert.True(AreNotAugmented(new List<Card>() { ra.CharacterCard, tachyon.CharacterCard }));
            Assert.True(HasAugment(cypher.CharacterCard, vascularAug));
            QuickHPCheck(1);
        }
        [Test]
        public void TestVascularAugEndOfTheirTurn()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");

            StartGame();

            SetHitPoints(ra, 16);

            Card vascularAug = GetCard(VascularAugCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { vascularAug }}
            });

            // Act
            QuickHPStorage(ra);
            GoToEndOfTurn(cypher);
            QuickHPCheck(0);
            GoToEndOfTurn(ra);
            QuickHPCheck(1);
        }

        [Test]
        public void TestFirstReponsePromoUnlock()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Cauldron.Cricket", "Cauldron.Echelon", "Cauldron.Vanish", "Cauldron.DocHavoc", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();


            AssertPromoCardIsUnlockableThisGame("FirstResponseCypherCharacter");

            IEnumerable<Card> augments = FindCardsWhere(c => IsAugment(c)).Take(4);
            DecisionSelectCards = new Card[] { cricket.CharacterCard, echelon.CharacterCard, doc.CharacterCard, vanish.CharacterCard, cricket.CharacterCard, echelon.CharacterCard, doc.CharacterCard, vanish.CharacterCard };
            foreach(Card augment in augments)
            {
                PlayCard(augment);
            }

            DestroyCards(c => IsAugment(c) && c.IsInPlayAndHasGameText);
            foreach (Card augment in augments)
            {
                PlayCard(augment);
            }

            AssertPromoCardNotUnlocked("FirstResponseCypherCharacter");
            DealDamage(cricket, baron, 100, DamageType.Radiant);
            DealDamage(cricket, baron, 100, DamageType.Radiant);

            AssertGameOver();
            AssertPromoCardUnlocked("FirstResponseCypherCharacter");
        }

        [Test]
        public void TestSwarmingProtocolPromoUnlock()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Cauldron.Cricket", "Cauldron.Echelon", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();



            DestroyCard(cypher);
            DestroyCard(cricket);
            DestroyCard(echelon);

            AssertGameOver();

            SetupGameController("BaronBlade", DeckNamespace, "Cauldron.Cricket", "Cauldron.Echelon", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card augment = FindCardsWhere(c => IsAugment(c)).FirstOrDefault();
            DecisionSelectCards = new Card[] { cypher.CharacterCard };







        }

        #region Augment Helpers

        private bool IsAugment(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "augment");
        }

        private bool AreAugmented(List<Card> heroes)
        {
            foreach (Card hero in heroes)
            {
                if (!hero.IsHeroCharacterCard || !hero.IsInPlayAndHasGameText || hero.IsIncapacitatedOrOutOfGame
                    || !hero.NextToLocation.HasCards || !hero.GetAllNextToCards(false).Any(IsAugment))
                {
                    return false;
                }
            }

            return true;
        }

        private bool AreNotAugmented(List<Card> heroes)
        {
            return !AreAugmented(heroes);
        }

        private bool HasAugment(Card hero, Card augment)
        {
            return HasAugments(hero, new List<Card>() { augment });
        }

        private bool HasAugments(Card hero, List<Card> augments)
        {
            return FindCardsWhere(card => card == hero && card.Location.IsNextToCard).All(augments.Contains);
        }

        private List<Card> GetAugmentsInPlay()
        {
            return FindCardsWhere(card => card.IsInPlay && card.Location.IsNextToCard
                                    && card.DoKeywordsContain("augment")).ToList();
        }

        private void PutAugmentsIntoPlay(Dictionary<Card, List<Card>> augDictionary)
        {
            foreach (KeyValuePair<Card, List<Card>> kvp in augDictionary)
            {
                DecisionSelectCard = kvp.Key;
                foreach (Card aug in kvp.Value)
                {
                    PlayCard(aug);
                }
            }
        }

        #endregion

    }
}
