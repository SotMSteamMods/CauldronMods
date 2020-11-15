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
            AssertIsInPlay(card);
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
            SetupGameController(new string[] {"BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs"});
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            GoToEndOfTurn(catacombs);
            Card initialRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            PrintSeparator("Go to next end of turn");
            //specifically selecting cursed vault/torture chamber as it doesn't introduce any additonal selectcard effects
            string identifier = "CursedVault";
            if(initialRoom.Identifier == "CursedVault")
            {
                identifier = "TortureChamber";
            }
            DecisionSelectCards = new Card[] { initialRoom, instructions.UnderLocation.Cards.Where((Card c) => c.Identifier == identifier).First() };


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
          
            SetupGameController(new string[] {"BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs"});
            StartGame();

            //set all hp so there is room to gain
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            SetHitPoints(new Card[] { baron.CharacterCard, mdp, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard }, 5);


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

            GoToPlayCardPhase(catacombs);

            //don't destroy aqueducts
            DecisionDoNotSelectCard = SelectionType.DestroyCard;
            GoToPlayCardPhase(catacombs);
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
            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();
            //change villain targets in play to make baron blade vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);
            Card battalion = PlayCard("BladeBattalion");

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


            //At the end of the environment turn, this card deals each target 2 psychic damage.
            PrintSeparator("check damage is dealt");
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            GoToEndOfTurn(catacombs);
            QuickHPCheck(-2, -2, -2, -2, -2);


        }

        [Test()]
        public void TestTortureChamber()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is TortureChamber in play
            if (playedRoom.Identifier != "TortureChamber")
            {
                DecisionSelectCard = GetCard("TortureChamber");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            Card mdp = GetCardInPlay("MobileDefensePlatform");


            //Increase damage dealt by villain targets by 1.
            PrintSeparator("check damage dealt by villains is increased by 1");
            Card[] targets = new Card[] { baron.CharacterCard, mdp, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard };

            int expectedDamage;
            foreach (Card source in targets)
            {
                QuickHPStorage(ra);

                if(source.IsVillainTarget)
                {
                    expectedDamage = -3;
                }
                else
                {
                    expectedDamage = -2;
                }
                
                DealDamage(source, ra.CharacterCard, 2, DamageType.Melee);
                QuickHPCheck(expectedDamage);
                
            }

        }

        [Test()]
        public void TestTwistingPassages_FewerThan2EnvironmentTargets()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is TwistingPassages in play
            if (playedRoom.Identifier != "TwistingPassages")
            {
                DecisionSelectCard = GetCard("TwistingPassages");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card envTarget = PlayCard("ScurryingEvil");


            //Increase damage dealt by environment cards by 1.
            //If there are fewer than 2 environment targets in play, increase damage dealt by hero targets by 1."
            //there is only 1 environment target, so no increase to heroes

            PrintSeparator("check damage dealt by environments is increased by 1");
            Card[] targets = new Card[] { baron.CharacterCard, mdp, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, envTarget };

            int expectedDamage;
            foreach (Card source in targets)
            {
                QuickHPStorage(ra);

                if (source.IsEnvironmentTarget)
                {
                    expectedDamage = -3;
                }
                else
                {
                    expectedDamage = -2;
                }

                DealDamage(source, ra.CharacterCard, 2, DamageType.Melee);
                QuickHPCheck(expectedDamage);

            }

        }

        [Test()]
        public void TestTwistingPassages_MoreThan2EnvironmentTargets()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is TwistingPassages in play
            if (playedRoom.Identifier != "TwistingPassages")
            {
                DecisionSelectCard = GetCard("TwistingPassages");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card envTarget1 = PlayCard("CoalKid");
            Card envTarget2 = PlayCard("Poltergeist");


            //Increase damage dealt by environment cards by 1.
            //If there are fewer than 2 environment targets in play, increase damage dealt by hero targets by 1."
            //there are 2 environment targets, so hero damage increased

            PrintSeparator("check damage dealt by environments is increased by 1");
            Card[] targets = new Card[] { baron.CharacterCard, mdp, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, envTarget1, envTarget2 };

            int expectedDamage;
            foreach (Card source in targets)
            {
                QuickHPStorage(ra);

                if (source.IsEnvironmentTarget || (source.IsHero && source.IsTarget))
                {
                    expectedDamage = -3;
                }
                else
                {
                    expectedDamage = -2;
                }

                DealDamage(source, ra.CharacterCard, 2, DamageType.Melee);
                QuickHPCheck(expectedDamage);

            }

        }

        [Test()]
        public void TestBreathStealNextTo()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Set Hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 25);
            SetHitPoints(haka.CharacterCard, 15);

            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is aqueducts in play
            if (playedRoom.Identifier != "Aqueducts")
            {
                DecisionSelectCard = GetCard("Aqueducts");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);

            //Play this card next to the hero with the lowest HP. That hero cannot regain HP.
            //haka is the lowest HP
            Card breath = PlayCard("BreathStealer");
            AssertNextToCard(breath, haka.CharacterCard);

            //try to gain HP
            QuickHPStorage(haka);
            PlayCard("VitalitySurge");
            QuickHPCheckZero();


        }

        [Test()]
        public void TestBreathStealerEndOfTurn()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Set Hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 25);
            SetHitPoints(haka.CharacterCard, 15);
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is aqueducts in play
            if (playedRoom.Identifier != "Aqueducts")
            {
                DecisionSelectCard = GetCard("Aqueducts");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            //haka is the lowest HP, so will be next to Haka
            Card breath = PlayCard("BreathStealer");
            AssertNextToCard(breath, haka.CharacterCard);
            //At the end of the environment turn, this card deals that hero 1 toxic damage.
            QuickHPStorage(haka);
            GoToEndOfTurn(catacombs);
            QuickHPCheck(-1);



        }

        [Test()]
        public void TestBreathStealer_AquaductsInPlay_Affected()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Set Hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 25);
            SetHitPoints(haka.CharacterCard, 15);
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is aqueducts in play
            if (playedRoom.Identifier != "Aqueducts")
            {
                DecisionSelectCard = GetCard("Aqueducts");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            //haka is the lowest HP, so will be next to Haka
            Card breath = PlayCard("BreathStealer");
            AssertNextToCard(breath, haka.CharacterCard);

            PrintSeparator("Check if breath stealer can be dealt damage from hero card");
            //This card may not be affected by hero cards unless Aqueducts is in play.
            QuickHPStorage(breath);
            DealDamage(ra.CharacterCard, breath, 1, DamageType.Fire);
            QuickHPCheck(-1);


        }

        [Test()]
        public void TestBreathStealer_AquaductsNotInPlay_NotAffected()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Set Hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 25);
            SetHitPoints(haka.CharacterCard, 15);

            //change villain targets in play to make baron blade vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is not aqueducts in play
            //putting torture chamber in play to simplify
            if (playedRoom.Identifier != "TortureChamber")
            {
                DecisionSelectCard = GetCard("TortureChamber");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            //haka is the lowest HP, so will be next to Haka
            Card breath = PlayCard("BreathStealer");
            AssertNextToCard(breath, haka.CharacterCard);

            PrintSeparator("Check if breath stealer can be dealt damage from hero card");
            //This card may not be affected by hero cards unless Aqueducts is in play.
            QuickHPStorage(breath);
            DealDamage(ra.CharacterCard, breath, 1, DamageType.Fire);
            QuickHPCheckZero();

            Card[] targets = new Card[] { baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, breath };
            
            QuickHPStorage(targets);
            DealDamage(ra.CharacterCard, (Card c) => c.IsTarget, 3, DamageType.Fire);
            QuickHPCheck(-3, -3, -3, -3, 0);




        }

        [Test()]
        public void TestCoalKid_TwistingPassagesInPlay_Affected()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is twisting passages in play
            if (playedRoom.Identifier != "TwistingPassages")
            {
                DecisionSelectCard = GetCard("TwistingPassages");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            
            Card kid = PlayCard("CoalKid");

            PrintSeparator("Check if coal kid can be dealt damage from hero card");
            //This card may not be affected by hero cards unless twisting passages is in play.
            QuickHPStorage(kid);
            DealDamage(ra.CharacterCard, kid, 1, DamageType.Fire);
            QuickHPCheck(-1);


        }

        [Test()]
        public void TestCoalKid_TwistingPassagesNotInPlay_NotAffected()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //change villain targets in play to make baron blade vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is not twisting passages in play
            //putting torture chamber in play to simplify
            if (playedRoom.Identifier != "TortureChamber")
            {
                DecisionSelectCard = GetCard("TortureChamber");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            Card kid = PlayCard("CoalKid");

            PrintSeparator("Check if coals kid can be dealt damage from hero card");
            //This card may not be affected by hero cards unless twisting passages is in play.
            QuickHPStorage(kid);
            DealDamage(ra.CharacterCard, kid, 1, DamageType.Fire);
            QuickHPCheckZero();

            Card[] targets = new Card[] { baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, kid };

            QuickHPStorage(targets);
            DealDamage(ra.CharacterCard, (Card c) => c.IsTarget, 3, DamageType.Fire);
            QuickHPCheck(-3, -3, -3, -3, 0);

        }

        [Test()]
        public void TestCoalKid_EndOfTurn()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //change villain targets in play to make baron blade vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is CursedVault in play for simplicity
            if (playedRoom.Identifier != "CursedVault")
            {
                DecisionSelectCard = GetCard("CursedVault");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            Card kid = PlayCard("CoalKid");

            //At the end of the environment turn, this card deals each hero target 2 fire damage.
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, kid);
            GoToEndOfTurn(catacombs);
            QuickHPCheck(0, -2, -2, -2, 0);


        }

        [Test()]
        public void TestDarkPassenger_CursedVaultInPlay_Affected()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
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
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;


            Card passenger = PlayCard("DarkPassenger");

            PrintSeparator("Check if dark passenger can be dealt damage from hero card");
            //This card may not be affected by hero cards unless cursed vault is in play.
            QuickHPStorage(passenger);
            DealDamage(ra.CharacterCard, passenger, 1, DamageType.Fire);
            QuickHPCheck(-1);


        }

        [Test()]
        public void TestDarkPassenger_CursedVaultNotInPlay_NotAffected()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //change villain targets in play to make baron blade vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is not cursed vault in play
            //putting torture chamber in play to simplify
            if (playedRoom.Identifier != "TortureChamber")
            {
                DecisionSelectCard = GetCard("TortureChamber");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            Card passenger = PlayCard("DarkPassenger");

            PrintSeparator("Check if dark passenger can be dealt damage from hero card");
            //This card may not be affected by hero cards unless dark passenger is in play.
            QuickHPStorage(passenger);
            DealDamage(ra.CharacterCard, passenger, 1, DamageType.Fire);
            QuickHPCheckZero();

            Card[] targets = new Card[] { baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, passenger };

            QuickHPStorage(targets);
            DealDamage(ra.CharacterCard, (Card c) => c.IsTarget, 3, DamageType.Fire);
            QuickHPCheck(-3, -3, -3, -3, 0);

        }

        [Test()]
        public void TestDarkPassengerNextTo()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Set Hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 25);
            SetHitPoints(haka.CharacterCard, 15);

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

            //Play this card next to the hero with the second highest HP. Reduce damage dealt by that hero by 1.
            //ra is the second highest
            Card passenger = PlayCard("DarkPassenger");
            AssertNextToCard(passenger, ra.CharacterCard);

            //try to have ra deal damage
            QuickHPStorage(haka);
            DealDamage(ra, haka, 5, DamageType.Fire);
            QuickHPCheck(-4);


        }

        [Test()]
        public void TestDarkPassengerEndOfTurn()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Set Hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 25);
            SetHitPoints(haka.CharacterCard, 15);

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
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            //ra is the second highest
            Card passenger = PlayCard("DarkPassenger");
            AssertNextToCard(passenger, ra.CharacterCard);

            //At the end of the environment turn, this card deals that hero 2 melee damage.
            QuickHPStorage(ra);
            GoToEndOfTurn(catacombs);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestLabyrinthGuideEnvironmentTurn_TwistingPassagesInPlay_DoNotTakeDamage()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //change villain targets in play to make baron blade vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is twisting passages in play
            if (playedRoom.Identifier != "TwistingPassages")
            {
                DecisionSelectCard = GetCard("TwistingPassages");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;
            DecisionYesNo = false;
            Card guide = PlayCard("LabyrinthGuide");
            GoToEndOfTurn(haka);
            //At the start of the environment turn, if Twisting Passages is not in play, this card deals each hero target 1 psychic damage or it is destroyed.
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            GoToStartOfTurn(catacombs);
            //since twisted passages is not in play, no damage should have been dealt
            QuickHPCheck(0, 0, 0, 0);
            AssertInPlayArea(catacombs, guide);
        }



        [Test()]
        public void TestLabyrinthGuideEnvironmentTurn_TwistingPassagesNotInPlay_TakeDamage()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();


            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is not twisting passages in play
            if (playedRoom.Identifier == "TwistingPassages")
            {
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            DecisionsYesNo = new bool[] { false, false, false, true };
            Card guide = PlayCard("LabyrinthGuide");
            GoToEndOfTurn(haka);
            //At the start of the environment turn, if Twisting Passages is not in play, this card deals each hero target 1 psychic damage or it is destroyed.
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            GoToStartOfTurn(catacombs);
            QuickHPCheck(0, -1, -1, -1);
            AssertInPlayArea(catacombs, guide);
        }

        [Test()]
        public void TestLabyrinthGuideEnvironmentTurn_TwistingPassagesNotInPlay_DontTakeDamage()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();


            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is not twisting passages in play
            if (playedRoom.Identifier == "TwistingPassages")
            {
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            DecisionsYesNo = new bool[] { false, false, false, false };
            Card guide = PlayCard("LabyrinthGuide");
            GoToEndOfTurn(haka);
            //At the start of the environment turn, if Twisting Passages is not in play, this card deals each hero target 1 psychic damage or it is destroyed.
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            GoToStartOfTurn(catacombs);
            QuickHPCheck(0, 0, 0, 0);
            AssertInTrash(guide);
        }

        [Test()]
        public void TestLabyrinthGuideHeroTurn()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();


            Card instructions = GetCardInPlay("StSimeonsCatacombs");


            GoToEndOfTurn(catacombs);
            Card initialRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            Card guide = PlayCard("LabyrinthGuide");

            //At the start of a hero's turn, that hero may discard 2 cards to destroy a Room in play.
            PrintSeparator("check hero start of turn effect");
            DecisionYesNo = true;
            QuickHandStorage(ra);
            GoToStartOfTurn(ra);
            QuickHandCheck(-2);

            Card newRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            Assert.IsTrue(initialRoom != newRoom, "A room was not destroyed");
            initialRoom = newRoom;
            PrintSeparator("check on each hero start of turn effect");
            QuickHandStorage(legacy);
            GoToStartOfTurn(legacy);
            QuickHandCheck(-2);

            newRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            Assert.IsTrue(initialRoom != newRoom, "A room was not destroyed");
        }

        [Test()]
        public void TestLabyrinthGuideHeroTurnOptional()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();


            Card instructions = GetCardInPlay("StSimeonsCatacombs");


            GoToEndOfTurn(catacombs);
            Card initialRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            Card guide = PlayCard("LabyrinthGuide");

            //At the start of a hero's turn, that hero may discard 2 cards to destroy a Room in play.
            PrintSeparator("check hero start of turn effect");
            DecisionYesNo = false;
            QuickHandStorage(ra);
            GoToStartOfTurn(ra);
            QuickHandCheck(0);

            Card newRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            Assert.IsTrue(initialRoom == newRoom, "A room was destroyed");
        }

        [Test()]
        public void TestLabyrinthGuideHeroTurnNoCards()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();


            Card instructions = GetCardInPlay("StSimeonsCatacombs");


            GoToEndOfTurn(catacombs);
            Card initialRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            Card guide = PlayCard("LabyrinthGuide");

            //At the start of a hero's turn, that hero may discard 2 cards to destroy a Room in play.
            PrintSeparator("check hero start of turn effect");
            DecisionYesNo = true;

            DiscardAllCards(ra);

            QuickHandStorage(ra);
            GoToStartOfTurn(ra);
            QuickHandCheck(0);

            Card newRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            Assert.IsTrue(initialRoom == newRoom, "A room was destroyed");
        }

        [Test()]
        public void TestLivingGeometry_Play()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();


            Card instructions = GetCardInPlay("StSimeonsCatacombs");


            GoToEndOfTurn(catacombs);
            Card initialRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            GoToPlayCardPhase(catacombs);

            //When this card enters play, destroy a room card. Its replacement is selected randomly from the 5 room cards, not chosen by the players.
            QuickShuffleStorage(instructions.UnderLocation);
            Card geometry = PlayCard("LivingGeometry");
            //should be 3 shuffles: once after instructions card moves it under itself, before this card plays a room and after it plays a room
            QuickShuffleCheck(3);
            AssertNoDecision();

        }

        [Test()]
        public void TestLivingGeometry_EndOfTurn()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            GoToEndOfTurn(catacombs);

            GoToPlayCardPhase(catacombs);

            Card topCard = PutOnDeck("LabyrinthGuide");
            
            Card geometry = PlayCard("LivingGeometry");
            //At the end of the environment turn, play the top card of the environment deck and destroy this card.
            GoToEndOfTurn(catacombs);
            AssertInPlayArea(catacombs, topCard);
            AssertInTrash(geometry);

        }

        [Test()]
        public void TestPanicNextTo()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Set Hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 25);
            SetHitPoints(haka.CharacterCard, 15);

            GoToEndOfTurn(catacombs);

            GoToPlayCardPhase(catacombs);

            //Play this next to the hero charcter with the highest HP.
            //legacy is the highest HP
            Card panic = PlayCard("Panic");
            AssertNextToCard(panic, legacy.CharacterCard);

        }

        [Test()]
        public void TestPanic_HeroStartOfTurn()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Set Hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 25);
            SetHitPoints(haka.CharacterCard, 15);

            GoToEndOfTurn(catacombs);

            GoToPlayCardPhase(catacombs);

            //panic attaches itself to legacy
            Card panic = PlayCard("Panic");

            //At the start of that hero's next turn, that hero uses their innate power twice, then immediately end their turn, draw a card, and destroy this card.
            QuickHandStorage(legacy);
            GoToStartOfTurn(legacy);
            //this start of phase should not have any actions allowed
            AssertPhaseActionCount(new int?(0));
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(legacy, Phase.End);
            QuickHandCheck(1);

            QuickHPStorage(ra);
            DealDamage(haka, ra, 2, DamageType.Melee);
            //galvanize should have been used twice
            QuickHPCheck(-4);

        }

        [Test()]
        public void TestPoltergeist_SacrificialShrineInPlay_Affected()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Set Hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 25);
            SetHitPoints(haka.CharacterCard, 15);
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is sacrificial shrine in play
            if (playedRoom.Identifier != "SacrificialShrine")
            {
                DecisionSelectCard = GetCard("SacrificialShrine");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            Card poltergeist = PlayCard("Poltergeist");

            PrintSeparator("Check if poltergeist can be dealt damage from hero card");
            //This card may not be affected by hero cards unless SacrificialShrine is in play.
            QuickHPStorage(poltergeist);
            DealDamage(ra.CharacterCard, poltergeist, 1, DamageType.Fire);
            QuickHPCheck(-1);


        }

        [Test()]
        public void TestPoltergeist_SacrificialShrineNotInPlay_NotAffected()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Set Hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 25);
            SetHitPoints(haka.CharacterCard, 15);

            //change villain targets in play to make baron blade vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is not SacrificialShrine in play
            //putting torture chamber in play to simplify
            if (playedRoom.Identifier != "TortureChamber")
            {
                DecisionSelectCard = GetCard("TortureChamber");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            Card poltergeist = PlayCard("Poltergeist");

            PrintSeparator("Check if poltergeist can be dealt damage from hero card");
            //This card may not be affected by hero cards unless SacrificialShrine is in play.
            QuickHPStorage(poltergeist);
            DealDamage(ra.CharacterCard, poltergeist, 1, DamageType.Fire);
            QuickHPCheckZero();

            Card[] targets = new Card[] { baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, poltergeist };

            QuickHPStorage(targets);
            DealDamage(ra.CharacterCard, (Card c) => c.IsTarget, 3, DamageType.Fire);
            QuickHPCheck(-3, -3, -3, -3, 0);

        }

        [Test()]
        public void TestPoltergeist_EndOfTurn()
        {

            SetupGameController(new string[] { "BaronBlade", "TheWraith", "Stuntman", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();


            //change villain targets in play to make baron blade vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            Card targetting = GetCard("MicroTargetingComputer");

            //make sure it is not SacrificialShrine in play
            //putting torture chamber in play to simplify
            if (playedRoom.Identifier != "TortureChamber")
            {
                DecisionSelectCards =  new Card[] { GetCard("TortureChamber"), targetting };
                DestroyCard(playedRoom, haka.CharacterCard);
            } else
            {
                DecisionSelectCards = new Card[] { targetting };
            }
            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            Card poltergeist = PlayCard("Poltergeist");

            //put 2 wraith equipments, 1 stuntman equipment, and 0 haka equipment in play
            PlayCard("RazorOrdnance");
            PlayCard(targetting);
            PlayCard("LanceFlammes");

            //At the end of the environment turn, this card deals each hero 1 projectile damage for each equipment card they have in play. Then, destroy 1 equipment card.
            QuickHPStorage(baron, wraith, stunt, haka);
            int numEquipmentInPlayBefore = GetNumberOfCardsInPlay((Card c) => base.GameController.IsEquipment(c) && c.IsHero);
            GoToEndOfTurn(catacombs);
            QuickHPCheck(0, -2, -1, 0);
            AssertNumberOfCardsInPlay((Card c) => base.GameController.IsEquipment(c) && c.IsHero, numEquipmentInPlayBefore - 1);
        }

        [Test()]
        public void TestPoltergeist_EndOfTurn_FirstDamageIncaps()
        {

            SetupGameController(new string[] { "BaronBlade", "TheWraith", "Stuntman", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            SetHitPoints(wraith, 1);

            //change villain targets in play to make baron blade vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            Card lance = GetCard("LanceFlammes");

            //make sure it is not SacrificialShrine in play
            //putting torture chamber in play to simplify
            if (playedRoom.Identifier != "TortureChamber")
            {
                DecisionSelectCards = new Card[] { GetCard("TortureChamber"), lance };
                DestroyCard(playedRoom, haka.CharacterCard);
            }

            DecisionSelectCards = new Card[] { lance };
            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            Card poltergeist = PlayCard("Poltergeist");

            //put 2 wraith equipments, 1 stuntman equipment, and 0 haka equipment in play
            PlayCard("RazorOrdnance");
            PlayCard("MicroTargetingComputer");
            PlayCard(lance);

            //At the end of the environment turn, this card deals each hero 1 projectile damage for each equipment card they have in play. Then, destroy 1 equipment card.
            QuickHPStorage(baron, stunt, haka);
            int numEquipmentInPlayBefore = GetNumberOfCardsInPlay((Card c) => base.GameController.IsEquipment(c) && c.IsHero);
            GoToEndOfTurn(catacombs);
            QuickHPCheck(0, -1, 0);
            AssertIncapacitated(wraith);
            //all of wraith's equipment disappeared when she was incapped + stuntman's being destroyed
            AssertNumberOfCardsInPlay((Card c) => base.GameController.IsEquipment(c) && c.IsHero, numEquipmentInPlayBefore - 3);
        }

        [Test()]
        public void TestPossessor_TortureChamberInPlay_Affected()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Set Hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 25);
            SetHitPoints(haka.CharacterCard, 15);
            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is Torture Chamber in play
            if (playedRoom.Identifier != "TortureChamber")
            {
                DecisionSelectCard = GetCard("TortureChamber");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            Card possessor = PlayCard("Possessor");

            PrintSeparator("Check if possessor can be dealt damage from hero card");
            //This card may not be affected by hero cards unless Torture Chamber is in play.
            QuickHPStorage(possessor);
            DealDamage(ra.CharacterCard, possessor, 1, DamageType.Fire);
            QuickHPCheck(-1);


        }

        [Test()]
        public void TestPossessor_TortureChamberNotInPlay_NotAffected()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Set Hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 25);
            SetHitPoints(haka.CharacterCard, 15);

            //change villain targets in play to make baron blade vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card instructions = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is not Torture Chamber in play
            //putting aqueducts in play to simplify
            if (playedRoom.Identifier != "Aqueducts")
            {
                DecisionSelectCard = GetCard("Aqueducts");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            Card possessor = PlayCard("Possessor");

            PrintSeparator("Check if Possessor can be dealt damage from hero card");
            //This card may not be affected by hero cards unless Torture Chamber is in play.
            QuickHPStorage(possessor);
            DealDamage(ra.CharacterCard, possessor, 1, DamageType.Fire);
            QuickHPCheckZero();

            Card[] targets = new Card[] { baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, possessor };

            QuickHPStorage(targets);
            DealDamage(ra.CharacterCard, (Card c) => c.IsTarget, 3, DamageType.Fire);
            QuickHPCheck(-3, -3, -3, -3, 0);

        }
    }
}
