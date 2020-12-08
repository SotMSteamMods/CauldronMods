using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cauldron.Cypher;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture]
    public class CypherTests : BaseTest
    {
        protected HeroTurnTakerController Cypher => FindHero("Cypher");

        private const string DeckNamespace = "Cauldron.Cypher";

        [Test]
        public void TestCypherLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", DeckNamespace, "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(Cypher);
            Assert.IsInstanceOf(typeof(CypherCharacterCardController), Cypher.CharacterCardController);

            Assert.AreEqual(26, Cypher.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestCypherDecklist()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            AssertHasKeyword("augment", new []
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
            });;

            QuickHandStorage(tachyon);
            DecisionSelectTurnTaker = tachyon.TurnTaker;

            GoToUsePowerPhase(Cypher);
            UsePower(Cypher);

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

            SetHitPoints(Cypher.CharacterCard, 1);
            DealDamage(baron, Cypher, 2, DamageType.Melee);

            QuickHandStorage(ra);

            // Act
            GoToUseIncapacitatedAbilityPhase(Cypher);
            UseIncapacitatedAbility(Cypher, 0);

            // Assert
            AssertIncapacitated(Cypher);
            AssertInTrash(fleshOfTheSunGod);
            QuickHandCheck(3);
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

            SetHitPoints(Cypher.CharacterCard, 1);
            DealDamage(baron, Cypher, 2, DamageType.Melee);

            DecisionSelectCard = tachyon.CharacterCard;
            QuickHandStorage(tachyon);

            // Act
            GoToUseIncapacitatedAbilityPhase(Cypher);
            UseIncapacitatedAbility(Cypher, 1);

            // Assert
            AssertIncapacitated(Cypher);
            AssertInTrash(hudGoogles);
            QuickHandCheck(3);
        }

        [Test]
        public void TestIncapacitateOption3()
        {
            // One target regains 1 HP.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            SetHitPoints(tachyon.CharacterCard, 20);
            SetHitPoints(Cypher.CharacterCard, 1);
            DealDamage(baron, Cypher, 2, DamageType.Melee);

            DecisionSelectCard = tachyon.CharacterCard;
            QuickHPStorage(tachyon);

            // Act
            GoToUseIncapacitatedAbilityPhase(Cypher);
            UseIncapacitatedAbility(Cypher, 2);

            // Assert
            AssertIncapacitated(Cypher);
            QuickHPCheck(1);
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
            Card cyborgBlaster = GetCard(CyborgBlasterCardController.Identifier);

            DecisionSelectCards = new[] {ra.CharacterCard, muscleAug, tachyon.CharacterCard, mdp};
            DecisionSelectTarget = mdp;


            PlayCard(muscleAug);
            PlayCard(cyborgBlaster);

            Assert.True(false, "TODO");

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
            Card cyborgPunch = GetCard(CyborgPunchCardController.Identifier);

            DecisionSelectCards = new[] { ra.CharacterCard, muscleAug, tachyon.CharacterCard, mdp };
            DecisionSelectTarget = mdp;


            PlayCard(muscleAug);
            PlayCard(cyborgPunch);

            Assert.True(false, "TODO");

        }

        [Test]
        public void TesDermalAug()
        {
            // Play this card next to a hero. The hero next to this card is augmented.
            // Reduce damage dealt to that hero by 1.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");

            StartGame();

            DecisionSelectTarget = Cypher.CharacterCard;

            Card dermalAug = GetCard(DermalAugCardController.Identifier);


            GoToPlayCardPhase(Cypher);
            PlayCard(dermalAug);
            GoToEndOfTurn(Cypher);

            Assert.True(false, "TODO");

        }

        [Test]
        public void TestElectroOpticalCloak()
        {
            // Augmented heroes are immune to damage.
            // At the start of your turn, destroy this card.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card electroCloak = GetCard(ElectroOpticalCloakCardController.Identifier);
            Card muscleAug = GetCard(MuscleAugCardController.Identifier);

            DecisionSelectCard = tachyon.CharacterCard;

            GoToPlayCardPhase(Cypher);
            PlayCard(muscleAug);
            PlayCard(electroCloak);

            DealDamage(baron, tachyon.CharacterCard, 4, DamageType.Energy);

            //GoToStartOfTurn(Cypher);
            
            Assert.True(false, "TODO");
        }


        [Test]
        public void TestMuscleAug()
        {
            // Play this card next to a hero. The hero next to this card is augmented.
            // Increase damage dealt by that hero by 1.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");

            StartGame();


            Card muscleAug = GetCard(MuscleAugCardController.Identifier);


            GoToPlayCardPhase(Cypher);
            PlayCard(muscleAug);

            Assert.True(false, "TODO");
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

            GoToPlayCardPhase(Cypher);
            PlayCard(rebuilt);


            Assert.True(false, "TODO");
        }

        [Test]
        public void TestRebuiltToSucceedAugmentsInTrash()
        {
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");

            Card dermal = GetCard(DermalAugCardController.Identifier);
            Card muscle = GetCard(MuscleAugCardController.Identifier);
            PutInTrash(Cypher, dermal);
            PutInTrash(Cypher, muscle);

            StartGame();

            Card rebuilt = GetCard(RebuiltToSucceedCardController.Identifier);

            DecisionSelectCards = new[] {dermal, muscle, ra.CharacterCard, GetCardFromHand(ra, 0)};
            DecisionMoveCardDestination = new MoveCardDestination(Cypher.HeroTurnTaker.Hand);

            GoToPlayCardPhase(Cypher);
            PlayCard(rebuilt);


            Assert.True(false, "TODO");
        }


        [Test]
        public void TestVascularAug()
        {
            // Play this card next to a hero. The hero next to this card is augmented.
            // That hero regains 1HP at the end of their turn.


            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");

            StartGame();

            DecisionSelectTarget = Cypher.CharacterCard;

            Card vascularAug = GetCard(VascularAugCardController.Identifier);


            GoToPlayCardPhase(Cypher);
            PlayCard(vascularAug);
            GoToEndOfTurn(Cypher);

            Assert.True(false, "TODO");

        }

        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                AssertCardHasKeyword(GetCard(id), keyword, false);
            }
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
