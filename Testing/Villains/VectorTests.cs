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
    public class VectorTests : CauldronBaseTest
    {

        private const string DeckNamespace = "Cauldron.Vector";

        protected bool IsVirus(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "virus");
        }

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

            Assert.IsNotNull(vector);
            Assert.IsInstanceOf(typeof(VectorCharacterCardController), vector.CharacterCardController);

            Assert.AreEqual(55, vector.CharacterCard.HitPoints);
        }

        [Test]
        public void TestVectorSetsUp()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(vector);
            Assert.IsInstanceOf(typeof(VectorCharacterCardController), vector.CharacterCardController);
            Assert.AreEqual(40, vector.CharacterCard.HitPoints);
        }

        [Test]
        public void TestVectorPlaysCardWhenHit()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            int vectorCardsInPlay = GetNumberOfCardsInPlay(vector);
            int vectorCardsInTrash = GetNumberOfCardsInTrash(vector);

            // Act
            GoToPlayCardPhase(legacy);
            DealDamage(legacy, vector, 2, DamageType.Melee);

            // Assert
            Assert.True(GetNumberOfCardsInPlay(vector) > vectorCardsInPlay || GetNumberOfCardsInTrash(vector) > vectorCardsInTrash, "A card was not played");
        }

        [Test]
        public void TestVectorDoesNotPlaysCardWhenHitForZero()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            int vectorCardsInPlay = GetNumberOfCardsInPlay(vector);
            int vectorCardsInTrash = GetNumberOfCardsInTrash(vector);

            AddImmuneToDamageTrigger(vector, false, true);

            // Act
            GoToPlayCardPhase(legacy);
            DealDamage(legacy, vector, 2, DamageType.Melee);

            // Assert
            Assert.True(GetNumberOfCardsInPlay(vector) == vectorCardsInPlay && GetNumberOfCardsInTrash(vector) == vectorCardsInTrash, "A card was played when no damage was dealt");
        }

        [Test]
        public void TestVectorFlipCondition()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card supervirus = PlayCard("Supervirus");

            //get H+2 (5) viruses to move on supervirus
            IEnumerable<Card> virusToMove = FindCardsWhere((Card c) => IsVirus(c) && vector.TurnTaker.Deck.HasCard(c)).Take(5);

            AssertNotFlipped(vector.CharacterCard);
            MoveCards(vector, virusToMove, supervirus.UnderLocation);
            AssertFlipped(vector.CharacterCard);
        }
        [Test]
        public void TestVectorFlipRemovesTriggers()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card sample1 = PutOnDeck("BloodSample");
            DealDamage(legacy, vector, 4, DamageType.Melee);
            AssertIsInPlay(sample1);

            FlipCard(vector);

            Card sample2 = PutOnDeck("BloodSample");
            DealDamage(legacy, vector, 4, DamageType.Melee);
            AssertOnTopOfDeck(sample2);
        }

        [Test]
        public void TestVectorWinsIfGainsFullHp()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            GainHP(vector, 15);

            AssertNotFlipped(vector);
            AssertGameOver(EndingResult.AlternateDefeat);
        }

        [Test]
        public void TestVectorBecomesImmuneToEnvFromEnvDamage()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "MaerynianRefuge");
            StartGame();

            QuickHPStorage(vector);

            Card hail = GetCard("SoftballSizedHail");
            PlayCard(hail);
            GoToEndOfTurn(env);

            //QuickHPCheck(-2); // Now immune to env damage

            string messageText = "Vector is immune to damage from environment cards.";
            AssertStatusEffectsContains(messageText);

            AssertNotFlipped(vector);
        }

        [Test]
        public void TestAdvancedGainHpAtEndOfVillainTurn()
        {
            SetupGameController(new[] { DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis" },
                advanced: true, advancedIdentifiers: new[] { DeckNamespace });

            StartGame();
            QuickHPStorage(vector);

            GoToEndOfTurn(vector);

            AssertNotFlipped(vector);
            QuickHPCheck(2);
        }

        [Test]
        public void TestWhenFlippedSuperVirusIsRemovedFromGame()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card superVirus = GetCard(SupervirusCardController.Identifier);

            StartGame();
            PlayCard(superVirus);

            FlipCard(vector.CharacterCardController);

            AssertFlipped(vector);
            AssertOutOfGame(superVirus);
        }

        [Test]
        public void TestWhenFlippedCardsUnderSuperVirusAreReturnedToVillainTrash()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card bloodSample = GetCard(BloodSampleCardController.Identifier); // Virus
            Card delayedSymptoms = GetCard(DelayedSymptomsCardController.Identifier); // Virus

            Card superVirus = GetCard(SupervirusCardController.Identifier);

            MoveCard(vector, bloodSample, superVirus.UnderLocation);
            MoveCard(vector, delayedSymptoms, superVirus.UnderLocation);

            StartGame();
            PlayCard(superVirus);

            FlipCard(vector.CharacterCardController);

            AssertFlipped(vector);
            AssertOutOfGame(superVirus);
            AssertInTrash(bloodSample);
            AssertInTrash(delayedSymptoms);

        }

        [Test]
        public void TestSuperVirusAndCardsUnderneathAreIndestructible()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "TheArgentAdept", "Megalopolis");

            Card bloodSample = GetCard(BloodSampleCardController.Identifier); // Virus
            Card delayedSymptoms = GetCard(DelayedSymptomsCardController.Identifier); // Virus

            Card superVirus = GetCard(SupervirusCardController.Identifier);

            MoveCard(vector, bloodSample, superVirus.UnderLocation);
            MoveCard(vector, delayedSymptoms, superVirus.UnderLocation);

            StartGame();
            PlayCard(superVirus);

            Card cedistic = PlayCard("CedisticDissonant");
            Card pipes = PlayCard("DrakesPipes");
            DecisionSelectCard = superVirus;
            UsePower(adept);
            AssertInPlayArea(vector, superVirus);

            foreach (Card under in superVirus.UnderLocation.Cards)
            {
                PlayCard(pipes);
                DecisionSelectCard = under;
                UsePower(adept);
                AssertUnderCard(superVirus, under);
            }
        }

        [Test]
        public void TestFlippedVectorPlaysCardAtEndOfVillainTurn()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            FlipCard(vector.CharacterCardController);
            AssertFlipped(vector);

            int vectorCardsInPlay = GetNumberOfCardsInPlay(vector);
            int vectorCardsInTrash = GetNumberOfCardsInTrash(vector);

            // Act
            GoToEndOfTurn(vector);

            // Assert
            Assert.True(GetNumberOfCardsInPlay(vector) > vectorCardsInPlay || GetNumberOfCardsInTrash(vector) > vectorCardsInTrash);
        }

        [Test]
        public void TestFlippedReduceVectorDamageTakenByVillainTargetCount()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            PutOnDeck(vector, GetCard(HotZoneCardController.Identifier));
            PutOnDeck(vector, GetCard(HyperactiveImmuneSystemCardController.Identifier));

            Card bioTerror = GetCard(BioterrorSquadCardController.Identifier);
            PlayCard(bioTerror);

            FlipCard(vector.CharacterCardController);

            QuickHPStorage(vector);

            GoToStartOfTurn(haka);
            DealDamage(haka, vector, 4, DamageType.Melee);


            QuickHPCheck(-2); // Damage was reduced by 2 (2 villain targets: Vector, Bio Terror Squad)
            AssertFlipped(vector);

            //Check for dynamicism
            PlayCard("QuarantineProtocols");
            DealDamage(haka, vector, 4, DamageType.Melee);
            QuickHPCheck(-1); //3 targets: Vector, Squad, Protocols
        }

        [Test]
        public void TestFlippedAdvancedIncreasedVectorDamage()
        {
            SetupGameController(new[] { DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis" },
                advanced: true, advancedIdentifiers: new[] { DeckNamespace });

            StartGame();
            FlipCard(vector.CharacterCardController);

            QuickHPStorage(legacy);

            DealDamage(vector, legacy, 2, DamageType.Toxic);

            QuickHPCheck(-4);
            AssertFlipped(vector);

        }

        [Test]
        public void TestAnticoagulant()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

 

            Card antiC = GetCard(AnticoagulantCardController.Identifier);

            PlayCard(antiC);

            //change phase to help with consistency
            GoToPlayCardPhase(legacy);
            PutOnDeck("BioterrorSquad");

            QuickHPStorage(vector, legacy, ra, haka);
            // Act
            DealDamage(legacy, vector, 5, DamageType.Melee);

            // Assert
            QuickHPCheck(-6, -6, -6, -6); // Anti Coag increases damage to Vector by 1, response to hit everyone
            AssertInTrash(antiC);
        }

        [Test]
        public void TestAnticoagulant_InfiniteLoop()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "AkashThriya", "Megalopolis");
            StartGame();

            Card antiC = GetCard(AnticoagulantCardController.Identifier);

            PlayCard(antiC);

            Card vitalizedThorn = MoveCard(thriya, "VitalizedThorns", env.TurnTaker.Deck);
            PlayCard(vitalizedThorn);

            //change phase to help with consistency
            GoToPlayCardPhase(legacy);
            PutOnDeck("BioterrorSquad");
            PutOnDeck("UndiagnosedSubject");


            QuickHPStorage(vector, legacy, ra, thriya);

            // Act
            DealDamage(legacy, vector, 1, DamageType.Melee);

            // Assert
            QuickHPCheck(-3, -2, -2, -2);
            AssertInTrash(antiC);
        }

        [Test]
        public void TestAnticoagulant_NoResponseOnZeroDamage()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            QuickHPStorage(vector);

            Card antiC = GetCard(AnticoagulantCardController.Identifier);

            PlayCard(antiC);

            AddImmuneToDamageTrigger(vector, false, true);

            // Act
            DealDamage(legacy, vector, 5, DamageType.Melee);

            // Assert
            AssertInPlayArea(vector, antiC);
        }

        [Test]
        public void TestAssassinsSignature_DestroyOngoing()
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
            QuickHPStorage(vector, legacy, ra, haka);

            // Act
            Card aSig = PlayCard(AssassinsSignatureCardController.Identifier);


            // Assert
            QuickHPCheck(0,0,0,-3);
            AssertInPlayArea(haka, new []{ mere, savageMana});
            AssertInTrash(haka, dominion);
        }

        [Test]
        public void TestAssassinsSignature_DestructionEvenIfNoDamage()
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
            QuickHPStorage(vector, legacy, ra, haka);

            AddImmuneToDamageTrigger(ra, true, false);

            // Act
            Card aSig = PlayCard(AssassinsSignatureCardController.Identifier);


            // Assert
            QuickHPCheck(0, 0, 0, 0);
            AssertInPlayArea(haka, new[] { mere, savageMana });
            AssertInTrash(haka, dominion);
        }
        [Test]
        public void TestAssassinsSignature_DestructionEvenIfCannotDealDamage()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card mere = GetCard("Mere");
            Card dominion = GetCard("Dominion");
            Card savageMana = GetCard("SavageMana");
            Card groundPound = GetCard("GroundPound");

            DecisionYesNo = true;
            PlayCard(groundPound);
            PlayCard(mere);
            PlayCard(dominion);
            PlayCard(savageMana);

            DecisionSelectCard = dominion;
            QuickHPStorage(vector, legacy, ra, haka);

            AddImmuneToDamageTrigger(ra, true, false);

            // Act
            Card aSig = PlayCard(AssassinsSignatureCardController.Identifier);


            // Assert
            QuickHPCheck(0, 0, 0, 0);
            AssertInPlayArea(haka, new[] { mere, savageMana });
            AssertInTrash(haka, dominion);
        }

        [Test]
        public void TestAssassinsSignature_DestroyEquipment()
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

            DecisionSelectCard = mere;
            QuickHPStorage(vector, legacy, ra, haka);

            // Act
            Card aSig = PlayCard(AssassinsSignatureCardController.Identifier);


            // Assert
            QuickHPCheck(0, 0, 0, -3);
            AssertInPlayArea(haka, new[] { dominion, savageMana });
            AssertInTrash(haka, mere);
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

            GoToPlayCardPhase(vector);
            QuickHPStorage(bioTerror, haka.CharacterCard);
            PlayCard(bioTerror);

            QuickShuffleStorage(vector);

            // Act
            GoToStartOfTurn(vector);

            // Assert
            QuickShuffleCheck(1);
            QuickHPCheck(-1, -3);
            AssertNotInTrash(delayedSymptoms);
        }

        [Test]
        public void TestBioTerrorSquad_NoVirusInTrash()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card bioTerror = GetCard(BioterrorSquadCardController.Identifier);

            StartGame();

            GoToPlayCardPhase(vector);
            PlayCard(bioTerror);

            QuickShuffleStorage(vector);

            // Act
            GoToStartOfTurn(vector);

            // Assert
            QuickShuffleCheck(0);

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
            GoToPlayCardPhase(vector);
            PlayCard(bloodSample);
            QuickHPCheck(-1, -1, -1);

            PlayCard(superVirus);

            DealDamage(legacy, vector, 6, DamageType.Melee);

            GoToStartOfTurn(vector);

            // Assert
             // Enough damage was done to offer to move Blood Sample under Super Virus
            AssertUnderCard(superVirus, bloodSample);
        }

        [Test]
        public void TestBloodSampleMoveUnderSuperVirus_FlipsVector()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card bloodSample = GetCard(BloodSampleCardController.Identifier);
            Card superVirus = GetCard(SupervirusCardController.Identifier);

            StartGame();
            QuickHPStorage(legacy, ra, haka);
            DecisionYesNo = true;

            // Act
            GoToPlayCardPhase(vector);
            PlayCard(bloodSample);
            QuickHPCheck(-1, -1, -1);

            PlayCard(superVirus);

            DealDamage(legacy, vector, 6, DamageType.Melee);

            //get H+1 (4) viruses to move on supervirus
            IEnumerable<Card> virusToMove = FindCardsWhere((Card c) => IsVirus(c) && vector.TurnTaker.Deck.HasCard(c)).Take(4);
            MoveCards(vector, virusToMove, superVirus.UnderLocation);

            GoToStartOfTurn(vector);

            // Assert

            AssertFlipped(vector.CharacterCard);


        }


        [Test]
        public void TestBloodSampleMoveUnderSuperVirus_Optional()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card bloodSample = GetCard(BloodSampleCardController.Identifier);
            Card superVirus = GetCard(SupervirusCardController.Identifier);

            StartGame();
            QuickHPStorage(legacy, ra, haka);
            DecisionYesNo = false;

            // Act
            GoToPlayCardPhase(vector);
            PlayCard(bloodSample);
            QuickHPCheck(-1, -1, -1);

            PlayCard(superVirus);

            DealDamage(legacy, vector, 6, DamageType.Melee);

            GoToStartOfTurn(vector);

            // Assert

            // Enough damage was done to offer to move Blood Sample under Super Virus
            AssertIsInPlayAndNotUnderCard(bloodSample);
        }

        [Test]
        public void TestBloodSampleDontAttemptMoving_NoSupervirus()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card bloodSample = GetCard(BloodSampleCardController.Identifier);

            StartGame();
            QuickHPStorage(legacy, ra, haka);
            DecisionYesNo = true;

            // Act
            GoToPlayCardPhase(vector);
            PlayCard(bloodSample);
            QuickHPCheck(-1, -1, -1);

            PutOnDeck("BioterrorSquad");
            DealDamage(legacy, vector, 6, DamageType.Melee);

            GoToStartOfTurn(vector);

            // Assert

            // Enough damage was done to offer to move Blood Sample under Super Virus
            AssertIsInPlayAndNotUnderCard(bloodSample);
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
            GoToPlayCardPhase(vector);
            PlayCard(bloodSample);
            QuickHPCheck(-1, -1, -1);

            PlayCard(superVirus);

            DealDamage(legacy, vector, 2, DamageType.Melee);

            GoToStartOfTurn(vector);

            // Assert

            // Not enough damage was done to offer to move Blood Sample under Super Virus
            AssertIsInPlayAndNotUnderCard(bloodSample);
        }


        [Test]
        public void TestDelayedSymptoms_DestroyOngoing()
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
            GoToPlayCardPhase(vector);
            PlayCard(delayedSymptoms);

            // Assert
            AssertInTrash(legacy, dangerSense);
            AssertInTrash(haka, dominion);
            Assert.AreEqual(2, GetNumberOfCardsInTrash(legacy));
            Assert.AreEqual(1, GetNumberOfCardsInTrash(ra));
            Assert.AreEqual(2, GetNumberOfCardsInTrash(haka));
        }

        public void TestDelayedSymptoms_DestroyEquipment()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card delayedSymptoms = GetCard(DelayedSymptomsCardController.Identifier);

            Card dangerSense = PlayCard("DangerSense");
            Card mere = PlayCard("Mere");

            
            PutIntoPlay("SavageMana");

            DecisionSelectCards = new[] { dangerSense, mere };

            StartGame();

            // Act
            GoToPlayCardPhase(vector);
            PlayCard(delayedSymptoms);

            // Assert
            AssertInTrash(legacy, dangerSense);
            AssertInTrash(haka, mere);
            Assert.AreEqual(2, GetNumberOfCardsInTrash(legacy));
            Assert.AreEqual(1, GetNumberOfCardsInTrash(ra));
            Assert.AreEqual(2, GetNumberOfCardsInTrash(haka));
        }

        [Test()]
        public void TestDelayedSymptoms_NoCardsInDeckForcesReshuffle()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            //put all legacy cards in trash
            DiscardTopCards(legacy, 36);
            Card delayedSymptoms = GetCard(DelayedSymptomsCardController.Identifier);

            Card dangerSense = PlayCard("DangerSense");
            Card mere = PlayCard("Mere");


            PutIntoPlay("SavageMana");

            DecisionSelectCards = new[] { dangerSense, mere };

            StartGame();

            // Act
            GoToPlayCardPhase(vector);

            QuickShuffleStorage(legacy);

            PlayCard(delayedSymptoms);

            // Assert
            AssertNotInPlayArea(legacy, dangerSense);
            AssertInTrash(haka, mere);
            Assert.AreEqual(1, GetNumberOfCardsInTrash(legacy));
            Assert.AreEqual(1, GetNumberOfCardsInTrash(ra));
            Assert.AreEqual(2, GetNumberOfCardsInTrash(haka));
            QuickShuffleCheck(1);
        }

        [Test]
        public void TestEliteTrainingIncreasesVillainDamage()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card eliteTraining = GetCard(EliteTrainingCardController.Identifier);

            StartGame();

            // Act
            GoToPlayCardPhase(vector);
            PlayCard(eliteTraining);

            QuickHPStorage(legacy, ra);

            DealDamage(vector, legacy, 3, DamageType.Toxic);

            //doesn't increase hero damage
            DealDamage(legacy, ra, 3, DamageType.Melee);

            // Assert
            QuickHPCheck(-4, -3);
        }

        [Test]
        public void TestEliteTrainingCausesPlayerDiscard()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card eliteTraining = GetCard(EliteTrainingCardController.Identifier);

            StartGame();

            // Act
            GoToPlayCardPhase(vector);
            PlayCard(eliteTraining);
            DiscardAllCards(ra);
            QuickHandStorage(new []{legacy, ra, haka});

            DestroyCard(eliteTraining);

            // Assert
            QuickHandCheck(new []{-2, 0, -2});
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
            GoToPlayCardPhase(vector);
            Card shield = PlayCard(hostageShield);
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
            GoToPlayCardPhase(vector);
            PlayCard(hostageShield);

            string messageText = $"Prevent damage from Ra.";
            AssertStatusEffectsContains(messageText);
            AssertNextToCard(hostageShield, ra.CharacterCard);

            GoToStartOfTurn(ra);
            AssertNotNextToCard(hostageShield, ra.CharacterCard);
            AssertStatusEffectsDoesNotContain(messageText);
        }

        [Test]
        public void TestHostageShield_EffectIsRemovedIfDestroyed()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card hostageShield = GetCard(HostageShieldCardController.Identifier);

            StartGame();

            DecisionYesNo = true;

            // Act
            GoToPlayCardPhase(vector);
            PlayCard(hostageShield);
            QuickHPStorage(legacy);
            DealDamage(ra, legacy, 3, DamageType.Fire);
            QuickHPCheck(0);
            string messageText = $"Prevent damage from Ra.";
            DestroyCard(hostageShield, vector.CharacterCard);
            AssertStatusEffectsDoesNotContain(messageText);

            QuickHPStorage(legacy);
            DealDamage(ra, legacy, 3, DamageType.Fire);
            QuickHPCheck(-3);
        }

        [Test]
        public void TestHostageShieldIsNotRemovedIfTurnIsNotSkipped()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card hostageShield = GetCard(HostageShieldCardController.Identifier);

            StartGame();

            DecisionYesNo = false;

            // Act
            GoToPlayCardPhase(vector);
            PlayCard(hostageShield);

            string messageText = $"Prevent damage from Ra.";
            AssertStatusEffectsContains(messageText);
            AssertNextToCard(hostageShield, ra.CharacterCard);

            GoToStartOfTurn(ra);
            GoToPlayCardPhase(ra);
            PlayCard(ra, GetCardFromHand(ra, 0));
            AssertNextToCard(hostageShield, ra.CharacterCard);
            AssertStatusEffectsContains(messageText);
        }

        [Test]
        public void TestHostageShieldAnyHeroMayRemove()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card hostageShield = GetCard(HostageShieldCardController.Identifier);

            StartGame();

            DecisionYesNo = true;

            // Act
            GoToPlayCardPhase(vector);
            PlayCard(hostageShield);

            string messageText = $"Prevent damage from Ra.";
            AssertStatusEffectsContains(messageText);
            AssertNextToCard(hostageShield, ra.CharacterCard);

            GoToStartOfTurn(legacy);
            AssertNotNextToCard(hostageShield, ra.CharacterCard);
            AssertStatusEffectsDoesNotContain(messageText);
        }

        [Test]
        public void TestHotZone()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "InsulaPrimalis");
            
            StartGame();

            PutIntoPlay("EnragedTRex");
            Card enragedTRex = GetCardInPlay("EnragedTRex");

            Card hotZone = GetCard(HotZoneCardController.Identifier);
            QuickHPStorage(vector.CharacterCard, legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, enragedTRex);

            // Act
            GoToPlayCardPhase(vector);
            PlayCard(hotZone);

            // Assert
            QuickHPCheck(0, -2, -2, -2, -2);
        }

        [Test]
        public void TestHyperactiveImmuneSystem()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();

            SetHitPoints(vector, 10);
            QuickHPStorage(vector);

            Card hyperactive = GetCard(HyperactiveImmuneSystemCardController.Identifier);


            PlayCard(legacy, "DangerSense");
            PlayCard(legacy, "TheLegacyRing");
            PlayCard(legacy, "Mere");

            // Act
            GoToPlayCardPhase(vector);
            PlayCard(hyperactive);
            GoToEndOfTurn(vector);

            // Assert
            QuickHPCheck(3);
        }

        [Test]
        public void TestLethalForce()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();


            DecisionLowestHP = ra.CharacterCard;

            QuickHPStorage(vector, legacy, ra, haka);

            Card lethalForce = GetCard(LethalForceCardController.Identifier);

            // Act
            GoToPlayCardPhase(vector);
            PlayCard(lethalForce);
            
            // Assert
            QuickHPCheck(0, -2, -1, 0);
        }

        [Test]
        public void TestQuarantineProtocolMakesEnvironmentImmuneToDamage()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "InsulaPrimalis");

            StartGame();

            PutIntoPlay("EnragedTRex");
            Card enragedTRex = GetCardInPlay("EnragedTRex");
            Card quarantine = GetCard(QuarantineProtocolsCardController.Identifier);

            QuickHPStorage(enragedTRex);

            // Act
            GoToPlayCardPhase(vector);
            PlayCard(quarantine);

            GoToStartOfTurn(legacy);
            DealDamage(legacy, enragedTRex, 4, DamageType.Melee);

            // Assert
            QuickHPCheck(0);
        }

        [Test]
        public void TestQuarantineProtocolCausesHeroDiscard()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "InsulaPrimalis");

            StartGame();

            Card quarantine = GetCard(QuarantineProtocolsCardController.Identifier);

            QuickHandStorage(legacy, ra, haka);

            // Act
            GoToPlayCardPhase(vector);
            PlayCard(quarantine);

            GoToEndOfTurn(vector);

            // Assert
            QuickHandCheck(-1, -1, -1);
        }

        [Test]
        public void TestQuarantineProtocolDealsHeroesDamageWhenDestroyed()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "InsulaPrimalis");

            StartGame();

            QuickHPStorage(legacy, ra, haka);

            Card quarantine = GetCard(QuarantineProtocolsCardController.Identifier);

            // Act
            GoToPlayCardPhase(vector);
            PlayCard(quarantine);

            DestroyCard(quarantine);

            // Assert
            QuickHPCheck(-3, -3, -3);
        }

        [Test]
        public void TestSupervirusAllowsPlacingVirusCardsUnderneath_FlipsCard()
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
           

            DecisionSelectCard = bloodSample;
            StartGame();
            GoToEndOfTurn(env);
            QuickHPStorage(vector, legacy, ra, haka);
            PlayCard(superVirus);

            //get H+1 (4) viruses to move on supervirus
            IEnumerable<Card> virusToMove = FindCardsWhere((Card c) => IsVirus(c) && vector.TurnTaker.Deck.HasCard(c)).Take(4);
            MoveCards(vector, virusToMove, superVirus.UnderLocation);

            GoToStartOfTurn(vector);

    


            // Assert
            QuickHPCheck(0, 0, 0, 0);
        }

        [Test]
        public void TestSupervirusAllowsPlacingVirusCardsUnderneath()
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

            DecisionSelectCard = bloodSample;

            StartGame();

            // Assert
            AssertUnderCard(superVirus, bloodSample);
        }
        [Test]
        public void TestSupervirusAllowsPlacingVirusCardsUnderneath_CannotPlayDoesNotPrevent()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card bloodSample = GetCard(BloodSampleCardController.Identifier); // Virus
            Card delayedSymptoms = GetCard(DelayedSymptomsCardController.Identifier); // Virus
            Card eliteTraining = GetCard(EliteTrainingCardController.Identifier);

            PutInTrash(bloodSample);
            PutInTrash(delayedSymptoms);
            PutInTrash(eliteTraining);
            StartGame();

            GoToStartOfTurn(base.env);
            Card superVirus = GetCard(SupervirusCardController.Identifier);
            PlayCard(superVirus);
            PlayCard("TakeDown");

            DecisionSelectCard = bloodSample;

            GoToStartOfTurn(vector);
            // Assert
            AssertUnderCard(superVirus, bloodSample);
        }



        [Test]
        public void TestSuperVirusDealsHeroesDamageAtVillainTurnStart()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            QuickHPStorage(legacy, ra, haka);

            Card superVirus = GetCard(SupervirusCardController.Identifier);
            PlayCard(superVirus);

            StartGame();

            // Act

            GoToStartOfTurn(haka);

            // Assert
            QuickHPCheck(-1, -1, -1);
        }

        [Test]
        public void TestSuperVirusHealsVectorAtVillainTurnStart()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card superVirus = GetCard(SupervirusCardController.Identifier);

            StartGame();
            QuickHPStorage(vector);
            PlayCard(superVirus);

            // Act

            GoToStartOfTurn(haka);
            GoToStartOfTurn(vector);

            // Assert
            QuickHPCheck(6); // H * 2 HP regained
        }

        [Test]
        public void TestSuperVirusCausesGameLossIfVectorIsDestroyedWhileItsOut()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card superVirus = GetCard(SupervirusCardController.Identifier);

            StartGame();
            
            GoToPlayCardPhase(vector);
            PlayCard(superVirus);

            // Act
            GoToStartOfTurn(legacy);
            DealDamage(legacy, vector, 100, DamageType.Melee);

            // Assert
            AssertGameOver(EndingResult.AlternateDefeat);
        }

        [Test]
        public void TestSuperVirusOnlyCausesGameLossIfVectorIsDestroyedWhileItsOut()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            StartGame();

            GoToPlayCardPhase(vector);

            // Act
            GoToStartOfTurn(legacy);
            DealDamage(legacy, vector, 100, DamageType.Melee);

            // Assert
            AssertGameOver(EndingResult.VillainDestroyedVictory);
        }

        [Test]
        public void TestSuperVirusOnlyCausesGameLossIfVectorIsDestroyedWhileItsOut_FixedPointTest()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "TimeCataclysm");


            StartGame();

            GoToPlayCardPhase(vector);

            // Act
            GoToStartOfTurn(legacy);
            PlayCard("FixedPoint");
            DealDamage(legacy, vector, 100, DamageType.Melee);

            // Assert
            AssertNotGameOver();
        }

        [Test]
        public void TestUndiagnosedSubjectPlaysVillainCardAtEndOfVillainTurn()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            PutOnDeck(vector, GetCard(HotZoneCardController.Identifier));

            StartGame();
            Card undiagnosed = GetCard(UndiagnosedSubjectCardController.Identifier);


            // Act
            QuickTopCardStorage(vector);

            GoToPlayCardPhase(vector);
            PlayCard(undiagnosed);
            GoToEndOfTurn(vector);
            GoToStartOfTurn(legacy);

            // Assert
            QuickTopCardCheck(ttc => ttc.TurnTaker.Trash);
        }

        [Test]
        public void TestUndiagnosedSubjectPlaysVillainCardWhenDestroyed_SuperVirusNotInPlayDiscard()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            PutOnDeck(vector, GetCard(HotZoneCardController.Identifier));

            StartGame();
            Card undiagnosed = GetCard(UndiagnosedSubjectCardController.Identifier);

            DecisionSelectFunction = 0; // Discard

            // Act
            QuickTopCardStorage(vector);

            GoToPlayCardPhase(vector);
            PlayCard(undiagnosed);

            DestroyCard(undiagnosed);

            // Assert
            QuickTopCardCheck(ttc => ttc.TurnTaker.Trash);
            AssertNumberOfCardsInRevealed(vector, 0);
        }

        [Test]
        public void TestUndiagnosedSubjectPlaysVillainCardWhenDestroyed_SuperVirusNotInPlayReplace()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            PutOnDeck(vector, GetCard(HotZoneCardController.Identifier));

            StartGame();
            Card undiagnosed = GetCard(UndiagnosedSubjectCardController.Identifier);

            DecisionSelectFunction = 1; // Replace

            // Act
            QuickTopCardStorage(vector);

            GoToPlayCardPhase(vector);
            PlayCard(undiagnosed);

            DestroyCard(undiagnosed);

            // Assert
            QuickTopCardCheck(ttc => ttc.TurnTaker.Deck);
            AssertNumberOfCardsInRevealed(vector, 0);

        }

        [Test]
        public void TestUndiagnosedSubjectPlaysVillainCardWhenDestroyed_SuperVirusInPlay()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card bioTerror = GetCard(BioterrorSquadCardController.Identifier);
            PutOnDeck(vector, bioTerror);

            StartGame();
            Card undiagnosed = GetCard(UndiagnosedSubjectCardController.Identifier);
            Card superVirus = GetCard(SupervirusCardController.Identifier);

            DecisionSelectFunction = 2; // Put under Super Virus

            // Act

            GoToPlayCardPhase(vector);
            PlayCard(superVirus);
            PlayCard(undiagnosed);

            DestroyCard(undiagnosed);

            // Assert
            AssertUnderCard(superVirus, bioTerror);
            AssertNumberOfCardsInRevealed(vector, 0);

        }

        [Test]
        public void TestUndiagnosedSubjectPlaysVillainCardWhenDestroyed_SuperVirusInPlay_TriggerFlip()
        {
            // Arrange
            SetupGameController(new string[] {DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis"}, randomSeed: new int?(928483507));

            Card bioTerror = GetCard(BioterrorSquadCardController.Identifier);
            PutOnDeck(vector, bioTerror);

            StartGame();
            Card undiagnosed = GetCard(UndiagnosedSubjectCardController.Identifier);
            Card superVirus = GetCard(SupervirusCardController.Identifier);

            DecisionSelectFunction = 2; // Put under Super Virus

            // Act

            GoToPlayCardPhase(vector);
            PlayCard(superVirus);
            //get H+1 (4) viruses to move on supervirus
            IEnumerable<Card> virusToMove = FindCardsWhere((Card c) => IsVirus(c) && vector.TurnTaker.Deck.HasCard(c) && c != bioTerror).Take(4);
            MoveCards(vector, virusToMove, superVirus.UnderLocation);

            PlayCard(undiagnosed);


            DestroyCard(undiagnosed);

            // Assert
            AssertNumberOfCardsInRevealed(vector, 0);
            AssertFlipped(vector.CharacterCard);

        }

        [Test]
        public void TestVendetta()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            StartGame();
            Card vendetta = GetCard(VendettaCardController.Identifier);
            QuickHPStorage(haka);
            QuickHandStorage(haka);

            // Act
            GoToPlayCardPhase(vector);
            PlayCard(vendetta);

            // Assert
            QuickHPCheck(-2); 
            QuickHandCheck(-1);
        }
        [Test]
        public void TestVendetta_DiscardEvenIfNoDamageDealt()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            StartGame();
            Card vendetta = GetCard(VendettaCardController.Identifier);
            QuickHPStorage(haka);
            QuickHandStorage(haka);

            // Act
            GoToPlayCardPhase(vector);
            AddImmuneToDamageTrigger(legacy, true, false);
            PlayCard(vendetta);

            // Assert
            QuickHPCheck(0);
            QuickHandCheck(-1);
        }
        [Test]
        public void TestVendetta_DiscardEvenIfCannotDealDamage()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            StartGame();
            Card vendetta = GetCard(VendettaCardController.Identifier);

            DecisionYesNo = true;
            PlayCard("GroundPound");

            QuickHPStorage(haka);
            QuickHandStorage(haka);
            // Act
            GoToPlayCardPhase(vector);
            AddImmuneToDamageTrigger(legacy, true, false);
            PlayCard(vendetta);

            // Assert
            QuickHPCheck(0);
            QuickHandCheck(-1);
        }

        [Test]
        public void TestVendetta_DiscardEvenIfRedirectedToDifferentHero()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Tachyon", "Megalopolis");


            StartGame();

            SetHitPoints(new List<TurnTakerController>() { legacy, ra, haka }, 10);
            Card vendetta = GetCard(VendettaCardController.Identifier);
            QuickHPStorage(tachyon, ra);
            QuickHandStorage(tachyon);

            // Act
            GoToPlayCardPhase(vector);
            PlayCard("SynapticInterruption");
            DecisionSelectCards = new Card[] { ra.CharacterCard, tachyon.HeroTurnTaker.Hand.TopCard };
            PlayCard(vendetta);

            // Assert
            QuickHPCheck(0, -3);
            QuickHandCheck(-1);
        }

        [Test]
        public void TestVendetta_DiscardEvenIfRedirectedToVillain()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Tachyon", "Megalopolis");


            StartGame();

            SetHitPoints(new List<TurnTakerController>() { legacy, ra, haka }, 10);
            Card vendetta = GetCard(VendettaCardController.Identifier);
            QuickHPStorage(tachyon, vector);
            QuickHandStorage(tachyon);

            // Act
            GoToPlayCardPhase(vector);
            PlayCard("SynapticInterruption");
            DecisionSelectCards = new Card[] { vector.CharacterCard, tachyon.HeroTurnTaker.Hand.TopCard };
            AddCannotPlayCardsStatusEffect(vector, false, true);

            PlayCard(vendetta, true);

            // Assert
            QuickHPCheck(0, -3);
            QuickHandCheck(-1);
        }

        [Test]
        public void TestVirulentBlade_SelfDamage()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");



            StartGame();
            Card virulentBlade = GetCard(VirulentBladeCardController.Identifier);
            QuickHPStorage(vector, legacy, ra, haka);

            // Act
            GoToPlayCardPhase(vector);
            AddCannotPlayCardsStatusEffect(vector, false, true);
            AddReduceDamageTrigger(vector, false, true, 2);
            PlayCard(virulentBlade, true);

            // Assert

            // Vector: -2 from Virulent Blade
            QuickHPCheck(-2, 0, 0, 0);

        }

        [Test]
        public void TestVirulentBlade_EndOfTurn()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");



            StartGame();
            Card virulentBlade = GetCard(VirulentBladeCardController.Identifier);

            // Act
            GoToPlayCardPhase(vector);
            AddCannotPlayCardsStatusEffect(vector, false, true);
            PlayCard(virulentBlade, true);
            QuickHPStorage(vector, legacy, ra, haka);
            GoToEndOfTurn(vector);
            // Assert

            // Heroes: -2 from Virulent Blade
            QuickHPCheck(0, -2, -2, -2);

        }


        [Test]
        public void TestVrRazortail()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();
            Card razortail = GetCard(VrRazortailCardController.Identifier);

            // Act
            GoToPlayCardPhase(vector);
            PlayCard(razortail);
            QuickHPStorage(razortail);

            DealDamage(haka, razortail, 4, DamageType.Melee);

            QuickHPCheck(-3);

            //check only reduces self

            QuickHPStorage(vector);
            AddCannotPlayCardsStatusEffect(vector, false, true);
            DealDamage(haka, vector, 4, DamageType.Melee);
            QuickHPCheck(-4);


            GoToEndOfTurn(haka);

            QuickHPStorage(vector);

            GoToStartOfTurn(vector);
            QuickHPCheck(3);
        }
        [Test]
        public void TestVectorChallenge()
        {
            SetupGameController(new[] { DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis" }, challenge: true);
            StartGame();

            AddCannotPlayCardsStatusEffect(ra, false, true);
            Card virus = GetCard("Supervirus");
            AssertUnderCard(vector.CharacterCard, virus);
            AssertNotInPlay(virus);

            DealDamage(legacy, vector, 30, DamageType.Melee);
            AssertNotInPlay(virus);
            DealDamage(legacy, vector, 15, DamageType.Fire);
            AssertIsInPlay(virus);
            AssertHitPoints(vector.CharacterCard, 10);

            FlipCard(vector);
            AssertOutOfGame(virus);

        }
    }
}
