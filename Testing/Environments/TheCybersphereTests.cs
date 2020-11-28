using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class TheCybersphereTests : BaseTest
    {

        #region CybersphereHelperFunctions

        protected TurnTakerController cybersphere { get { return FindEnvironment(); } }

        #endregion

        [Test()]
        public void TestCybersphereLoads()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheCybersphere");
            StartGame();
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

        }

        [Test()]
        [Sequential]
        public void DecklistTestGridVirus_IsGridVirus([Values("B3h3mth", "Dr3Dnt", "Fo551l", "Gho5t", "Glitch", "H3l1x", "InfectedFirewall", "InfectedHoloweapon", "N1nj4", "Sp4rk")] string gridVirus)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheCybersphere");
            StartGame();

            GoToPlayCardPhase(cybersphere);

            Card card = PlayCard(gridVirus);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "grid virus", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTestGridProgram_IsGridProgram([Values("HologameArena", "HolocycleRace")] string gridProgram)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheCybersphere");
            StartGame();

            GoToPlayCardPhase(cybersphere);

            Card card = PlayCard(gridProgram);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "grid program", false);
        }

        [Test()]
        public void TestB3h3mth()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheCybersphere");
            StartGame();

            SetHitPoints(haka, 15);

            GoToPlayCardPhase(cybersphere);
            Card b3h3mth = PlayCard("B3h3mth");

            //At the end of the environment turn, this card deals the non-environment target with the second lowest HP 4 fire damage.
            //2nd lowest is Haka

            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, b3h3mth);
            GoToEndOfTurn(cybersphere);
            QuickHPCheck(0, 0, 0, -4, 0);

        }

        [Test()]
        public void TestDr3Dnt()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheCybersphere");
            StartGame();

            //destroy mdp to make him vulnerable
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            GoToPlayCardPhase(cybersphere);
            Card dr3Dnt = PlayCard("Dr3Dnt");

            //At the end of the environment turn, this card deals each other target 2 projectile damage.
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, dr3Dnt);
            GoToEndOfTurn(cybersphere);
            QuickHPCheck(-2, -2, -2, -2, 0);

        }

    }
}
