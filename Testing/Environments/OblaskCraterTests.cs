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
    public class OblaskCraterTests : CauldronBaseTest
    {
        private const string DeckNamespace = "Cauldron.OblaskCrater";

        protected TurnTakerController OblaskCrater => FindEnvironment();


        [Test]
        public void TestOblaskCraterLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            // Assert
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());
        }

        #region Test Fresh Tracks

        /*
         * When this card enters play, reveal cards from the top of the environment deck 
         * until a target is revealed, place it beneath this card, and discard the rest.
         * 
         * Cards beneath this one are not considered in play. When a target enters play, 
         * the players may destroy this card to play the card beneath it
        */
        [Test]
        public void TestFreshTracks()
        {
            Card freshTracks;
            Card oblaskObjectY3A;
            Card inscrutableEcology;
            Card shadowOfOblask;
            int numberOfCardsInDeck;
            int numberOfCardsInTrash;

            // Arrange & Act
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);
            freshTracks = base.GetCard("FreshTracks");
            oblaskObjectY3A = base.GetCard("OblaskObjectY3A");
            inscrutableEcology = base.GetCard("InscrutableEcology");
            shadowOfOblask = base.GetCard("ShadowOfOblask");

            base.env.PutOnDeck(shadowOfOblask);
            base.env.PutOnDeck(inscrutableEcology);
            base.env.PutOnDeck(oblaskObjectY3A);

            numberOfCardsInDeck = base.env.TurnTaker.Deck.NumberOfCards;
            numberOfCardsInTrash = base.env.TurnTaker.Trash.NumberOfCards;

            freshTracks = PlayCard(freshTracks);

            AssertNumberOfCardsInDeck(base.env, numberOfCardsInDeck - 4);
            AssertNumberOfCardsInTrash(base.env, numberOfCardsInTrash + 2);
            AssertNumberOfCardsUnderCard(freshTracks, 1);
            AssertNotInPlay(freshTracks.UnderLocation.Cards);

            DecisionsYesNo = new bool[] { true };
            PlayCard("BladeBattalion");
            AssertIsInPlay(shadowOfOblask);
        }

        /*
         * When this card enters play, play the top card of the environment deck.
         * Whenever a predator card destroys another target, that predator deals each hero {H - 2} melee damage.
        */
        [Test]
        public void TestGameTrail()
        {
            int numberOfCardsInDeck;
            Card moonWatcher;
            Card gameTrail;
            Card shadowOfOblask;
            Card bladeBattalion;

            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);
            StartGame();

            moonWatcher = base.PutOnDeck("MoonWatcher");
            shadowOfOblask = base.PutIntoPlay("ShadowOfOblask");
            bladeBattalion = base.PutIntoPlay("BladeBattalion");

            numberOfCardsInDeck = base.env.TurnTaker.Deck.NumberOfCards;

            gameTrail = base.PlayCard("GameTrail");
            AssertNumberOfCardsInDeck(base.env, numberOfCardsInDeck - 2);

            QuickHPStorage(baron, ra, legacy, haka);
            DealDamage(shadowOfOblask, bladeBattalion, 6, DamageType.Melee);
            QuickHPCheck(0, -1, -1, -1);

        }
        #endregion Test Fresh Tracks

        #region Test Gentle Transport

        /* 
         * At the end of the environment turn, move this card next to a hero. Then the predator card with the highest HP deals
         * each target in this card's play area {H - 1} melee damage. 
         * If the hero next to this card plays a card with a power on it, they may immediately use a power on that card.
         */
        [Test]
        public void TestGentleTransport()
        {
            Card gentleTransport;
            Card swiftBot;
            Card turretBot;
            Card shadowOfOblask;

            SetupGameController("BaronBlade", "Unity", "Legacy", "Haka", DeckNamespace);
            StartGame();
            swiftBot = PutIntoPlay("SwiftBot");
            turretBot = PutIntoPlay("TurretBot");
            shadowOfOblask = base.PutIntoPlay("ShadowOfOblask");
            gentleTransport = base.PutIntoPlay("GentleTransport");

            DecisionSelectCards = new Card[] { baron.CharacterCard, unity.CharacterCard, unity.CharacterCard, gentleTransport, swiftBot, turretBot };
            base.GameController.SkipToTurnTakerTurn(base.env);

            QuickHPStorage(unity.CharacterCard, gentleTransport, swiftBot, turretBot, shadowOfOblask, legacy.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(base.env);
            QuickHPCheck(-2, -2, -2, -2, 0, 0, 0);
        }

        /* 
         * At the end of the environment turn, move this card next to a hero. Then the predator card with the highest HP deals
         * each target in this card's play area {H - 1} melee damage. 
         * If the hero next to this card plays a card with a power on it, they may immediately use a power on that card.
         */
        [Test]
        public void TestGentleTransportUsePower()
        {
            Card gentleTransport;

            SetupGameController("BaronBlade", "Unity", "Legacy", "Haka", DeckNamespace);
            StartGame();

            gentleTransport = base.PutIntoPlay("GentleTransport");

            DecisionSelectCards = new Card[] { legacy.CharacterCard, baron.CharacterCard };
            base.GameController.SkipToTurnTakerTurn(base.env);

            SetHitPoints(legacy, legacy.CharacterCard.MaximumHitPoints.Value - 1);
            QuickHPStorage(baron.CharacterCard, unity.CharacterCard, gentleTransport, legacy.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(base.env);
            QuickHPCheck(0, 0, 0, 0, 0);

            base.DestroyNonCharacterVillainCards();
            base.GameController.SkipToTurnTakerTurn(legacy);
            GoToPlayCardPhase(legacy);

            PlayCard("MotivationalCharge");
            QuickHPCheck(-3, 0, 0, 1, 0);
        }
        #endregion

        #region Test Industrious Hulk

        /*
         * Whenever a hero draws a card, this card deals them 1 melee damage.
         * When this card is destroyed, each player may put the top card of 
         * their deck into their hand.
        */
        [Test]
        public void TestIndustriousHulk()
        {
            Card industriousHulk;

            SetupGameController("BaronBlade", "Unity", "Legacy", "Haka", DeckNamespace);
            StartGame();
            base.DestroyNonCharacterVillainCards();

            industriousHulk = base.PutIntoPlay("IndustriousHulk");

            QuickHPStorage(baron, unity, legacy, haka);
            base.DrawCard(unity, 1);
            base.DrawCard(haka, 1);
            QuickHPCheck(0, -1, 0, -1);

            DecisionsYesNo = new bool[] { true, true, true };
            QuickHandStorage(unity, legacy, haka);
            DealDamage(legacy.CharacterCard, industriousHulk, 10, DamageType.Melee);
            QuickHandCheck(1, 1, 1);
        }

        #endregion Test Industrious Hulk

        #region Load Inscrutable Ecology

        /* 
         * When this card enters play, reveal cards from the top of the environmnet deck until {H - 1} targets are 
         * revealed, put them into play, and discard the other revealed cards.
         * Reduce damage dealt by environment targets by 1. At the start of the environment turn, destroy this card.
         */
        [Test]
        public void LoadInscrutableEcology()
        {
            Card oblaskObjectY3A;
            Card inscrutableEcology;
            Card shadowOfOblask;
            Card swarmOfFangs;
            int numberOfCardsInDeck;
            int numberOfCardsInTrash;

            // Arrange & Act
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);
            inscrutableEcology = base.GetCard("InscrutableEcology");
            oblaskObjectY3A = base.GetCard("OblaskObjectY3A");
            shadowOfOblask = base.GetCard("ShadowOfOblask");
            swarmOfFangs = base.GetCard("SwarmOfFangs");

            base.env.PutOnDeck(shadowOfOblask);
            base.env.PutOnDeck(inscrutableEcology);
            base.env.PutOnDeck(oblaskObjectY3A);
            base.env.PutOnDeck(swarmOfFangs);

            numberOfCardsInDeck = base.env.TurnTaker.Deck.NumberOfCards;
            numberOfCardsInTrash = base.env.TurnTaker.Trash.NumberOfCards;

            inscrutableEcology = PlayCard(inscrutableEcology);

            AssertNumberOfCardsInDeck(base.env, numberOfCardsInDeck - 4);
            AssertNumberOfCardsInTrash(base.env, numberOfCardsInTrash + 1);

            AssertIsInPlay(shadowOfOblask);
            AssertIsInPlay(swarmOfFangs);

            QuickHPStorage(baron.CharacterCard, shadowOfOblask, swarmOfFangs, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            
            DealDamage(shadowOfOblask, legacy, 1, DamageType.Cold);
            QuickHPCheck(0, 0, 0, 0, 0, 0);
            DealDamage(shadowOfOblask, legacy, 2, DamageType.Cold);
            QuickHPCheck(0, 0, 0, 0, -1, 0);
        }

        #endregion Load Inscrutable Ecology
    }
}
