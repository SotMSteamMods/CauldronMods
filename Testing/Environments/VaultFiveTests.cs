using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class VaultFiveTests : CauldronBaseTest
    {

        #region VaultFiveHelperFunctions

        protected TurnTakerController vault5 { get { return FindEnvironment(); } }
        protected bool IsArtifact(Card card)
        {
            return card != null && card.DoKeywordsContain("artifact");
        }

        #endregion

        [Test()]
        public void TestVaultFiveWorks()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.VaultFive");
            StartGame();
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

        }

        [Test()]
        [Sequential]
        public void DecklistTest_Artifact_IsArtifact([Values("ByrgsNail", "MapToLostChoth", "SigQuilsSign", "TheCoinThatStrays", "TheEyeOfFomirhet")] string artifact)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.VaultFive");
            StartGame();

            GoToPlayCardPhase(vault5);

            DecisionSelectTurnTakers = new TurnTaker[] { haka.TurnTaker, haka.TurnTaker, ra.TurnTaker, legacy.TurnTaker };
            Card card = PlayCard(artifact);
            AssertInDeck(haka, card);
            AssertCardHasKeyword(card, "artifact", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Cryptid_IsCryptid([Values("Pelaga")] string cryptid)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.VaultFive");
            StartGame();

            GoToPlayCardPhase(vault5);

            Card card = PlayCard(cryptid);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "cryptid", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_OccultExper_IsOccultExpert([Values("DirectorWells")] string expert)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.VaultFive");
            StartGame();

            GoToPlayCardPhase(vault5);

            Card card = PlayCard(expert);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "occult expert", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Talent_IsTalent([Values("PhoebeNelson")] string talent)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.VaultFive");
            StartGame();

            GoToPlayCardPhase(vault5);

            Card card = PlayCard(talent);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "talent", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_NoKeyword_HasNoKeyword([Values("DeeperIntoTheVault", "ForbiddenKnowledge", "MomentOfSanity", 
            "ShatteredDisplayCase", "TheAncientsStir", "TheOuterMusic", "TheRatsInTheWalls")] string keywordLess)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.VaultFive");
            StartGame();

            GoToPlayCardPhase(vault5);

            Card card = PlayCard(keywordLess);
            AssertIsInPlay(card);
            Assert.IsFalse(card.Definition.Keywords.Any(), $"{card.Title} has keywords when it shouldn't.");
        }

        [Test()]
        public void TestArtifactsRemovedOnIncap()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.VaultFive");
            StartGame();

            DecisionSelectTurnTaker = haka.TurnTaker;
            Card artifact = PlayCard("ByrgsNail");
            AssertInDeck(haka, artifact);
            //incap haka
            DealDamage(baron, haka, 99, DamageType.Radiant);
            AssertIncapacitated(haka);
            AssertOutOfGame(artifact);
        }

        [Test()]
        [Sequential]
        public void TestArtifactsMoveUnderDeck([Values("ByrgsNail", "MapToLostChoth", "SigQuilsSign", "TheCoinThatStrays", "TheEyeOfFomirhet")] string artifactId)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.VaultFive");
            StartGame();

            DecisionSelectTurnTakers = new TurnTaker[] { haka.TurnTaker, haka.TurnTaker, ra.TurnTaker, legacy.TurnTaker };
            Card artifact = GetCard(artifactId);
            PlayCard(artifact);
            AssertInDeck(haka, artifact);
            AssertOnTopOfDeck(haka, artifact, 2);
        }

        [Test()]
        public void TestArtifactNotBreakReload()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.VaultFive");
            StartGame();

            Card nail = PlayCard("ByrgsNail");
            GoToStartOfTurn(ra);
            PrintSpecialStringsForCard(nail);
            SaveAndLoad();
            nail = GetCard("ByrgsNail");
            PrintSpecialStringsForCard(nail);

        }

        [Test()]
        public void TestByrgsNail()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            //a hero from its deck deals the 3 targets with the lowest HP 3 melee damage each
            DecisionSelectTurnTaker = haka.TurnTaker;
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            Card artifact = PlayCard("ByrgsNail");
            //lowest target is battalion, ra, legacy
            QuickHPCheck(0, -3, -3, -3, 0);     
        }

        [Test()]
        public void TestByrgsNail_EmptyDeck()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            //move all cards in deck to trash
            MoveCards(haka, haka.TurnTaker.Deck.Cards, haka.TurnTaker.Trash);
            //a hero from its deck deals the 3 targets with the lowest HP 3 melee damage each
            DecisionSelectTurnTaker = haka.TurnTaker;
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            Card artifact = PlayCard("ByrgsNail");
            //lowest target is battalion, ra, legacy
            QuickHPCheck(0, -3, -3, -3, 0);
            PrintSpecialStringsForCard(artifact);
        }

        [Test()]
        public void TestByrgsNail_Sentinels()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "TheSentinels", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            SetHitPoints(legacy, 6);
            SetHitPoints(ra, 7);
            //a hero from its deck deals the 3 targets with the lowest HP 3 melee damage each
            DecisionSelectTurnTaker = sentinels.TurnTaker;
            DecisionSelectCard = medico;
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, idealist, medico, mainstay, writhe);
            Card artifact = PlayCard("ByrgsNail");
            //lowest target is battalion, ra, legacy
            QuickHPCheck(0, -3, -3, -3, 0, 0,0,0,0);
        }

        [Test()]
        public void TestByrgsNail_Skyscraper()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "SkyScraper", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            SetHitPoints(legacy, 6);
            SetHitPoints(ra, 7);
            //a hero from its deck deals the 3 targets with the lowest HP 3 melee damage each
            DecisionSelectTurnTaker = sky.TurnTaker;
            IEnumerable<Card> offToSideSky = sky.TurnTaker.OffToTheSide.Cards.Where(c => c.IsCharacter);
            IEnumerable<Card> inPlaySky = sky.TurnTaker.PlayArea.Cards.Where(c => c.IsCharacter);
            AssertNextDecisionChoices(notIncluded: offToSideSky);
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, sky.CharacterCard);
            Card artifact = PlayCard("ByrgsNail");
            //lowest target is battalion, ra, legacy
            QuickHPCheck(0, -3, -3, -3, 0, 0);
        }

        [Test()]
        public void TestDeeperIntoTheVault_Reveal()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card art1 = PutOnDeck("ByrgsNail");
            Card art2 = PutOnDeck("SigQuilsSign");
            Card nonArt1 = PutOnDeck("MomentOfSanity");
            Card nonArt2 = PutOnDeck("PhoebeNelson");
            //When this card enters play, reveal the top {H} cards of the environment deck. Put any Artifact cards revealed into play and discard the other revealed cards.
            DecisionSelectTurnTaker = haka.TurnTaker;
            PlayCard("DeeperIntoTheVault");
            AssertInDeck(haka, art1);
            AssertInDeck(haka, art2);
            AssertInTrash(vault5, nonArt1);
            AssertInTrash(vault5, nonArt2);

        }

        [Test()]
        public void TestDeeperIntoTheVault_Increase()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            //put all artifacts in the env trash so that they can't be played
            PutInTrash(FindCardsWhere(c => vault5.TurnTaker.Deck.HasCard(c) && IsArtifact(c)));
            //Increase all psychic damage dealt by 1.
            PlayCard("DeeperIntoTheVault");
            QuickHPStorage(baron, tachyon);
            DealDamage(ra, baron, 2, DamageType.Psychic);
            DealDamage(tachyon, tachyon, 1, DamageType.Psychic);
            QuickHPCheck(-3, -2);

            //only psychic
            QuickHPUpdate();
            DealDamage(ra, baron, 2, DamageType.Fire);
            DealDamage(tachyon, tachyon, 1, DamageType.Melee);
            QuickHPCheck(-2, -1);
        }


        [Test()]
        public void TestDirectorWells_Oblivaeon_0Heroes()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "Cauldron.VaultFive", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Play this card next to the hero with the lowest HP
            //since there are no heroes in this battlezone, it should go to the trash
            Card wells = PlayCard("DirectorWells");
            AssertInTrash(wells);

        }
        [Test()]
        public void TestDirectorWells_MoveNextToImmunity()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Play this card next to the hero with the lowest HP
            Card dirWells = PlayCard("DirectorWells");
            AssertNextToCard(dirWells, tachyon.CharacterCard);

            //That hero is immune to psychic damage.
            QuickHPStorage(baron, ra, legacy, haka, tachyon);
            DealDamage(dirWells, c => c.IsNonEnvironmentTarget, 2, DamageType.Psychic);
            QuickHPCheck(-2, -2, -2, -2, 0);

            //only psychic damage
            QuickHPUpdate();
            DealDamage(dirWells, c => c.IsNonEnvironmentTarget, 2, DamageType.Fire);
            QuickHPCheck(-2, -2, -2, -2, -2);

        }

        [Test()]
        public void TestDirectorWells_CannotDrawCards()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon/TachyonFreedomSix", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card dirWells = PlayCard("DirectorWells");

            //Players with Artifact cards in their hand cannot draw cards.
            MoveCard(vault5, "ByrgsNail", haka.HeroTurnTaker.Hand);

            QuickHandStorage(ra, legacy, haka, tachyon);
            UsePower(tachyon.CharacterCard);
            QuickHandCheck(1, 1, 0, 1);

        }

        [Test()]
        public void TestForbiddenKnowledge_EndOfEnvironmentTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(vault5);
            PlayCard("ForbiddenKnowledge");
            //At the end of the environment turn, this card deals 1 hero 2 infernal damage.
            DecisionSelectTarget = haka.CharacterCard;
            QuickHPStorage(baron, ra, legacy, haka, tachyon);
            GoToEndOfTurn(vault5);
            QuickHPCheck(0, 0, 0, -2, 0);
        }

        [Test()]
        public void TestForbiddenKnowledge_EndOfHeroTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(ra);
            PlayCard("ForbiddenKnowledge");
            MoveCard(vault5, "ByrgsNail", ra.HeroTurnTaker.Hand);
            //At the end of each hero's turn, if they have an Artifact card in their hand, this card deals that hero 2 psychic damage.
            QuickHPStorage(baron, ra, legacy, haka, tachyon);
            GoToEndOfTurn(ra);
            QuickHPCheck(0, -2, 0, 0, 0);

            QuickHPUpdate();
            GoToEndOfTurn(legacy);
            QuickHPCheck(0, 0, 0, 0, 0);

        }

        [Test()]
        public void TestMapToLostChoth()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card hakaToDiscard = haka.HeroTurnTaker.Hand.TopCard;
            Card raToPlay = PutInHand("FlameBarrier");
            //When this card enters play, 1 player discards a card, a second player plays a card, and a third player’s hero uses a power.
            DecisionSelectTurnTakers = new TurnTaker[] { haka.TurnTaker, haka.TurnTaker, ra.TurnTaker, bunker.TurnTaker };
            DecisionSelectCards = new Card[] { hakaToDiscard, raToPlay };
            QuickHandStorage(haka, ra, bunker);
            PlayCard("MapToLostChoth");
            QuickHandCheck(-1, -1, 1);
            AssertInTrash(hakaToDiscard);
            AssertIsInPlay(raToPlay);
            
        }

        [Test()]
         public void TestMapToLostChoth_Only1AvailableHero()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();

            DealDamage(baron, ra, 100, DamageType.Fire);
            DealDamage(baron, bunker, 100, DamageType.Fire);
            Card hakaToDiscard = haka.HeroTurnTaker.Hand.TopCard;
            //When this card enters play, 1 player discards a card, a second player plays a card, and a third player’s hero uses a power.
            DecisionSelectTurnTakers = new TurnTaker[] { haka.TurnTaker, haka.TurnTaker};
            DecisionSelectCards = new Card[] { hakaToDiscard};
            QuickHandStorage(haka);
            AssertNextMessages(new string[] { "Map to Lost Choth is now a part of Haka's deck!", "There are no valid players to play a card.", "There are no valid players to use a power.", "Map to Lost Choth was moved to Haka's deck."});
            PlayCard("MapToLostChoth");
            QuickHandCheck(-1);
            AssertInTrash(hakaToDiscard);

        }

        [Test()]
        public void TestMapToLostChoth_NoAvailableHeroes()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "MobileDefensePlatform", "Cauldron.VaultFive", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            AssertNextMessages(new string[] { "There are no valid players to choose from.", "There are no valid players to discard a card.", "There are no valid players to play a card.", "There are no valid players to use a power.", "Map to Lost Choth was moved to Vault Five's deck." });
            PlayCard("MapToLostChoth");

        }

        [Test()]
        public void TestMomentOfSanity_DestroyOngoings()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka","Bunker", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            //When this card enters play, destroy {H - 2} ongoing cards.
            Card ongoing1 = PlayCard("LivingForceField");
            Card ongoing2 = PlayCard("FlameBarrier");
            Card ongoing3 = PlayCard("NextEvolution");
            Card ongoing4 = PlayCard("InspiringPresence");
            Card ongoing5 = PlayCard("Dominion");

            DecisionSelectCards = new Card[] { ongoing1, ongoing3 };
            PlayCard("MomentOfSanity");
            AssertInTrash(ongoing1);
            AssertIsInPlay(ongoing2);
            AssertInTrash(ongoing3);
            AssertIsInPlay(ongoing4);
            AssertIsInPlay(ongoing5);

        }

        [Test()]
        public void TestMomentOfSanity_Power_PlayCard()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Bunker", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Discard 1 Artifact card. If you do, play a card or destroy 1 environment card. Then destroy this card.
            Card moment = PlayCard("MomentOfSanity");
            DecisionSelectTurnTaker = ra.TurnTaker;
            Card artifact = PlayCard("ByrgsNail");
            PutInHand(artifact);
            Card raToPlay = PutInHand("FlameBarrier");
            DecisionSelectCard = raToPlay;
            DecisionSelectFunction = 0;
            UsePower(ra.CharacterCard, 1);
            AssertInTrash(ra, artifact);
            AssertInPlayArea(ra, raToPlay);
            AssertInTrash(vault5, moment);
        }

        [Test()]
        public void TestIssue813_MomentOfSanityGivesNonRealCardsPowers()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "SkyScraper", "TheSentinels", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Discard 1 Artifact card. If you do, play a card or destroy 1 environment card. Then destroy this card.
            Card moment = PlayCard("MomentOfSanity");
            Card instructions = sentinels.HeroTurnTaker.GetAllCards(realCardsOnly: false).Where(c => !c.IsRealCard).FirstOrDefault();
            Assert.That(FindCardController(moment).AskIfContributesPowersToCardController(FindCardController(instructions)) == null, "Moment of Sanity granted a power to a non-real card!");
            IEnumerable<Card> offToSideSky = sky.TurnTaker.OffToTheSide.Cards.Where(c => c.IsCharacter);
            foreach(Card notSky in offToSideSky)
            {
                Assert.That(FindCardController(moment).AskIfContributesPowersToCardController(FindCardController(notSky)) == null, "Moment of Sanity granted a power to an off to the side card!");

            }
        }

        [Test()]
        public void TestMomentOfSanity_Power_DestroyEnvironmentCard()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Bunker", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Discard 1 Artifact card. If you do, play a card or destroy 1 environment card. Then destroy this card.
            Card moment = PlayCard("MomentOfSanity");
            Card director = PlayCard("DirectorWells");
            DecisionSelectTurnTaker = ra.TurnTaker;
            Card artifact = PlayCard("ByrgsNail");
            PutInHand(artifact);
            DecisionSelectFunction = 1;
            UsePower(ra.CharacterCard, 1);
            AssertInTrash(ra, artifact);
            AssertInTrash(vault5, moment);
            AssertInTrash(director);
        }

        [Test()]
        public void TestPelaga_EndOfTurnEnvironment()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "Bunker", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            //At the end of the environment turn, this card deals each target from the hero deck with the most Artifacts 3 toxic damage.
            DecisionSelectTurnTaker = luminary.TurnTaker;
            PlayCard("ByrgsNail");
            Card turret = PlayCard("RegressionTurret");
            PlayCard("Pelaga");
            GoToPlayCardPhase(vault5);
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, luminary.CharacterCard, haka.CharacterCard, bunker.CharacterCard, turret);
            GoToEndOfTurn(vault5);
            QuickHPCheck(0, 0, -3, 0, 0, -3);

        }

        [Test()]
        public void TestPelaga_DealtDamage()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "Bunker", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Whenever this card is dealt damage by a hero target, that hero must discard 1 non-Artifact card.   
            Card pelaga = PlayCard("Pelaga");
            DecisionSelectTurnTaker = ra.TurnTaker;
            Card artifact = PlayCard("ByrgsNail");
            PutInHand(artifact);
            AssertNextDecisionChoices(notIncluded: new Card[] { artifact });
            QuickHandStorage(ra);
            DealDamage(ra, pelaga, 2, DamageType.Fire);
            QuickHandCheck(-1);

        }

        [Test()]
        public void TestPhoebeNelson_ArtifactEntersPlay()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "Bunker", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Whenever an Artifact card enters play, this card deals itself 2 psychic damage.
            Card phoebe = PlayCard("PhoebeNelson");
            DecisionSelectTurnTaker = ra.TurnTaker;
            QuickHPStorage(phoebe);
            Card artifact = PlayCard("SigQuilsSign");
            QuickHPCheck(-2);
            QuickHPUpdate();
            PlayCard(artifact);
            QuickHPCheck(-2);
            QuickHPUpdate();
            PlayCard("FlameBarrier");
            QuickHPCheckZero();
            PlayCard("DirectorWells");
            QuickHPCheckZero();

        }

        [Test()]
        public void TestPhoebeNelson_EndOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "Bunker", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            Card artifact = PlayCard("SigQuilsSign");
            //At the end of the environment turn, this card deals each villain target 1 fire damage and each hero with an Artifact card in their hand 1 fire damage.
            GoToPlayCardPhase(vault5);
            Card phoebe = PlayCard("PhoebeNelson");
            DecisionSelectTurnTaker = ra.TurnTaker;
            QuickHPStorage(phoebe);
            MoveCard(ra, artifact, ra.HeroTurnTaker.Hand);
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, luminary.CharacterCard, haka.CharacterCard, bunker.CharacterCard, battalion);
            GoToEndOfTurn(vault5);
            QuickHPCheck(-1, -1, 0, 0, 0, -1);
        }

        [Test()]
        public void TestShatteredDisplayCase_EndOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "Bunker", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            DecisionSelectTurnTakers = new TurnTaker[] { ra.TurnTaker, bunker.TurnTaker, bunker.TurnTaker };
            Card artifact1 = PlayCard("SigQuilsSign");
            Card artifact2 = PlayCard("ByrgsNail");
            Card artifact3 = PlayCard("TheEyeOfFomirhet");
            MoveCard(ra, artifact1, ra.TurnTaker.Trash);
            MoveCard(bunker, artifact2, bunker.TurnTaker.Trash);
            MoveCard(bunker, artifact3, bunker.TurnTaker.Trash);

            GoToPlayCardPhase(vault5);
            //At the end of the environment turn, place all Artifact cards in all trash piles on top of their respective decks.
            PlayCard("ShatteredDisplayCase");
            GoToEndOfTurn(vault5);
            AssertOnTopOfDeck(ra, artifact1);
            AssertOnTopOfDeck(bunker, artifact2, 1);
            AssertOnTopOfDeck(bunker, artifact3, 0);

        }

        // https://github.com/SotMSteamMods/CauldronMods/issues/1708
        [Test()]
        public void TestShatteredDisplayCase_EndOfTurn_OnlyMovesArtifacts()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "Bunker", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();

            GoToStartOfTurn(vault5);
            DecisionSelectTurnTakers = [ra.TurnTaker, bunker.TurnTaker, bunker.TurnTaker];
            Card artifact1 = PlayCard("SigQuilsSign");
            Card artifact2 = PlayCard("ByrgsNail");
            Card artifact3 = PlayCard("TheEyeOfFomirhet");
            MoveCard(ra, artifact1, ra.TurnTaker.Trash);
            MoveCard(bunker, artifact2, bunker.TurnTaker.Trash);
            MoveCard(bunker, artifact3, bunker.TurnTaker.Trash);

            // move some extra card into the trash
            Card staff = MoveCard(ra, "TheStaffOfRa", ra.TurnTaker.Trash);

            GoToPlayCardPhase(vault5);
            //At the end of the environment turn, place all Artifact cards in all trash piles on top of their respective decks.
            PlayCard("ShatteredDisplayCase");
            GoToEndOfTurn(vault5);
            AssertOnTopOfDeck(ra, artifact1);
            AssertInTrash(staff);
            AssertOnTopOfDeck(bunker, artifact2, 1);
            AssertOnTopOfDeck(bunker, artifact3, 0);

        }

        [Test()]
        public void TestShatteredDisplayCase_TargetDestroyed_NonArtifactStacked()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "Bunker", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card target = PlayCard("BladeBattalion");
            GoToPlayCardPhase(vault5);
            //Whenever a target is destroyed, discard the top card of the environment deck. If an Artifact card is discarded this way, put it into play and destroy this card.
            Card shattered = PlayCard("ShatteredDisplayCase");
            Card topDeck = PutOnDeck("MomentOfSanity");
            DestroyCard(target, haka.CharacterCard);
            AssertInTrash(topDeck);
            AssertIsInPlay(shattered);
        }

        [Test()]
        public void TestShatteredDisplayCase_TargetDestroyed_ArtifactStacked()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "Bunker", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card target = PlayCard("BladeBattalion");
            GoToPlayCardPhase(vault5);
            //Whenever a target is destroyed, discard the top card of the environment deck. If an Artifact card is discarded this way, put it into play and destroy this card.
            Card shattered = PlayCard("ShatteredDisplayCase");
            Card topDeck = PutOnDeck("ByrgsNail");
            DecisionSelectTurnTaker = bunker.TurnTaker;
            DestroyCard(target, haka.CharacterCard);
            AssertInTrash(shattered);
            AssertInDeck(bunker, topDeck);
        }

        [Test()]
        public void TestSigQuilsSign()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            //When this card enters play, increase damage dealt to and by targets from its deck by 1 until the start of their next turn DecisionSelectTurnTaker = haka.TurnTaker;
            DecisionSelectTurnTaker = luminary.TurnTaker;
            Card artifact = PlayCard("SigQuilsSign");
            Card turret = PlayCard("RegressionTurret");
            QuickHPStorage(luminary.CharacterCard, baron.CharacterCard, ra.CharacterCard, turret);
            DealDamage(baron, luminary, 1, DamageType.Fire);
            DealDamage(baron, turret, 1, DamageType.Fire);
            DealDamage(baron, ra, 1, DamageType.Fire);
            DealDamage(luminary, baron, 1, DamageType.Fire);
            DealDamage(turret, baron, 1, DamageType.Fire);
            DealDamage(ra, baron, 1, DamageType.Fire);
            QuickHPCheck(-2, -5, -1, -2);

            GoToStartOfTurn(luminary);
            SetAllTargetsToMaxHP();
            QuickHPUpdate();
            DealDamage(baron, luminary, 1, DamageType.Fire);
            DealDamage(baron, turret, 1, DamageType.Fire);
            DealDamage(baron, ra, 1, DamageType.Fire);
            DealDamage(luminary, baron, 1, DamageType.Fire);
            DealDamage(turret, baron, 1, DamageType.Fire);
            DealDamage(ra, baron, 1, DamageType.Fire);
            QuickHPCheck(-1, -3, -1, -1);

        }

        [Test()]
        public void TestTheAncientsStir_EndOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            //At the end of the environment turn, this card deals the target with the second-highest HP 1 infernal damage.
            GoToPlayCardPhase(vault5);
            PlayCard("TheAncientsStir");
            //second highest hp is haka
            QuickHPStorage(baron, ra, luminary, haka);
            GoToEndOfTurn(vault5);
            QuickHPCheck(0, 0, 0, -1);
        }

        [Test()]
        public void TestTheAncientsStir_IncreaseDamage()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Whenever an Artifact card enters play, increase damage dealt by this card by 1.
            Card ancient = PlayCard("TheAncientsStir");
            QuickHPStorage(baron);
            DealDamage(ancient, baron, 1, DamageType.Infernal);
            QuickHPCheck(-1);

            DecisionSelectTurnTaker = haka.TurnTaker;
            Card artifact = PlayCard("SigQuilsSign");

            QuickHPUpdate();
            DealDamage(ancient, baron, 1, DamageType.Infernal);
            QuickHPCheck(-2);

            PlayCard(artifact);

            QuickHPUpdate();
            DealDamage(ancient, baron, 1, DamageType.Infernal);
            QuickHPCheck(-3);

            //check persistent over rounds
            GoToStartOfTurn(baron);

            PlayCard(artifact);

            QuickHPUpdate();
            DealDamage(ancient, baron, 1, DamageType.Infernal);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestTheCoinThatStrays()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            //When this card enters play, a hero from its deck deals themselves and 1 other target 1 psychic damage each and plays a card.
            DecisionSelectTurnTaker = haka.TurnTaker;
            DecisionSelectTarget = baron.CharacterCard;
            Card cardToPlay = PutInHand("Mere");
            DecisionSelectCard = cardToPlay;
            QuickHPStorage(baron, ra, luminary, haka);
            Card artifact = PlayCard("TheCoinThatStrays");
            QuickHPCheck(-1, 0, 0, -1);
            AssertIsInPlay(cardToPlay);
        }

        [Test()]
        public void TestTheCoinThatStrays_Sentinels()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "TheSentinels", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            //When this card enters play, a hero from its deck deals themselves and 1 other target 1 psychic damage each and plays a card.
            DecisionSelectTurnTaker = sentinels.TurnTaker;
            Card cardToPlay = PutInHand("SentinelTactics");
            DecisionSelectCards =  new Card[] { mainstay, baron.CharacterCard, cardToPlay };
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, luminary.CharacterCard, haka.CharacterCard, medico, mainstay, idealist, writhe);
            Card artifact = PlayCard("TheCoinThatStrays");
            QuickHPCheck(-1, 0, 0, 0,0,-1,0,0);
            AssertIsInPlay(cardToPlay);
        }

        [Test()]
        public void TestTheEyeOfFomirhet()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(haka, 20);
            //When this card enters play, a hero from its deck regains 4HP and deals each other hero target 1 infernal damage.
            DecisionSelectTurnTaker = haka.TurnTaker;
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, luminary.CharacterCard, haka.CharacterCard);
            PlayCard("TheEyeOfFomirhet");
            QuickHPCheck(0, -1, -1, 4);
           
        }

        [Test()]
        public void TestTheEyeOfFomirhet_Sentinels()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "TheSentinels", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(mainstay, 6);
            //When this card enters play, a hero from its deck regains 4HP and deals each other hero target 1 infernal damage.
            DecisionSelectTurnTaker = sentinels.TurnTaker;
            DecisionSelectCard = mainstay;
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, luminary.CharacterCard, haka.CharacterCard, idealist, mainstay, medico, writhe);
            PlayCard("TheEyeOfFomirhet");
            //sentinels are nemesis with each other, so +1
            QuickHPCheck(0, -1, -1, -1,-2, 4, -2, -2);

        }

        [Test()]
        public void TestTheOuterMusic_EndOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card turret = PlayCard("RegressionTurret");
            //At the end of the environment turn, each hero target deals itself 1 psychic damage.
            GoToPlayCardPhase(vault5);
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, luminary.CharacterCard, haka.CharacterCard, turret);
            PlayCard("TheOuterMusic");
            GoToEndOfTurn(vault5);
            QuickHPCheck(0, -1, -1, -1, -1);

        }

        [Test()]
        public void TestTheOuterMusic_Destroy()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            //When a player plays an Artifact card from their hand, destroy this card."
            Card  outerMusic = PlayCard("TheOuterMusic");
            DecisionSelectTurnTaker = ra.TurnTaker;
            Card artifact = PlayCard("ByrgsNail");
            AssertIsInPlay(outerMusic);
            PutInHand(artifact);
            PlayCard(artifact);
            AssertInTrash(outerMusic);

        }

        [Test()]
        public void TestTheRatsInTheWall_Increase()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Increase damage dealt to heroes with Artifact cards in their hands by 1
            DecisionSelectTurnTaker = ra.TurnTaker;
            Card artifact = PlayCard("ByrgsNail");
            PutInHand(artifact);
            PlayCard("TheRatsInTheWalls");

            QuickHPStorage(ra, luminary, haka);
            DealDamage(baron, c => c.IsHeroCharacterCard, 1, DamageType.Melee);
            QuickHPCheck(-2, -1, -1);

        }

        [Test()]
        public void TestTheRatsInTheWall_Draw()
        {
            SetupGameController("BaronBlade", "Ra", "Luminary", "Haka", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            
            DecisionSelectTurnTaker = ra.TurnTaker;
            Card artifact = PlayCard("ByrgsNail");
            PutOnDeck(haka, artifact);
            Card vitality = PutInHand("VitalitySurge");
            PlayCard("TheRatsInTheWalls");

            //Whenever a player draws an Artifact card, they may draw a card.
            QuickHandStorage(haka);
            PlayCard(vitality);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestTheRatsInTheWall_DrawOptional()
        {
            SetupGameController("Omnitron", "Ra", "Luminary", "Haka", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            DecisionSelectTurnTaker = ra.TurnTaker;
            Card artifact = PlayCard("ByrgsNail");
            PutOnDeck(haka, artifact);
            Card vitality = PutInHand("VitalitySurge");
            PlayCard("TheRatsInTheWalls");
            PlayCard("InterpolationBeam");
            //Whenever a player draws an Artifact card, they may draw a card.
            QuickHandStorage(haka);
            DecisionYesNo = false;
            PlayCard(vitality);
            QuickHandCheck(0);
        }

    }
}
