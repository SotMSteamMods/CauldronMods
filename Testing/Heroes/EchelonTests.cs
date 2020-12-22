using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cauldron.Echelon;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture]
    public class EchelonTests : BaseTest
    {
        #region echelonhelperfunctions
        protected HeroTurnTakerController echelon => FindHero("Echelon");

        private const string DeckNamespace = "Cauldron.Echelon";

        [Test]
        public void TestEchelonLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", DeckNamespace, "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(echelon);
            Assert.IsInstanceOf(typeof(EchelonCharacterCardController), echelon.CharacterCardController);

            Assert.AreEqual(27, echelon.CharacterCard.HitPoints);
        }


        private readonly DamageType DTM = DamageType.Melee;
        private Card MDP => GetCardInPlay("MobileDefensePlatform");
        #endregion

        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                AssertCardHasKeyword(GetCard(id), keyword, false);
            }
        }
        [Test]
        public void TestEchelonDecklist()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            AssertHasKeyword("tactic", new []
            {
                "AdvanceAndRegroup",
                "BreakThrough",
                "KnowYourEnemy",
                "PracticedTeamwork",
                "RemoteObservation",
                "RuthlessIntimidation",
                "StaggeredAssault",
                "SurpriseAttack"
            });

            AssertHasKeyword("ongoing", new[]
            {
                "AdvanceAndRegroup",
                "BreakThrough",
                "FindAWayIn",
                "KnowYourEnemy",
                "NeedAWayOut",
                "Overwatch",
                "PracticedTeamwork",
                "RemoteObservation",
                "RuthlessIntimidation",
                "StaggeredAssault",
                "SurpriseAttack"
            });

            AssertHasKeyword("equipment", new[]
            {
                "DatabaseUplink",
                "TeslaKnuckles",
                "TheKestrelMarkII"
            });

            AssertHasKeyword("limited", new[]
            {
                "FindAWayIn",
                "NeedAWayOut",
                "Overwatch",
                "TeslaKnuckles",
                "TheKestrelMarkII"
            });

            AssertHasKeyword("one-shot", new[]
            {
                "CommandAndConquer",
                "FirstResponder",
                "StrategicDeployment"
            });
        }
        [Test]
        public void TestExtensibleTacticKeep([Values("KnowYourEnemy",
                                            "PracticedTeamwork",
                                            "RemoteObservation",
                                            "RuthlessIntimidation",
                                            "StaggeredAssault",
                                            "SurpriseAttack")] string tacticID)
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card tactic = PlayCard(tacticID);

            QuickHandStorage(echelon);
            GoToStartOfTurn(echelon);
            AssertIsInPlay(tactic);
            QuickHandCheck(-1);
        }
        [Test]
        public void TestExtensibleTacticDrop([Values("KnowYourEnemy",
                                            "PracticedTeamwork",
                                            "RemoteObservation",
                                            "RuthlessIntimidation",
                                            "StaggeredAssault",
                                            "SurpriseAttack")] string tacticID)
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            //Practiced Teamwork is the only extensible tactic that doesn't draw when dropped 
            //(it gives you the draw up-front instead)
            int gain = tacticID == "PracticedTeamwork" ? 0 : 1;
            Card tactic = PlayCard(tacticID);

            DecisionDoNotSelectCard = SelectionType.DiscardCard;

            QuickHandStorage(echelon);

            GoToStartOfTurn(echelon);
            AssertInTrash(tactic);
            QuickHandCheck(gain);
        }
        
        [Test]
        public void TestNonExtensibleTacticSelfDestroy([Values("AdvanceAndRegroup", "BreakThrough")] string tacticID)
        {        
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card tactic = PlayCard(tacticID);

            AssertNoDecision();
            GoToStartOfTurn(echelon);
            AssertInTrash(tactic);
        }
        /*
        [Test]
        public void TestFindAwayIn()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card findAWayIn = GetCard(FindAWayInCardController.Identifier);

            GoToPlayCardPhase(echelon);
            PlayCard(findAWayIn);

            DecisionYesNo = true;

            GoToStartOfTurn(ra);
            GoToPlayCardPhase(ra);
            PlayCard(ra, GetCardFromHand(ra, 0));
            //GoToUsePowerPhase(ra);
            //UsePower(ra);


            GoToEndOfTurn(ra);

            Assert.True(false, "TODO");
        }

        [Test]
        public void TestFirstResponder()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card firstRespond = GetCard(FirstResponderCardController.Identifier);

            DecisionSelectCards = null;
            DecisionDiscardCard = null;

            GoToPlayCardPhase(echelon);
            PlayCard(firstRespond);

            DecisionYesNo = false;

            Assert.True(false, "TODO");
        }
        */

    }
}
