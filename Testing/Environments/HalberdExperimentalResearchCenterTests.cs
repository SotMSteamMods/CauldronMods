using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class HalberdExperimentalResearchCenterTests : BaseTest
    {

        #region HalberdExperimentalResearchCenterHelperFunctions

        protected TurnTakerController halberd { get { return FindEnvironment(); } }

        

        #endregion

        [Test()]
        public void TestHalberdWorks()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());
            
        }
        [Test()]
        public void TestEmergencyProtocolsPlay()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();
            //stack the deck so we know what the played card will be
            Card prophet = GetCard("HalberdProphet");
            PutOnDeck(halberd, prophet);
            GoToPlayCardPhase(halberd);
            AssertInDeck(prophet);
            //At the end of the environment turn, play the top card of the environment deck.

            PlayCard("EmergencyReleaseProtocol");
            GoToEndOfTurn(halberd);
            //since the top card of the deck was played, prophet should be in play
            AssertIsInPlay(prophet);

        }

        [Test()]
        public void TestEmergencyProtocolsDestroy()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.HalberdExperimentalResearchCenter");
            StartGame();

            Card emergency = GetCard("EmergencyReleaseProtocol");
            PlayCard(emergency);
            AssertIsInPlay(emergency);

            //At the start of their turn, a player may skip the rest of their turn to destroy this card.
            //yes we want the player to skip their turn
            DecisionYesNo = true;
            GoToStartOfTurn(ra);
            AssertInTrash(emergency);

        }




    }
}
