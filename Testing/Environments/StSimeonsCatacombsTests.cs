using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using Cauldron.StSimeonsCatacombs;
using System;

namespace CauldronTests
{
    [TestFixture()]
    public class StSimeonsCatacombsTests : CauldronBaseTest
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

            //flip catacomb card so cards can be played
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");
            Card instructions = GetCard("StSimeonsCatacombsInstructions");
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

            //flip catacomb card so cards can be played
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");
            Card instructions = GetCard("StSimeonsCatacombsInstructions");
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
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");
            List<Card> roomCards = catacombs.TurnTaker.Deck.Cards.Where(c => c.IsRoom).ToList();
            List<Card> nonRoomCards = catacombs.TurnTaker.Deck.Cards.Where(c => !c.IsRoom).ToList();


            //check that all rooms have been moved to under the catacomb card
            foreach (Card c in roomCards)
            {
                AssertUnderCard(catacomb, c);
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
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");
            List<Card> roomCards = catacombs.TurnTaker.Deck.Cards.Where(c => c.IsRoom).ToList();

            FlipCard(catacomb);

            //check that all rooms have been moved to under the catacomb card
            foreach (Card c in roomCards)
            {
                AssertUnderCard(catacomb, c);
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
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

            GoToEndOfTurn(catacombs);

            Card playedRoom = catacombs.TurnTaker.PlayArea.Cards.Where(c => c != catacomb && c.IsRealCard).FirstOrDefault();
            AssertHasGameText(playedRoom);
            AssertNumberOfCardsInPlay((Card c) => catacombs.TurnTaker.PlayArea.Cards.Contains(c),2);
            AssertNumberOfCardsUnderCard(catacomb, 4);
            AssertCardHasKeyword(playedRoom, "room", false);

        }

        [Test()]
        public void TestCatacombsFrontEndOfTurn_Flip()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCard("StSimeonsCatacombsInstructions");
            GoToEndOfTurn(catacombs);

            AssertFlipped(instructions);

        }

        [Test()]
        public void TestCatacombsCantPlayCardsOnFront()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCard("StSimeonsCatacombsInstructions");
            Card testCard = GetCard("CoalKid");
            AssertNotFlipped(instructions);
            AssertCannotPlayCards(catacombs, testCard);

        }

        //[Test()]
        //public void TestCatacombsCantPlayCardsOnFront_FlippedBack()
        //{
        //    SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
        //    StartGame();
        //    Card instructions = GetCard("StSimeonsCatacombsInstructions"); Card testCard = GetCard("CoalKid");
        //    FlipCard(instructions);
        //    FlipCard(instructions);
        //    AssertNotFlipped(instructions);
        //    AssertCannotPlayCards(catacombs, testCard);

        //}

        [Test()]
        public void TestCatacombsCanPlayCardsOnBack()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card instructions = GetCard("StSimeonsCatacombsInstructions"); 
            FlipCard(instructions);
            AssertFlipped(instructions);
            AssertCanPlayCards(catacombs);

        }


        [Test()]
        public void TestCatacombsCanOnRoomDestroy_MoveRoom()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

            GoToEndOfTurn(catacombs);

            PrintSeparator("Destroy Room");
            Card playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            //Whenever a room card would leave play, instead place it face up beneath this card.
            DestroyCard(playedRoom, ra.CharacterCard);
            AssertUnderCard(catacomb, playedRoom);

        }

        [Test()]
        public void TestCatacombsCanOnRoomDestroy_ChooseNewRoom()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

            GoToEndOfTurn(catacombs);
            Card playedRoom;

            GoToStartOfTurn(legacy);
            PrintSeparator("Destroy Room 1");
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            //Whenever a room card would leave play, instead place it face up beneath this card.
            DestroyCard(playedRoom, ra.CharacterCard);
            AssertUnderCard(catacomb, playedRoom);

            PrintSeparator("Destroy Room 2");
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            //Whenever a room card would leave play, instead place it face up beneath this card.
            DestroyCard(playedRoom, ra.CharacterCard);
            AssertUnderCard(catacomb, playedRoom);

            PrintSeparator("Destroy Room 3");
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            //Whenever a room card would leave play, instead place it face up beneath this card.
            DestroyCard(playedRoom, ra.CharacterCard);
            AssertUnderCard(catacomb, playedRoom);

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
            AssertUnderCard(catacomb, playedRoom);

        }

        [Test()]
        public void TestCatacombsBackEndOfTurnFreeRoom()
        {
            SetupGameController(new string[] {"BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs"});
            StartGame();
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

            GoToEndOfTurn(catacombs);
            Card initialRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            PrintSeparator("Go to next end of turn");
            //specifically selecting cursed vault/torture chamber as it doesn't introduce any additonal selectcard effects
            string identifier = "CursedVault";
            if(initialRoom.Identifier == "CursedVault")
            {
                identifier = "TortureChamber";
            }
            DecisionSelectCards = new Card[] { initialRoom, catacomb.UnderLocation.Cards.Where((Card c) => c.Identifier == identifier).First() };


            GoToEndOfTurn(catacombs);
            
            Card newRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            AssertUnderCard(catacomb, initialRoom);
            Assert.IsTrue(initialRoom != newRoom, "A new room did not come out");


        }

        [Test()]
        public void TestCatacombsBackEndOfTurnFreeRoom_Optional()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            PrintSeparator("Destroy a room this turn");
            GoToStartOfTurn(catacombs);
            PrintSeparator("Destroy Room 1");
            //Whenever a room card would leave play, instead place it face up beneath this card.
            DestroyCard(playedRoom, ra.CharacterCard);
            AssertUnderCard(catacomb, playedRoom);

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


            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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
            //At the start of the villain turn, each target regains 1 HP.
            PrintSeparator("check all targets regain 1 HP");
            QuickHPStorage(baron.CharacterCard, mdp, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);

            GoToStartOfTurn(baron);

            QuickHPCheck(1, 1, 1, 1, 1);


        }

        [Test()]
        public void TestCursedVault()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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
            //there is only 1 environment target, so increase to heroes

            PrintSeparator("check damage dealt by environments is increased by 1");
            Card[] targets = new Card[] { baron.CharacterCard, mdp, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, envTarget };

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
        public void TestTwistingPassages_MoreThan2EnvironmentTargets()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs");
            StartGame();
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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
            Card envTarget2 = PlayCard("ScurryingEvil");


            //Increase damage dealt by environment cards by 1.
            //If there are fewer than 2 environment targets in play, increase damage dealt by hero targets by 1."
            //there are 2 environment targets, so hero damage is not increased

            PrintSeparator("check damage dealt by environments is increased by 1");
            Card[] targets = new Card[] { baron.CharacterCard, mdp, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, envTarget1, envTarget2 };

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
        public void TestBreathStealer_Oblivaeon_0Heroes()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "Cauldron.StSimeonsCatacombs", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Play this card next to a hero.
            //since there are no heroes in this battlezone, it should go to the trash
            PlayCard("InjuredWorker");

            GoToEndOfTurn(envTwo);
            Card breath = PlayCard("BreathStealer");
            AssertInTrash(breath);

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

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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
        public void TestCoalKid_TwistingPassagesInPlay_NotAffected()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();
            //change villain targets in play to make baron blade vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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

            PrintSeparator("Check if coals kid can be dealt damage from hero card");
            //This card may not be affected by hero cards unless twisting passages is in play.
            QuickHPStorage(kid);
            DealDamage(ra.CharacterCard, kid, 1, DamageType.Fire);
            QuickHPCheckZero();

            Card[] targets = new Card[] { baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, kid };

            //all damage is increased by twisting passages
            QuickHPStorage(targets);
            DealDamage(ra.CharacterCard, (Card c) => c.IsTarget, 3, DamageType.Fire);
            QuickHPCheck(-4, -4, -4, -4, 0);
           


        }

        [Test()]
        public void TestCoalKid_TwistingPassagesNotInPlay_Affected()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //change villain targets in play to make baron blade vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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

            PrintSeparator("Check if coal kid can be dealt damage from hero card");
            //This card may not be affected by hero cards if twisting passages is in play.
            QuickHPStorage(kid);
            DealDamage(ra.CharacterCard, kid, 1, DamageType.Fire);
            QuickHPCheck(-1);


        }

        [Test()]
        public void TestCoalKid_EndOfTurn()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //change villain targets in play to make baron blade vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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
        public void TestDarkPassenger_Oblivaeon_0Heroes()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "Cauldron.StSimeonsCatacombs", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Play this card next to a hero.
            //since there are no heroes in this battlezone, it should go to the trash

            GoToEndOfTurn(envTwo);
            Card passenger = PlayCard("DarkPassenger");
            AssertInTrash(passenger);

        }

        [Test()]
        public void TestDarkPassenger_Oblivaeon_1Hero()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "Cauldron.StSimeonsCatacombs", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();

            SwitchBattleZone(ra);
            //Play this card next to a hero.
            //since there are no heroes in this battlezone, it should go to the trash

            GoToEndOfTurn(envTwo);
            Card passenger = PlayCard("DarkPassenger");
            AssertInTrash(passenger);

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

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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


            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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
        public void TestLabyrinthGuideEnvironmentTurn_TwistingPassagesNotInPlay_NoAvailableHeroes()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "Cauldron.StSimeonsCatacombs", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(envTwo);
            playedRoom = FindCard((Card c) => c.IsRoom && envTwo.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is not twisting passages in play
            if (playedRoom.Identifier == "TwistingPassages")
            {
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToEndOfTurn(scionTwo);
            Card guide = PlayCard("LabyrinthGuide");
            QuickHPStorage(ra, legacy, haka, tachyon, luminary);
            GoToStartOfTurn(envTwo);
            QuickHPCheckZero();
            AssertInTrash(envTwo, guide);
        }

        [Test()]
        public void TestLabyrinthGuideEnvironmentTurn_TwistingPassagesNotInPlay_DontTakeDamage()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();


            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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


            Card catacomb = GetCardInPlay("StSimeonsCatacombs");


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


            Card catacomb = GetCardInPlay("StSimeonsCatacombs");


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


            Card catacomb = GetCardInPlay("StSimeonsCatacombs");


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


            Card catacomb = GetCardInPlay("StSimeonsCatacombs");


            GoToEndOfTurn(catacombs);
            Card initialRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            GoToPlayCardPhase(catacombs);

            //When this card enters play, destroy a room card. Its replacement is selected randomly from the 5 room cards, not chosen by the players.
            QuickShuffleStorage(catacomb.UnderLocation);
            Card geometry = PlayCard("LivingGeometry");
            //should be 3 shuffles: once after catacomb card moves it under itself, before this card plays a room and after it plays a room
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
        public void TestPanic_Oblivaeon_0Heroes()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "Cauldron.StSimeonsCatacombs", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Play this card next to a hero.
            //since there are no heroes in this battlezone, it should go to the trash

            GoToEndOfTurn(envTwo);
            Card panic = PlayCard("Panic");
            AssertInTrash(panic);

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
            Card currentRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            DealDamage(haka, ra, 2, DamageType.Melee);
            //galvanize should have been used twice
            if(currentRoom.Identifier == "TwistingPassages")
            {
                QuickHPCheck(-5);
            } else
            {
                QuickHPCheck(-4);

            }

        }

        [Test()]
        public void TestPanic_HeroStartOfTurn_HeroDestroyedMidTurn()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Set Hitpoints to start
            SetHitPoints(ra.CharacterCard, 2);
            SetHitPoints(legacy.CharacterCard, 1);
            SetHitPoints(haka.CharacterCard, 1);

            GoToPlayCardPhase(catacombs);
            AddImmuneToDamageTrigger(env, true, false, cardSource: ra.CharacterCardController.GetCardSource());

            GoToEndOfTurn(catacombs);
            Card playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is Torture Chamber in play to avoid shenanigans
            if (playedRoom.Identifier != "TortureChamber")
            {
                DecisionSelectCards = new Card[] { GetCard("TortureChamber"), null, null,  ra.CharacterCard, ra.CharacterCard };
                DestroyCard(playedRoom, ra.CharacterCard);
            } else
            {
                DecisionSelectCards = new Card[] {  null, null,  ra.CharacterCard, ra.CharacterCard };
            }

            GoToPlayCardPhase(catacombs);

            //panic attaches itself to legacy
            Card panic = PlayCard("Panic");

            //At the start of that hero's next turn, that hero uses their innate power twice, then immediately end their turn, draw a card, and destroy this card.
            GoToStartOfTurn(ra);
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(ra, Phase.End);

        }

        [Test()]
            public void TestPanic_HeroStartOfTurn_DestroyInMiddle()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "SkyScraper/ExtremistSkyScraperNormal", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Set Hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(sky.CharacterCard, 25);
            SetHitPoints(haka.CharacterCard, 15);

            //switch to huge
            PlayCard("ThorathianMonolith");
            GoToEndOfTurn(catacombs);

            GoToPlayCardPhase(catacombs);

            //panic attaches itself to skyscraper
            Card panic = PlayCard("Panic");

            //At the start of that hero's next turn, that hero uses their innate power twice, then immediately end their turn, draw a card, and destroy this card.
            GoToEndOfTurn(ra);
            QuickHandStorage(sky);
            DecisionSelectCard = panic;
            GoToStartOfTurn(sky);
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(sky, Phase.PlayCard);


        }
        [Test()]
        public void TestPanic_HeroStartOfTurn_OnCharacterWithMultiplePowers()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Guise/SantaGuise", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Set Hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 10);
            SetHitPoints(guise.CharacterCard, 24);

            GoToEndOfTurn(catacombs);

            GoToPlayCardPhase(catacombs);

            //panic attaches itself to guise
            Card panic = PlayCard("Panic");

            //At the start of that hero's next turn, that hero uses their innate power twice, then immediately end their turn, draw a card, and destroy this card.
            QuickHandStorage(guise);
            DecisionSelectPowerIndex = 1;
            DecisionSelectPower = guise.CharacterCard;
            GoToStartOfTurn(guise);
            //this start of phase should not have any actions allowed
            AssertPhaseActionCount(new int?(0));
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(guise, Phase.End);
            QuickHandCheck(1);

            AssertNotGameOver();

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
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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

           

            //change villain targets in play to make baron blade vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;
            SetHitPoints(wraith, 3);
            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            //account for if Sacrificial Shrine was the initial room
            if(playedRoom.Identifier != "SacrificialShrine")
            {
                SetHitPoints(wraith, 1);
            }
            Card lance = GetCard("LanceFlammes");

            //make sure it is not SacrificialShrine in play
            //putting torture chamber in play to simplify
            if (playedRoom.Identifier != "TortureChamber")
            {
                DecisionSelectCards = new Card[] { GetCard("TortureChamber"), lance };
                DestroyCard(playedRoom, haka.CharacterCard);
            }
            else
            {
                DecisionSelectCards = new Card[] { lance };
            }

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
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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

        [Test()]
        public void TestPossessor_Oblivaeon_0Heroes()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "Cauldron.StSimeonsCatacombs", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Play this card next to the hero with the most cards in hand.
            //since there are no heroes in this battlezone, it should go to the trash

            GoToEndOfTurn(envTwo);
            Card possessor = PlayCard("Possessor");
            AssertInTrash(possessor);

        }

        [Test()]
        public void TestPossessor_Oblivaeon_HeroWithIncapInZone()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "Cauldron.StSimeonsCatacombs", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();

            SwitchBattleZone(ra);
            Card raCharacterCard = ra.CharacterCard;
            DealDamage(oblivaeon, ra, 100, DamageType.Fire, isIrreducible: true, ignoreBattleZone: true);
            GoToAfterEndOfTurn(oblivaeon);
            DecisionSelectFromBoxIdentifiers = new string[] { "TheSentinels" };
            DecisionSelectFromBoxTurnTakerIdentifier = "TheSentinels";
            RunActiveTurnPhase();

            DrawCard(sentinels, numberOfCards: 3);

            //Play this card next to the hero with the most cards in hand.
            //knight is the most number of cards, possessor should be next to knight
            //ra is incapped in zone, so it should be ignored

            GoToEndOfTurn(envTwo);
            AssertNextDecisionChoices(notIncluded: new List<Card> { raCharacterCard });
            DecisionSelectCard = writhe;
            Card possessor = PlayCard("Possessor");
            AssertNextToCard(possessor, writhe);

        }

        [Test()]
        public void TestPossessorNextTo()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Draw an extra card for Haka
             DrawCard(haka);
            

            GoToEndOfTurn(catacombs);

            GoToPlayCardPhase(catacombs);

            //Play this card next to the hero with the most cards in hand.
            //haka has the most cards in play
            Card possessor = PlayCard("Possessor");
            AssertNextToCard(possessor, haka.CharacterCard);

        }

        [Test()]
        public void TestPossessorNextTo_NoPowersOrPlays()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Draw an extra card for Haka
            DrawCard(haka);


            GoToEndOfTurn(catacombs);

            GoToPlayCardPhase(catacombs);

            //Play this card next to the hero with the most cards in hand.
            //haka has the most cards in play
            Card possessor = PlayCard("Possessor");
            AssertNextToCard(possessor, haka.CharacterCard);

            //That hero may not play cards or use powers.
            AssertCannotPlayCards(haka);
            AssertNotUsablePower(haka, haka.CharacterCard);

            //check that others can still play cards and use powers
            AssertCanPlayCards(ra);
            AssertCanPlayCards(legacy);
            AssertCanPlayCards(baron);
            AssertCanPlayCards(catacombs);
            AssertUsablePower(ra, ra.CharacterCard);
            AssertUsablePower(legacy, legacy.CharacterCard);

        }

        [Test()]
        public void TestPossessorStartOfHero_2CardsInHand()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Draw an extra card for Haka
            DrawCard(haka);



            GoToEndOfTurn(catacombs);

            GoToPlayCardPhase(catacombs);

            //Play this card next to the hero with the most cards in hand.
            //haka has the most cards in play
            Card possessor = PlayCard("Possessor");

            //set up haka's hand
            DiscardAllCards(haka);
            Card haka1 = PutInHand("Mere");
            Card haka2 = PutInHand("PunishTheWeak");

            //At the start of that hero's turn, put 2 cards from their hand into play at random.
            GoToStartOfTurn(haka);
            AssertInPlayArea(haka, haka1);
            AssertInPlayArea(haka, haka2);

        }

        [Test()]
        public void TestPossessorStartOfHero_1CardInHand()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Draw an extra card for Haka
            DrawCard(haka);



            GoToEndOfTurn(catacombs);

            GoToPlayCardPhase(catacombs);

            //Play this card next to the hero with the most cards in hand.
            //haka has the most cards in play
            Card possessor = PlayCard("Possessor");

            //set up haka's hand
            DiscardAllCards(haka);
            Card haka1 = PutInHand("Mere");


            //At the start of that hero's turn, put 2 cards from their hand into play at random.
            GoToStartOfTurn(haka);
            AssertInPlayArea(haka, haka1);

        }

        [Test()]
        public void TestPossessorStartOfHero_0CardsInHand()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Draw an extra card for Haka
            DrawCard(haka);



            GoToEndOfTurn(catacombs);

            GoToPlayCardPhase(catacombs);

            //Play this card next to the hero with the most cards in hand.
            //haka has the most cards in play
            Card possessor = PlayCard("Possessor");

            //set up haka's hand
            DiscardAllCards(haka);


            //At the start of that hero's turn, put 2 cards from their hand into play at random.
            GoToStartOfTurn(haka);
            AssertNumberOfCardsInHand(haka, 0);

        }
        [Test()]
        public void TestPossessor_MidPlayDestruction()
        {
            SetupGameController(new string[] { "BaronBlade", "Tempest", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            DrawCard(tempest);
            GoToEndOfTurn(catacombs);

            Card possessor = PlayCard("Possessor");
            AssertNextToCard(possessor, tempest.CharacterCard);
            MoveAllCardsFromHandToDeck(tempest);
            Card flash1 = PutInHand("FlashFlood");
            Card flash2 = PutInHand("FlashFlood");

            Card ducts = GetCard("Aqueducts");
            Card torture = GetCard("TortureChamber");

            if (!ducts.IsInPlayAndHasGameText)
            {
                DecisionSelectCard = ducts;
                DestroyCard(FindCardsWhere(c => c.IsRoom && c.IsInPlayAndHasGameText).First());
            }

            DecisionSelectCard = null;
            DecisionSelectCards = new Card[] { ducts, torture, possessor };
            GoToStartOfTurn(tempest);
            AssertNumberOfCardsInTrash(tempest, 1);
            AssertNumberOfCardsInHand(tempest, 1);
        }

        [Test()]
        public void TestScurryingEvil_IndestructibleUnlessLessThan0()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            GoToEndOfTurn(catacombs);

            GoToPlayCardPhase(catacombs);

            Card scurrying = PlayCard("ScurryingEvil");

            //This card is indestructible until it has 0 or fewer HP.

            DestroyCard(scurrying, ra.CharacterCard);
            AssertInPlayArea(catacombs, scurrying);

            DealDamage(ra.CharacterCard, scurrying, 5, DamageType.Fire);

            AssertInTrash(scurrying);

        }

        [Test()]
        public void TestScurryingEvil_EndOfTurn()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            GoToEndOfTurn(catacombs);

            GoToPlayCardPhase(catacombs);

            PlayCard("ScurryingEvil");

            //At the end of the environment turn, play the top card of the environment deck.
            Card guide = PutOnDeck("LabyrinthGuide");
            GoToEndOfTurn(catacombs);
            AssertInPlayArea(catacombs, guide);

        }

        [Test()]
        public void TestScurryingEvil_DamageImmunity()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();


            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));


            GoToPlayCardPhase(catacombs);

            Card scurrying = PlayCard("ScurryingEvil");
            //Whenever this card is dealt damage, it becomes immune to damage until a different Room card enters play.
            QuickHPStorage(scurrying);
            DealDamage(ra.CharacterCard, scurrying, 1, DamageType.Fire);
            if (playedRoom.Identifier == "TwistingPassages")
            {
                QuickHPCheck(-2);
            }
            else
            {
                QuickHPCheck(-1);

            }

            //scurrying evil should now be immune
            QuickHPStorage(scurrying);
            DealDamage(haka.CharacterCard, scurrying, 1, DamageType.Melee);
            QuickHPCheck(0);

            //force a room change
            DestroyCard(playedRoom, ra.CharacterCard);
            QuickHPStorage(scurrying);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            DealDamage(haka.CharacterCard, scurrying, 1, DamageType.Melee);
            if (playedRoom.Identifier == "TwistingPassages")
            {
                QuickHPCheck(-2);
            }
            else
            {
                QuickHPCheck(-1);

            }

            //stack deck to prevent living geometry from destroying more rooms
            Card guide = PutOnDeck("LabyrinthGuide");

            GoToNextTurn();
            //scurrying evil should now be immune
            //check that it persists through turns
            QuickHPStorage(scurrying);
            DealDamage(ra.CharacterCard, scurrying, 1, DamageType.Fire);
            QuickHPCheck(0);

        }

        [Test()]
        public void TestScurryingEvil_DamageImmunity_LivingGeometry()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();


            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));


            GoToPlayCardPhase(catacombs);

            Card scurrying = PlayCard("ScurryingEvil");
            //Whenever this card is dealt damage, it becomes immune to damage until a different Room card enters play.
            QuickHPStorage(scurrying);
            DealDamage(ra.CharacterCard, scurrying, 1, DamageType.Fire);
            if (playedRoom.Identifier == "TwistingPassages")
            {
                QuickHPCheck(-2);
            }
            else
            {
                QuickHPCheck(-1);
            }

            //scurrying evil should now be immune
            QuickHPUpdate();
            DealDamage(haka.CharacterCard, scurrying, 1, DamageType.Melee);
            QuickHPCheck(0);

            //force a room change
            DestroyCard(playedRoom, ra.CharacterCard);
            Card oldRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            SetAllTargetsToMaxHP();
            QuickHPStorage(scurrying);
            DealDamage(haka.CharacterCard, scurrying, 1, DamageType.Melee);
            if(oldRoom.Identifier == "TwistingPassages")
            {
                QuickHPCheck(-2);
            }
            else
            {
                QuickHPCheck(-1);
            }

            Card guide = PutOnDeck("LivingGeometry");
            GoToNextTurn();
            Card currentRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            //destroy all environment targets to ensure if twisting passages is in play the buff is there
            DestroyCards((Card c) => c.IsEnvironmentTarget);
            if(oldRoom != currentRoom)
            {
                //living geometry destroyed a room at the end of the turn so should be non-immune
                QuickHPStorage(scurrying);
                DealDamage(ra.CharacterCard, scurrying, 1, DamageType.Fire);
                if(currentRoom.Identifier == "TwistingPassages")
                {
                    //damage has been increased by 1
                    QuickHPCheck(-2);
                } else if(currentRoom.Identifier == "SacrificialShrine")
                {
                    //extra damage has been dealt, so will be immune to damage
                    QuickHPCheck(0);
                } else
                {
                    QuickHPCheck(-1);
                }


                //should now be immune
                QuickHPStorage(scurrying);
                DealDamage(haka.CharacterCard, scurrying, 1, DamageType.Melee);
                QuickHPCheck(0);
            } else
            {
                //living geometry played the same room so should be immune
                QuickHPStorage(scurrying);
                DealDamage(haka.CharacterCard, scurrying, 1, DamageType.Melee);
                QuickHPCheck(0);
            }

        }

        [Test()]
        public void TestTerriblePresence_TortureChamberInPlay_Affected()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

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

            Card terrible = PlayCard("TerriblePresence");

            PrintSeparator("Check if terrible presence can be dealt damage from hero card");
            //This card may not be affected by hero cards unless Torture Chamber is in play.
            QuickHPStorage(terrible);
            DealDamage(ra.CharacterCard, terrible, 1, DamageType.Fire);
            QuickHPCheck(-1);


        }

        [Test()]
        public void TestTerriblePresence_AqueductsInPlay_Affected()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is Aqueducts in play
            if (playedRoom.Identifier != "Aqueducts")
            {
                DecisionSelectCard = GetCard("Aqueducts");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            Card terrible = PlayCard("TerriblePresence");

            PrintSeparator("Check if terrible presence can be dealt damage from hero card");
            //This card may not be affected by hero cards unless aqueducts is in play.
            QuickHPStorage(terrible);
            DealDamage(ra.CharacterCard, terrible, 1, DamageType.Fire);
            QuickHPCheck(-1);


        }

        [Test()]
        public void TestTerriblePresence_TortureChamberAndAqueductNotInPlay_NotAffected()
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

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is not Torture Chamber in play
            //putting aqueducts in play to simplify
            if (playedRoom.Identifier != "SacrificialShrine")
            {
                DecisionSelectCard = GetCard("SacrificialShrine");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            Card terrible = PlayCard("TerriblePresence");

            PrintSeparator("Check if Terrible Presence can be dealt damage from hero card");
            //This card may not be affected by hero cards unless Torture Chamber or aqueducts is in play.
            QuickHPStorage(terrible);
            DealDamage(ra.CharacterCard, terrible, 1, DamageType.Fire);
            QuickHPCheckZero();

            Card[] targets = new Card[] { baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, terrible };

            QuickHPStorage(targets);
            DealDamage(ra.CharacterCard, (Card c) => c.IsTarget, 3, DamageType.Fire);
            QuickHPCheck(-3, -3, -3, -3, 0);

        }

        [Test()]
        public void TestTerriblePresenceEndOfTurn()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.StSimeonsCatacombs" });
            StartGame();

            //Set Hitpoints to start
            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(legacy.CharacterCard, 25);
            SetHitPoints(haka.CharacterCard, 15);
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is torture chamber in play
            if (playedRoom.Identifier != "TortureChamber")
            {
                DecisionSelectCard = GetCard("TortureChamber");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            GoToPlayCardPhase(catacombs);
            //don't mess with the room in play
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            // At the end of the environment turn, this card deals the 2 non - ghost targets with the lowest HP 2 cold damage each.
            Card terrible = PlayCard("TerriblePresence");

            //2 lowest non ghosts are haka and mdp
            QuickHPStorage(baron.CharacterCard, mdp, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, terrible);
            GoToEndOfTurn(catacombs);
            QuickHPCheck(0, -2, 0, 0, -2, 0);
            
           

        }


        [Test()]
        public void TestRoomsNotVisible()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy", "Tachyon", "Cauldron.StSimeonsCatacombs" });
            StartGame();


            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

            Card playedRoom;

            GoToEndOfTurn(catacombs);
            playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));

            //make sure it is torture chamber in play
            if (playedRoom.Identifier != "TortureChamber")
            {
                DecisionSelectCard = GetCard("TortureChamber");
                DestroyCard(playedRoom, ra.CharacterCard);
            }

            Card twisting = GetCard("TwistingPassages");


            DecisionSelectCard = twisting;

            Assert.Throws(typeof(AssertionException), () => PlayCard("BlindingSpeed"), "Was able to target a room under Catacombs", null) ;
        }

        [Test()]
        public void TestRoomsNotVisibleBeforeFlip()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy/FreedomFiveLegacy", "Tachyon", "Cauldron.StSimeonsCatacombs" });
            StartGame();


            Card catacomb = GetCardInPlay("StSimeonsCatacombs");

            Card twisting = GetCard("TwistingPassages");


            DecisionSelectCard = twisting;
            UsePower(legacy, 0);
            AssertNotInTrash(twisting);
            //we would like these to not be targetable, but indestructible will have to do
            //Assert.Throws(typeof(AssertionException), () => UsePower(legacy, 0), "Was able to target a room under Catacombs", null);
        }

        [Test()]
        public void TestBreathStealerBug()
        {

            SetupGameController(new string[] { "BaronBlade", "Ra", "Legacy/FreedomFiveLegacy", "Cauldron.DocHavoc", "Cauldron.StSimeonsCatacombs" });
            StartGame();

             HeroTurnTakerController docHavoc = FindHero("DocHavoc");
            SetHitPoints(docHavoc, 10);

            Card catacomb = GetCardInPlay("StSimeonsCatacombs");


            GoToEndOfTurn(catacombs);
            Card playedRoom = FindCard((Card c) => c.IsRoom && catacombs.TurnTaker.PlayArea.Cards.Contains(c));
            Card breath = PlayCard("BreathStealer");
            //make sure it is aqueducts in play
            if (playedRoom.Identifier != "Aqueducts")
            {
                DecisionSelectCards = new Card[] { GetCard("Aqueducts"), breath };
                DestroyCard(playedRoom, ra.CharacterCard);
            }
            else
            {
                DecisionSelectCards = new Card[] { breath };
            }


            GoToUsePowerPhase(docHavoc);
            Card brawler = PlayCard("Brawler");
            UsePower(brawler);
            AssertNotGameOver();

        }
    }
}
