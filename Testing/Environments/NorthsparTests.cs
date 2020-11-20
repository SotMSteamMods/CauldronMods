using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class NorthsparTests : BaseTest
    {

        #region NorthsparHelperFunctions

        protected TurnTakerController northspar { get { return FindEnvironment(); } }

        #endregion

        [Test()]
        public void TestNorthsparWorks()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Northspar");
            StartGame();
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

        }

        [Test()]
        [Sequential]
        public void DecklistTest_Mercenary_IsMercenary([Values("TakAhab")] string mercenary)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Northspar");
            StartGame();

            GoToPlayCardPhase(northspar);

            Card card = PlayCard(mercenary);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "mercenary", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_FirstWaypoint_IsFirstWaypoint([Values("MakeshiftShelter")] string firstWaypoint)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Northspar");
            StartGame();

            GoToPlayCardPhase(northspar);

            Card card = PlayCard(firstWaypoint);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "first waypoint", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_SecondWaypoint_IsSecondWaypoint([Values("SupplyDepot")] string secondWaypoint)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Northspar");
            StartGame();

            GoToPlayCardPhase(northspar);

            //put 1st waypoint in play so no autodestroy
            PlayCard("MakeshiftShelter");

            Card card = PlayCard(secondWaypoint);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "second waypoint", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_ThirdWaypoint_IsThirdWaypoint([Values("DemolishedCamp", "LandingSite")] string thirdWaypoint)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Northspar");
            StartGame();

            GoToPlayCardPhase(northspar);

            //put 1st and 2nd waypoint in play so no autodestroy
            PlayCard("MakeshiftShelter");
            PlayCard("SupplyDepot");

            Card card = PlayCard(thirdWaypoint);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "third waypoint", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_MineralDeposit_IsMineralDeposit([Values("AethiumVein")] string mineralDeposit)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Northspar");
            StartGame();

            GoToPlayCardPhase(northspar);

            Card card = PlayCard(mineralDeposit);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "mineral deposit", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Frozen_IsFrozen([Values("LostInTheSnow", "RagingBlizzard", "Frostbite", "SnowShrieker", "FrozenSolid","WhatsLeftOfThem", "BitterCold" )] string frozen)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Northspar");
            StartGame();

            GoToPlayCardPhase(northspar);

            Card card = PlayCard(frozen);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "frozen", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Creature_IsCreature([Values("AlienFungus", "SnowShrieker")] string creature)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Northspar");
            StartGame();

            GoToPlayCardPhase(northspar);

            Card card = PlayCard(creature);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "creature", false);
        }

        [Test()]
        public void TestThirdWaypoint_PlayTopCardAndDestroy_NotIndestructible()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Northspar");
            StartGame();

            GoToPlayCardPhase(northspar);
            Card bitter = PutOnDeck("BitterCold");
            Card thirdWaypoint = GetCard("LandingSite");
            //When this card enters play, play the top card of the environment deck and destroy this card. 
            AssertInDeck(bitter);
            PlayCard(thirdWaypoint);
            AssertInPlayArea(northspar, bitter);
            AssertInTrash(thirdWaypoint);
        }

        [Test()]
        public void TestThirdWaypoint_PlayTopCardAndDestroy_Indestructible()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Northspar");
            StartGame();

            GoToPlayCardPhase(northspar);
            //put 1st and 2nd waypoint in play so no autodestroy
            PlayCard("MakeshiftShelter");
            PlayCard("SupplyDepot");

            Card bitter = PutOnDeck("BitterCold");
            Card thirdWaypoint = GetCard("LandingSite");

            //When this card enters play, play the top card of the environment deck and destroy this card. 
            AssertInDeck(bitter);
            PlayCard(thirdWaypoint);
            AssertInPlayArea(northspar, bitter);
            AssertInPlayArea(northspar, thirdWaypoint);
        }

        [Test()]
        public void TestThirdWaypoint_OtherThirdWaypointEntersPlay()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Northspar");
            StartGame();

            GoToPlayCardPhase(northspar);
            //put 1st and 2nd waypoint in play so no autodestroy
            PlayCard("MakeshiftShelter");
            PlayCard("SupplyDepot");

            Card bitter = PutOnDeck("BitterCold");
            Card thirdWaypoint = GetCard("LandingSite");
            Card otherThirdWaypoint = GetCard("DemolishedCamp");

            //If Demolished Camp/Landing Site would enter play, instead remove it from the game.
            AssertInDeck(otherThirdWaypoint);
            PlayCard(thirdWaypoint);
            PlayCard(otherThirdWaypoint);
            AssertOutOfGame(otherThirdWaypoint);
        }

        [Test()]
        public void TestLandingSiteStartOfTurn_NoTakAhab()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Northspar");
            StartGame();

            GoToEndOfTurn(haka);

            //play an equipment to destroy
            Card equipment = PlayCard("Mere");

            //put 1st and 2nd waypoint in play so no autodestroy
            PlayCard("MakeshiftShelter");
            PlayCard("SupplyDepot");

            //stack deck to prevent tak ahab from entering play
            PutOnDeck("BitterCold");
            Card landingSite = GetCard("LandingSite");

            PlayCard(landingSite);
            AssertInPlayArea(northspar, landingSite);

            //At the start of the environment turn, if Tak Ahab is in play, you may destroy 1 hero ongoing or equipment card. If you do, place the top card of any deck beneath Tak Ahab.
            //Tak Ahab is not in play so nothing should be destroyed and no cards should be under
            GoToStartOfTurn(northspar);
            AssertInPlayArea(haka, equipment);
        }

        [Test()]
        public void TestLandingSiteStartOfTurn_TakAhabInPlay_DestroyEquipment()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Northspar");
            StartGame();

            GoToEndOfTurn(haka);

            //play an equipment to destroy
            Card equipment = PlayCard("Mere");

            //put 1st and 2nd waypoint in play so no autodestroy
            PlayCard("MakeshiftShelter");
            PlayCard("SupplyDepot");

            //stack deck to prevent tak ahab from entering play
            PutOnDeck("BitterCold");
            Card landingSite = GetCard("LandingSite");

            Card takAhab = PlayCard("TakAhab");
            int numCardsBeneathTakAhab = GetNumberOfCardsUnderCard(takAhab);
            PlayCard(landingSite);
            AssertInPlayArea(northspar, landingSite);

            //At the start of the environment turn, if Tak Ahab is in play, you may destroy 1 hero ongoing or equipment card. If you do, place the top card of any deck beneath Tak Ahab.
            //Tak Ahab is in play, so equipment will be destroyed
            //should be one additional card under takAhab
            GoToStartOfTurn(northspar);
            AssertInTrash(haka, equipment);
            AssertNumberOfCardsUnderCard(takAhab, numCardsBeneathTakAhab + 1);
        }

        [Test()]
        public void TestLandingSiteStartOfTurn_TakAhabInPlay_DestroyOngoing()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Northspar");
            StartGame();

            GoToEndOfTurn(haka);

            //play an ongoing to destroy
            Card ongoing = PlayCard("Dominion");

            //put 1st and 2nd waypoint in play so no autodestroy
            PlayCard("MakeshiftShelter");
            PlayCard("SupplyDepot");

            //stack deck to prevent tak ahab from entering play
            PutOnDeck("BitterCold");
            Card landingSite = GetCard("LandingSite");

            Card takAhab = PlayCard("TakAhab");
            int numCardsBeneathTakAhab = GetNumberOfCardsUnderCard(takAhab);
            PlayCard(landingSite);
            AssertInPlayArea(northspar, landingSite);

            //At the start of the environment turn, if Tak Ahab is in play, you may destroy 1 hero ongoing or equipment card. If you do, place the top card of any deck beneath Tak Ahab.
            //Tak Ahab is in play, so ongoing will be destroyed
            //should be one additional card under takAhab
            GoToStartOfTurn(northspar);
            AssertInTrash(haka, ongoing);
            AssertNumberOfCardsUnderCard(takAhab, numCardsBeneathTakAhab + 1);
        }

        [Test()]
        public void TestLandingSiteStartOfTurn_TakAhabInPlay_MoveTopCardOfAnyDeck()
        {
            SetupGameController("KaargraWarfang", "Ra", "Legacy", "Haka", "Cauldron.Northspar");
            StartGame();

            GoToEndOfTurn(haka);

            //play an ongoing to destroy
            Card ongoing = PlayCard("Dominion");

            //put 1st and 2nd waypoint in play so no autodestroy
            PlayCard("MakeshiftShelter");
            PlayCard("SupplyDepot");

            //stack deck to prevent tak ahab from entering play
            PutOnDeck("BitterCold");
            Card landingSite = GetCard("LandingSite");

            Card takAhab = PlayCard("TakAhab");
            int numCardsBeneathTakAhab = GetNumberOfCardsUnderCard(takAhab);
            PlayCard(landingSite);
            AssertInPlayArea(northspar, landingSite);

            DecisionSelectLocation = new LocationChoice(ra.TurnTaker.Deck);
            Card topCard = ra.TurnTaker.Deck.TopCard;
            //At the start of the environment turn, if Tak Ahab is in play, you may destroy 1 hero ongoing or equipment card. If you do, place the top card of any deck beneath Tak Ahab.
            GoToStartOfTurn(northspar);
            AssertUnderCard(takAhab, topCard);
        }

        [Test()]
        public void TestDemolishedCamp_IncreaseDamage()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Northspar");
            StartGame();

            GoToEndOfTurn(haka);
            DestroyCard(GetCardInPlay("MobileDefensePlatform"), haka.CharacterCard);
            //play an equipment to destroy
            Card equipment = PlayCard("Mere");

            //put 1st and 2nd waypoint in play so no autodestroy
            PlayCard("MakeshiftShelter");
            PlayCard("SupplyDepot");

            //stack deck to prevent tak ahab from entering play
            PutOnDeck("BitterCold");
            Card demolishedCamp = GetCard("DemolishedCamp");

            Card takAhab = PlayCard("TakAhab");
            PlayCard(demolishedCamp);
            AssertInPlayArea(northspar, demolishedCamp);

            // Increase damage dealt to Tak Ahab by 1.
            QuickHPStorage(takAhab);
            DealDamage(haka.CharacterCard, takAhab, 2, DamageType.Projectile);
            QuickHPCheck(-3);

            //should only affect tak ahab

            //check villain
            QuickHPStorage(baron);
            DealDamage(haka, baron, 2, DamageType.Projectile);
            QuickHPCheck(-2);

            //check hero
            QuickHPStorage(haka);
            DealDamage(ra, haka, 2, DamageType.Fire);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestDemolishedCamp_RemoveOnDestruction()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.Northspar");
            StartGame();

            GoToEndOfTurn(haka);
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            //play an equipment to destroy
            Card equipment = PlayCard("Mere");

            //put 1st and 2nd waypoint in play so no autodestroy
            PlayCard("MakeshiftShelter");
            PlayCard("SupplyDepot");

            //stack deck to prevent tak ahab from entering play
            PutOnDeck("BitterCold");
            Card demolishedCamp = GetCard("DemolishedCamp");

            Card takAhab = PlayCard("TakAhab");
            PlayCard(demolishedCamp);
            AssertInPlayArea(northspar, demolishedCamp);

            // If Tak Ahab is destroyed, remove him from the game.
            DestroyCard(takAhab, haka.CharacterCard);
            AssertOutOfGame(takAhab);

            //should only affect Tak Ahab
            DestroyCard(mdp, haka.CharacterCard);
            AssertInTrash(mdp);


        }


    }
}
