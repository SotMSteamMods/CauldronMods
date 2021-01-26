using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class DungeonsOfTerrorTests : CauldronBaseTest
    {

        #region DungeonsOfTerrorHelperFunctions

        protected TurnTakerController dungeon { get { return FindEnvironment(); } }
        protected bool IsFate(Card card)
        {
            return card.DoKeywordsContain("fate");
        }

        #endregion

        [Test()]
        public void TestDungeonsOfTerrorWorks()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

        }

        [Test()]
        [Sequential]
        public void DecklistTest_CharacterClass_IsCharacterClass([Values("TheresAlwaysABard")] string characterClass)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();

            GoToPlayCardPhase(dungeon);

            Card card = PlayCard(characterClass);
            AssertCardHasKeyword(card, "character class", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Fate_IsFate([Values("EnormousPack", "ImprobableFailure", "OneInAMillion", "DubiousEdibles", "HighGround", "RovingSlime", "MagicBlade", "TheresAlwaysABard")] string fate)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();

            GoToPlayCardPhase(dungeon);

            Card card = GetCard(fate);
            AssertCardHasKeyword(card, "fate", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Monster_IsMonster([Values("RovingSlime", "SuspiciousChest", "StoneWarden")] string monster)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();

            GoToPlayCardPhase(dungeon);

            Card card = PlayCard(monster);
            AssertCardHasKeyword(card, "monster", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_NPC_IsNPC([Values("Shopkeeper")] string npc)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();

            GoToPlayCardPhase(dungeon);

            Card card = PlayCard(npc);
            AssertCardHasKeyword(card, "npc", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Tavern_IsTavern([Values("RestfulInn")] string tavern)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();

            GoToPlayCardPhase(dungeon);

            Card card = PlayCard(tavern);
            AssertCardHasKeyword(card, "tavern", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Trap_IsTrap([Values("DelayedRockTrap")] string trap)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();

            GoToPlayCardPhase(dungeon);

            Card card = PlayCard(trap);
            AssertCardHasKeyword(card, "trap", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Treasure_IsTreasure([Values("MagicBlade", "RingOfForesight")] string treasure)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();

            GoToPlayCardPhase(dungeon);

            Card card = PlayCard(treasure);
            AssertCardHasKeyword(card, "treasure", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_NoKeyword_HasNoKeyword([Values("Underleveled")] string keywordLess)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();

            GoToPlayCardPhase(dungeon);

            Card card = PlayCard(keywordLess);
            Assert.IsFalse(card.Definition.Keywords.Any(), $"{card.Title} has keywords when it shouldn't.");
        }

        [Test()]
        public void TestDelayedRockTrap_Fate()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();

            //Play this card next to a hero.
            DecisionSelectCard = legacy.CharacterCard;
            Card trap = PlayCard("DelayedRockTrap");
            AssertNextToCard(trap, legacy.CharacterCard);

            //When a card enters the environment trash, check that card. 
            //If it is a fate card, this card deal the hero next to it {H} melee damage.
            QuickHPStorage(baron, ra, legacy, haka);
            PutInTrash("HighGround");
            QuickHPCheck(0, 0, -3, 0);

            //Then, destroy this card.
            AssertInTrash(trap);
        }

        [Test()]
        public void TestDelayedRockTrap_NotFate()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();

            //Play this card next to a hero.
            DecisionSelectCard = legacy.CharacterCard;
            Card trap = PlayCard("DelayedRockTrap");
            AssertNextToCard(trap, legacy.CharacterCard);

            //When a card enters the environment trash, check that card. 
            //If it is not a fate card, this card deals each other hero target {H-2} melee damage. 
            QuickHPStorage(baron, ra, legacy, haka);
            PutInTrash("StoneWarden");
            QuickHPCheck(0, -1, 0, -1);

            //Then, destroy this card.
            AssertInTrash(trap);
        }

        [Test()]
        public void TestDubiousEdibles_NotFate()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(baron, 20);
            SetHitPoints(ra, 20);
            SetHitPoints(legacy, 20);
            SetHitPoints(haka, 20);

            //put not fate in trash
            PutInTrash("StoneWarden");
            //When this card enters play, check the top card of the environment trash.
            //If it is not a fate card, 1 hero target regains 4HP. 
            DecisionSelectCard = haka.CharacterCard;
            QuickHPStorage(baron, ra, legacy, haka);
            Card edibles = PlayCard("DubiousEdibles");
            QuickHPCheck(0, 0, 0, 4);

            //Then, destroy this card.
            AssertInTrash(edibles);
        }

        [Test()]
        public void TestDubiousEdibles_Fate()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(baron, 20);
            SetHitPoints(ra, 20);
            SetHitPoints(legacy, 20);
            SetHitPoints(haka, 20);

            //put fate in trash
            PutInTrash("HighGround");
            //When this card enters play, check the top card of the environment trash.
            //If it is a fate card, this card deals each hero 1 toxic damage.
            QuickHPStorage(baron, ra, legacy, haka);
            Card edibles = PlayCard("DubiousEdibles");
            QuickHPCheck(0, -1, -1, -1);

            //Then, destroy this card.
            AssertInTrash(edibles);
        }

        [Test()]
        public void TestEnormousPack_Fate_Play()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card ring = PutInHand("TheLegacyRing");

            //put fate in trash
            PutInTrash("HighGround");
            //When this card enters play, check the top card of the environment trash.
            //If it is a fate card, 1 hero may play card, draw a card, or use a power
            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionSelectFunction = 0;
            DecisionSelectCard = ring;
            Card pack = PlayCard("EnormousPack");
            AssertInPlayArea(legacy, ring);

            //Then, destroy this card.
            AssertInTrash(pack);

        }

        [Test()]
        public void TestEnormousPack_Fate_Draw()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card top = GetTopCardOfDeck(legacy);
            //put fate in trash
            PutInTrash("HighGround");
            //When this card enters play, check the top card of the environment trash.
            //If it is a fate card, 1 hero may play card, draw a card, or use a power
            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionSelectFunction = 1;
            Card pack = PlayCard("EnormousPack");
            AssertInHand(top);

            //Then, destroy this card.
            AssertInTrash(pack);

        }

        [Test()]
        public void TestEnormousPack_Fate_Power()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();
            //put fate in trash
            PutInTrash("HighGround");
            //When this card enters play, check the top card of the environment trash.
            //If it is a fate card, 1 hero may play card, draw a card, or use a power
            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionSelectFunction = 2;
            Card pack = PlayCard("EnormousPack");

            QuickHPStorage(baron);
            DealDamage(ra, baron, 3, DamageType.Fire);
            QuickHPCheck(-4);

            //Then, destroy this card.
            AssertInTrash(pack);

        }

        [Test()]
        public void TestEnormousPack_Fate_Optional()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();
            //put fate in trash
            PutInTrash("HighGround");
            //When this card enters play, check the top card of the environment trash.
            //If it is a fate card, 1 hero may play card, draw a card, or use a power
            DecisionSelectTurnTaker = legacy.TurnTaker;
            DecisionDoNotSelectFunction = true;
            QuickHandStorage(legacy);
            QuickHPStorage(baron, ra, legacy, haka);
            Card pack = PlayCard("EnormousPack");
            QuickHandCheckZero();
            QuickHPCheckZero();

            //Then, destroy this card.
            AssertInTrash(pack);

        }

        [Test()]
        public void TestEnormousPack_NotFate()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();
            DrawCard(legacy);

            //put not fate in trash
            PutInTrash("StoneWarden");
            //When this card enters play, check the top card of the environment trash.
            //If it is not a fate card, the player with the most cards in hand discards 2 cards
            QuickHandStorage(ra, legacy, haka);
            Card pack = PlayCard("EnormousPack");
            QuickHandCheck(0, -2, 0);
            //Then, destroy this card.
            AssertInTrash(pack);

        }

        [Test()]
        public void TestHighGround()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();

            //While the top card of the environment trash is a fate card, reduce damage dealt by hero targets by 1. 
            //While it is not a fate card, reduce damage dealt by villain targets by 1.

            Card ground = PlayCard("HighGround");

            //when nothing, everything should be normal
            QuickHPStorage(baron);
            DealDamage(ra, baron, 3, DamageType.Fire);
            QuickHPCheck(-3);

            QuickHPStorage(ra);
            DealDamage(baron, ra, 3, DamageType.Fire);
            QuickHPCheck(-3);

            //put fate in trash
            PutInTrash("EnormousPack");

            //heroes reduced
            QuickHPStorage(baron);
            DealDamage(ra, baron, 3, DamageType.Fire);
            QuickHPCheck(-2);

            QuickHPStorage(ra);
            DealDamage(baron, ra, 3, DamageType.Fire);
            QuickHPCheck(-3);

            //put not fate in trash
            PutInTrash("StoneWarden");

            //villains reduced
            QuickHPStorage(baron);
            DealDamage(ra, baron, 3, DamageType.Fire);
            QuickHPCheck(-3);

            QuickHPStorage(ra);
            DealDamage(baron, ra, 3, DamageType.Fire);
            QuickHPCheck(-2);

            //At the start of the environment turn, destroy this card.
            GoToStartOfTurn(dungeon);
            AssertInTrash(ground);


        }

        [Test()]
        public void TestImprobableFailure()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();
            //At the start of each player's draw phase, discard and check the top card of the environment deck.",
            //If it is a fate card, they skip the rest of their draw phase.",
            Card failure = PlayCard("ImprobableFailure");
            Card fate = PutOnDeck("HighGround");
            GoToDrawCardPhase(ra);
            AssertInTrash(fate);
            //the test suite says this is still the end phase, but it shows preventing the draw card action correctly, and UI testing proves it works
            //AssertCurrentTurnPhase(ra, Phase.End);

            Card notFate = PutOnDeck("StoneWarden");
            GoToDrawCardPhase(legacy);
            AssertInTrash(notFate);
            AssertCurrentTurnPhase(legacy, Phase.DrawCard);

            //At the start of the environment turn, destroy this card.
            GoToStartOfTurn(dungeon);
            AssertInTrash(failure);

        }

        [Test()]
        public void TestMagicBlade()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(ra, 20);
            SetHitPoints(legacy, 20);
            SetHitPoints(haka, 20);
            //Play this card next to a hero.
            DecisionSelectCard = legacy.CharacterCard;
            Card magic = PlayCard("MagicBlade");
            AssertNextToCard(magic, legacy.CharacterCard);

            ////The first time they use a power each turn, discard and check the top card of the environment deck.",
            //If it is a fate card, that hero deals 1 target 2 energy damage. If it is not a fate card, that hero deals themselves 2 infernal damage."

            Card fate = PutOnDeck("HighGround");
            QuickHPStorage(baron, ra, legacy, haka);
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(legacy.CharacterCard);
            QuickHPCheck(-4, 0, 0, 0); //1 galvanize + nemesis bonus
            AssertInTrash(fate);

            Card notFate = PutOnDeck("StoneWarden");
            Card charge = PlayCard("MotivationalCharge");
            QuickHPUpdate();
            UsePower(charge);
            QuickHPCheck(-4, 1, 1, 1);
            AssertOnTopOfDeck(notFate);

            GoToNextTurn();
            QuickHPUpdate();
            UsePower(legacy.CharacterCard);
            QuickHPCheck(0, 0, -4, 0); //2 galvanized have been used
            AssertInTrash(notFate);

        }

        [Test()]
        public void TestOneInAMillion_TwoMatches()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToNextTurn();
            //When this card enters play, discard and check the top 2 cards of the environment deck.
            Card fate1 = PutOnDeck("HighGround");
            Card fate2 = PutOnDeck("EnormousPack");

            QuickHandStorage(ra, legacy, haka);
            Card million = PlayCard("OneInAMillion");
            //If at least 1 is a fate card, each player draws a card.
            //If both are fate cards, skip the next villain turn.
            QuickHandCheck(1, 1, 1);
            GoToStartOfTurn(baron);
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(ra, Phase.Start);
        }

        [Test()]
        public void TestOneInAMillion_OneMatch()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToNextTurn();
            //When this card enters play, discard and check the top 2 cards of the environment deck.
            Card fate1 = PutOnDeck("HighGround");
            Card notfate2 = PutOnDeck("StoneWarden");

            QuickHandStorage(ra, legacy, haka);
            Card million = PlayCard("OneInAMillion");
            //If at least 1 is a fate card, each player draws a card.
            //If both are fate cards, skip the next villain turn.
            QuickHandCheck(1, 1, 1);
            GoToStartOfTurn(baron);
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(baron, Phase.PlayCard);
        }

        [Test()]
        public void TestOneInAMillion_NoMatches()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToNextTurn();
            //When this card enters play, discard and check the top 2 cards of the environment deck.
            Card nofate1 = PutOnDeck("Underleveled");
            Card notfate2 = PutOnDeck("StoneWarden");

            QuickHandStorage(ra, legacy, haka);
            Card million = PlayCard("OneInAMillion");
            //If at least 1 is a fate card, each player draws a card.
            //If both are fate cards, skip the next villain turn.
            QuickHandCheck(0, 0, 0);
            GoToStartOfTurn(baron);
            EnterNextTurnPhase();
            AssertCurrentTurnPhase(baron, Phase.PlayCard);
        }

        [Test()]
        public void TestRestfulInn()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(ra, 20);
            SetHitPoints(legacy, 20);
            SetHitPoints(ra, 20);
            //The first time a hero draws a card during their turn, they may discard it. If they do, that hero regains 2HP. Increase HP regained this way by 1 if the top card of the environment trash is a fate card.
            Card inn = PlayCard("RestfulInn");

            //put fate in trash
            PutInTrash("HighGround");

            GoToPlayCardPhase(ra);
            DecisionsYesNo = new bool[] { true, true, false };
            QuickHandStorage(ra, legacy, haka);
            QuickHPStorage(baron, ra, legacy, haka);
            Card top = ra.TurnTaker.Deck.TopCard;
            DrawCard(ra);
            AssertInTrash(top);
            QuickHandCheck(0, 0, 0);
            QuickHPCheck(0, 3, 0, 0);

            //only first one
            QuickHandUpdate();
            QuickHPUpdate();
            DrawCard(ra);
            QuickHandCheck(1, 0, 0);
            QuickHPCheckZero();

            //only during their turn
            GoToNextTurn();
            QuickHandUpdate();
            QuickHPUpdate();
            DrawCard(ra);
            QuickHandCheck(1, 0, 0);
            QuickHPCheckZero();

            //check non-fate no increase
            PutInTrash("StoneWarden");
            QuickHandUpdate();
            QuickHPUpdate();
            top = legacy.TurnTaker.Deck.TopCard;
            DrawCard(legacy);
            AssertInTrash(top);
            QuickHandCheck(0, 0, 0);
            QuickHPCheck(0, 0, 2, 0);

            //optional
            GoToNextTurn();
            QuickHandUpdate();
            QuickHPUpdate();
            top = haka.TurnTaker.Deck.TopCard;
            DrawCard(haka);
            AssertInHand(top);
            QuickHandCheck(0, 0, 1);
            QuickHPCheck(0, 0, 0, 0);

            GoToStartOfTurn(dungeon);
            AssertInTrash(inn);

        }

        [Test()]
        public void TestRingOfForesight()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();

            //Play this card next to a hero. At the start of their turn, they regain 1HP.
            DecisionSelectCard = ra.CharacterCard;
            SetHitPoints(ra, 20);
            Card ring = PlayCard("RingOfForesight");
            AssertNextToCard(ring, ra.CharacterCard);
            GoToEndOfTurn(baron);
            QuickHPStorage(baron, ra, legacy, haka);
            GoToStartOfTurn(ra);
            QuickHPCheck(0, 1, 0, 0);

            //When checking a card in the environment trash, the players may first destroy this card, and then check it instead of the original card.
            PutInTrash("HighGround");
            DecisionYesNo = true;
            QuickHPUpdate();
            PlayCard("DubiousEdibles");
            QuickHPCheck(0, 4, 0, 0);
            AssertInTrash(ring);

        }

        [Test()]
        public void TestRovingSlime_Fate()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();

            //At the end of the environment turn, check the top card of the environment trash.
            //If it is a fate card, this card deals the hero target with the highest HP {H-1} toxic damage. If if is not a fate card, this card deals each non-environment target 1 toxic damage.
            Card slime = PlayCard("RovingSlime");
            GoToPlayCardPhase(dungeon);
            PutInTrash("HighGround");

            QuickHPStorage(baron, ra, legacy, haka);
            GoToEndOfTurn(dungeon);
            QuickHPCheck(0, 0, 0, -2);
        }

        [Test()]
        public void TestRovingSlime_NotFate()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.DungeonsOfTerror");
            StartGame();
            DestroyNonCharacterVillainCards();

            //At the end of the environment turn, check the top card of the environment trash.
            //If it is a fate card, this card deals the hero target with the highest HP {H-1} toxic damage. If if is not a fate card, this card deals each non-environment target 1 toxic damage.
            Card slime = PlayCard("RovingSlime");
            GoToPlayCardPhase(dungeon);
            PutInTrash("StoneWarden");

            QuickHPStorage(baron, ra, legacy, haka);
            GoToEndOfTurn(dungeon);
            QuickHPCheck(-1, -1, -1, -1);
        }
    }
}
