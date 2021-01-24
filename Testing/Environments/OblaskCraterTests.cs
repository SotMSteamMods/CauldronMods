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

        #region Test Inscrutable Ecology

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

        #endregion Test Inscrutable Ecology

        #region Test Invasive Growth

        /* 
         * When this card enters play, discard the top 2 cards of the enviroment deck, then put a random target 
         * from the environment trash into play.
         * When an environment target is destroyed, all other environment cards become indestructible until the 
         * start of the next turn.
         */
        [Test]
        public void TestInvasiveGrowthEnterPlay()
        {
            /* 
             * When this card enters play, discard the top 2 cards of the enviroment deck, then put a random target 
             * from the environment trash into play.
             */
            Card invasiveGrowth;
            Card oblaskObjectY3A;
            Card inscrutableEcology;
            Card swarmOfFangs;
            int numberOfCardsInDeck;
            int numberOfCardsInTrash;

            // Arrange & Act
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);
            invasiveGrowth = base.GetCard("InvasiveGrowth");
            swarmOfFangs = base.GetCard("SwarmOfFangs");
            inscrutableEcology = base.GetCard("InscrutableEcology");
            oblaskObjectY3A = base.GetCard("OblaskObjectY3A");

            base.env.PutOnDeck(inscrutableEcology);
            base.env.PutOnDeck(swarmOfFangs);
            base.env.PutOnDeck(oblaskObjectY3A);

            numberOfCardsInDeck = base.env.TurnTaker.Deck.NumberOfCards;
            numberOfCardsInTrash = base.env.TurnTaker.Trash.NumberOfCards;

            invasiveGrowth = PlayCard(invasiveGrowth);

            AssertNumberOfCardsInDeck(base.env, numberOfCardsInDeck - 3);
            AssertNumberOfCardsInTrash(base.env, numberOfCardsInTrash + 1);
            AssertIsInPlay(swarmOfFangs);
        }

        [Test]
        public void TestInvasiveGrowthEnviromentTargetDestroyed()
        {
            /* 
             * When an environment target is destroyed, all other environment cards become indestructible until the 
             * start of the next turn.
             */
            Card invasiveGrowth;
            Card oblaskObjectY3A;
            Card inscrutableEcology;
            Card swarmOfFangs;
            Card shadowOfOblask;

            // Arrange & Act
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);
            invasiveGrowth = base.GetCard("InvasiveGrowth");
            swarmOfFangs = base.GetCard("SwarmOfFangs");
            inscrutableEcology = base.GetCard("InscrutableEcology");
            oblaskObjectY3A = base.GetCard("OblaskObjectY3A");

            base.env.PutOnDeck(inscrutableEcology);
            base.env.PutOnDeck(swarmOfFangs);
            base.env.PutOnDeck(oblaskObjectY3A);

            invasiveGrowth = PlayCard(invasiveGrowth);
            shadowOfOblask = PutIntoPlay("ShadowOfOblask");

            GoToStartOfTurn(ra);
            DealDamage(ra, swarmOfFangs, 4, DamageType.Melee);
            DealDamage(ra, shadowOfOblask, 20, DamageType.Melee);
            AssertIsInPlay(shadowOfOblask);

            GoToStartOfTurn(legacy);
            AssertNotInPlay(shadowOfOblask);
        }
        #endregion Test Invasive Growth

        #region Test Metal Scavenger

        [Test]
        public void TestMetalScavenger()
        {
            Card metalScavenger;
            int numberOfCardsInBaronTrash;
            int numberOfCardsInRaTrash;
            int numberOfCardsInLegacyTrash;
            int numberOfCardsInHakaTrash;
            int numberOfCardsInEnvTrash;
            /* 
             * At the end of the environment turn, move the top card of each other trash pile beneath this card.
             * Then, this card deals each other target 1 toxic damage and itself 1 fire damage.
             * Cards beneath this one are not considered in play.
             */
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);
            PutInTrash("BladeBattalion", "BlazingTornado", "BackFistStrike", "Dominion");

            base.GameController.SkipToTurnTakerTurn(env);

            numberOfCardsInBaronTrash = baron.TurnTaker.Trash.NumberOfCards;
            numberOfCardsInRaTrash = ra.TurnTaker.Trash.NumberOfCards;
            numberOfCardsInLegacyTrash = legacy.TurnTaker.Trash.NumberOfCards;
            numberOfCardsInHakaTrash = haka.TurnTaker.Trash.NumberOfCards;
            numberOfCardsInEnvTrash = env.TurnTaker.Trash.NumberOfCards;
            metalScavenger = PutIntoPlay("MetalScavenger");

            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, metalScavenger);
            GoToEndOfTurn(env);
            QuickHPCheck(-1, -1, -1, -1, -1);
            AssertNumberOfCardsInTrash(baron, numberOfCardsInBaronTrash - 1);
            AssertNumberOfCardsInTrash(ra, numberOfCardsInRaTrash - 1);
            AssertNumberOfCardsInTrash(legacy, numberOfCardsInLegacyTrash - 1);
            AssertNumberOfCardsInTrash(haka, numberOfCardsInHakaTrash - 1);
            AssertNumberOfCardsInTrash(env, numberOfCardsInEnvTrash);
            AssertNumberOfCardsUnderCard(metalScavenger, 4);
            AssertNotInPlay(metalScavenger.UnderLocation.Cards);
        }

        #endregion Test Metal Scavenger

        #region Test Moon Watcher
        /*
         * At the end of the environment turn, this card deals each non-environment target 1 sonic damage.
         * When this card is destroyed, if it has 0 or fewer HP, reveal the top card of each deck and 
         * replace or discard each one.
         */
        [Test]
        public void TestMoonWatcherEndOfTurn()
        {
            /*
             * At the end of the environment turn, this card deals each non-environment target 1 sonic damage.
             */
            Card moonWatcher;

            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            base.GameController.SkipToTurnTakerTurn(env);
            moonWatcher = PutIntoPlay("MoonWatcher");
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, moonWatcher);
            GoToEndOfTurn(env);
            QuickHPCheck(-1, -1, -1, -1, 0);
        }
        [Test]
        public void TestMoonWatcherDestroyReduceTo0()
        {
            /*
             * When this card is destroyed, if it has 0 or fewer HP, reveal the top card of each deck and 
             * replace or discard each one.
             */
            Card moonWatcher;
            int numberOfCardsInBaronTrash;
            int numberOfCardsInRaTrash;
            int numberOfCardsInLegacyTrash;
            int numberOfCardsInHakaTrash;
            int numberOfCardsInEnvTrash;

            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            moonWatcher = PutIntoPlay("MoonWatcher");
            numberOfCardsInBaronTrash = baron.TurnTaker.Trash.NumberOfCards;
            numberOfCardsInRaTrash = ra.TurnTaker.Trash.NumberOfCards;
            numberOfCardsInLegacyTrash = legacy.TurnTaker.Trash.NumberOfCards;
            numberOfCardsInHakaTrash = haka.TurnTaker.Trash.NumberOfCards;
            numberOfCardsInEnvTrash = env.TurnTaker.Trash.NumberOfCards;

            DecisionSelectTurnTakers = new TurnTaker[] { baron.TurnTaker, ra.TurnTaker, legacy.TurnTaker, haka.TurnTaker, env.TurnTaker };
            DecisionMoveCardDestinations = new MoveCardDestination[] 
            { 
                new MoveCardDestination( baron.TurnTaker.Trash), 
                new MoveCardDestination(ra.TurnTaker.Trash), 
                new MoveCardDestination(legacy.TurnTaker.Trash), 
                new MoveCardDestination(haka.TurnTaker.Trash),
                new MoveCardDestination(env.TurnTaker.Trash)
            };
            base.DealDamage(legacy, moonWatcher, 15, DamageType.Melee);

            AssertNumberOfCardsInTrash(baron, numberOfCardsInBaronTrash + 1);
            AssertNumberOfCardsInTrash(ra, numberOfCardsInRaTrash + 1);
            AssertNumberOfCardsInTrash(legacy, numberOfCardsInLegacyTrash + 1);
            AssertNumberOfCardsInTrash(haka, numberOfCardsInHakaTrash + 1);
            AssertNumberOfCardsInTrash(env, numberOfCardsInEnvTrash + 2);
        }
        [Test]
        public void TestMoonWatcherDestroyReduceBelow0()
        {
            /*
             * When this card is destroyed, if it has 0 or fewer HP, reveal the top card of each deck and 
             * replace or discard each one.
             */
            Card moonWatcher;
            int numberOfCardsInBaronTrash;
            int numberOfCardsInRaTrash;
            int numberOfCardsInLegacyTrash;
            int numberOfCardsInHakaTrash;
            int numberOfCardsInEnvTrash;

            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);

            moonWatcher = PutIntoPlay("MoonWatcher");
            numberOfCardsInBaronTrash = baron.TurnTaker.Trash.NumberOfCards;
            numberOfCardsInRaTrash = ra.TurnTaker.Trash.NumberOfCards;
            numberOfCardsInLegacyTrash = legacy.TurnTaker.Trash.NumberOfCards;
            numberOfCardsInHakaTrash = haka.TurnTaker.Trash.NumberOfCards;
            numberOfCardsInEnvTrash = env.TurnTaker.Trash.NumberOfCards;

            DecisionSelectTurnTakers = new TurnTaker[] { baron.TurnTaker, ra.TurnTaker, legacy.TurnTaker, haka.TurnTaker, env.TurnTaker };
            DecisionMoveCardDestinations = new MoveCardDestination[]
            {
                new MoveCardDestination( baron.TurnTaker.Trash),
                new MoveCardDestination(ra.TurnTaker.Trash),
                new MoveCardDestination(legacy.TurnTaker.Trash),
                new MoveCardDestination(haka.TurnTaker.Trash),
                new MoveCardDestination(env.TurnTaker.Trash)
            };
            base.DealDamage(legacy, moonWatcher, 16, DamageType.Melee);

            AssertNumberOfCardsInTrash(baron, numberOfCardsInBaronTrash + 1);
            AssertNumberOfCardsInTrash(ra, numberOfCardsInRaTrash + 1);
            AssertNumberOfCardsInTrash(legacy, numberOfCardsInLegacyTrash + 1);
            AssertNumberOfCardsInTrash(haka, numberOfCardsInHakaTrash + 1);
            AssertNumberOfCardsInTrash(env, numberOfCardsInEnvTrash + 2);
        }
        [Test]
        public void TestMoonWatcherDestroyByEffect()
        {
            /*
             * When this card is destroyed, if it has 0 or fewer HP, reveal the top card of each deck and 
             * replace or discard each one.
             */
            Card moonWatcher;
            int numberOfCardsInBaronTrash;
            int numberOfCardsInRaTrash;
            int numberOfCardsInLegacyTrash;
            int numberOfCardsInHakaTrash;
            int numberOfCardsInEnvTrash;

            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);
            StartGame();

            moonWatcher = PutIntoPlay("MoonWatcher");
            numberOfCardsInBaronTrash = baron.TurnTaker.Trash.NumberOfCards;
            numberOfCardsInRaTrash = ra.TurnTaker.Trash.NumberOfCards;
            numberOfCardsInLegacyTrash = legacy.TurnTaker.Trash.NumberOfCards;
            numberOfCardsInHakaTrash = haka.TurnTaker.Trash.NumberOfCards;
            numberOfCardsInEnvTrash = env.TurnTaker.Trash.NumberOfCards;

            DecisionSelectTurnTakers = new TurnTaker[] { baron.TurnTaker, ra.TurnTaker, legacy.TurnTaker, haka.TurnTaker, env.TurnTaker };
            DecisionMoveCardDestinations = new MoveCardDestination[]
            {
                new MoveCardDestination( baron.TurnTaker.Trash),
                new MoveCardDestination(ra.TurnTaker.Trash),
                new MoveCardDestination(legacy.TurnTaker.Trash),
                new MoveCardDestination(haka.TurnTaker.Trash),
                new MoveCardDestination(env.TurnTaker.Trash)
            };
            base.DealDamage(legacy, moonWatcher, 14, DamageType.Melee);
            base.PlayCard("WrathfulGaze");
            base.UsePower(base.PlayCard("WrathfulGaze"));

            AssertNumberOfCardsInTrash(baron, numberOfCardsInBaronTrash);
            AssertNumberOfCardsInTrash(ra, numberOfCardsInRaTrash);
            AssertNumberOfCardsInTrash(legacy, numberOfCardsInLegacyTrash);
            AssertNumberOfCardsInTrash(haka, numberOfCardsInHakaTrash);
            AssertNumberOfCardsInTrash(env, numberOfCardsInEnvTrash + 1);

        }

        #endregion Test Moon Watcher

        #region Test Oblask Object Y-3A
        /* 
         * Play this card next to a hero, then play the top card of the environment deck. 
         * When the hero next to this card would be dealt damage by an environment target, 
         * they may discard a card. If they do, redirect that damage.
         */
        [Test]
        public void TestOblaskObjectY3A()
        {
            Card oblaskObjectY3A;
            Card shadowOfOblask;
            Card drawnToTheFlame;
            Card blazingTornado;

            // Arrange & Act
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);
            StartGame();

            base.DestroyNonCharacterVillainCards();

            oblaskObjectY3A = base.GetCard("OblaskObjectY3A");
            shadowOfOblask = base.GetCard("ShadowOfOblask");
            drawnToTheFlame = base.PutInHand("DrawnToTheFlame");
            blazingTornado = base.PutInHand("BlazingTornado");

            base.env.PutOnDeck(shadowOfOblask);

            DecisionSelectCards = new Card[] { ra.CharacterCard, drawnToTheFlame, baron.CharacterCard, blazingTornado, shadowOfOblask };
            PlayCard(oblaskObjectY3A);
            AssertIsInPlay(shadowOfOblask);

            QuickHPStorage(baron.CharacterCard, shadowOfOblask, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            DealDamage(shadowOfOblask, ra, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0, 0);

            DealDamage(shadowOfOblask, ra, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0, 0, 0);
        }

        // test non environment damage to make sure no redirect happens
        #endregion Test Oblask Object Y-3A

        #region Test Open Ground
        /* 
         * Once per turn when a hero target would be dealt damage by a non-hero target, they may increase that damage
         * by 1 and play the top card of the environment deck.
         * If that card is a target, redirect that damage to it.
         */
        [Test]
        public void TestOpenGround()
        {
            Card shadowOfOblask;
            // Arrange & Act
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);
            StartGame();

            PutIntoPlay("OpenGround");
            shadowOfOblask = PutOnDeck("ShadowOfOblask");

            DecisionsYesNo = new bool[] { true, true };

            QuickHPStorage(baron.CharacterCard, shadowOfOblask, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);

            DealDamage(baron, ra, 2, DamageType.Cold);
            QuickHPCheck(0, -3, 0, 0, 0);

            DealDamage(baron, ra, 2, DamageType.Cold);
            QuickHPCheck(0, 0, -2, 0, 0);
        }

        #endregion Test Open Ground
    }
}
