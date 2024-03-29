using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class SuperstormAkelaTests : CauldronBaseTest
    {

        #region SuperstormAkelaHelperFunctions

        protected TurnTakerController superstorm { get { return FindEnvironment(); } }
        
        private void PrintPlayAreaPositions(TurnTaker tt)
        {
            foreach (var card in GetOrderedCardsInLocation(tt.PlayArea))
            {
                Console.WriteLine(card.Title + " is in Position " + GetOrderedCardsInLocation(tt.PlayArea).ToList().IndexOf(card));
            }
        }

        private IEnumerable<Card> GetOrderedCardsInLocation(Location location)
        {
            return location.Cards.OrderBy((Card c) => c.PlayIndex);
        }
        #endregion

        [Test()]
        public void TestSuperstormAkelaWorks()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

        }

        [Test()]
        [Sequential]
        public void DecklistTest_Vehicle_IsVehicle([Values("FlyingBus")] string vehicle)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();

            GoToPlayCardPhase(superstorm);

            Card card = PlayCard(vehicle);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "vehicle", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Contact_IsContact([Values("SkulkingIntermediary")] string contact)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();

            GoToPlayCardPhase(superstorm);

            Card card = PlayCard(contact);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "contact", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Obstacle_IsObstacle([Values("AscendedEdifice")] string obstacle)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();

            GoToPlayCardPhase(superstorm);

            Card card = PlayCard(obstacle);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "obstacle", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Guardian_IsGuardian([Values("ForgottenDjinn")] string guardian)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();

            GoToPlayCardPhase(superstorm);

            Card card = PlayCard(guardian);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "guardian", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Device_IsDevice([Values("GeogravLocus")] string device)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();

            GoToPlayCardPhase(superstorm);

            Card card = PlayCard(device);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "device", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Criminal_IsCriminal([Values("GeminiIndra", "GeminiMaya")] string criminal)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();

            GoToPlayCardPhase(superstorm);

            Card card = PlayCard(criminal);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "criminal", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Structure_IsStructure([Values("ToppledSkyscraper")] string structure)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();

            GoToPlayCardPhase(superstorm);

            Card card = PlayCard(structure);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "structure", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Storm_IsStorm([Values("FlailingWires")] string storm)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();

            GoToPlayCardPhase(superstorm);

            Card card = PlayCard(storm);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "storm", false);
        }

        [Test()]
        public void TestRideTheCurrents_MoveCardsAround()
        {
            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela" });
            StartGame();
            //stack deck to reduce variability
            PutOnDeck("TheStaffOfRa");

            GoToPlayCardPhase(superstorm);
            PutInTrash("Scatterburst");
            PutInTrash("GeogravLocus");
            Card currents = PlayCard("RideTheCurrents");
            IEnumerable<Card> cardsToPlay = FindCardsWhere((Card c) => superstorm.TurnTaker.Deck.HasCard(c)).Take(4);
            PlayCards(cardsToPlay);
            DecisionSelectFunction = 1;
            //selecting currents and moving it to the right of the 4th card played
            DecisionAutoDecideIfAble = true;
            DecisionSelectCards = new Card[] {currents, cardsToPlay.ElementAt(3) };
            PrintPlayAreaPositions(superstorm.TurnTaker);
            int nextToPosition = GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea).ToList().IndexOf(cardsToPlay.ElementAt(3));
            GoToStartOfTurn(baron);

            PrintPlayAreaPositions(superstorm.TurnTaker);
            //all cards shift one over since we are removing the one at the far left
            int expected = (nextToPosition - 1) + 1;
            int actual = GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea).ToList().IndexOf(currents);
            Assert.IsTrue(GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea).ElementAt(expected) == currents, currents.Title + " is not in the correct position. Expected: " + expected + ", Actual: " + actual);

        }

        [Test()]
        public void TestRideTheCurrents_MoveTargetsAround()
        {
            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela" });
            StartGame();
            //stack deck to reduce variability
            PutOnDeck("TheStaffOfRa");

            GoToPlayCardPhase(superstorm);
            PutInTrash("Scatterburst");
            PutInTrash("GeogravLocus");
            Card currents = PlayCard("RideTheCurrents");
            Card maya = GetCard("GeminiMaya");
            IEnumerable<Card> cardsToPlay = FindCardsWhere((Card c) => superstorm.TurnTaker.Deck.HasCard(c) && c != maya).Take(4);
            PlayCards(cardsToPlay);
            PlayCard(maya);

            DealDamage(ra, maya, 3, DamageType.Fire);

            GoToEndOfTurn(superstorm);

            QuickHPStorage(maya);

            DecisionSelectFunction = 1;
            //selecting maya and moving it to the right of the 2nd card played
            DecisionAutoDecideIfAble = true;
            DecisionSelectCards = new Card[] { maya, cardsToPlay.ElementAt(1) };
            PrintPlayAreaPositions(superstorm.TurnTaker);
            int nextToPosition = GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea).ToList().IndexOf(cardsToPlay.ElementAt(1));
            int mayaPosition = GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea).ToList().IndexOf(maya);

            GoToStartOfTurn(baron);

            PrintPlayAreaPositions(superstorm.TurnTaker);
            int expected = mayaPosition < nextToPosition ? nextToPosition : nextToPosition + 1;
            int actual = GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea).ToList().IndexOf(maya);
            Assert.IsTrue(GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea).ElementAt(expected) == maya, maya.Title + " is not in the correct position. Expected: " + expected + ", Actual: " + actual);

            QuickHPCheckZero();
        }

        [Test()]
        public void TestRideTheCurrents_Play()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();
            GoToPlayCardPhase(superstorm);

            PlayCard("BladeBattalion");

            DealDamage(baron, ra, 100, DamageType.Fire);
            PlayCard("Mere");
            PlayCard("Dominion");
            PlayCard("InspiringPresence");

            //When this card enters play, select the deck with the least number of non-character cards in play. Put the top card of that deck into play.
            Card topCard = legacy.TurnTaker.Deck.TopCard;
            Card currents = PlayCard("RideTheCurrents");
            AssertNotInDeck(topCard);


        }

        [Test()]
        public void TestRideTheCurrents_Play_Oblivaeon()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            GoToPlayCardPhase(superstorm);
            Card topCard = ra.TurnTaker.Deck.TopCard;
            Card secondEnvTopCard = envTwo.TurnTaker.Deck.TopCard;

            //When this card enters play, select the deck with the least number of non-character cards in play. Put the top card of that deck into play.

            // valid targets: 3 heroes & 1st battle zone scion
            List<TurnTakerController> validControllers = new List<TurnTakerController>() { ra, legacy, haka, scionOne };
            List<TurnTakerController> inValidControllers = new List<TurnTakerController>() { oblivaeon, envOne, envTwo, scionTwo };
            AssertNumberOfChoicesInNextDecision(4);
            AssertNextDecisionChoices(validControllers, inValidControllers);
            PlayCard("RideTheCurrents");

            // The top card of a deck in Akela's battle zone should be played, the top card in the other battle zone should now
            AssertNotInDeck(topCard);
            AssertInDeck(secondEnvTopCard);
        }


        [Test()]
        public void TestFlailingWires_EndOfTurn()
        {
            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela" });
            StartGame();
            GoToPlayCardPhase(superstorm);
            Card wires = GetCard("FlailingWires");
            PlayCard("PressureDrop");
            //stack deck to prevent extra damage
            PutOnDeck("ToppledSkyscraper");
            PlayCard(wires);
            QuickHPStorage(ra, legacy, haka);
            //At the end of the environment turn, this card deals the X+1 hero targets with the highest HP 1 lightning damage each, where X is the number of environment cards to the left of this one.
            PrintPlayAreaPositions(superstorm.TurnTaker);
            GoToEndOfTurn(superstorm);
            QuickHPCheck(0, -1, -1);

        }

        [Test()]
        public void TestFlailingWires_DestroyedByReaction()
        {
            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela" });
            StartGame();
            GoToPlayCardPhase(superstorm);
            Card wires = GetCard("FlailingWires");
            PlayCard("PressureDrop");
            //stack deck to prevent extra damage
            PutOnDeck("ToppledSkyscraper");
            PlayCard(wires);

            //set up lethal reaction damage
            PlayCard("TheStaffOfRa");
            PlayCard("FlameBarrier");
            SetHitPoints(legacy, 20);

            QuickHPStorage(ra, legacy, haka);
            //At the end of the environment turn, this card deals the X+1 hero targets with the highest HP 1 lightning damage each, where X is the number of environment cards to the left of this one.
            PrintPlayAreaPositions(superstorm.TurnTaker);
            GoToEndOfTurn(superstorm);
            QuickHPCheck(-1, 0, 0);

        }
        [Test()]
        public void TestFlailingWires_Play()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();
            GoToPlayCardPhase(superstorm);
            Card wires = GetCard("FlailingWires");
            Card currents = PutOnDeck("RideTheCurrents");
            //"When this card enters play, play the top card of the environment deck.",
            PlayCard(wires);
            AssertInPlayArea(superstorm, currents);

        }



        [Test()]
        public void TestFracturedSky_Play()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();

            GoToPlayCardPhase(superstorm);

            PutOnDeck("TheStaffOfRa");
            Card sky = PutInTrash("FracturedSky");
            IEnumerable<Card> cardsToPlay = FindCardsWhere((Card c) => superstorm.TurnTaker.Deck.HasCard(c) && c != sky).Take(3);
            PlayCards(cardsToPlay);
            //stack deck to never play extra cards
            Card card1 = PutOnDeck("GeminiIndra");
            Card card2 = PutOnDeck("GeminiMaya");
            IEnumerable<Card> topCards = new Card[] { card1, card2 };
            DecisionSelectCards = topCards;
;
            PlayCard(sky);

            PrintPlayAreaPositions(superstorm.TurnTaker);
            Assert.IsTrue(GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea).ElementAt(0) == topCards.ElementAt(0), topCards.ElementAt(0).Title + " is not in the correct position.");
            Assert.IsTrue(GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea).Last() == topCards.ElementAt(1), topCards.ElementAt(1).Title + " is not in the correct position.");


        }

        [Test()]
        public void TestFracturedSky_InterruptPlay()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();

            GoToPlayCardPhase(superstorm);

            PutOnDeck("TheStaffOfRa");
            Card sky = PutInTrash("FracturedSky");
            IEnumerable<Card> cardsToPlay = FindCardsWhere((Card c) => superstorm.TurnTaker.Deck.HasCard(c) && c != sky).Take(3);
            PlayCards(cardsToPlay);
            Card extraCard = PutOnDeck("GeminiMaya");
            Card card1 = PutOnDeck("GeminiIndra");
            Card card2 = PutOnDeck("FlailingWires");
            IEnumerable<Card> topCards = new Card[] { card2, card1 };
            DecisionSelectCards = topCards;
            
            PlayCard(sky);

            PrintPlayAreaPositions(superstorm.TurnTaker);
            Assert.IsTrue(GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea).ElementAt(0) == topCards.ElementAt(0), topCards.ElementAt(0).Title + " is not in the correct position.");
            Assert.IsTrue(GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea).Last() == topCards.ElementAt(1), topCards.ElementAt(1).Title + " is not in the correct position.");


        }

        [Test()]
        public void TestFracturedSky_OnDestroy()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();
            GoToPlayCardPhase(superstorm);
            Card pressure = PlayCard("PressureDrop");
            Card maya = PlayCard("GeminiMaya");
            Card sky = PlayCard("FracturedSky");

            AssertInPlayArea(superstorm, sky);

            //should only be destroyed on environment target destruction
            DestroyCard(pressure, baron.CharacterCard);
            AssertInPlayArea(superstorm, sky);

            DestroyCard(maya, baron.CharacterCard);
            //When an environment target is destroyed, destroy this card.
            AssertInTrash(sky);


        }

        [Test()]
        public void TestChurningVoid_Indestructible()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.SuperstormAkela");
            StartGame();
            GoToPlayCardPhase(superstorm);
            
            Card churning = PlayCard("ChurningVoid");
            DecisionSelectCard = churning;
            //this card is indestructible
            PlayCard("BlindingSpeed");
            AssertInPlayArea(superstorm, churning);

        }

        [Test()]
        public void TestChurningVoid_StartOfTurn_DealDamage_3CardsLeft()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.SuperstormAkela");
            StartGame();
            GoToEndOfTurn(haka);

            //destroy mdp
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            PlayCard("GeminiIndra");
            PlayCard("GeogravLocus");
            PlayCard("ForgottenDjinn");
            Card churning = PlayCard("ChurningVoid");
            //At the start of the environment turn, this card deals the { H} targets with the highest HP X projectile damage each, where X is the number of environment cards to the left of this one.
            //4 highest HP targets are baron, ra, legacy, and haka
            //3 cards to the left of churning
            QuickHPStorage(baron, ra, legacy, haka, tachyon);
            GoToStartOfTurn(superstorm);
            QuickHPCheck(-3, -3, -3, -3, 0);

        }

        [Test()]
        public void TestChurningVoid_StartOfTurn_DealDamage_1CardsLeft()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.SuperstormAkela");
            StartGame();
            GoToEndOfTurn(haka);

            //destroy mdp
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            PlayCard("GeminiIndra");
            Card churning = PlayCard("ChurningVoid");
            //At the start of the environment turn, this card deals the { H} targets with the highest HP X projectile damage each, where X is the number of environment cards to the left of this one.
            //4 highest HP targets are baron, ra, legacy, and haka
            //3 cards to the left of churning
            QuickHPStorage(baron, ra, legacy, haka, tachyon);
            GoToStartOfTurn(superstorm);
            QuickHPCheck(-1, -1, -1, -1, 0);

        }

        [Test()]
        public void TestChurningVoid_StartOfTurn_MoveCard_2CardsRight()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.SuperstormAkela");
            StartGame();
            GoToEndOfTurn(haka);

            //destroy mdp
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card djinn = PlayCard("ForgottenDjinn");
            Card churning = PlayCard("ChurningVoid");
            Card indra = PlayCard("GeminiIndra");
            Card locus = PlayCard("GeogravLocus");

            //After all other start of turn effects have taken place, move this card 1 space to the right in the environment play area.
            GoToStartOfTurn(superstorm);
            PrintPlayAreaPositions(superstorm.TurnTaker);
            Assert.IsTrue(GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea).ElementAt(1) == churning, churning.Title + " is not in the correct position.");
            GoToPlayCardPhase(superstorm);
            PrintPlayAreaPositions(superstorm.TurnTaker);
            Assert.IsTrue(GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea).ElementAt(2) == churning, churning.Title + " is not in the correct position.");
            AssertNotFlipped(churning);

        }

        [Test()]
        public void TestChurningVoid_StartOfTurn_MoveCard_NoCardsRight()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.SuperstormAkela");
            StartGame();
            GoToEndOfTurn(haka);

            //destroy mdp
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card djinn = PlayCard("ForgottenDjinn");
            Card churning = PlayCard("ChurningVoid");


            //After all other start of turn effects have taken place, move this card 1 space to the right in the environment play area.
            GoToStartOfTurn(superstorm);
            PrintPlayAreaPositions(superstorm.TurnTaker);
            Assert.IsTrue(GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea).ElementAt(1) == churning, churning.Title + " is not in the correct position.");
            GoToPlayCardPhase(superstorm);
            PrintPlayAreaPositions(superstorm.TurnTaker);
            Assert.IsTrue(GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea).ElementAt(1) == churning, churning.Title + " is not in the correct position.");
            AssertNotFlipped(churning);


        }

        [Test()]
        public void TestScatterburst()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.SuperstormAkela");
            StartGame();
            GoToPlayCardPhase(haka);
            SetHitPoints(new TurnTakerController[] { baron, ra, legacy, haka, tachyon }, 10);
            IEnumerable<Card> villainCardsToPlay = FindCardsWhere((Card c) => baron.TurnTaker.Deck.Cards.Contains(c) && !c.IsOneShot).Take(5);
            PlayCards(villainCardsToPlay);

            Card scatterburst = PlayCard("Scatterburst");
            IEnumerable<Card> envCardsToPlay = FindCardsWhere((Card c) => superstorm.TurnTaker.Deck.Cards.Contains(c) && c != scatterburst).Take(5);
            PlayCards(envCardsToPlay);

            //On the turn this card enters play, after all other end of turn effects have taken place, shuffle all non-character villain cards from the villain play area and replace them in a random order. 
            //Do the same for environment cards in the environment play area.
            //Then, each hero target regains 1HP and this card is destroyed.
            PrintPlayAreaPositions(baron.TurnTaker);
            PrintPlayAreaPositions(superstorm.TurnTaker);
            QuickHPStorage(baron, ra, legacy, haka, tachyon);
            GoToNextTurn();
            QuickHPCheck(0, 1, 1, 1, 1);
            PrintPlayAreaPositions(baron.TurnTaker);
            PrintPlayAreaPositions(superstorm.TurnTaker);
       }

        [Test()]
        public void TestToppledSkyscraperRedirect()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.SuperstormAkela");
            StartGame();

            //swap mdp for battalion
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card battalion = PlayCard("BladeBattalion");

            //The first time a villain target would be dealt damage each turn, redirect it to this card.
            Card toppled = PlayCard("ToppledSkyscraper");
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, tachyon.CharacterCard, toppled);
            DealDamage(ra, (Card c) => c.IsTarget, 1, DamageType.Toxic);
            QuickHPCheck(0, -1, -1, -1, -1, -1, -2);

            //only first damage
            QuickHPUpdate();
            DealDamage(ra, (Card c) => c.IsTarget, 1, DamageType.Toxic);
            QuickHPCheck(-1, -1, -1, -1, -1, -1, -1);

            //resets on each turn
            GoToNextTurn();
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, tachyon.CharacterCard, toppled);
            DealDamage(ra, (Card c) => c.IsTarget, 1, DamageType.Toxic);
            QuickHPCheck(0, -1, -1, -1, -1, -1, -2);
        }

        [Test()]
        public void TestToppledSkyscraperChangeOrder()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.SuperstormAkela");
            StartGame();



            //At the start of each hero’s turn, they may choose what order to perform that turn's play, power, and draw phases in.
            Card toppled = PlayCard("ToppledSkyscraper");
            GoToStartOfTurn(ra);
            DecisionSelectTurnPhases = new TurnPhase[] { ra.HeroTurnTaker.TurnPhases.ElementAt(3), ra.HeroTurnTaker.TurnPhases.ElementAt(1) };
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(ra, Phase.DrawCard);
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(ra, Phase.PlayCard);
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(ra, Phase.UsePower);
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(ra, Phase.End);

        }

        [Test()]
        public void TestToppledSkyscraper_IncappedHero()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.SuperstormAkela");
            StartGame();


            DealDamage(baron, ra, 100, DamageType.Melee);
            AssertIncapacitated(ra);

            //At the start of each hero’s turn, they may choose what order to perform that turn's play, power, and draw phases in.
            Card toppled = PlayCard("ToppledSkyscraper");
            GoToStartOfTurn(ra);
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(ra, Phase.UseIncapacitatedAbility);
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(ra, Phase.End);

        }

        [Test()]
        public void TestAscendedEdifice_PlayCard()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.SuperstormAkela");
            StartGame();

            GoToPlayCardPhase(superstorm);
            Card edifice = GetCard("AscendedEdifice");
            Card currents = PutOnDeck("RideTheCurrents");
            //When this card enters play, play the top card of the environment deck.
            PlayCard(edifice);
            AssertInPlayArea(superstorm, currents);

        }

        [Test()]
        public void TestAscendedEdifice_Reduce()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.SuperstormAkela");
            StartGame();
            //swap mdp for battalion
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card battalion = PlayCard("BladeBattalion");

            GoToPlayCardPhase(superstorm);
            Card edifice = GetCard("AscendedEdifice");
            Card indra = PutOnDeck("GeminiIndra");

            //Reduce damage dealt to villain cards by 1.
            PlayCard(edifice);
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, tachyon.CharacterCard, edifice);
            DealDamage(haka, (Card c) => c.IsTarget, 2, DamageType.Toxic);
            QuickHPCheck(-1, -1, -2, -2, -2, -2, -2);

        }

        [Test()]
        public void TestCutLoose_IncreaseDamage()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.SuperstormAkela");
            StartGame();
            //swap mdp for battalion
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card battalion = PlayCard("BladeBattalion");

            GoToPlayCardPhase(superstorm);
            Card cutLoose = PlayCard("CutLoose");
            Card indra = PlayCard("GeminiIndra");

            //Increase all damage dealt by 1.

            //check villain
            QuickHPStorage(haka);
            DealDamage(baron, haka, 1, DamageType.Melee);
            QuickHPCheck(-2);

            //check hero
            QuickHPUpdate();
            DealDamage(ra, haka, 1, DamageType.Fire);
            QuickHPCheck(-2);

            //check environment
            QuickHPUpdate();
            DealDamage(indra, haka.CharacterCard, 1, DamageType.Melee);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestCutLoose_TargetDestroyed()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.SuperstormAkela");
            StartGame();
            //swap mdp for battalion
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card battalion = PlayCard("BladeBattalion");

            GoToPlayCardPhase(superstorm);
            Card cutLoose = PlayCard("CutLoose");

            //When a hero destroys a target, this card deals that hero {H} projectile damage and is destroyed
            //H=4 +1 from cut loose buff

            //don't trigger on villain destruction
            Card indra = PlayCard("GeminiIndra");
            QuickHPStorage(baron);
            DestroyCard(indra, baron.CharacterCard);
            QuickHPCheckZero();
            AssertInPlayArea(superstorm, cutLoose);

            //trigger on hero destruction
            PlayCard(indra);
            QuickHPStorage(haka);
            DestroyCard(indra, haka.CharacterCard);
            QuickHPCheck(-5);
            AssertInTrash(superstorm, cutLoose);

        }


        [Test()]
        public void TestFlyingBus()
        {
            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela" });
            StartGame();
            GoToPlayCardPhase(superstorm);
            Card bus = GetCard("FlyingBus");
            PlayCard("PressureDrop");
            //stack deck to prevent extra targets
            PutOnDeck("ToppledSkyscraper");
            PlayCard(bus);
            QuickHPStorage(ra, legacy, haka);
            //At the end of the environment turn, this card deals the X+1 hero targets with the highest HP {H} projectile damage each, where X is the number of environment cards to the left of this one
           
            PrintPlayAreaPositions(superstorm.TurnTaker);
            GoToEndOfTurn(superstorm);
            QuickHPCheck(0, -3, -3);

        }

        [Test()]
        public void TestForgottenDjinn()
        {
            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela" });
            StartGame();
            GoToPlayCardPhase(superstorm);
            Card djinn = GetCard("ForgottenDjinn");
            PlayCard("PressureDrop");
            //stack deck to prevent extra targets
            PutOnDeck("ToppledSkyscraper");
            PlayCard(djinn);
            QuickHPStorage(ra, legacy, haka);
            //At the end of the environment turn, this card deals the non-environment target with the second highest HP X+2 melee damage, where X is the number of environment cards to the left of this one.
            //2nd highest is haka
            //X = 1
            PrintPlayAreaPositions(superstorm.TurnTaker);
            GoToEndOfTurn(superstorm);
            QuickHPCheck(0, 0, -3);

        }

        [Test()]
        public void TestGeminiIndra_IncreaseLightningDamage()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.SuperstormAkela");
            StartGame();
            //swap mdp for battalion
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card battalion = PlayCard("BladeBattalion");

            GoToPlayCardPhase(superstorm);
            Card indra = PlayCard("GeminiIndra");

            //Increase all lightning damage dealt by 1.

            //check villain
            QuickHPStorage(haka);
            DealDamage(baron, haka, 1, DamageType.Lightning);
            QuickHPCheck(-2);

            //check hero
            QuickHPUpdate();
            DealDamage(ra, haka, 1, DamageType.Lightning);
            QuickHPCheck(-2);

            //check environment
            QuickHPUpdate();
            DealDamage(indra, haka.CharacterCard, 1, DamageType.Lightning);
            QuickHPCheck(-2);

            //check only lightning
            QuickHPUpdate();
            DealDamage(ra, haka, 1, DamageType.Fire);
            QuickHPCheck(-1);

        }

        [Test()]
        public void TestGeminiMaya_IncreaseProjectileDamage()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Tachyon", "Cauldron.SuperstormAkela");
            StartGame();
            //swap mdp for battalion
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card battalion = PlayCard("BladeBattalion");

            GoToPlayCardPhase(superstorm);
            Card maya = PlayCard("GeminiMaya");

            //Increase all projectile damage dealt by 1.

            //check villain
            QuickHPStorage(haka);
            DealDamage(baron, haka, 1, DamageType.Projectile);
            QuickHPCheck(-2);

            //check hero
            QuickHPUpdate();
            DealDamage(ra, haka, 1, DamageType.Projectile);
            QuickHPCheck(-2);

            //check environment
            QuickHPUpdate();
            DealDamage(maya, haka.CharacterCard, 1, DamageType.Projectile);
            QuickHPCheck(-2);

            //check only projectile
            QuickHPUpdate();
            DealDamage(ra, haka, 1, DamageType.Fire);
            QuickHPCheck(-1);

        }

        [Test()]
        public void TestGeminiIndra_EndOfTurn()
        {
            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela" });
            StartGame();
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            GoToPlayCardPhase(superstorm);
            Card indra = GetCard("GeminiIndra");
            PlayCard("PressureDrop");
            //stack deck to prevent extra targets
            PutOnDeck("ToppledSkyscraper");
            PlayCard(indra);
            QuickHPStorage(baron, ra, legacy, haka);
            //At the end of the environment turn, this card deals the 2 targets with the highest HP X+1 projectile damage each, where X is the number of environment cards to the left of this one.
            //2 highest are baron and haka
            //X = 1
            PrintPlayAreaPositions(superstorm.TurnTaker);
            GoToEndOfTurn(superstorm);
            QuickHPCheck(-2, 0, 0, -2);

        }

        [Test()]
        public void TestGeminiMaya_EndOfTurn()
        {
            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela" });
            StartGame();
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            GoToPlayCardPhase(superstorm);
            Card maya = GetCard("GeminiMaya");
            PlayCard("PressureDrop");
            //stack deck to prevent extra targets
            PutOnDeck("ToppledSkyscraper");
            PlayCard(maya);
            QuickHPStorage(baron, ra, legacy, haka);
            //At the end of the environment turn, this card deals the 2 targets with the highest HP X+1 lightning damage each, where X is the number of environment cards to the left of this one.
            //2 highest are baron and haka
            //X = 1
            PrintPlayAreaPositions(superstorm.TurnTaker);
            GoToEndOfTurn(superstorm);
            QuickHPCheck(-2, 0, 0, -2);

        }

        [Test()]
        public void TestGeogravLocus_EndOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();
            GoToPlayCardPhase(superstorm);
            Card locus = GetCard("GeogravLocus");
            Card pressure = PutOnDeck("PressureDrop");
            PlayCard(locus);
            //At the end of the environment turn, play the top card of the environment deck.
            GoToEndOfTurn(superstorm);
            AssertInPlayArea(superstorm, pressure);

        }

        [Test()]
        public void TestGeogravLocus_ReduceDamage()
        {
            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela" });
            StartGame();
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            GoToPlayCardPhase(superstorm);
            Card geograv = GetCard("GeogravLocus");
            PlayCard("PressureDrop");
            Card indra = PlayCard("GeminiIndra");
            PlayCard(geograv);
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, geograv);
           //Reduce damage dealt to this card by X, where X is the number of environment cards to the left of this one.
           //X = 2
            PrintPlayAreaPositions(superstorm.TurnTaker);
            DealDamage(ra, (Card c) => c.IsTarget, 4, DamageType.Fire);
            QuickHPCheck(-4, -4, -4, -4, -2);

            //check dynamic
            DestroyCard(indra, baron.CharacterCard);
            QuickHPUpdate();
            PrintPlayAreaPositions(superstorm.TurnTaker);
            DealDamage(ra, (Card c) => c.IsTarget, 4, DamageType.Fire);
            QuickHPCheck(-4, -4, -4, -4, -3);



        }

        [Test()]
        public void TestPressureDrop_Discard()
        {
            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela" });
            StartGame();

            SetHitPoints(new TurnTakerController[] { ra, legacy }, 10);
            GoToPlayCardPhase(superstorm);
            Card geograv = PlayCard("GeogravLocus");
            Card indra = PlayCard("GeminiIndra");

            QuickHandStorage(legacy);
            SelectCardsForNextDecision( legacy.CharacterCard, legacy.HeroTurnTaker.Hand.Cards.ElementAt(0), legacy.HeroTurnTaker.Hand.Cards.ElementAt(1));
            PrintPlayAreaPositions(superstorm.TurnTaker);
            //When this card enters play, the hero with the lowest HP must discard X cards, where X is the number of environment cards to the left of this one.
            Card pressure = PlayCard("PressureDrop");
            QuickHandCheck(-2);
            
        }

        [Test()]
        public void TestPressureDrop_Destroy()
        {
            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela" });
            StartGame();

            //At the start of the environment turn, if there are 3 cards to the right of this one, destroy this card.
            Card pressure = PlayCard("PressureDrop");
            PlayCard("GeminiMaya");
            PlayCard("GeminiIndra");

            //Make sure Pressure Drop does not destroy itself with 2 cards to the right
            GoToEndOfTurn(haka);
            PrintPlayAreaPositions(superstorm.TurnTaker);
            GoToStartOfTurn(superstorm);
            AssertIsInPlay(pressure);

            //Check that Pressure Drop destroys itself with 3 cards to the right
            PlayCard("FlyingBus");
            GoToEndOfTurn(haka);
            PrintPlayAreaPositions(superstorm.TurnTaker);
            GoToStartOfTurn(superstorm);
            AssertInTrash(pressure);
        }

        [Test()]
        public void TestSkulkingIntermediary_Immune()
        {
            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela" });
            StartGame();

            GoToPlayCardPhase(superstorm);

            Card skulking = PlayCard("SkulkingIntermediary");
            //This card is immune to damage dealt by villain targets.
            QuickHPStorage(skulking);
            DealDamage(baron, skulking, 6, DamageType.Melee);
            QuickHPCheckZero();

            //check only for villain damage
            QuickHPUpdate();
            DealDamage(ra, skulking, 6, DamageType.Fire);
            QuickHPCheck(-6);

        }

        [Test()]
        public void TestSkulkingIntermediary_EndOfTurn()
        {
            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela" });
            StartGame();


            GoToPlayCardPhase(superstorm);

            Card geograv = PlayCard("PressureDrop");
            Card indra = PlayCard("ToppledSkyscraper");
            Card skulking = PlayCard("SkulkingIntermediary");
            IEnumerable<Card> listOfTargets = FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.IsTarget);
            IEnumerable<Card> listOfEnvTargets = listOfTargets.Where((Card c) => c.IsEnvironment || c.IsVillainTarget);
            IEnumerable<Card> listOfHeroTargets = listOfTargets.Where((Card c) => c.IsHero && c.IsTarget);
            SetHitPoints(listOfTargets, 3);
            //At the end of the environment turn, each environment and villain target regains X+1 HP, where X is the number of environment cards to the left of this one.
            GoToEndOfTurn(superstorm);
            Assert.IsTrue(listOfEnvTargets.All((Card c) => c.HitPoints == 6), "Not all environment and villain targets gained HP");
            Assert.IsTrue(listOfHeroTargets.All((Card c) => c.HitPoints == 3), "Some hero targets incorrectly gained HP");
            

        }



    }
}
