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
    }
}
