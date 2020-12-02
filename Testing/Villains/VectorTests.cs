using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Cauldron.Vector;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;

using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture]
    public class VectorTests : BaseTest
    {
        protected TurnTakerController Vector => FindVillain("Vector");

        private const string DeckNamespace = "Cauldron.Vector";

        [Test]
        public void TestVectorDeckList()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card anticoagulant = GetCard(AnticoagulantCardController.Identifier);
            Assert.IsTrue(anticoagulant.DoKeywordsContain(new []{ "virus", "ongoing" }));

            Card assassinsSignature = GetCard(AssassinsSignatureCardController.Identifier);
            Assert.IsTrue(assassinsSignature.DoKeywordsContain("one-shot"));

            Card bioterrorSquad = GetCard(BioterrorSquadCardController.Identifier);
            Assert.IsTrue(bioterrorSquad.DoKeywordsContain(new []{ "pawn", "virus" }));

            Card bloodSample = GetCard(BloodSampleCardController.Identifier);
            Assert.IsTrue(bloodSample.DoKeywordsContain(new []{ "ongoing", "virus" }));

            Card delayedSymptoms = GetCard(DelayedSymptomsCardController.Identifier);
            Assert.IsTrue(delayedSymptoms.DoKeywordsContain(new [] { "one-shot", "virus" }));

            Card eliteTraining = GetCard(EliteTrainingCardController.Identifier);
            Assert.IsTrue(eliteTraining.DoKeywordsContain("ongoing"));

            Card hostageShield = GetCard(HostageShieldCardController.Identifier);
            Assert.IsTrue(hostageShield.DoKeywordsContain("ongoing"));

            Card hotZone = GetCard(HotZoneCardController.Identifier);
            Assert.IsTrue(hotZone.DoKeywordsContain("one-shot"));

            Card hyperactiveImmuneSystem = GetCard(HyperactiveImmuneSystemCardController.Identifier);
            Assert.IsTrue(hyperactiveImmuneSystem.DoKeywordsContain("ongoing"));

            Card lethalForce = GetCard(LethalForceCardController.Identifier);
            Assert.IsTrue(lethalForce.DoKeywordsContain("one-shot"));

            Card quarantineProtocols = GetCard(QuarantineProtocolsCardController.Identifier);
            Assert.IsTrue(quarantineProtocols.DoKeywordsContain("device"));

            Card supervirus = GetCard(SupervirusCardController.Identifier);
            Assert.IsTrue(supervirus.DoKeywordsContain(new [] { "ongoing", "virus" }));

            Card undiagnosedSubject = GetCard(UndiagnosedSubjectCardController.Identifier);
            Assert.IsTrue(undiagnosedSubject.DoKeywordsContain("pawn"));

            Card vendetta = GetCard(VendettaCardController.Identifier);
            Assert.IsTrue(vendetta.DoKeywordsContain("one-shot"));

            Card virulentBlade = GetCard(VirulentBladeCardController.Identifier);
            Assert.IsTrue(virulentBlade.DoKeywordsContain(new[] { "device", "virus" }));

            Card vrRazortail = GetCard(VrRazortailCardController.Identifier);
            Assert.IsTrue(vrRazortail.DoKeywordsContain("vehicle"));
        }

        [Test]
        public void TestVectorLoads()
        {
            SetupGameController(DeckNamespace, "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Vector);
            Assert.IsInstanceOf(typeof(VectorCharacterCardController), Vector.CharacterCardController);

            Assert.AreEqual(55, Vector.CharacterCard.HitPoints);
        }

        [Test]
        public void TestVectorSetsUp()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Vector);
            Assert.IsInstanceOf(typeof(VectorCharacterCardController), Vector.CharacterCardController);

            Assert.AreEqual(40, Vector.CharacterCard.HitPoints);
        }

        [Test]
        public void TestVectorPlaysCardWhenHit()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            int vectorCardsInPlay = GetNumberOfCardsInPlay(Vector);

            // Act
            GoToPlayCardPhase(legacy);
            DealDamage(legacy, Vector, 2, DamageType.Melee);

            // Assert
            Assert.AreEqual(vectorCardsInPlay + 1, GetNumberOfCardsInPlay(Vector));
        }

        [Test]
        public void TestAnticoagulant()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            QuickHPStorage(Vector, legacy, ra, haka);

            Card antiC = GetCard(AnticoagulantCardController.Identifier);

            PlayCard(antiC);

            // Act
            GoToPlayCardPhase(legacy);
            DealDamage(legacy, Vector, 5, DamageType.Melee);

            // Assert
            QuickHPCheck(-6, -6, -6, -6); // Anti Coag increases damage to Vector by 1
        }

        [Test]
        public void TestAssassinsSignature()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card mere = GetCard("Mere");
            Card dominion = GetCard("Dominion");
            Card savageMana = GetCard("SavageMana");

            PutIntoPlay(mere.Identifier);
            PutIntoPlay(dominion.Identifier);
            PutIntoPlay(savageMana.Identifier);

            DecisionSelectCard = dominion;
            QuickHPStorage(haka);

            // Act
            Card aSig = GetCard(AssassinsSignatureCardController.Identifier);
            PlayCard(aSig);


            // Assert
            QuickHPCheck(-3);
            AssertInPlayArea(haka, new []{ mere, savageMana});
            AssertInTrash(haka, dominion);
        }

        [Test]
        public void TestBioTerrorSquad()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            
            Card bloodSample = GetCard(BloodSampleCardController.Identifier); // Virus
            Card delayedSymptoms = GetCard(DelayedSymptomsCardController.Identifier); // Virus
            Card eliteTraining = GetCard(EliteTrainingCardController.Identifier);

            Card bioTerror = GetCard(BioterrorSquadCardController.Identifier);

            PutInTrash(bloodSample);
            PutInTrash(delayedSymptoms);
            PutInTrash(eliteTraining);

            StartGame();
            DecisionSelectCard = GetCard(DelayedSymptomsCardController.Identifier);

            GoToPlayCardPhase(Vector);
            QuickHPStorage(bioTerror, haka.CharacterCard);
            PlayCard(bioTerror);

            QuickShuffleStorage(Vector);

            // Act
            GoToStartOfTurn(Vector);

            // Assert
            QuickShuffleCheck(1);
            QuickHPCheck(-1, -3);
            AssertNotInTrash(delayedSymptoms);
        }

        [Test]
        public void TestBloodSampleMoveUnderSuperVirus()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card bloodSample = GetCard(BloodSampleCardController.Identifier);
            Card superVirus = GetCard(SupervirusCardController.Identifier);

            StartGame();
            QuickHPStorage(legacy, ra, haka);
            DecisionYesNo = true;

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(bloodSample);
            QuickHPCheck(-1, -1, -1);

            PlayCard(superVirus);

            DealDamage(legacy, Vector, 6, DamageType.Melee);

            GoToStartOfTurn(Vector);

            // Assert

            // Enough damage was done to offer to move Blood Sample under Super Virus
            AssertUnderCard(superVirus, bloodSample);
        }

        [Test]
        public void TestBloodSampleNotEnoughMoveDamage()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card bloodSample = GetCard(BloodSampleCardController.Identifier);
            Card superVirus = GetCard(SupervirusCardController.Identifier);

            StartGame();
            QuickHPStorage(legacy, ra, haka);
            DecisionYesNo = true;

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(bloodSample);
            QuickHPCheck(-1, -1, -1);

            PlayCard(superVirus);

            DealDamage(legacy, Vector, 2, DamageType.Melee);

            GoToStartOfTurn(Vector);

            // Assert

            // Not enough damage was done to offer to move Blood Sample under Super Virus
            AssertIsInPlayAndNotUnderCard(bloodSample);
        }


        [Test]
        public void TestDelayedSymptoms()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card delayedSymptoms = GetCard(DelayedSymptomsCardController.Identifier);

            Card dangerSense = GetCard("DangerSense");
            Card dominion = GetCard("Dominion");

            PutIntoPlay("DangerSense");
            PutIntoPlay("Dominion");
            PutIntoPlay("SavageMana");

            DecisionSelectCards = new[] {dangerSense, dominion};

            StartGame();

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(delayedSymptoms);

            // Assert
            AssertInTrash(legacy, dangerSense);
            AssertInTrash(haka, dominion);
            Assert.AreEqual(2, GetNumberOfCardsInTrash(legacy));
            Assert.AreEqual(1, GetNumberOfCardsInTrash(ra));
            Assert.AreEqual(2, GetNumberOfCardsInTrash(haka));
        }

        [Test]
        public void TestEliteTrainingIncreasesVillainDamage()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card eliteTraining = GetCard(EliteTrainingCardController.Identifier);

            StartGame();

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(eliteTraining);

            QuickHPStorage(legacy);

            DealDamage(Vector, legacy, 3, DamageType.Toxic);

            // Assert
            QuickHPCheck(-4);
        }

        [Test]
        public void TestEliteTrainingCausesPlayerDiscard()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card eliteTraining = GetCard(EliteTrainingCardController.Identifier);

            StartGame();

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(eliteTraining);

            QuickHandStorage(new []{legacy, ra, haka});

            DestroyCard(eliteTraining);

            // Assert
            QuickHandCheck(new []{-2, -2, -2});
        }


        [Test]
        public void TestHostageShieldAppliesNoDamageDealingEffect()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card hostageShield = GetCard(HostageShieldCardController.Identifier);

            StartGame();

            DecisionYesNo = true;

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(hostageShield);

            string messageText = $"Prevent damage from Ra.";
            AssertStatusEffectsContains(messageText);
            AssertNextToCard(hostageShield, ra.CharacterCard);
        }

        [Test]
        public void TestHostageShieldIsRemovedIfTurnIsSkipped()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card hostageShield = GetCard(HostageShieldCardController.Identifier);

            StartGame();

            DecisionYesNo = true;

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(hostageShield);

            string messageText = $"Prevent damage from Ra.";
            AssertStatusEffectsContains(messageText);
            AssertNextToCard(hostageShield, ra.CharacterCard);

            GoToStartOfTurn(ra);
            AssertNotNextToCard(hostageShield, ra.CharacterCard);
            AssertStatusEffectsDoesNotContain(messageText);
        }



        [Test]
        public void TestHotZone()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            
            StartGame();

            Card hotZone = GetCard(HotZoneCardController.Identifier);
            QuickHPStorage(legacy, ra, haka);


            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(hotZone);

            // Assert
            QuickHPCheck(-2, -2, -2);
            Assert.True(false, "TODO");
        }

        [Test]
        public void TestHyperactiveImmuneSystem()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();

            SetHitPoints(Vector, 10);

            Card hyperactive = GetCard(HyperactiveImmuneSystemCardController.Identifier);


            PlayCard(legacy, "DangerSense");
            PlayCard(legacy, "TheLegacyRing");
            PlayCard(legacy, "Mere");

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(hyperactive);
            GoToEndOfTurn(Vector);

            // Assert
            Assert.True(false, "TODO");
        }

        [Test]
        public void TestLethalForce()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();

            Card lethalForce = GetCard(LethalForceCardController.Identifier);

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(lethalForce);
            
            // Assert
            Assert.True(false, "TODO");
        }

        [Test]
        public void TestQuarrantineProtocol()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();

            Card quarantine = GetCard(QuarantineProtocolsCardController.Identifier);

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(quarantine);

            GoToEndOfTurn(Vector);

            // Assert
            Assert.True(false, "TODO");
        }

        [Test]
        public void TestSupervirus()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card bloodSample = GetCard(BloodSampleCardController.Identifier); // Virus
            Card delayedSymptoms = GetCard(DelayedSymptomsCardController.Identifier); // Virus
            Card eliteTraining = GetCard(EliteTrainingCardController.Identifier);

            PutInTrash(bloodSample);
            PutInTrash(delayedSymptoms);
            PutInTrash(eliteTraining);

            Card superVirus = GetCard(SupervirusCardController.Identifier);
            PlayCard(superVirus);

            StartGame();

            // Act

            GoToStartOfTurn(haka);

            DealDamage(haka, Vector, 100, DamageType.Melee);

            // Assert
            Assert.True(false, "TODO");
        }

        [Test]
        public void TestUndiagnosedSubject()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            StartGame();
            Card undiagnosed = GetCard(UndiagnosedSubjectCardController.Identifier);

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(undiagnosed);
            GoToStartOfTurn(legacy);

            // Assert
            Assert.True(false, "TODO");

        }

        [Test]
        public void TestVendetta()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            StartGame();
            Card vendetta = GetCard(VendettaCardController.Identifier);

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(vendetta);

            // Assert
            Assert.True(false, "TODO");
        }

        [Test]
        public void TestVirulentBlade()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            StartGame();
            Card virulentBlade = GetCard(VirulentBladeCardController.Identifier);

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(virulentBlade);
            GoToStartOfTurn(legacy);

            // Assert
            Assert.True(false, "TODO");
        }

        [Test]
        public void TestVrRazortail()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card policeBackup = GetCard("PoliceBackup");

            StartGame();
            Card razortail = GetCard(VrRazortailCardController.Identifier);

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(razortail);
            GoToEndOfTurn(haka);

            GoToStartOfTurn(env);
            PlayCard(policeBackup);

            GoToStartOfTurn(Vector);


            // Assert
            Assert.True(false, "TODO");
        }
    }
}
