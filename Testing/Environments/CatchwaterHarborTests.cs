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
        public void TestSSEscapeTravel()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            SetHitPoints(new Card[] { baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard }, 2);
            Card transport = PlayCard("SSEscape");
            //Each player draws a card.Each villain target regains 3HP.
            QuickHandStorage(ra, legacy, haka);
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            ActivateAbility("travel", transport);
            QuickHandCheck(1, 1, 1);
            QuickHPCheck(3, 3, 0, 0, 0);

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
