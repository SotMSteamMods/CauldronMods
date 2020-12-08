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

        [Test]
        public void TestCyborgBlaster()
        {
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
        public void TestElectroOpticalCloak()
        {
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
            // Arrange
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");

            StartGame();


            Card muscleAug = GetCard(MuscleAugCardController.Identifier);


            GoToPlayCardPhase(Cypher);
            PlayCard(muscleAug);

            Assert.True(false, "TODO");
        }

        [Test]
        public void TestVascularAug()
        {
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

        [Test]
        public void TesDermalAug()
        {
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
        public void TestRebuiltToSucceedNoAugmentsInTrash()
        {
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

            DecisionSelectCards = new[] {dermal, muscle, ra.CharacterCard};
            DecisionMoveCardDestination = new MoveCardDestination(Cypher.HeroTurnTaker.Hand);

            GoToPlayCardPhase(Cypher);
            PlayCard(rebuilt);


            Assert.True(false, "TODO");
        }
    }
}
