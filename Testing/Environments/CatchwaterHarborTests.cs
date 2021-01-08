using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class CatchwaterHarborTests : BaseTest
    {

        #region CatchwaterHarborHelperFunctions

        protected TurnTakerController catchwater { get { return FindEnvironment(); } }
        protected bool IsTransport(Card card)
        {
            return card.DoKeywordsContain("transport");
        }

        #endregion

        [Test()]
        public void TestCatchwaterWorks()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());
        }

        [Test()]
        [Sequential]
        public void TestTransportPlay_AllAboardInDeck([Values("SSEscape", "ToOverbrook", "UnmooredZeppelin")] string transport)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            Card allAboard = GetCard("AllAboard");
            //stack deck for 3:10 To Overbrook to not mess things up
            StackAfterShuffle(catchwater.TurnTaker.Deck, new string[] { "AlteringHistory" });
            //When this card enters play, search the environment deck and trash for All Aboard and put it into play, then shuffle the deck.
            QuickShuffleStorage(catchwater.TurnTaker.Deck);
            PlayCard(transport);
            QuickShuffleCheck(1);
            AssertIsInPlay(allAboard);
        }

        [Test()]
        [Sequential]
        public void TestTransportPlay_AllAboardInTrash([Values("SSEscape", "ToOverbrook", "UnmooredZeppelin")] string transport)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            Card allAboard = PutInTrash("AllAboard");
            //stack deck for 3:10 To Overbrook to not mess things up
            StackAfterShuffle(catchwater.TurnTaker.Deck, new string[] { "AlteringHistory" });
            //When this card enters play, search the environment deck and trash for All Aboard and put it into play, then shuffle the deck.
            QuickShuffleStorage(catchwater.TurnTaker.Deck);
            PlayCard(transport);
            QuickShuffleCheck(1);
            AssertIsInPlay(allAboard);
        }

        [Test()]
        public void TestSSEscapePlay()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card battalion = PlayCard("BladeBattalion");
            SetHitPoints(new Card[] { baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard }, 2);
            //each target regains 2HP.
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            Card transport = PlayCard("SSEscape");
            QuickHPCheck(2, 2, 2, 2, 2);

        }


        [Test()]
        public void TestSSEscapeTravel()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            Card transport = PlayCard("SSEscape");
            SetHitPoints(new Card[] { baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard }, 2);
            //Each player draws a card.Each villain target regains 3HP.
            QuickHandStorage(ra, legacy, haka);
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            ActivateAbility("travel", transport);
            QuickHandCheck(1, 1, 1);
            QuickHPCheck(3, 3, 0, 0, 0);

        }

        [Test()]
        public void TestToOverbrookPlay()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();

            //play its top card.
            Card altering = GetCard("AlteringHistory");
            StackAfterShuffle(catchwater.TurnTaker.Deck, new string[] { "AlteringHistory" });
            Card transport = PlayCard("ToOverbrook");
            AssertIsInPlay(altering);
           
        }

        [Test()]
        public void TestToOverbrookTravel()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
        
            Card transport = PlayCard("ToOverbrook");
            //Play the top card of each other deck in turn order, starting with the villain deck.
            Card baronTop = PutOnDeck("BladeBattalion");
            Card raTop = PutOnDeck("FlameBarrier");
            Card legacyTop = PutOnDeck("NextEvolution");
            Card hakaTop = PutOnDeck("Mere");
            Card envTop = PutOnDeck("AlteringHistory");
            ActivateAbility("travel", transport);
            AssertIsInPlay(baronTop);
            AssertIsInPlay(raTop);
            AssertIsInPlay(legacyTop);
            AssertIsInPlay(raTop);
            AssertOnTopOfDeck(envTop);

        }

        [Test()]
        public void TestUnmooredZeppelinPlay()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();

            //this card deals each target 2 projectile damage.
            QuickHPStorage(baron, ra, bunker, haka);
            Card transport = PlayCard("UnmooredZeppelin");
            QuickHPCheck(-2, -2, -2, -2);
            
        }

        [Test()]
        public void TestUnmooredZeppelinTravel()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card transport = PlayCard("UnmooredZeppelin");
            //Increase all damage dealt by 1 until the start of the next environment turn.
            ActivateAbility("travel", transport);
            QuickHPStorage(baron, haka);
            DealDamage(baron, haka, 1, DamageType.Melee);
            DealDamage(ra, baron, 1, DamageType.Melee);
            DealDamage(bunker, baron, 1, DamageType.Melee);
            DealDamage(haka, baron, 1, DamageType.Melee);
            QuickHPCheck(-6, -2);

            GoToStartOfTurn(catchwater);
            QuickHPUpdate();
            DealDamage(baron, haka, 1, DamageType.Melee);
            DealDamage(ra, baron, 1, DamageType.Melee);
            DealDamage(bunker, baron, 1, DamageType.Melee);
            DealDamage(haka, baron, 1, DamageType.Melee);
            QuickHPCheck(-3, -1);

        }

    }
}
