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
    public class SuperstormAkelaTests : BaseTest
    {

        #region SuperstormAkelaHelperFunctions

        protected TurnTakerController superstorm { get { return FindEnvironment(); } }
        
        private void PrintPlayAreaPositions()
        {
            foreach (var card in GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea))
            {
                Console.WriteLine(card.Title + " is in Position " + GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea).ToList().IndexOf(card));
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
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();
            //stack deck to reduce variability
            PutOnDeck("TheStaffOfRa");

            GoToPlayCardPhase(superstorm);
            Card currents = PlayCard("RideTheCurrents");
            IEnumerable<Card> cardsToPlay = FindCardsWhere((Card c) => superstorm.TurnTaker.Deck.HasCard(c)).Take(4);
            PlayCards(cardsToPlay);
            DecisionSelectFunction = 1;
            //selecting the first card played and moving it to the last position
            DecisionAutoDecideIfAble = true;
            DecisionSelectCards = new Card[] {currents, cardsToPlay.ElementAt(3) };
            PrintPlayAreaPositions();
            GoToStartOfTurn(baron);

            PrintPlayAreaPositions();

            Assert.IsTrue(GetOrderedCardsInLocation(superstorm.TurnTaker.PlayArea).ElementAt(4) == currents, currents.Title + " is not in the correct position.");

        }

        [Test()]
        public void TestRideTheCurrents_Play()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();
            GoToPlayCardPhase(superstorm);

            //When this card enters play, select the deck with the least number of non-character cards in play. Put the top card of that deck into play.
            DecisionSelectTurnTaker = legacy.TurnTaker;
            Card topCard = legacy.TurnTaker.Deck.TopCard;
            Card currents = PlayCard("RideTheCurrents");
            AssertNotInDeck(topCard);
           

        }

        [Test()]
        public void TestFlailingWires_EndOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.SuperstormAkela");
            StartGame();
            GoToPlayCardPhase(superstorm);
            Card wires = GetCard("FlailingWires");
            IEnumerable<Card> cardsToPlay = FindCardsWhere((Card c) => superstorm.TurnTaker.Deck.HasCard(c) && c != wires).Take(1);
            PlayCards(cardsToPlay);
            PlayCard(wires);
            QuickHPStorage(ra, legacy, haka);
            //At the end of the environment turn, this card deals the X+1 hero targets with the highest HP 1 lightning damage each, where X is the number of environment cards to the left of this one.

            GoToEndOfTurn(superstorm);
            QuickHPCheck(0, -1, -1);

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



    }
}
