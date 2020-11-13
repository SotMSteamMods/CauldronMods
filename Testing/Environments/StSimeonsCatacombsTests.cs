using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Cauldron.StSimeonsCatacombs;

namespace CauldronTests
{
    [TestFixture()]
    public class StSimeonsCatacombsTests : BaseTest
    {

        #region StSimeonsCatacombsHelperFunctions

        protected TurnTakerController catacombs { get { return FindEnvironment(); } }
        private bool IsDefinitionRoom(Card card)
        {
            return card != null && card.Definition.Keywords.Contains("room");
        }

        #endregion

        [Test()]
        [Sequential]
        public void DecklistTest_Room_IsRoom([Values("TwistingPassages", "SacrificialShrine", "TortureChamber", "CursedVault", "Aqueducts")] string room)
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Fanatic", "Cauldron.StSimeonsCatacombs");
            StartGame();

            //flip instructions card so cards can be played
            Card instructions = GetCardInPlay("StSimeonsCatacombs");
            FlipCard(instructions);

            GoToPlayCardPhase(catacombs);

            Card card = MoveCard(catacombs, room, catacombs.TurnTaker.PlayArea);
            AssertInPlayArea(catacombs, card);
            AssertCardHasKeyword(card, "room", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Ghost_IsGhost([Values("TerriblePresence", "DarkPassenger", "Possessor", "ScurryingEvil", "CoalKid", "BreathStealer", "Poltergeist", "LabyrinthGuide")] string ghost)
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Fanatic", "Cauldron.StSimeonsCatacombs");
            StartGame();

            //flip instructions card so cards can be played
            Card instructions = GetCardInPlay("StSimeonsCatacombs");
            FlipCard(instructions);

            GoToPlayCardPhase(catacombs);

            Card card = PlayCard(ghost);
            AssertInPlayArea(catacombs, card);
            AssertCardHasKeyword(card, "ghost", false);
        }

        [Test()]
        public void TestCatacombsWorks()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

        }

        [Test()]
        public void TestCatacombsStartOfGame_MoveRoomsUnder()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");
            List<Card> roomCards = catacombs.TurnTaker.Deck.Cards.Where(c => c.IsRoom).ToList();
            List<Card> nonRoomCards = catacombs.TurnTaker.Deck.Cards.Where(c => !c.IsRoom).ToList();


            //check that all rooms have been moved to under the instructions card
            foreach (Card c in roomCards)
            {
                AssertUnderCard(instructions, c);
                AssertDoesNotHaveGameText(c);

                //check that indestructible
                DestroyCard(c, haka.CharacterCard);
                AssertNumberOfCardsInTrash(catacombs, 0);

            }

            //check that all non-rooms are still in the deck
            foreach (Card c in nonRoomCards)
            {
                AssertInDeck(catacombs, c);
                AssertHasGameText(c);
            }

        }


        [Test()]
        public void TestCatacombsCheckIndestructibleAndNoGameTextWhenFlipped()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");
            List<Card> roomCards = catacombs.TurnTaker.Deck.Cards.Where(c => c.IsRoom).ToList();

            FlipCard(instructions);

            //check that all rooms have been moved to under the instructions card
            foreach (Card c in roomCards)
            {
                AssertUnderCard(instructions, c);
                AssertDoesNotHaveGameText(c);

                //check that indestructible
                DestroyCard(c, haka.CharacterCard);
                AssertNumberOfCardsInTrash(catacombs, 0);

            }

        }

        [Test()]
        public void TestCatacombsFrontEndOfTurn_PutRoomInPlay()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            GoToEndOfTurn(catacombs);

            Card playedRoom = catacombs.TurnTaker.PlayArea.Cards.Where(c => c != instructions).FirstOrDefault();
            AssertHasGameText(playedRoom);
            AssertDoesNotHaveGameText(instructions);
            AssertNumberOfCardsInPlay((Card c) => catacombs.TurnTaker.PlayArea.Cards.Contains(c),1);
            AssertNumberOfCardsUnderCard(instructions, 4);
            AssertCardHasKeyword(playedRoom, "room", false);

        }

        [Test()]
        public void TestCatacombsFrontEndOfTurn_Flip()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");
            GoToEndOfTurn(catacombs);

            AssertFlipped(instructions);

        }

        [Test()]
        public void TestCatacombsCantPlayCardsOnFront()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");
            Card testCard = GetCard("CoalKid");
            AssertNotFlipped(instructions);
            AssertCannotPlayCards(catacombs, testCard);

        }

        [Test()]
        public void TestCatacombsCantPlayCardsOnFront_FlippedBack()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");
            Card testCard = GetCard("CoalKid");
            FlipCard(instructions);
            FlipCard(instructions);
            AssertNotFlipped(instructions);
            AssertCannotPlayCards(catacombs, testCard);

        }

        [Test()]
        public void TestCatacombsCanPlayCardsOnBack()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");
            FlipCard(instructions);
            AssertFlipped(instructions);
            AssertCanPlayCards(catacombs);

        }


        [Test()]
        public void TestCatacombsCanOnRoomDestroy_MoveRoom()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            GoToEndOfTurn(catacombs);

            PrintSeparator("Destroy Room");
            Card playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            //Whenever a room card would leave play, instead place it face up beneath this card.
            DestroyCard(playedRoom, ra.CharacterCard);
            AssertUnderCard(instructions, playedRoom);

        }

        [Test()]
        public void TestCatacombsCanOnRoomDestroy_ChooseNewRoom()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            GoToEndOfTurn(catacombs);
            Card playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            PrintSeparator("Destroy room in play to trigger next room coming out");
            //Then choose a different room beneath this card and put it into play.
            DestroyCard(playedRoom, haka.CharacterCard);

            Card newRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            Assert.IsTrue(playedRoom != newRoom, "The same room was played out as before");

        }

        [Test()]
        public void TestCatacombsIndestructibleOn3Changes()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            GoToEndOfTurn(catacombs);
            Card playedRoom;

            GoToStartOfTurn(legacy);
            PrintSeparator("Destroy Room 1");
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            //Whenever a room card would leave play, instead place it face up beneath this card.
            DestroyCard(playedRoom, ra.CharacterCard);
            AssertUnderCard(instructions, playedRoom);

            PrintSeparator("Destroy Room 2");
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            //Whenever a room card would leave play, instead place it face up beneath this card.
            DestroyCard(playedRoom, ra.CharacterCard);
            AssertUnderCard(instructions, playedRoom);

            PrintSeparator("Destroy Room 3");
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            //Whenever a room card would leave play, instead place it face up beneath this card.
            DestroyCard(playedRoom, ra.CharacterCard);
            AssertUnderCard(instructions, playedRoom);

            PrintSeparator("Rooms should be indestructible");
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            //Whenever a room card would leave play, instead place it face up beneath this card.
            DestroyCard(playedRoom, ra.CharacterCard);
            AssertInPlayArea(catacombs, playedRoom);

            PrintSeparator("Able to be destroyed in the next turn");
            GoToNextTurn();
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            //Whenever a room card would leave play, instead place it face up beneath this card.
            DestroyCard(playedRoom, ra.CharacterCard);
            AssertUnderCard(instructions, playedRoom);

        }

        [Test()]
        public void TestCatacombsBackEndOfTurnFreeRoom()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            GoToEndOfTurn(catacombs);
            Card initialRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            PrintSeparator("Go to next end of turn");
            //specifically selecting cursed vault as it doesn't introduce any additonal selectcard effects
            DecisionSelectCards = new Card[] { initialRoom, instructions.UnderLocation.Cards.Where((Card c) => c.Identifier != "CursedVault").First() };


            GoToEndOfTurn(catacombs);

            Card newRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            AssertUnderCard(instructions, initialRoom);
            Assert.IsTrue(initialRoom != newRoom, "A new room did not come out");


        }

        [Test()]
        public void TestCatacombsBackEndOfTurnFreeRoom_Optional()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            GoToEndOfTurn(catacombs);
            Card initialRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            PrintSeparator("Go to next end of turn");
            DecisionDoNotSelectCard = SelectionType.DestroyCard;


            AssertInPlayArea(catacombs, initialRoom);


        }

        [Test()]
        public void TestCatacombsBackEndOfTurnFreeRoom_OnlyWhenNoRoomsChanges()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            PrintSeparator("Destroy a room this turn");
            GoToStartOfTurn(catacombs);
            PrintSeparator("Destroy Room 1");
            //Whenever a room card would leave play, instead place it face up beneath this card.
            DestroyCard(playedRoom, ra.CharacterCard);
            AssertUnderCard(instructions, playedRoom);

            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            PrintSeparator("Go to next end of turn");
            GoToEndOfTurn(catacombs);
            //no choice to destroy this card
            AssertInPlayArea(catacombs, playedRoom);


        }

        [Test()]
        public void TestAqueducts()
        {
            //failing random seed 1963543475
            //seems that sometimes aqueducts effect doesn't go off
            SetupGameController(new string[] {"BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs"});
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is aqueducts in play
            if(playedRoom.Identifier != "Aqueducts")
            {
                DecisionSelectCard = GetCard("Aqueducts");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            //don't destroy aqueducts
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            GoToPlayCardPhase(catacombs);
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            //set all hp so there is room to gain
            SetHitPoints(new Card[] { baron.CharacterCard, mdp, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard }, 5);


            //At the end of the environment turn, each target regains 1 HP.
            PrintSeparator("check all targets regain 1 HP");
            QuickHPStorage(baron.CharacterCard, mdp, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(catacombs);
            QuickHPCheck(1, 1, 1, 1, 1);


        }

        [Test()]
        public void TestCursedVault()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is cursed vault in play
            if (playedRoom.Identifier != "CursedVault")
            {
                DecisionSelectCard = GetCard("CursedVault");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //change villain targets in play to make baron blade vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);
            Card battalion = PlayCard("BladeBattalion");

            //Reduce damage dealt to villain targets by 1.
            PrintSeparator("check damage dealt to villains is reduced by 1");
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard );
            DealDamage(ra.CharacterCard, (Card c) => c.IsTarget, 3, DamageType.Fire);
            //only the damage to villains should have been reduced
            QuickHPCheck(-2, -2, -3, -3, -3);


        }

        [Test()]
        public void TestSacrificialShrine()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is SacrificialShrine in play
            if (playedRoom.Identifier != "SacrificialShrine")
            {
                DecisionSelectCard = GetCard("SacrificialShrine");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            //don't destroy SacrificialShrine
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            GoToPlayCardPhase(catacombs);
            //change villain targets in play to make baron blade vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);
            Card battalion = PlayCard("BladeBattalion");

            //At the end of the environment turn, this card deals each target 2 psychic damage.
            PrintSeparator("check damage is dealt");
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(catacombs);
            QuickHPCheck(-2, -2, -2, -2, -2);


        }
    }
}
