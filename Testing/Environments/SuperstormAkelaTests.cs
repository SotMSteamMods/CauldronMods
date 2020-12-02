using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class SuperstormAkelaTests : BaseTest
    {

        #region SuperstormAkelaHelperFunctions

        protected TurnTakerController superstorm { get { return FindEnvironment(); } }
      
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



    }
}
