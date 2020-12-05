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

        protected void AddImmuneToDamageTrigger(TurnTakerController ttc, bool heroesImmune, bool villainsImmune)
        {
            ImmuneToDamageStatusEffect immuneToDamageStatusEffect = new ImmuneToDamageStatusEffect();
            immuneToDamageStatusEffect.TargetCriteria.IsHero = new bool?(heroesImmune);
            immuneToDamageStatusEffect.TargetCriteria.IsVillain = new bool?(villainsImmune);
            immuneToDamageStatusEffect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(immuneToDamageStatusEffect, true, new CardSource(ttc.CharacterCardController)));
        }

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
            int vectorCardsInTrash = GetNumberOfCardsInTrash(Vector);

            // Act
            GoToPlayCardPhase(legacy);
            DealDamage(legacy, Vector, 2, DamageType.Melee);

            // Assert
            Assert.True(GetNumberOfCardsInPlay(Vector) > vectorCardsInPlay || GetNumberOfCardsInTrash(Vector) > vectorCardsInTrash, "A card was not played");
        }

        [Test]
        public void TestVectorDoesNotPlaysCardWhenHitForZero()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            int vectorCardsInPlay = GetNumberOfCardsInPlay(Vector);
            int vectorCardsInTrash = GetNumberOfCardsInTrash(Vector);

            AddImmuneToDamageTrigger(Vector, false, true);

            // Act
            GoToPlayCardPhase(legacy);
            DealDamage(legacy, Vector, 2, DamageType.Melee);

            // Assert
            Assert.True(GetNumberOfCardsInPlay(Vector) == vectorCardsInPlay && GetNumberOfCardsInTrash(Vector) == vectorCardsInTrash, "A card was played when no damage was dealt");
        }

        [Test]
        public void TestVectorFlipCondition()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card supervirus = PlayCard("Supervirus");

            //get H+2 (5) viruses to move on supervirus
            IEnumerable<Card> virusToMove = FindCardsWhere((Card c) => IsVirus(c) && Vector.TurnTaker.Deck.HasCard(c)).Take(5);

            AssertNotFlipped(Vector.CharacterCard);
            MoveCards(Vector, virusToMove, supervirus.UnderLocation);
            AssertFlipped(Vector.CharacterCard);
        }

        [Test]
        public void TestVectorWinsIfGainsFullHp()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            GainHP(Vector, 15);

            AssertNotFlipped(Vector);
            AssertGameOver(EndingResult.AlternateDefeat);
        }

        [Test]
        public void TestVectorBecomesImmuneToEnvFromEnvDamage()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "MaerynianRefuge");
            StartGame();

            QuickHPStorage(Vector);

            Card hail = GetCard("SoftballSizedHail");
            PlayCard(hail);
            GoToEndOfTurn(env);

            //QuickHPCheck(-2); // Now immune to env damage

            string messageText = "Vector is immune to damage from environment cards.";
            AssertStatusEffectsContains(messageText);

            AssertNotFlipped(Vector);
        }

        [Test]
        public void TestAdvancedGainHpAtEndOfVillainTurn()
        {
            SetupGameController(new[] { DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis" },
                advanced: true, advancedIdentifiers: new[] { DeckNamespace });

            StartGame();
            QuickHPStorage(Vector);

            GoToEndOfTurn(Vector);

            AssertNotFlipped(Vector);
            QuickHPCheck(2);
        }

        [Test]
        public void TestWhenFlippedSuperVirusIsRemovedFromGame()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card superVirus = GetCard(SupervirusCardController.Identifier);

            StartGame();
            PlayCard(superVirus);

            FlipCard(Vector.CharacterCardController);

            AssertFlipped(Vector);
            AssertOutOfGame(superVirus);
        }

        [Test]
        public void TestWhenFlippedCardsUnderSuperVirusAreReturnedToVillainTrash()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card bloodSample = GetCard(BloodSampleCardController.Identifier); // Virus
            Card delayedSymptoms = GetCard(DelayedSymptomsCardController.Identifier); // Virus

            Card superVirus = GetCard(SupervirusCardController.Identifier);

            MoveCard(Vector, bloodSample, superVirus.UnderLocation);
            MoveCard(Vector, delayedSymptoms, superVirus.UnderLocation);

            StartGame();
            PlayCard(superVirus);

            FlipCard(Vector.CharacterCardController);

            AssertFlipped(Vector);
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

            MoveCard(Vector, bloodSample, superVirus.UnderLocation);
            MoveCard(Vector, delayedSymptoms, superVirus.UnderLocation);

            StartGame();
            PlayCard(superVirus);

            Card cedistic = PlayCard("CedisticDissonant");
            Card pipes = PlayCard("DrakesPipes");
            DecisionSelectCard = superVirus;
            UsePower(adept);
            AssertInPlayArea(Vector, superVirus);

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

            FlipCard(Vector.CharacterCardController);
            AssertFlipped(Vector);

            int vectorCardsInPlay = GetNumberOfCardsInPlay(Vector);
            int vectorCardsInTrash = GetNumberOfCardsInTrash(Vector);

            // Act
            GoToEndOfTurn(Vector);

            // Assert
            Assert.True(GetNumberOfCardsInPlay(Vector) > vectorCardsInPlay || GetNumberOfCardsInTrash(Vector) > vectorCardsInTrash);
        }

        [Test]
        public void TestFlippedReduceVectorDamageTakenByVillainTargetCount()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            PutOnDeck(Vector, GetCard(HotZoneCardController.Identifier));
            PutOnDeck(Vector, GetCard(HyperactiveImmuneSystemCardController.Identifier));

            Card bioTerror = GetCard(BioterrorSquadCardController.Identifier);
            PlayCard(bioTerror);

            FlipCard(Vector.CharacterCardController);

            QuickHPStorage(Vector);

            GoToStartOfTurn(haka);
            DealDamage(haka, Vector, 4, DamageType.Melee);


            QuickHPCheck(-2); // Damage was reduced by 2 (2 villain targets: Vector, Bio Terror Squad)
            AssertFlipped(Vector);
        }

        [Test]
        public void TestFlippedAdvancedIncreasedVectorDamage()
        {
            SetupGameController(new[] { DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis" },
                advanced: true, advancedIdentifiers: new[] { DeckNamespace });

            StartGame();
            FlipCard(Vector.CharacterCardController);

            QuickHPStorage(legacy);

            DealDamage(Vector, legacy, 2, DamageType.Toxic);

            QuickHPCheck(-4);
            AssertFlipped(Vector);

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

            QuickHPStorage(Vector, legacy, ra, haka);
            // Act
            DealDamage(legacy, Vector, 5, DamageType.Melee);

            // Assert
            QuickHPCheck(-6, -6, -6, -6); // Anti Coag increases damage to Vector by 1, response to hit everyone
            AssertInTrash(antiC);
        }

        [Test]
        public void TestAnticoagulant_NoResponseOnZeroDamage()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            QuickHPStorage(Vector);

            Card antiC = GetCard(AnticoagulantCardController.Identifier);

            PlayCard(antiC);

            AddImmuneToDamageTrigger(Vector, false, true);

            // Act
            DealDamage(legacy, Vector, 5, DamageType.Melee);

            // Assert
            AssertInPlayArea(Vector, antiC);
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
            QuickHPStorage(Vector, legacy, ra, haka);

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
            QuickHPStorage(Vector, legacy, ra, haka);

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
            QuickHPStorage(Vector, legacy, ra, haka);

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
        public void TestBioTerrorSquad_NoVirusInTrash()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card bioTerror = GetCard(BioterrorSquadCardController.Identifier);

            StartGame();

            GoToPlayCardPhase(Vector);
            PlayCard(bioTerror);

            QuickShuffleStorage(Vector);

            // Act
            GoToStartOfTurn(Vector);

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
            GoToPlayCardPhase(Vector);
            PlayCard(bloodSample);
            QuickHPCheck(-1, -1, -1);

            PlayCard(superVirus);

            DealDamage(legacy, Vector, 6, DamageType.Melee);

            //get H+1 (4) viruses to move on supervirus
            IEnumerable<Card> virusToMove = FindCardsWhere((Card c) => IsVirus(c) && Vector.TurnTaker.Deck.HasCard(c)).Take(4);
            MoveCards(Vector, virusToMove, superVirus.UnderLocation);

            GoToStartOfTurn(Vector);

            // Assert

            AssertFlipped(Vector.CharacterCard);


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
            GoToPlayCardPhase(Vector);
            PlayCard(bloodSample);
            QuickHPCheck(-1, -1, -1);

            PlayCard(superVirus);

            DealDamage(legacy, Vector, 6, DamageType.Melee);

            GoToStartOfTurn(Vector);

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
            GoToPlayCardPhase(Vector);
            PlayCard(bloodSample);
            QuickHPCheck(-1, -1, -1);


            DealDamage(legacy, Vector, 6, DamageType.Melee);

            GoToStartOfTurn(Vector);

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
            GoToPlayCardPhase(Vector);
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
            GoToPlayCardPhase(Vector);
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
            GoToPlayCardPhase(Vector);

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
            GoToPlayCardPhase(Vector);
            PlayCard(eliteTraining);

            QuickHPStorage(legacy, ra);

            DealDamage(Vector, legacy, 3, DamageType.Toxic);

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
            GoToPlayCardPhase(Vector);
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
            GoToPlayCardPhase(Vector);
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
        public void TestHostageShield_EffectIsRemovedIfDestroyed()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card hostageShield = GetCard(HostageShieldCardController.Identifier);

            StartGame();

            DecisionYesNo = true;

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(hostageShield);
            QuickHPStorage(legacy);
            DealDamage(ra, legacy, 3, DamageType.Fire);
            QuickHPCheck(0);
            string messageText = $"Prevent damage from Ra.";
            DestroyCard(hostageShield, Vector.CharacterCard);
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
            GoToPlayCardPhase(Vector);
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
        public void TestHotZone()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "InsulaPrimalis");
            
            StartGame();

            PutIntoPlay("EnragedTRex");
            Card enragedTRex = GetCardInPlay("EnragedTRex");

            Card hotZone = GetCard(HotZoneCardController.Identifier);
            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, enragedTRex);

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(hotZone);

            // Assert
            QuickHPCheck(-2, -2, -2, -2);
        }

        [Test]
        public void TestHyperactiveImmuneSystem()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();

            SetHitPoints(Vector, 10);
            QuickHPStorage(Vector);

            Card hyperactive = GetCard(HyperactiveImmuneSystemCardController.Identifier);


            PlayCard(legacy, "DangerSense");
            PlayCard(legacy, "TheLegacyRing");
            PlayCard(legacy, "Mere");

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(hyperactive);
            GoToEndOfTurn(Vector);

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

            QuickHPStorage(legacy, ra);

            Card lethalForce = GetCard(LethalForceCardController.Identifier);

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(lethalForce);
            
            // Assert
            QuickHPCheck(-2, -1);
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
            GoToPlayCardPhase(Vector);
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
            GoToPlayCardPhase(Vector);
            PlayCard(quarantine);

            GoToEndOfTurn(Vector);

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
            GoToPlayCardPhase(Vector);
            PlayCard(quarantine);

            DestroyCard(quarantine);

            // Assert
            QuickHPCheck(-3, -3, -3);
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
            QuickHPStorage(Vector);
            PlayCard(superVirus);

            // Act

            GoToStartOfTurn(haka);
            GoToStartOfTurn(Vector);

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
            
            GoToPlayCardPhase(Vector);
            PlayCard(superVirus);

            // Act
            GoToStartOfTurn(legacy);
            DealDamage(legacy, Vector, 100, DamageType.Melee);

            // Assert
            AssertGameOver(EndingResult.AlternateDefeat);
        }

        [Test]
        public void TestUndiagnosedSubjectPlaysVillainCardAtEndOfVillainTurn()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            PutOnDeck(Vector, GetCard(HotZoneCardController.Identifier));

            StartGame();
            Card undiagnosed = GetCard(UndiagnosedSubjectCardController.Identifier);


            // Act
            QuickTopCardStorage(Vector);

            GoToPlayCardPhase(Vector);
            PlayCard(undiagnosed);
            GoToEndOfTurn(Vector);
            GoToStartOfTurn(legacy);

            // Assert
            QuickTopCardCheck(ttc => ttc.TurnTaker.Trash);
        }

        [Test]
        public void TestUndiagnosedSubjectPlaysVillainCardWhenDestroyed_SuperVirusNotInPlayDiscard()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            PutOnDeck(Vector, GetCard(HotZoneCardController.Identifier));

            StartGame();
            Card undiagnosed = GetCard(UndiagnosedSubjectCardController.Identifier);

            DecisionSelectFunction = 0; // Discard

            // Act
            QuickTopCardStorage(Vector);

            GoToPlayCardPhase(Vector);
            PlayCard(undiagnosed);

            DestroyCard(undiagnosed);

            // Assert
            QuickTopCardCheck(ttc => ttc.TurnTaker.Trash);
        }

        [Test]
        public void TestUndiagnosedSubjectPlaysVillainCardWhenDestroyed_SuperVirusNotInPlayReplace()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            PutOnDeck(Vector, GetCard(HotZoneCardController.Identifier));

            StartGame();
            Card undiagnosed = GetCard(UndiagnosedSubjectCardController.Identifier);

            DecisionSelectFunction = 1; // Replace

            // Act
            QuickTopCardStorage(Vector);

            GoToPlayCardPhase(Vector);
            PlayCard(undiagnosed);

            DestroyCard(undiagnosed);

            // Assert
            QuickTopCardCheck(ttc => ttc.TurnTaker.Deck);
        }

        [Test]
        public void TestUndiagnosedSubjectPlaysVillainCardWhenDestroyed_SuperVirusInPlay()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card bioTerror = GetCard(BioterrorSquadCardController.Identifier);
            PutOnDeck(Vector, bioTerror);

            StartGame();
            Card undiagnosed = GetCard(UndiagnosedSubjectCardController.Identifier);
            Card superVirus = GetCard(SupervirusCardController.Identifier);

            DecisionSelectFunction = 2; // Put under Super Virus

            // Act

            GoToPlayCardPhase(Vector);
            PlayCard(superVirus);
            PlayCard(undiagnosed);

            DestroyCard(undiagnosed);

            // Assert
            AssertUnderCard(superVirus, bioTerror);
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
            GoToPlayCardPhase(Vector);
            PlayCard(vendetta);

            // Assert
            QuickHPCheck(-2); 
            QuickHandCheck(-1);
        }

        [Test]
        public void TestVirulentBlade()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            PutOnDeck(Vector, GetCard(HotZoneCardController.Identifier));


            StartGame();
            Card virulentBlade = GetCard(VirulentBladeCardController.Identifier);
            QuickHPStorage(Vector, legacy, ra, haka);

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(virulentBlade);

            GoToStartOfTurn(legacy);

            // Assert

            // Vector: -2 from Virulent Blade, Heroes: -4 (Hot zone -2, Virulent Blade -2)
            QuickHPCheck(-2, -4, -4, -4);

        }

        [Test]
        public void TestVrRazortail()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();
            Card razortail = GetCard(VrRazortailCardController.Identifier);

            // Act
            GoToPlayCardPhase(Vector);
            PlayCard(razortail);
            QuickHPStorage(razortail);

            DealDamage(haka, razortail, 4, DamageType.Melee);

            QuickHPCheck(-3);


            GoToEndOfTurn(haka);

            QuickHPStorage(Vector);

            GoToStartOfTurn(Vector);
            QuickHPCheck(3);
        }
    }
}
