using System.Linq;
using System.Collections.Generic;

using Cauldron.BlackwoodForest;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;

using NUnit.Framework;
using System;

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

        [Test]
        public void TestFlavorText()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace, "Cauldron.DungeonsOfTerror");

            Card gameTrail = GetCard("GameTrail");

            Console.WriteLine("FLAVOR TEXT: " + gameTrail.Definition.FlavorText);
            Assert.That(gameTrail.Definition.FlavorText.Contains("“"), "The quote mark is not showing up in the Flavor Text definition for " + gameTrail.Title);
            Card edibles = GetCard("DubiousEdibles");
            Console.WriteLine("FLAVOR TEXT: " + edibles.Definition.FlavorText);
            Assert.That(edibles.Definition.FlavorText.Contains("“"), "The quote mark is not showing up in the Flavor Text definition for " + edibles.Title);


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

            PrintSpecialStringsForCard(freshTracks);

            DecisionsYesNo = new bool[] { true };
            PlayCard("BladeBattalion");
            AssertIsInPlay(shadowOfOblask);

            PrintSpecialStringsForCard(freshTracks);


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
            QuickHPCheck(0, -2, -2, -2);

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
            QuickHPCheck(-3, -3, -3, -7, 0, 0, 0);
        }

        [Test]
        public void TestGentleTransport_Oblivaeon()
        {
 

            SetupGameController(new string[] { "OblivAeon", "Cauldron.Vanish", "Legacy", "Haka", "Cauldron.WindmillCity", "Cauldron.OblaskCrater", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            GoToPlayCardPhase(envTwo);
            Card predator = PlayCard("ShadowOfOblask");
            Card gentleTransport = PlayCard("GentleTransport");

            MoveToSpecificBattleZone(bzOne, vanish);
            MoveToSpecificBattleZone(bzOne, legacy);
            MoveToSpecificBattleZone(bzOne, haka);

            GoToEndOfTurn(envTwo);

            AssertNotGameOver();


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
            QuickHPCheckZero();
        }

        #endregion Test Industrious Hulk

        #region Test Inscrutable Ecology

        /* 
         * When this card enters play, reveal cards from the top of the environmnet deck until {H - 1} targets are 
         * revealed, put them into play, and discard the other revealed cards.
         * Reduce damage dealt by environment targets by 1. At the start of the environment turn, destroy this card.
         */
        [Test]
        public void TestInscrutableEcology()
        {
            Card oblaskObjectY3A;
            Card inscrutableEcology;
            Card shadowOfOblask;
            Card swarmOfFangs;
            int numberOfCardsInDeck;
            int numberOfCardsInTrash;

            // Arrange & Act
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);
            StartGame();
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

            List<Location> locations = new List<Location>() { baron.TurnTaker.Deck, ra.TurnTaker.Deck, legacy.TurnTaker.Deck, haka.TurnTaker.Deck, env.TurnTaker.Deck };
            DecisionSelectLocations = locations.Select(loc => new LocationChoice(loc)).ToArray();
            IEnumerable<Card> topCards = locations.Select(loc => loc.TopCard).ToArray();

            DecisionsYesNo = new bool[] { true, true, true, true, true };
            base.DealDamage(legacy, moonWatcher, 15, DamageType.Melee);

            AssertNumberOfCardsInTrash(baron, numberOfCardsInBaronTrash + 1);
            AssertNumberOfCardsInTrash(ra, numberOfCardsInRaTrash + 1);
            AssertNumberOfCardsInTrash(legacy, numberOfCardsInLegacyTrash + 1);
            AssertNumberOfCardsInTrash(haka, numberOfCardsInHakaTrash + 1);
            AssertNumberOfCardsInTrash(env, numberOfCardsInEnvTrash + 2);

            AssertInTrash(topCards);
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


            List<Location> locations = new List<Location>() { baron.TurnTaker.Deck, ra.TurnTaker.Deck, legacy.TurnTaker.Deck, haka.TurnTaker.Deck, env.TurnTaker.Deck };
            DecisionSelectLocations = locations.Select(loc => new LocationChoice(loc)).ToArray();
            IEnumerable<Card> topCards = locations.Select(loc => loc.TopCard).ToArray();

            DecisionsYesNo = new bool[] { true, true, true, true, true };
            base.DealDamage(legacy, moonWatcher, 16, DamageType.Melee);

            AssertNumberOfCardsInTrash(baron, numberOfCardsInBaronTrash + 1);
            AssertNumberOfCardsInTrash(ra, numberOfCardsInRaTrash + 1);
            AssertNumberOfCardsInTrash(legacy, numberOfCardsInLegacyTrash + 1);
            AssertNumberOfCardsInTrash(haka, numberOfCardsInHakaTrash + 1);
            AssertNumberOfCardsInTrash(env, numberOfCardsInEnvTrash + 2);

            AssertInTrash(topCards);
        }

        [Test]
        public void TestMoonWatcherDestroyReduceBelow0_Kaargra()
        {
            /*
             * When this card is destroyed, if it has 0 or fewer HP, reveal the top card of each deck and 
             * replace or discard each one.
             */
            Card moonWatcher;

            SetupGameController("KaargraWarfang", "Ra", "Legacy", "Haka", DeckNamespace);

            moonWatcher = PutIntoPlay("MoonWatcher");

            List<Location> locations = new List<Location>() { warfang.TurnTaker.Decks.Last(), warfang.TurnTaker.Decks.First(), ra.TurnTaker.Deck, legacy.TurnTaker.Deck, haka.TurnTaker.Deck, env.TurnTaker.Deck };
            DecisionSelectLocations = locations.Select(loc => new LocationChoice(loc)).ToArray();
            IEnumerable<Card> topCards = locations.Select(loc => loc == warfang.TurnTaker.Decks.Last() ? loc.Cards.Reverse().ElementAt(1) : loc.TopCard).ToArray();

            DecisionsYesNo = new bool[] { false, true, true, true, true, true };
            DealDamage(warfang, moonWatcher, 100, DamageType.Fire);
            AssertInTrash(topCards.Reverse().Take(5));
            Assert.That(warfang.TurnTaker.Decks.Last().TopCard == topCards.First(), $"{topCards.First().Title} was supposed to be the top card of {warfang.TurnTaker.Decks.Last().GetFriendlyName()} but instead was in {topCards.First().Location.GetFriendlyName()}");
           
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
            QuickHPCheck(-3, 0, 0, 0, 0);

            DealDamage(shadowOfOblask, ra, 2, DamageType.Melee);
            QuickHPCheck(0, -3, 0, 0, 0);
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

        #region Test Plated Giant
        /* 
         * Play this card next to a hero. The hero next to this card is immune to damage from enviroment targets 
         * other than this one. 
         * At the end of the environment turn, this card deals the hero next to it {H - 1} melee damage.
         */
        [Test]
        public void TestPlatedGiant()
        {
            Card platedGiant;
            Card swarmOfFangs;

            // Arrange & Act
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);
            StartGame();

            base.DestroyNonCharacterVillainCards();

            DecisionSelectCards = new Card[] { ra.CharacterCard };
            platedGiant = base.PutIntoPlay("PlatedGiant");
            swarmOfFangs = base.PutIntoPlay("SwarmOfFangs");

            base.GameController.SkipToTurnTakerTurn(base.env);

            QuickHPStorage(baron.CharacterCard, platedGiant, swarmOfFangs, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);

            DealDamage(swarmOfFangs, ra, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, 0);

            DealDamage(platedGiant, ra, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, -1, 0, 0);

            DealDamage(baron, ra, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, -1, 0, 0);

            GoToEndOfTurn(base.env);
            QuickHPCheck(0, -2, 0, -2, 0, 0);
        }
        #endregion Test Plated Giant

        #region Test Shadow of Oblask
        /*
         * At the end of the environment turn, this card deals the hero target with the second lowest HP {H} energy damage.
         * If no other predator cards are in play, increase damage dealt by this card by 1.
         */
        [Test]
        public void TestShadowOfOblask()
        {
            Card shadowOfOblask;
            Card swarmOfFangs;

            // Ra: 30
            // Legacy: 32
            // Haka: 34
            // Arrange & Act
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);
            StartGame();

            base.DestroyNonCharacterVillainCards();

            DecisionSelectCards = new Card[] { ra.CharacterCard };
            shadowOfOblask = base.PutIntoPlay("ShadowOfOblask");

            base.GameController.SkipToTurnTakerTurn(base.env);
            PrintSpecialStringsForCard(shadowOfOblask);
            QuickHPStorage(baron.CharacterCard, shadowOfOblask, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(base.env);
            QuickHPCheck(0, 0, 0, -4, 0);

            swarmOfFangs = base.PutIntoPlay("SwarmOfFangs");
            PrintSpecialStringsForCard(shadowOfOblask);
            base.GameController.SkipToTurnTakerTurn(base.env);
            QuickHPStorage(baron.CharacterCard, shadowOfOblask, swarmOfFangs, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(base.env);
            QuickHPCheck(0, -2, 0, -3, 0, 0);
        }
        #endregion Test Shadow of Oblask

        #region Test Swarm of Fangs
        /*
         * At the end of the environment turn, this card deals the target other than itself
         * with the lowest HP 2 melee damage 
         * If this damage destroys a target, repeat the text of this card.
         */
        [Test]
        public void TestSwarmOfFangsDamage()
        {
            Card swarmOfFangs;

            // Ra: 30
            // Legacy: 32
            // Haka: 34
            // Arrange & Act
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", DeckNamespace);
            StartGame();

            base.DestroyNonCharacterVillainCards();

            swarmOfFangs = base.PutIntoPlay("SwarmOfFangs");
            PrintSpecialStringsForCard(swarmOfFangs);
            base.GameController.SkipToTurnTakerTurn(base.env);
            QuickHPStorage(baron.CharacterCard, swarmOfFangs, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(base.env);
            QuickHPCheck(0, 0, -2, 0, 0);
        }

        [Test]
        public void TestSwarmOfFangsRepeatCardText()
        {
            Card huginn;
            Card buildingOfRooks1;
            Card buildingOfRooks2;
            Card buildingOfRooks3;
            Card clatteringOfJackdaws1;
            Card clatteringOfJackdaws2;
            Card clatteringOfJackdaws3;
            Card kettleOfVultures1;
            Card kettleOfVultures2;
            Card kettleOfVultures3;
            Card murderOfCrows1;
            Card murderOfCrows2;
            Card murderOfCrows3;
            Card unkindnessOfRavens1;
            Card unkindnessOfRavens2;
            Card unkindnessOfRavens3;
            Card swarmOfFangs;

            SetupGameController("TheMatriarch", "Ra", "Legacy", "Haka", DeckNamespace);
            StartGame();
            base.DestroyNonCharacterVillainCards();

            huginn = PutOnDeck(matriarch, GetCard("Huginn", 0));
            buildingOfRooks1 = PutOnDeck(matriarch, GetCard("BuildingOfRooks", 0));
            buildingOfRooks2 = PutOnDeck(matriarch, GetCard("BuildingOfRooks", 1));
            buildingOfRooks3 = PutOnDeck(matriarch, GetCard("BuildingOfRooks", 2));
            clatteringOfJackdaws1 = PutOnDeck(matriarch, GetCard("ClatteringOfJackdaws", 0));
            clatteringOfJackdaws2 = PutOnDeck(matriarch, GetCard("ClatteringOfJackdaws", 1));
            clatteringOfJackdaws3 = PutOnDeck(matriarch, GetCard("ClatteringOfJackdaws", 2));
            kettleOfVultures1 = PutOnDeck(matriarch, GetCard("KettleOfVultures", 0));
            kettleOfVultures2 = PutOnDeck(matriarch, GetCard("KettleOfVultures", 1));
            kettleOfVultures3 = PutOnDeck(matriarch, GetCard("KettleOfVultures", 2));
            murderOfCrows1 = PutOnDeck(matriarch, GetCard("MurderOfCrows", 0));
            murderOfCrows2 = PutOnDeck(matriarch, GetCard("MurderOfCrows", 1));
            murderOfCrows3 = PutOnDeck(matriarch, GetCard("MurderOfCrows", 2));
            unkindnessOfRavens1 = PutOnDeck(matriarch, GetCard("UnkindnessOfRavens", 0));
            unkindnessOfRavens2 = PutOnDeck(matriarch, GetCard("UnkindnessOfRavens", 1));
            unkindnessOfRavens3 = PutOnDeck(matriarch, GetCard("UnkindnessOfRavens", 2));

            base.PlayCard(unkindnessOfRavens3);
            swarmOfFangs = base.PutIntoPlay("SwarmOfFangs");
            base.GameController.SkipToTurnTakerTurn(base.env);
            base.GoToEndOfTurn(base.env);

            base.AssertNotInPlay(buildingOfRooks1);
            base.AssertNotInPlay(buildingOfRooks2);
            base.AssertNotInPlay(buildingOfRooks3);
            base.AssertNotInPlay(clatteringOfJackdaws1);
            base.AssertNotInPlay(clatteringOfJackdaws2);
            base.AssertNotInPlay(clatteringOfJackdaws3);
            base.AssertNotInPlay(kettleOfVultures1);
            base.AssertNotInPlay(kettleOfVultures2);
            base.AssertNotInPlay(kettleOfVultures3);
            base.AssertNotInPlay(murderOfCrows1);
            base.AssertNotInPlay(murderOfCrows2);
            base.AssertNotInPlay(murderOfCrows3);
            base.AssertNotInPlay(unkindnessOfRavens1);
            base.AssertNotInPlay(unkindnessOfRavens2);
            base.AssertNotInPlay(unkindnessOfRavens3);
        }
        #endregion

        #region Test Unknown Herds
        /* 
         * [ORIGNIAL TEXT - The current engine does not provide a way to determine if a power will or will not damage]
         * At the end of the environment turn, if there are 0 or 1 predator cards in play, 1 hero may use a power that 
         * does not deal damage. 
         * When this card is destroyed, each predator and villain target regains {H} HP. 
         */
        /* [ALTERNATE TEXT - Use until the engine supports additional metadata for powers]
         * At the end of the environment turn, if there are 0 or 1 predator cards in play, 1 hero may use a power. If that 
         * power would deal damage, prevent it. 
         * When this card is destroyed, each predator and villain target regains {H} HP. 
         */
        [Test]
        public void TestUnknownHerdsUsePowerPreventDamage()
        {
            Card motivationalCharge;
            Card unknownHerds;
            SetupGameController("BaronBlade", "Legacy", "Ra", "Haka", DeckNamespace);
            StartGame();

            base.DestroyNonCharacterVillainCards();
            motivationalCharge = PutIntoPlay("MotivationalCharge");
            PutIntoPlay("NextEvolution");
            unknownHerds = PutIntoPlay("UnknownHerds");
            SetHitPoints(legacy, legacy.CharacterCard.MaximumHitPoints.Value - 1);
            SetHitPoints(ra, ra.CharacterCard.MaximumHitPoints.Value - 1);
            SetHitPoints(haka, haka.CharacterCard.MaximumHitPoints.Value - 1);
            DecisionSelectPowers = new Card[] { motivationalCharge };
            base.GameController.SkipToTurnTakerTurn(base.env);
            QuickHPStorage(baron.CharacterCard, unknownHerds, legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard);
            base.GoToEndOfTurn(base.env);
            QuickHPCheck(0, 0, 1, 1, 1);
        }
        [Test]
        public void TestUnknownHerdsUsePowerOnlySpecificPower()
        {
            SetupGameController("BaronBlade", "Legacy/AmericasGreatestLegacy", "Ra", "Haka", DeckNamespace);
            StartGame();
            DestroyNonCharacterVillainCards();

            DecisionSelectCards = new Card[] { legacy.CharacterCard, baron.CharacterCard };
            SetHitPoints(legacy, 15);
            QuickHPStorage(legacy, baron);
            PlayCard("UnknownHerds");
            PlayCard("MotivationalCharge");

            GoToEndOfTurn(base.env);
            QuickHPCheck(2, -3);
        }
        [Test()]
        public void TestUnknownHerdsUsePowerVoidBelter()
        {
            SetupGameController("BaronBlade", "VoidGuardMainstay", "Ra", "Haka", DeckNamespace);
            StartGame();
            DestroyNonCharacterVillainCards();

            Card belter = PlayCard("VoidBelter");
            DecisionSelectPower = belter;
            QuickHPStorage(baron);
            PlayCard("UnknownHerds");

            GoToEndOfTurn(base.env);
            //direct power damage is prevented, but the on-destruction should not be
            QuickHPCheck(-4);
        }
        [Test]
        public void TestUnknownHerdsTooManyPredators()
        {
            SetupGameController("BaronBlade", "Legacy", "Ra", "Haka", DeckNamespace);
            StartGame();

            PlayCard("UnknownHerds");
            PlayCard("UnseenTerror");
            PlayCard("ShadowOfOblask");

            GoToEndOfTurn(base.env);
            AssertNumberOfUsablePowers(legacy, 1);
        }

        [Test]
        public void TestUnknownHerdsDestroyed()
        {
            Card unknownHerds;
            Card shadowOfOblask;
            Card bladeBattalion;
            Card moonWatcher;
            SetupGameController("BaronBlade", "Legacy", "Ra", "Haka", DeckNamespace);
            StartGame();

            base.DestroyNonCharacterVillainCards();

            unknownHerds = PutIntoPlay("UnknownHerds");
            shadowOfOblask = PutIntoPlay("ShadowOfOblask");
            moonWatcher = PutIntoPlay("MoonWatcher");
            bladeBattalion = PutIntoPlay("BladeBattalion");

            PrintSpecialStringsForCard(unknownHerds);

            SetHitPoints(shadowOfOblask, shadowOfOblask.MaximumHitPoints.Value - 3);
            SetHitPoints(moonWatcher, moonWatcher.MaximumHitPoints.Value - 3);
            SetHitPoints(bladeBattalion, bladeBattalion.MaximumHitPoints.Value - 3);
            SetHitPoints(baron, baron.CharacterCard.MaximumHitPoints.Value - 3);

            QuickHPStorage(baron.CharacterCard, shadowOfOblask, moonWatcher, bladeBattalion, legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard);
            DestroyCard(unknownHerds);
            QuickHPCheck(3, 3, 0, 3, 0, 0, 0);
        }
        #endregion Test Unknown Herds

        #region Test Unseen Terror

        /*
         * At the end of the environment turn, this card deals the 3 other targets with the highest hp {H - 2} cold damage each.
         * Until the end of the next environment turn, this card becomes immune to damage from targets that were not damaged this way.
         */
        [Test]
        public void TestUnseenTerror()
        {
            Card unseenTerror;

            SetupGameController("BaronBlade", "Legacy", "Ra", "Haka", DeckNamespace);
            StartGame();

            base.DestroyNonCharacterVillainCards();

            unseenTerror = PutIntoPlay("UnseenTerror");

            SetHitPoints(baron, 27);
            SetHitPoints(legacy, 26);
            SetHitPoints(ra, 25);
            SetHitPoints(haka, 24);

            QuickHPStorage(baron.CharacterCard, unseenTerror, legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard);

            base.GameController.SkipToTurnTakerTurn(base.env);
            base.GoToEndOfTurn(base.env);
            QuickHPCheck(-1, 0, -1, -1, 0);
            // baron: 26
            // legacy: 25
            // ra: 24
            // haka: 24

            PrintTriggers();

            DealDamage(haka, unseenTerror, 1, DamageType.Fire);
            QuickHPCheckZero();
            DealDamage(baron, unseenTerror, 1, DamageType.Melee);
            DealDamage(legacy, unseenTerror, 1, DamageType.Melee);
            DealDamage(ra, unseenTerror, 1, DamageType.Melee);
            QuickHPCheck(0, -3, 0, 0, 0);


            SetHitPoints(haka, 28);
            SetHitPoints(baron, 27);
            SetHitPoints(legacy, 26);
            SetHitPoints(ra, 25);
            SetHitPoints(unseenTerror, 4);
            QuickHPUpdate();

            base.GameController.SkipToTurnTakerTurn(base.env);
            DealDamage(haka, unseenTerror, 3, DamageType.Fire);
            QuickHPCheckZero();
            base.GoToEndOfTurn(base.env);
            QuickHPCheck(-1, 0, -1, 0, -1);
            GoToNextTurn();
            PrintTriggers();
            DealDamage(haka, unseenTerror, 3, DamageType.Melee);
            QuickHPCheck(0, -3, 0, 0, 0);
            DealDamage(ra, unseenTerror, 4, DamageType.Fire);
            QuickHPCheckZero();
        }

        #endregion Test Unseen Terror
    }
}
