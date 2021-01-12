using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class NightloreCitadelTests : BaseTest
    {

        #region NightloreCitadelHelperFunctions

        protected TurnTakerController nightlore { get { return FindEnvironment(); } }
        protected bool IsConstellation(Card card)
        {
            return card.DoKeywordsContain("constellation");
        }

        #endregion

        [Test()]
        public void TestNightloreCitadelWorks()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

        }

        [Test()]
        [Sequential]
        public void DecklistTest_NightloreTrainer_IsNightloreTrainer([Values("TalinBrosk")] string nightloreTrainer)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            GoToPlayCardPhase(nightlore);

            Card card = PlayCard(nightloreTrainer);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "nightlore trainer", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Superweapon_IsSuperweapon([Values("AethiumCannon")] string superweapon)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            GoToPlayCardPhase(nightlore);

            Card card = PlayCard(superweapon);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "superweapon", false);
        }
        [Test()]
        [Sequential]
        public void DecklistTest_NightloreCouncilor_IsNighloreCouncilor([Values("ArtemisVector")] string nightloreCouncilor)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            GoToPlayCardPhase(nightlore);

            Card card = PlayCard(nightloreCouncilor);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "nightlore councilor", false);
        }
        [Test()]
        [Sequential]
        public void DecklistTest_NightloreAgent_IsNighloreAgent([Values("StarlightOfZzeck", "StarlightOfNoome")] string nightloreAgent)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            GoToPlayCardPhase(nightlore);

            Card card = PlayCard(nightloreAgent);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "nightlore agent", false);
        }
        [Test()]
        [Sequential]
        public void DecklistTest_AncientAutomaton_IsAncientAutomaton([Values("CitadelGarrison")] string ancientAutomation)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            GoToPlayCardPhase(nightlore);

            Card card = PlayCard(ancientAutomation);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "ancient automaton", false);
        }
        [Test()]
        [Sequential]
        public void DecklistTest_NightloreTraitor_IsNighloreTraitor([Values("StarlightOfOros")] string nightloreTraitor)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            GoToPlayCardPhase(nightlore);

            Card card = PlayCard(nightloreTraitor);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "nightlore traitor", false);
        }
        [Test()]
        [Sequential]
        public void DecklistTest_Constellation_IsConstellation([Values("RogueConstellation")] string constellation)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            GoToPlayCardPhase(nightlore);

            Card card = PlayCard(constellation);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "constellation", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_NoKeyword_HasNoKeyword([Values("WarpBridge", "GravityFluctuation", "LonelyCalling", "AssembleTheCouncil", "WatchedByTheStars", "UrgentMission", "AethiumRage")] string keywordLess)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.NightloreCitadel");
            StartGame();

            GoToPlayCardPhase(nightlore);

            Card card = PlayCard(keywordLess);
            AssertIsInPlay(card);
            Assert.IsFalse(card.Definition.Keywords.Any(), $"{card.Title} has keywords when it shouldn't.");
        }
    }
}
