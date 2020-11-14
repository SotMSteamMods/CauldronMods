using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cauldron.BlackwoodForest;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;

using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture]
    public class BlackwoodForestTests : BaseTest
    {
        private const string DeckNamespace =  "Cauldron.BlackwoodForest";

        protected TurnTakerController BlackwoodForest => FindEnvironment();


        [Test]
        public void TestBlackForestLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            // Assert
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());
        }


        [Test]
        public void TestOldBonesEmptyTrashPiles()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();

            // Act
            PutIntoPlay(OldBonesCardController.Identifier);
            Card oldBones = GetCardInPlay(OldBonesCardController.Identifier);

            GoToStartOfTurn(BlackwoodForest);

            // Assert
            AssertNotInPlay(oldBones);
            Assert.AreEqual(0, ra.TurnTaker.Trash.Cards.Count());
            Assert.AreEqual(0, legacy.TurnTaker.Trash.Cards.Count());
            Assert.AreEqual(0, haka.TurnTaker.Trash.Cards.Count());
            Assert.AreEqual(0, baron.TurnTaker.Trash.Cards.Count());
        }

        [Test]
        public void TestOldBonesSeededTrashPiles()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();

            // Seed trash decks
            PutInTrash(ra, GetBottomCardOfDeck(ra));
            PutInTrash(ra, GetBottomCardOfDeck(ra));
            PutInTrash(legacy, GetBottomCardOfDeck(legacy));
            PutInTrash(legacy, GetBottomCardOfDeck(legacy));
            PutInTrash(legacy, GetBottomCardOfDeck(legacy));
            PutInTrash(haka, GetBottomCardOfDeck(haka));
            PutInTrash(baron, GetBottomCardOfDeck(baron));
            PutInTrash(baron, GetBottomCardOfDeck(baron));
            PutInTrash(baron, GetBottomCardOfDeck(baron));
            PutInTrash(baron, GetBottomCardOfDeck(baron));

            // Act
            PutIntoPlay(OldBonesCardController.Identifier);
            Card oldBones = GetCardInPlay(OldBonesCardController.Identifier);

            GoToStartOfTurn(BlackwoodForest);

            // Assert
            AssertNotInPlay(oldBones);
            Assert.AreEqual(1, ra.TurnTaker.Trash.Cards.Count());
            Assert.AreEqual(2, legacy.TurnTaker.Trash.Cards.Count());
            Assert.AreEqual(0, haka.TurnTaker.Trash.Cards.Count());
            Assert.AreEqual(3, baron.TurnTaker.Trash.Cards.Count());
        }

        [Test]
        public void TestDontStrayFromThePathHoundRevealed()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();
            QuickShuffleStorage(BlackwoodForest);

            Card theHound = GetCard(TheHoundCardController.Identifier);
            PutOnDeck(BlackwoodForest, theHound); // Top deck The Hound

            // Act
            PutIntoPlay(DontStrayFromThePathCardController.Identifier);
            Card dontStray = GetCardInPlay(DontStrayFromThePathCardController.Identifier);

            PutIntoPlay(ShadowWeaverCardController.Identifier); // Has to be at least 1 other env card in play to proc Don't Stray

            GoToStartOfTurn(BlackwoodForest);

            // Assert
            AssertNotInPlay(theHound); // The Hound was shuffled back into the environment deck
            AssertInDeck(theHound); // The Hound should be back in the deck
            QuickShuffleCheck(1); // The Hound caused an additional shuffle of the env. deck
        }

        [Test]
        public void TestDontStrayFromThePathHoundNotRevealed()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();
            QuickShuffleStorage(BlackwoodForest);


            Card theHound = GetCard(TheHoundCardController.Identifier);
            PutOnDeck(BlackwoodForest, GetCard(OvergrownCathedralCardController.Identifier)); // Top deck something other than The Hound

            // Act
            PutIntoPlay(DontStrayFromThePathCardController.Identifier);
            Card dontStray = GetCardInPlay(DontStrayFromThePathCardController.Identifier);

            PutIntoPlay(ShadowWeaverCardController.Identifier); // Has to be at least 1 other env card in play to proc Don't Stray

            GoToStartOfTurn(BlackwoodForest);

            // Assert
            AssertNotInPlay(theHound);
            QuickShuffleCheck(0); // No shuffles were done beyond the game start shuffle

        }

        [Test]
        public void TestDontStrayFromThePathNotEnoughOtherEnvCardsOutToProc()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();
            QuickShuffleStorage(BlackwoodForest);

            // Act
            PutIntoPlay(DontStrayFromThePathCardController.Identifier);
            Card dontStray = GetCardInPlay(DontStrayFromThePathCardController.Identifier);

            GoToStartOfTurn(BlackwoodForest);

            // Assert
            QuickShuffleCheck(0); // No shuffles were done beyond the game start shuffle
        }

        [Test]
        public void TestDontStrayFromThePathProcsOnFutureRoundNoHound()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();

            // Act
            GoToStartOfTurn(BlackwoodForest);
            PutIntoPlay(DontStrayFromThePathCardController.Identifier);

            GoToStartOfTurn(BlackwoodForest);
            GoToPlayCardPhase(BlackwoodForest);
            PlayCard(ShadowStalkerCardController.Identifier);

            PutOnDeck(BlackwoodForest, GetCard(OvergrownCathedralCardController.Identifier)); // Top deck something other than The Hound
            GoToStartOfTurn(BlackwoodForest);

            // Assert
            AssertNumberOfCardsInTrash(BlackwoodForest, 1); // Overgrown Cathedral was not The Hound so it was placed in trash
            
        }

        [Test]
        public void TestDontStrayFromThePathProcsOnFutureRoundWithHound()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();
            QuickShuffleStorage(BlackwoodForest);

            // Act

            GoToStartOfTurn(BlackwoodForest);
            PutIntoPlay(DontStrayFromThePathCardController.Identifier);

            GoToStartOfTurn(BlackwoodForest);
            GoToPlayCardPhase(BlackwoodForest);
            PlayCard(ShadowStalkerCardController.Identifier);

            Card theHound = GetCard(TheHoundCardController.Identifier);
            PutOnDeck(BlackwoodForest, theHound); // Top deck something other than The Hound
            GoToStartOfTurn(BlackwoodForest);

            // Assert
            QuickShuffleCheck(1); // The Hound caused an additional shuffle of the env. deck
            AssertNotInPlay(theHound); // The Hound was shuffled back into the environment deck
            AssertInDeck(theHound); // The Hound should be back in the deck
            AssertNumberOfCardsInTrash(BlackwoodForest, 0); // No cards should be in the trash

        }


        [Test]
        public void TestDenseBrambles()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            string statusEffectMessageRa = $"{ra.Name} is immune to damage.";
            string statusEffectMessageMdp = $"{mdp.Title} is immune to damage.";

            // Act
            PutIntoPlay(DenseBramblesCardController.Identifier);
            Card denseBrambles = GetCardInPlay(DenseBramblesCardController.Identifier);

            QuickHPStorage(ra.CharacterCard, mdp);

            // 2 lowest HP characters are Ra and MDP
            AssertStatusEffectsContains(statusEffectMessageRa); // Ra has immune to damage status effect
            AssertStatusEffectsContains(statusEffectMessageMdp); // MDP has immune to damage status effect
            DealDamage(baron, ra, 3, DamageType.Toxic); // Ra is immune
            

            GoToStartOfTurn(BlackwoodForest); // Dense Brambles is destroyed

            DealDamage(ra, mdp, 2, DamageType.Fire);

            // Assert
            AssertStatusEffectsDoesNotContain(statusEffectMessageRa); // Ra no longer has immune to damage
            AssertStatusEffectsDoesNotContain(statusEffectMessageMdp); // MDP no longer has immune to damage
            QuickHPCheck(0, -2); // Ra took no damage from baron's attack due to Dense Brambles, MDP was damaged after Dense Brambles was destroyed
        }

        [Test]
        public void TestDenseBramblesThreeWayTieForLowest()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            SetHitPoints(legacy, 10);
            SetHitPoints(ra, 10);

            string statusEffectMessageRa = $"{ra.Name} is immune to damage.";
            string statusEffectMessageMdp = $"{mdp.Title} is immune to damage.";

            DecisionSelectCards = new[] {mdp, ra.CharacterCard};

            // Act
            PutIntoPlay(DenseBramblesCardController.Identifier);
            Card denseBrambles = GetCardInPlay(DenseBramblesCardController.Identifier);

            QuickHPStorage(ra.CharacterCard, mdp);

            // 2 lowest HP characters are Ra and MDP
            AssertStatusEffectsContains(statusEffectMessageRa); // Ra has immune to damage status effect
            AssertStatusEffectsContains(statusEffectMessageMdp); // MDP has immune to damage status effect
            DealDamage(baron, ra, 3, DamageType.Toxic); // Ra is immune


            GoToStartOfTurn(BlackwoodForest); // Dense Brambles is destroyed

            DealDamage(ra, mdp, 2, DamageType.Fire);

            // Assert
            AssertStatusEffectsDoesNotContain(statusEffectMessageRa); // Ra no longer has immune to damage
            AssertStatusEffectsDoesNotContain(statusEffectMessageMdp); // MDP no longer has immune to damage
            QuickHPCheck(0, -2); // Ra took no damage from baron's attack due to Dense Brambles, MDP was damaged after Dense Brambles was destroyed
        }

        [Test]
        public void TestTheHoundWithOngoing()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            Card backlashField = GetCard("BacklashField");
            PutIntoPlay(backlashField.Identifier);

            Card dangerSense = GetCard("DangerSense");
            PutIntoPlay(dangerSense.Identifier);

            // Act
            PutIntoPlay(TheHoundCardController.Identifier);

            // Assert
            QuickHPCheck(-4); // (-4 HP from The Hound)
            AssertIsInPlay(backlashField); // Still in play, The Hound only targets hero ongoing and equipment cards
            AssertNotInPlay(dangerSense); // Destroyed by The Hound
            AssertNotInPlay(GetCard(TheHoundCardController.Identifier)); // The Hound was shuffled back into the environment deck
            AssertInDeck(GetCard(TheHoundCardController.Identifier)); // The Hound should be back in the deck
        }

        [Test]
        public void TestTheHoundWithEquipment()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            Card backlashField = GetCard("BacklashField");
            PutIntoPlay(backlashField.Identifier);

            Card legacyRing = GetCard("TheLegacyRing");
            PutIntoPlay(legacyRing.Identifier);

            // Act
            PutIntoPlay(TheHoundCardController.Identifier);

            // Assert
            QuickHPCheck(-4); // (-4 HP from The Hound)
            AssertIsInPlay(backlashField); // Still in play, The Hound only targets hero ongoing and equipment cards
            AssertNotInPlay(legacyRing); // Destroyed by The Hound
            AssertNotInPlay(GetCard(TheHoundCardController.Identifier)); // The Hound was shuffled back into the environment deck
            AssertInDeck(GetCard(TheHoundCardController.Identifier)); // The Hound should be back in the deck
        }

        [Test]
        public void TestTheHoundWithOngoingAndEquipment()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(mdp);

            Card backlashField = GetCard("BacklashField");
            PutIntoPlay(backlashField.Identifier);

            Card legacyRing = GetCard("TheLegacyRing");
            PutIntoPlay(legacyRing.Identifier);

            Card dangerSense = GetCard("DangerSense");
            PutIntoPlay(dangerSense.Identifier);

            DecisionSelectCard = legacyRing;

            // Act
            PutIntoPlay(TheHoundCardController.Identifier);

            // Assert
            QuickHPCheck(-4); // (-4 HP from The Hound)
            AssertIsInPlay(backlashField); // Still in play, The Hound only targets hero ongoing and equipment cards
            AssertNotInPlay(legacyRing); // Destroyed by The Hound
            AssertIsInPlay(dangerSense); // Still in play
            AssertNotInPlay(GetCard(TheHoundCardController.Identifier)); // The Hound was shuffled back into the environment deck
            AssertInDeck(GetCard(TheHoundCardController.Identifier)); // The Hound should be back in the deck
        }

        [Test]
        public void TestOvergrownCathedral()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            QuickHPStorage(baron.CharacterCard, mdp, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);

            Card overgrownCathedral = GetCard(OvergrownCathedralCardController.Identifier);

            DecisionYesNo = true;

            // Act
            GoToStartOfTurn(BlackwoodForest);
            PlayCard(overgrownCathedral);

            // Ra will be dealt damage first, then Cathedral will interject and deal damage to everyone else
            PlayCard(baron, "SlashAndBurn");

            GoToStartOfTurn(BlackwoodForest);

            // Assert

            // Baron: 0 - (Immune), MDP: -1 (Cathedral), Ra: -3 (Slash and Burn)
            // Legacy: -1 (Cathedral), Haka (Slash and Burn, Cathedral)
            QuickHPCheck(0, -1, -3, -1, -6);
            AssertNotInPlay(overgrownCathedral);
        }

        [Test]
        public void TestShadowWeaver()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();

            Card shadowWeaver = GetCard(ShadowWeaverCardController.Identifier);
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            PlayCard(shadowWeaver);

            QuickHPStorage(baron.CharacterCard, mdp, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);

            // Act
            GoToEndOfTurn(BlackwoodForest);

            GoToStartOfTurn(ra);
            DealDamage(ra, shadowWeaver, 10, DamageType.Fire);

            // Assert
            AssertInTrash(BlackwoodForest, shadowWeaver); // Shadow Weaver is in trash

            // Baron: 0 (Immune)
            // MDP: -1 (Shadow Weaver destruction trigger)
            // Ra: -2 (Shadow Weaver end of env. turn, destruction trigger)
            // Legacy: -1 (Shadow Weaver destruction trigger)
            // Haka: -1 (Shadow Weaver destruction trigger)
            QuickHPCheck(0, -1, -2, -1, -1);
        }

        [Test]
        public void TestShadowStalker()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();
            Card backlashField = GetCard("BacklashField");
            PlayCard(backlashField);

            Card shadowStalker = GetCard(ShadowStalkerCardController.Identifier);
            PlayCard(shadowStalker);
            QuickHPStorage(ra, legacy, haka);

            // Act
            GoToEndOfTurn(BlackwoodForest);

            // Assert
            QuickHPCheck(-1, -1, -1); // (Ra, Legacy, Haka: -1 from start of turn Shadow Stalker effect)
            AssertNotInPlay(backlashField);
            AssertInTrash(baron, backlashField);
        }

        [Test]
        public void TestWillOTheWisp()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", DeckNamespace);

            string statusEffectMessageBlackwoodForest = $"Skip {BlackwoodForest.Name}'s Draw Card phase.";

            StartGame();

            Card willOTheWisp = GetCard(WillOTheWispCardController.Identifier);

            // Act
            GoToStartOfTurn(BlackwoodForest);
            PlayCard(willOTheWisp);

            // Assert
            AssertNumberOfCardsInPlay(card => card.IsEnvironment, 1);

            GoToEndOfTurn(baron);
            AssertNumberOfCardsInPlay(card => card.IsEnvironment, 3); // 2 cards drawn at the end of villain turn

            AssertStatusEffectsContains(statusEffectMessageBlackwoodForest); // Blackwood Forest skips its draw phase
        }

    }
}
