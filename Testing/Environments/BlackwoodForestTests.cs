using System.Linq;
using System.Collections.Generic;

using Cauldron.BlackwoodForest;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;

using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture]
    public class BlackwoodForestTests : CauldronBaseTest
    {
        private const string DeckNamespace = "Cauldron.BlackwoodForest";

        protected TurnTakerController BlackwoodForest => FindEnvironment();


        [Test]
        public void TestBlackForestLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            // Assert
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());
        }

        [Test()]
        public void TestBlackForestLoadsInOblivaeon()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Cauldron.BlackwoodForest", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            Assert.AreEqual(8, this.GameController.TurnTakerControllers.Count());
            AssertBattleZone(BlackwoodForest, bzOne);

            Card spirit = PlayCard("VengefulSpirits");
            PrintSpecialStringsForCard(spirit);

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
            AssertNumberOfCardsInRevealed(BlackwoodForest, 0);
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
            AssertNumberOfCardsInRevealed(BlackwoodForest, 0);
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
            PlayCard(ShadowWeaverCardController.Identifier);

            Card theHound = GetCard(TheHoundCardController.Identifier);
            PutOnDeck("WillOTheWisp");
            PutOnDeck(BlackwoodForest, theHound);
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

            // Act
            PutIntoPlay(DenseBramblesCardController.Identifier);
            Card denseBrambles = GetCardInPlay(DenseBramblesCardController.Identifier);

            QuickHPStorage(ra.CharacterCard, mdp);

            // 2 lowest HP characters are Ra and MDP
            DealDamage(baron, ra, 3, DamageType.Toxic); // Ra is immune
            DealDamage(baron, mdp, 3, DamageType.Toxic); // MDP is immune

            AssertCardSpecialString(denseBrambles, 0, "2 targets with the lowest HP: Mobile Defense Platform, Ra.");

            GoToStartOfTurn(BlackwoodForest); // Dense Brambles is destroyed

            DealDamage(ra, mdp, 2, DamageType.Fire);

            // Assert
            QuickHPCheck(0, -2); // Ra took no damage from baron's attack due to Dense Brambles, MDP was damaged after Dense Brambles was destroyed
        }

        [Test]
        public void TestDenseBramblesThreeWayTieForLowestChooseYes()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            // mdp, legacy, and ra are all tied for lowest
            SetHitPoints(legacy, 10);
            SetHitPoints(ra, 10);

            // Act
            PutIntoPlay(DenseBramblesCardController.Identifier);
            Card denseBrambles = GetCardInPlay(DenseBramblesCardController.Identifier);

            // All 3 targets are listed as the 2 lowest
            AssertCardSpecialString(denseBrambles, 0, "2 targets with the lowest HP: Mobile Defense Platform, Ra, Legacy.");

            QuickHPStorage(ra.CharacterCard, mdp);

            DecisionYesNo = true; //Choose Ra is one of the 2 lowest
            DealDamage(baron, ra, 3, DamageType.Toxic);

            // Assert
            QuickHPCheck(0, 0); // Ra was immune
        }

        [Test]
        public void TestDenseBramblesThreeWayTieForLowestChooseNo()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            // mdp, legacy, and ra are all tied for lowest
            SetHitPoints(legacy, 10);
            SetHitPoints(ra, 10);

            // Act
            PutIntoPlay(DenseBramblesCardController.Identifier);
            Card denseBrambles = GetCardInPlay(DenseBramblesCardController.Identifier);

            // All 3 targets are listed as the 2 lowest
            AssertCardSpecialString(denseBrambles, 0, "2 targets with the lowest HP: Mobile Defense Platform, Ra, Legacy.");

            QuickHPStorage(ra.CharacterCard, mdp);

            DecisionYesNo = false; // Choose Ra is NOT one of the 2 lowest
            DealDamage(baron, ra, 3, DamageType.Toxic);

            // Assert
            QuickHPCheck(-3, 0); // Ra took damage when chosen not to be among the 2 lowest
        }

        [Test]
        public void TestDenseBramblesThreeWayTieForSecondLowest()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            // MDP is lowest
            // Legacy and Ra are tied for 2nd
            SetHitPoints(legacy, 15);
            SetHitPoints(ra, 15);

            // Act
            PutIntoPlay(DenseBramblesCardController.Identifier);
            Card denseBrambles = GetCardInPlay(DenseBramblesCardController.Identifier);

            // All 3 targets are listed as the 2 lowest
            AssertCardSpecialString(denseBrambles, 0, "2 targets with the lowest HP: Mobile Defense Platform, Ra, Legacy.");

            QuickHPStorage(mdp, legacy.CharacterCard, ra.CharacterCard);

            DecisionsYesNo = new bool[] { }; // MDP is within the lowest 2 without a choice
            DealDamage(ra, mdp, 3, DamageType.Fire);

            base.ResetDecisions();
            DecisionsYesNo = new bool[] { true }; // Choose Legacy is one of the 2 lowest
            DealDamage(baron, legacy, 3, DamageType.Toxic);

            base.ResetDecisions();
            DecisionsYesNo = new bool[] { false }; // Choose Ra is NOT one of the 2 lowest
            DealDamage(baron, ra, 3, DamageType.Toxic);

            // Assert
            QuickHPCheck(0, 0, -3); // MDP and Legacy were immune, Ra took damage
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

            //pick a card that definitely won't give the players another SelectCardDecision
            PutOnDeck("DenseBrambles");

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

            QuickHPStorage(ra, legacy, haka, baron);


            DestroyCard("MobileDefensePlatform");

            Card backlashField = GetCard("BacklashField");
            PlayCard(backlashField);

            Card shadowStalker = GetCard(ShadowStalkerCardController.Identifier);
            PlayCard(shadowStalker);

            Card shadowWeaver = GetCard(ShadowWeaverCardController.Identifier);
            PlayCard(shadowWeaver);

            GoToStartOfTurn(legacy);
            DealDamage(legacy, shadowWeaver, 20, DamageType.Melee);


            // Act
            GoToEndOfTurn(BlackwoodForest);

            // Assert

            /*
             * Ra: -1 Shadow Weaver destruction, -1 start of turn Shadow Stalker effect
             * Legacy: -1 Shadow Weaver destruction, -1 start of turn Shadow Stalker effect
             * Haka: -1 Shadow Weaver destruction, -1 start of turn Shadow Stalker effect
             * Baron: -1 Shadow Weaver destruction, -5 from Shadow Stalker when another env card was destroyed as he was highest HP
             */
            QuickHPCheck(-2, -2, -2, -6);

            AssertNotInPlay(backlashField);
            AssertInTrash(baron, backlashField);
            AssertInTrash(BlackwoodForest, shadowWeaver);
        }

        [Test]
        public void TestWillOTheWisp()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", DeckNamespace);

            string statusEffectMessageBlackwoodForest = $"Skip {BlackwoodForest.Name}'s Draw Card phase.";

            StartGame();

            // Arrange top of deck so our asserts work reliably
            PutOnDeck(BlackwoodForest, GetCard(ShadowWeaverCardController.Identifier));
            PutOnDeck(BlackwoodForest, GetCard(DontStrayFromThePathCardController.Identifier));

            Card willOTheWisp = GetCard(WillOTheWispCardController.Identifier);

            // Act
            GoToStartOfTurn(BlackwoodForest);
            PlayCard(willOTheWisp);
            Card staff = PlayCard("TheStaffOfRa");
            // Assert
            AssertIsInPlay(staff);
            AssertNumberOfCardsInPlay(card => card.IsEnvironment, 1);

            GoToEndOfTurn(baron);
            AssertNumberOfCardsInPlay(card => card.IsEnvironment, 3); // 2 cards drawn at the end of villain turn
        }

        [Test]
        public void TestMirrorWraithEligibleTargets_CloneVillainTarget()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            QuickHPStorage(legacy, ra);

            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card bb = GetCard("BladeBattalion");
            PlayCard(bb);

            //Card dangerSense = GetCard("DangerSense");

            // Act
            GoToStartOfTurn(BlackwoodForest);
            Card mirrorWraith = GetCard(MirrorWraithCardController.Identifier);
            PlayCard(mirrorWraith);

            GoToStartOfTurn(ra);

            // Assert

            AssertHitPoints(mirrorWraith, 5); // Mirror Wraith now has max HP of BB
            AssertCardHasKeyword(mirrorWraith, "minion", false);
            QuickHPCheck(-5, -5); // Legacy hit for 5 by real BB, Ra then hit for 5 from Mirror Wraith (BB game text)
        }

        [Test]
        public void TestMirrorWraithEligibleTargets_CloneUnityRaptorBot()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Unity", DeckNamespace);


            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card raptor = GetCard("RaptorBot");
            PlayCard(raptor);

            DecisionSelectTarget = mdp;
            QuickHPStorage(mdp);

            // Act
            GoToStartOfTurn(unity);

            GoToStartOfTurn(BlackwoodForest);
            Card mirrorWraith = GetCard(MirrorWraithCardController.Identifier);
            PlayCard(mirrorWraith);

            GoToEndOfTurn(unity);

            // Assert

            AssertHitPoints(mirrorWraith, 2); // Mirror Wraith now has max HP of Raptor Bot
            AssertCardHasKeyword(mirrorWraith, "mechanical golem", false);
            QuickHPCheck(-8); // Raptor 1st hit (2), 2nd hit (3 Mirror wraith clone boosting it by 1), 3rd hit by MW for 3
        }

        [Test]
        public void TestMirrorWraithEligibleTargets_CloneUnityChampionBot()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Unity", DeckNamespace);

            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card raptor = GetCard("RaptorBot");
            Card championBot = GetCard("ChampionBot");
            PlayCard(championBot);

            DecisionSelectTarget = mdp;
            QuickHPStorage(mdp);

            // Act

            GoToPlayCardPhase(BlackwoodForest);
            Card mirrorWraith = GetCard(MirrorWraithCardController.Identifier);
            PlayCard(mirrorWraith);

            GoToPlayCardPhase(unity);
            PlayCard(raptor);
            GoToEndOfTurn(unity);

            // Assert

            AssertHitPoints(mirrorWraith, 8); // Mirror Wraith now has max HP of Champion Bot
            AssertCardHasKeyword(mirrorWraith, "mechanical golem", false);

            /* Raptor
             * 6 Damage dealt
             * 1 x 3 golems in play +1 (Raptor, ChampBot, Mirror Wraith) = 4 dmg
             * +1 from Champion Bot
             * +1 from Mirror Wraith
             */

            QuickHPCheck(-6);
        }

        [Test]
        public void TestMirrorWraithEligibleTargets_CloneUnitySwiftBot()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "Unity", DeckNamespace);

            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card swiftBot = GetCard("SwiftBot");
            Card modularWorkbench = GetCard("ModularWorkbench");

            // Act
            //Can't play Swift Bot during Unity's play phase, if it happens to end up in hand.
            PlayCard(swiftBot);

            GoToPlayCardPhaseAndPlayCard(unity, "ConstructionPylon");
            GoToDrawCardPhase(unity);

            AssertPhaseActionCount(2); // Normal draw + 1 from swiftbot

            GoToEndOfTurn(unity);

            PrintHand(unity);

            GoToPlayCardPhase(BlackwoodForest);
            Card mirrorWraith = GetCard(MirrorWraithCardController.Identifier);
            PlayCard(mirrorWraith);

            GoToPlayCardPhase(unity);
            PlayCard(modularWorkbench);
            GoToDrawCardPhase(unity);

            //AssertPhaseActionCount(3); // Normal draw + 1 from swiftbot + 1 from mirror wraith
            GoToEndOfTurn(unity);

            // Assert
            AssertHitPoints(mirrorWraith, 6); // Mirror Wraith now has max HP of Swift Bot
            AssertCardHasKeyword(mirrorWraith, "mechanical golem", false);

        }

        [Test]
        public void TestMirrorWraithEligibleTargets_CloneProletariatClone()
        {
            // Arrange
            SetupGameController("ProletariatTeam", "Ra", "FrightTrainTeam", "Legacy", DeckNamespace);

            Card regroupAndRecover = GetCard("RegroupAndRecover");

            StartGame();
            Card proletariatClone = GetCardInPlay("Proletariat");
            DecisionLowestHP = proletariatClone;

            // Act
            GoToPlayCardPhase(BlackwoodForest);
            Card mirrorWraith = GetCard(MirrorWraithCardController.Identifier);
            PlayCard(mirrorWraith);
            AssertCardHasKeyword(mirrorWraith, "clone", false);

            GoToStartOfTurn(proleTeam);
            PlayCard(regroupAndRecover);

            // Assert
            AssertInTrash(mirrorWraith);
        }

        [Test]
        public void TestMirrorWraithEligibleTargets_CloneAkashSeed()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "AkashThriya", DeckNamespace);

            Card healingPollen = GetCard("HealingPollen");
            Card healingPollen2 = GetCard("HealingPollen", 1);
            PutInHand(thriya, healingPollen);
            PutInHand(thriya, healingPollen2);

            StartGame();
            DecisionLowestHP = healingPollen;

            Card verdantExplosion = GetCard("VerdantExplosion");

            GoToPlayCardPhase(thriya);
            var p1 = PlayCardFromHand(thriya, "HealingPollen");
            var p2 = PlayCardFromHand(thriya, "HealingPollen");

            GoToStartOfTurn(BlackwoodForest);
            Card mirrorWraith = GetCard(MirrorWraithCardController.Identifier);
            PlayCard(mirrorWraith);
            AssertCardHasKeyword(mirrorWraith, "primordial seed", false);

            GoToPlayCardPhase(thriya);

            PlayCard(verdantExplosion);
            AssertInTrash(mirrorWraith);
            AssertInTrash(p1);
            AssertInTrash(p2);
        }


        [Test]
        //[Ignore("Current implementation cannot handle cards that Move Next to other cards.")]
        public void TestMirrorWraithEligibleTargets_ClonePin()
        {
            // Arrange
            SetupGameController("GloomWeaver", "Ra", "Legacy", "AkashThriya", DeckNamespace);

            StartGame();
            DestroyNonCharacterVillainCards();

            var pin = GetCard("CrimsonPin");
            PlayCard(pin);
            AssertNextToCard(pin, thriya.CharacterCard);

            GoToStartOfTurn(BlackwoodForest);
            Card mirrorWraith = GetCard(MirrorWraithCardController.Identifier);

            PlayCard(mirrorWraith);
            AssertNextToCard(mirrorWraith, thriya.CharacterCard);
        }


        [Test]
        public void TestMirrorWraithEligibleTargets_CloneAcrossReload()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", "AkashThriya", DeckNamespace);

            Card healingPollen = GetCard("HealingPollen");
            Card healingPollen2 = GetCard("HealingPollen", 1);
            PutInHand(thriya, healingPollen);
            PutInHand(thriya, healingPollen2);

            StartGame();
            DecisionLowestHP = healingPollen;

            Card verdantExplosion = GetCard("VerdantExplosion");

            GoToPlayCardPhase(thriya);
            PlayCardFromHand(thriya, "HealingPollen");
            PlayCardFromHand(thriya, "HealingPollen");

            GoToStartOfTurn(BlackwoodForest);
            Card mirrorWraith = GetCard(MirrorWraithCardController.Identifier);
            PlayCard(mirrorWraith);
            var cc = FindCardController(mirrorWraith);
            var sss = cc.GetSpecialStrings(false, true).Select(ss => ss.GeneratedString()).ToArray();

            GoToPlayCardPhase(thriya);

            var cards = FindCardsWhere(c => c.IsPrimordialSeed && c.IsInPlay).ToList();
            Assert.AreEqual(3, cards.Count, "There should be 3 primodialSeeds in play, 2 real, 1 mirror wraith");

            SaveAndLoad();

            cards = FindCardsWhere(c => c.IsPrimordialSeed && c.IsInPlay).ToList();
            Assert.AreEqual(3, cards.Count, "There should be 3 primodialSeeds in play, 2 real, 1 mirror wraith");
        }

        [Test]
        public void TestMirrorWraithNoEligibleTargets()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            StartGame();

            // Destroy MDP so Mirror Wraith has no valid targets to copy
            DestroyCard("MobileDefensePlatform");
            QuickHPStorage(baron, ra, legacy, haka);

            Card mirrorWraith = GetCard(MirrorWraithCardController.Identifier);
            PlayCard(mirrorWraith);

            // Act
            GoToEndOfTurn(BlackwoodForest);

            // Assert

            // Mirror Wraith found no eligible target to copy, instead dealt 2 sonic damage to all targets
            AssertInTrash(BlackwoodForest, mirrorWraith);
            QuickHPCheck(-2, -2, -2, -2);

        }

        [Test]
        public void TestMirrorWraith_Limited()
        {
            SetupGameController("BaronBlade", "CaptainCosmic", "Legacy", "Haka", DeckNamespace);
            StartGame();

            Card crest = PlayCard("CosmicCrest");
            Card mirrorWraith1 = PlayCard(MirrorWraithCardController.Identifier, 0);
            //Mirror Wraith 1 is limited and thus prevents the play of another Mirror Wraith
            Card mirrorWraith2 = PlayCard(MirrorWraithCardController.Identifier, 1);
            AssertInTrash(mirrorWraith2);
        }
        [Test]
        public void TestMirrorWraith_CopyMirrorWraith()
        {
            SetupGameController("BaronBlade", "Unity", "Legacy", "Haka", DeckNamespace);
            StartGame();

            Card bot = PlayCard("RaptorBot");
            Card mirrorWraith1 = PlayCard(MirrorWraithCardController.Identifier, 0);
            //Mirror Wraith 1 is limited and thus prevents the play of another Mirror Wraith
            DestroyCard(bot);
            Card mirrorWraith2 = PlayCard(MirrorWraithCardController.Identifier, 1);
            AssertIsInPlay(mirrorWraith2);
        }

        [Test]
        public void TestVengefulSpiritsDiscardToDestroy()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", DeckNamespace);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            PutInTrash(mdp);

            Card bb = GetCard("BladeBattalion");
            PutInTrash(new[] { "BladeBattalion", "BacklashField" });

            Card vengefulSpirit = GetCard(VengefulSpiritsCardController.Identifier);
            PlayCard(vengefulSpirit);

            QuickHandStorage(ra, legacy);
            QuickShuffleStorage(baron.TurnTaker.Trash);

            // Act
            GoToStartOfTurn(BlackwoodForest);
            DecisionsYesNo = new bool[] { true, true };
            DecisionSelectCards = new[]
            {
                GetCardFromHand(ra, 0),
                GetCardFromHand(ra, 1),
                GetCardFromHand(legacy, 0),
                GetCardFromHand(legacy, 1),
            };

            GoToEndOfTurn(BlackwoodForest);

            // Assert
            AssertInTrash(BlackwoodForest, vengefulSpirit); // Required cards were discarded, Vengeful was trashed
            QuickHandCheck(-2, -2); // 2 cards each discarded to get rid of Vengeful
            QuickShuffleCheck(1); // Baron's trash shuffled due to Vengeful

            // One of these targets should be in play due to Vengeful drawing them from the trash
            // The other should be in the trash
            Assert.AreEqual(1,
                this.GameController.FindCardsWhere(card => card.IsInPlay && (card.Equals(mdp) || card.Equals(bb))).Count());
            Assert.AreEqual(1,
                this.GameController.FindCardsWhere(card => card.IsInTrash && (card.Equals(mdp) || card.Equals(bb))).Count());

        }

        [Test]
        public void TestVengefulSpiritsNoDiscard()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", DeckNamespace);

            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            PutInTrash(mdp);

            Card bb = GetCard("BladeBattalion");
            PutInTrash(new[] { "BladeBattalion", "BacklashField" });

            Card vengefulSpirit = GetCard(VengefulSpiritsCardController.Identifier);
            PlayCard(vengefulSpirit);

            QuickHandStorage(ra, legacy);
            QuickShuffleStorage(baron.TurnTaker.Trash);

            // Act
            GoToStartOfTurn(BlackwoodForest);

            DecisionsYesNo = new bool[] { false, false };

            GoToEndOfTurn(BlackwoodForest);

            // Assert
            AssertNotInTrash(BlackwoodForest, vengefulSpirit.Identifier); // Required cards were NOT discarded, Vengeful still in play
            QuickHandCheck(0, 0); // 2 cards each discarded to get rid of Vengeful
            QuickShuffleCheck(1); // Baron's trash shuffled due to Vengeful

            // One of these targets should be in play due to Vengeful drawing them from the trash
            // The other should be in the trash
            Assert.AreEqual(1,
                this.GameController.FindCardsWhere(card => card.IsInPlay && (card.Equals(mdp) || card.Equals(bb))).Count());
            Assert.AreEqual(1,
                this.GameController.FindCardsWhere(card => card.IsInTrash && (card.Equals(mdp) || card.Equals(bb))).Count());

        }

        [Test]
        public void TestDesolation()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", DeckNamespace);

            StartGame();
            QuickHPStorage(ra, legacy);
            QuickHandStorage(ra, legacy);

            DecisionSelectFunctions = new int?[] { 1, 0 };

            // Act
            Card desolation = GetCard(DesolationCardController.Identifier);
            PlayCard(desolation);

            GoToEndOfTurn(BlackwoodForest);

            // Assert
            AssertInTrash(BlackwoodForest, DesolationCardController.Identifier);

            // Ra opted not to discard required cards, so -3 HP, Legacy discarded so no damage taken
            QuickHPCheck(-3, 0);

            // Ra still has all of his hand, Legacy discard down to 1 card to avoid damage from Desolation
            QuickHandCheck(0, -3);

        }

        [Test]
        public void TestTheBlackTreeCheckCardProcedures()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", DeckNamespace);

            StartGame();

            int baronDeckCount = GetNumberOfCardsInDeck(baron);
            int raDeckCount = GetNumberOfCardsInDeck(ra);
            int legacyDeckCount = GetNumberOfCardsInDeck(legacy);


            // Act
            GoToStartOfTurn(BlackwoodForest);
            Card theBlackTree = GetCard(TheBlackTreeCardController.Identifier);
            PlayCard(theBlackTree);

            // All decks should have 1 less card due to being placed under The Black Tree
            Assert.AreEqual(baronDeckCount - 1, GetNumberOfCardsInDeck(baron));
            Assert.AreEqual(raDeckCount - 1, GetNumberOfCardsInDeck(ra));
            Assert.AreEqual(legacyDeckCount - 1, GetNumberOfCardsInDeck(legacy));


            GoToEndOfTurn(BlackwoodForest);

            // Assert

            // 2 cards left under The Black Tree after playing one
            Assert.AreEqual(2, GetCardsUnderCard(theBlackTree).Count());

        }

        [Test]
        public void TestTheBlackTree_Oblivaeon()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Cauldron.BlackwoodForest", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            Dictionary<Location, Card> topCardsOfSubDecks = new Dictionary<Location, Card>();

            SwitchBattleZone(haka);

            PlayCard(oblivaeon, GetCard("AeonWarrior"), isPutIntoPlay: true, overridePlayLocation: scionOne.TurnTaker.PlayArea);

            //When this card enters play, place the top card of each hero and villain deck face-down beneath it.
            Card tree = PlayCard("TheBlackTree");

            // Visible: OA, Ra, Legacy, Aeon Deck - 4
            // Invisible: Haka, Scion Deck, Mission Deck
            AssertNumberOfCardsUnderCard(tree, 4);

            IEnumerable<Card> underCards = tree.UnderLocation.Cards;
            foreach(Card c in underCards)
            {
                System.Console.WriteLine($"There is a card from the deck with name: {c.NativeDeck.GetFriendlyName()}");
            }
            Assert.That(underCards.Any(c => c.NativeDeck.GetFriendlyName() == "OblivAeon's deck"), () => "There was not an OblivAeon card under the Black Tree.");
            Assert.That(underCards.Any(c => c.NativeDeck.GetFriendlyName() == "Ra's deck"), () => "There was not a Ra card under the Black Tree.");
            Assert.That(underCards.Any(c => c.NativeDeck.GetFriendlyName() == "Legacy's deck"), () => "There was not a Legacy card under the Black Tree.");
            Assert.That(underCards.Any(c => c.NativeDeck.GetFriendlyName() == "The Aeon Men Deck"), () => "There was not an Aeon Men card under the Black Tree.");
            Assert.That(!underCards.Any(c => c.NativeDeck.GetFriendlyName() == "Haka's deck"), () => "There was a Haka card under the Black Tree.");
            Assert.That(!underCards.Any(c => c.NativeDeck.GetFriendlyName() == "The Scion Deck"), () => "There was a Scion card under the Black Tree.");
            Assert.That(!underCards.Any(c => c.NativeDeck.GetFriendlyName() == "The Mission Deck"), () => "There was a Mission card under the Black Tree.");

        }

        [Test]
        public void TestTheBlackTreeExhaustCardsUnderneath()
        {
            // Arrange
            SetupGameController("BaronBlade", "Ra", "Legacy", DeckNamespace);

            StartGame();

            // Act
            GoToStartOfTurn(BlackwoodForest);
            Card theBlackTree = GetCard(TheBlackTreeCardController.Identifier);
            PlayCard(theBlackTree);

            // Run thru the env. turn 3 times to exhaust the cards beneath it
            GoToEndOfTurn(BlackwoodForest);
            GoToEndOfTurn(BlackwoodForest);
            GoToEndOfTurn(BlackwoodForest);

            // Assert

            // Played all 3 cards underneath, The Black Tree should now have destroyed itself
            AssertInTrash(BlackwoodForest, theBlackTree);
        }
        [Test]
        public void TestMirrorWraithCopiesImpeccablePompadour()
        {
            SetupGameController("BaronBladeTeam", "Ra", "GreazerTeam", "TheWraith", "FrictionTeam", "Haka", "Cauldron.BlackwoodForest");
            StartGame();

            DestroyCards((Card c) => c.IsVillain && !c.IsCharacter);

            Card hair = GetCardInPlay("ImpeccablePompadour");

            GoToStartOfTurn(ra);

            QuickHPStorage(ra, greazerTeam);
            DealDamage(ra, hair, 1, DamageType.Melee);
            QuickHPCheck(-2, 0);

            Card mirror = PlayCard("MirrorWraith");
            AssertMaximumHitPoints(mirror, 4);

            //inherits retaliation damage
            DealDamage(ra, mirror, 1, DamageType.Melee);
            QuickHPCheck(-2, 0);

            //inherits indestructibility
            DealDamage(haka, hair, 5, DamageType.Melee);
            DealDamage(haka, mirror, 5, DamageType.Melee);
            AssertIsInPlay(hair);
            AssertIsInPlay(mirror);

            //inherits geazer self-damage
            GoToStartOfTurn(greazerTeam);
            QuickHPCheck(0, -6);
            //3 for real pompadour, 3, for the wraith
            Assert.AreEqual(4, mirror.HitPoints);
        }
        [Test]
        public void TestMirrorWraithWorksThroughSaveLoad()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Unity", DeckNamespace);
            StartGame();

            PlayCard("PlatformBot");
            PlayCard("MirrorWraith");

            SaveAndLoad();
            Card mirror = GetCardInPlay("MirrorWraith");
            QuickHPStorage(mirror);
            DealDamage(unity, mirror, 1, DamageType.Melee);
            QuickHPCheckZero();
        }
        [Test]
        public void TestMirrorWraithWorksChainCopyingOutOfPlayTarget()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Unity", DeckNamespace);
            StartGame();

            Card bot = PlayCard("PlatformBot");
            Card mirror1 = PlayCard("MirrorWraith");
            DestroyCard(bot);
            Card mirror2 = PlayCard("MirrorWraith");
            AssertMaximumHitPoints(mirror2, 3);
            QuickHPStorage(mirror2);
            DealDamage(unity, mirror2, 1, DamageType.Melee);
            QuickHPCheckZero();
        }
        [Test]
        public void TestMirrorWraithWorksThroughSaveLoadWhenTargetOutOfPlay()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Unity", DeckNamespace);
            StartGame();

            Card bot = PlayCard("PlatformBot");
            PlayCard("MirrorWraith");

            DestroyCard(bot);

            SaveAndLoad();
            Card mirror = GetCardInPlay("MirrorWraith");
            QuickHPStorage(mirror);
            DealDamage(unity, mirror, 1, DamageType.Melee);
            QuickHPCheckZero();
        }
        [Test]
        public void TestMirrorWraithWorksWithDestructionTriggers()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "AkashThriya", DeckNamespace);
            StartGame();

            Card seed = PlayCard("HealingPollen");
            Card mirror = PlayCard("MirrorWraith");

            QuickHandStorage(thriya);
            DestroyCard(mirror);
            QuickHandCheck(1);
        }
    }
}
