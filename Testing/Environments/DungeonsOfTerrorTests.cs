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

            Card card = PlayCard(fate);
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

        }
    }
}
