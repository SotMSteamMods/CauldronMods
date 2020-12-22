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

        private readonly DamageType DTM = DamageType.Melee;
        private Card MDP => GetCardInPlay("MobileDefensePlatform");
        
        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                AssertCardHasKeyword(GetCard(id), keyword, false);
            }
        }
        #endregion
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
        [Test]
        public void TestEchelonPower()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();
            DestroyCard(MDP);

            QuickHPStorage(baron);
            UsePower(echelon);
            QuickHPCheck(-1);

            PlayCard("LivingForceField");
            UsePower(echelon);
            QuickHPCheck(-1);
        }
        [Test]
        public void TestEchelonIncap1()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();
            DestroyCard(MDP);
            DealDamage(baron, echelon, 50, DTM);

            Card flesh = PlayCard("FleshOfTheSunGod");
            UseIncapacitatedAbility(echelon, 0);
            AssertInTrash(flesh);
        }
        [Test]
        public void TestEchelonIncap2()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();
            DestroyCard(MDP);
            DealDamage(baron, echelon, 50, DTM);

            Card punch = PutInHand("SuckerPunch");
            Card assault = PutInHand("AcceleratedAssault");
            Card goggles = PutOnDeck("HUDGoggles");
            Card limit = PutOnDeck("PushingTheLimits");
            DecisionYesNo = true;
            DecisionSelectTurnTaker = tachyon.TurnTaker;
            DecisionSelectCards = new Card[] { punch, goggles };

            QuickHandStorage(tachyon, ra);
            UseIncapacitatedAbility(echelon, 1);
            QuickHandCheckZero();
            AssertInTrash(punch, goggles);
            AssertInHand(assault, limit);
        }
        [Test]
        public void TestEchelonIncap2Optional()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();
            DestroyCard(MDP);
            DealDamage(baron, echelon, 50, DTM);

            Card punch = PutInHand("SuckerPunch");
            Card assault = PutInHand("AcceleratedAssault");
            DecisionSelectTurnTaker = tachyon.TurnTaker;

            Card goggles = PutOnDeck("HUDGoggles");
            Card limit = PutOnDeck("PushingTheLimits");
            DecisionYesNo = false;

            QuickHandStorage(tachyon, ra);
            UseIncapacitatedAbility(echelon, 1);
            QuickHandCheckZero();
            AssertInHand(punch, assault);
            AssertOnTopOfDeck(limit);
            AssertOnTopOfDeck(goggles, 1);
        }
        [Test]
        public void TestEchelonIncap3()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();
            DestroyCard(MDP);
            DealDamage(baron, echelon, 50, DTM);

            Card topOfDeck = GetTopCardOfDeck(baron);
            UseIncapacitatedAbility(echelon, 2);
            AssertOnBottomOfDeck(topOfDeck);
        }
        [Test]
        public void TestAdvanceAndRegroupEffect()
        {
            SetupGameController("BaronBlade", DeckNamespace, "Ra", "Tachyon", "Megalopolis");
            StartGame();

            PlayCard("AdvanceAndRegroup");
            SetHitPoints(echelon, 20);
            SetHitPoints(ra, 20);

            QuickHPStorage(ra, echelon);
            DecisionSelectCards = new Card[] { echelon.CharacterCard, ra.CharacterCard };

            DestroyCard(MDP);
            QuickHPCheck(0, 2);

            Card batt = PlayCard("BladeBattalion");
            DestroyCard(batt);
            QuickHPCheck(2, 0);

            Card traffic = PlayCard("TrafficPileup");
            DestroyCard(traffic);
            QuickHPCheck(2, 0);
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
