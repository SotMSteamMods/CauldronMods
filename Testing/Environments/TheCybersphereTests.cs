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

        [Test()]
        public void TestF0551l()
        {
            SetupGameController("BaronBlade", "Ra", "Stuntman", "Haka", "Cauldron.TheCybersphere");
            StartGame();

            //this will result in stuntman being the second highest
            SetHitPoints(ra, 20);
            SetHitPoints(haka, 15);
            SetHitPoints(stunt, 25);
            SetHitPoints(baron, 25);

            //destroy mdp to make him vulnerable
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            //Play out CouteQueCoute to verify there were two instances of damage
            PlayCard("CouteQueCoute");

            GoToPlayCardPhase(cybersphere);
            Card f0551l = PlayCard("Fo551l");

            //At the end of the environment turn, this card deals the non-environment target with the second highest HP 3 melee damage and 1 lightning damage.
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, stunt.CharacterCard, haka.CharacterCard, f0551l);
            //should allow you to select if there is a tie
            DecisionSelectTarget = stunt.CharacterCard;
            GoToEndOfTurn(cybersphere);
            //3 +1, 1+1 -> 6 damage total
            QuickHPCheck(0, 0, -6, 0, 0);

        }

        [Test()]
        public void TestGho5t_EndOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheCybersphere");
            StartGame();

            string[] inPlay = new string[] { "TheStaffOfRa", "Dominion" };
            IEnumerable<Card> inPlayCards = PlayCards(inPlay);

            string[] inTrash = new string[] { "FlameBarrier", "Mere" };
            IEnumerable<Card> inTrashCards = PlayCards(inTrash);

            //destroy mdp to make him vulnerable
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            GoToPlayCardPhase(cybersphere);
            Card gho5t = PlayCard("Gho5t");

            //At the end of the environment turn, each hero must destroy 1 of their ongoing or equipment cards.
            DecisionSelectCards = inTrashCards;
            GoToEndOfTurn(cybersphere);
            AssertInTrash(inTrashCards);
            AssertIsInPlay(inPlayCards);

        }

        [Test()]
        public void TestGho5t_Destroyed()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheCybersphere");
            StartGame();

            //destroy mdp to make him vulnerable
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            GoToPlayCardPhase(cybersphere);
            Card gho5t = PlayCard("Gho5t");

            //When this card is destroyed, each player may draw a card.
            QuickHandStorage(ra, legacy, haka);
            DestroyCard(gho5t, baron.CharacterCard);
            QuickHandCheck(1, 1, 1);


        }

        [Test()]
        public void TestGlitch_Destroyed()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.TheCybersphere");
            StartGame();

            SetHitPoints(ra, 20);
            SetHitPoints(haka, 15);

            //destroy mdp to make him vulnerable
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            GoToPlayCardPhase(cybersphere);
            Card glitch = PlayCard("Glitch");

            //When this card is destroyed, it deals the non-environment target with the second highest HP 5 lightning damage.
            //legacy is second highest
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            DestroyCard(glitch, baron.CharacterCard);
            QuickHPCheck(0, 0, -5, 0);


        }

    }
}
