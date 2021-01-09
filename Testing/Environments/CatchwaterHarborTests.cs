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
        public void DecklistTest_Transport_IsTransport([Values("SSEscape", "ToOverbrook", "UnmooredZeppelin")] string transport)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();

            GoToPlayCardPhase(catchwater);

            Card card = PlayCard(transport);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "transport", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Gangster_IsGangster([Values("HarkinParishJr", "SmoothCriminal")] string gangster)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();

            GoToPlayCardPhase(catchwater);

            Card card = PlayCard(gangster);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "gangster", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Anomaly_IsAnomaly([Values("OminousLoop")] string anomaly)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();

            GoToPlayCardPhase(catchwater);

            Card card = PlayCard(anomaly);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "anomaly", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Civilian_IsCivilian([Values("FrightenedOnlookers")] string anomaly)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();

            GoToPlayCardPhase(catchwater);

            Card card = PlayCard(anomaly);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "civilian", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Speakeasy_IsSpeakeasy([Values("TheCervantesClub")] string speakeasy)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();

            GoToPlayCardPhase(catchwater);

            Card card = PlayCard(speakeasy);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "speakeasy", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Structure_IsStructure([Values("HarborCrane")] string structure)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();

            GoToPlayCardPhase(catchwater);

            Card card = PlayCard(structure);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "structure", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_NoKeyword_HasNoKeyword([Values("AllAboard", "LeftBehind", "AbandonedFactory",
            "AlteringHistory", "RadioPlaza", "ThisJustIn")] string keywordLess)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();

            GoToPlayCardPhase(catchwater);

            Card card = PlayCard(keywordLess);
            AssertIsInPlay(card);
            Assert.IsFalse(card.Definition.Keywords.Any(), $"{card.Title} has keywords when it shouldn't.");
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

        [Test()]
        public void TestAllAboard_Indestructible()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();

            //this card is indestructible
            Card allAboard = PlayCard("AllAboard");
            DestroyCard(allAboard, baron.CharacterCard);
            AssertIsInPlay(allAboard);

        }

        [Test()]
        public void TestAllAboard_EndOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();

            //At the end of the environment turn, the players may activate the Travel text of a Transport card. If they do, destroy that card at the start of the next environment turn.
            GoToPlayCardPhase(catchwater);
            Card allAboard = PlayCard("AllAboard");
            Card transport1 = PlayCard("UnmooredZeppelin");
            Card transport2 = PlayCard("SSEscape");
            DecisionSelectCard = transport1;
            GoToEndOfTurn(catchwater);
            QuickHPStorage(baron, haka);
            DealDamage(baron, haka, 1, DamageType.Melee);
            DealDamage(ra, baron, 1, DamageType.Melee);
            DealDamage(bunker, baron, 1, DamageType.Melee);
            DealDamage(haka, baron, 1, DamageType.Melee);
            QuickHPCheck(-6, -2);
            GoToStartOfTurn(catchwater);
            AssertInTrash(transport1);
            AssertIsInPlay(transport2);

        }

        [Test()]
        public void TestAllAboard_EndOfTurnOptional()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();

            //At the end of the environment turn, the players may activate the Travel text of a Transport card. If they do, destroy that card at the start of the next environment turn.
            GoToPlayCardPhase(catchwater);
            Card allAboard = PlayCard("AllAboard");
            Card transport1 = PlayCard("UnmooredZeppelin");
            Card transport2 = PlayCard("SSEscape");
            DecisionDoNotSelectCard = SelectionType.ActivateAbility;
            GoToEndOfTurn(catchwater);
            QuickHPStorage(baron, haka);
            DealDamage(baron, haka, 1, DamageType.Melee);
            DealDamage(ra, baron, 1, DamageType.Melee);
            DealDamage(bunker, baron, 1, DamageType.Melee);
            DealDamage(haka, baron, 1, DamageType.Melee);
            QuickHPCheck(-3, -1);
            GoToStartOfTurn(catchwater);
            AssertIsInPlay(transport1);
            AssertIsInPlay(transport2);

        }

        [Test()]
        public void TestAbandonedFactory_EnterTrash()
        {
            SetupGameController("TheEnnead", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            Card lowestVillain = FindCardsWhere(c => ennead.CharacterCards.Contains(c) && c.IsInPlayAndHasGameText).First();
            SetHitPoints(lowestVillain, 5);
            //Whenever an ongoing or equipment card enters a hero trash pile, the villain character target with the lowest HP regains 2HP.
            PlayCard("AbandonedFactory");

            //ongoing
            QuickHPStorage(lowestVillain);
            MoveCard(ra, "FlameBarrier", ra.TurnTaker.Trash);
            QuickHPCheck(2);

            //equipment
            QuickHPUpdate();
            MoveCard(ra, "TheStaffOfRa", ra.TurnTaker.Trash);
            QuickHPCheck(2);

            //only hero trashes
            QuickHPUpdate();
            MoveCard(haka, "Mere", ennead.TurnTaker.Trash);
            QuickHPCheckZero();
        }

        [Test()]
        public void TestAbandonedFactory_DestroyInstead()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();

            Card factory = PlayCard("AbandonedFactory");
            Card transport = PlayCard("SSEscape");

            //When a Transport card would be destroyed, you may destroy this card instead.
            DecisionYesNo = true;
            DestroyCard(transport, ra.CharacterCard);
            AssertInTrash(factory);
            AssertIsInPlay(transport);

        }

        [Test()]
        public void TestAbandonedFactory_DestroyInsteadOptional()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();

            Card factory = PlayCard("AbandonedFactory");
            Card transport = PlayCard("SSEscape");

            //When a Transport card would be destroyed, you may destroy this card instead.
            DecisionYesNo = false;
            DestroyCard(transport, ra.CharacterCard);
            AssertInTrash(transport);
            AssertIsInPlay(factory);

        }


        [Test()]
        public void TestAlteringHistory()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card factory = PlayCard("AbandonedFactory");
            Card altering = PlayCard("AlteringHistory");

            //Whenever this card or any other environment card is destroyed, this card deals the 2 non-environment targets with the lowest HP 2 psychic damage each.
            QuickHPStorage(baron, ra, bunker, haka);
            DestroyCard(factory, baron.CharacterCard);
            QuickHPCheck(0, -2, -2, 0);

            QuickHPUpdate();
            DestroyCard(altering, baron.CharacterCard);
            QuickHPCheck(0, -2, -2, 0);

        }

        [Test()]
        public void TestFrightenedOnlookers_StartOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();

            //At the start of the environment turn, 1 player may play a card.
            PlayCard("FrightenedOnlookers");
            Card mere = PutInHand("Mere");
            DecisionSelectTurnTaker = haka.TurnTaker;
            DecisionSelectCard = mere;
            GoToStartOfTurn(catchwater);
            AssertInPlayArea(haka, mere);

        }

        [Test()]
        public void TestFrightenedOnlookers_StartOfTurn_Optional()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();

            //At the start of the environment turn, 1 player may play a card.
            PlayCard("FrightenedOnlookers");
            Card mere = PutInHand("Mere");
            DecisionSelectTurnTaker = haka.TurnTaker;
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            GoToStartOfTurn(catchwater);
            AssertInHand(haka, mere);

        }

        [Test()]
        public void TestFrightenedOnlookers_DamageSelf()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();

            //Whenever a target is dealt 4 or more damage from a single source, this card deals itself 1 projectile damage.
            Card onlooker = PlayCard("FrightenedOnlookers");
            QuickHPStorage(onlooker);
            DealDamage(baron, ra, 4, DamageType.Fire);
            QuickHPCheck(-1);

            //only when exceeds 4
            QuickHPUpdate();
            DealDamage(baron, ra, 3, DamageType.Fire);
            QuickHPCheck(0);

            //when any target damaged for > 4
            QuickHPUpdate();
            DealDamage(ra, baron, 8, DamageType.Fire);
            QuickHPCheck(-1);

        }

        [Test()]
        public void TestHarborCrane_MoveNextTo()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();

            //Whenever this card is dealt damage by a target, move it next to that target.
            Card crane = PlayCard("HarborCrane");
            DealDamage(ra, crane, 1, DamageType.Melee);
            AssertNextToCard(crane, ra.CharacterCard);

            DealDamage(baron, crane, 1, DamageType.Melee);
            AssertNextToCard(crane, baron.CharacterCard);
        }

        [Test()]
        public void TestHarborCrane_Increase()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();

            //Increase damage dealt by the target next to this card by 1.
            Card crane = PlayCard("HarborCrane");
            DealDamage(ra, crane, 1, DamageType.Melee);
            AssertNextToCard(crane, ra.CharacterCard);

            QuickHPStorage(baron);
            DealDamage(ra, baron, 3, DamageType.Fire);
            QuickHPCheck(-4);
        }

        [Test()]
        public void TestHarborCrane_WhenDestroyed()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();

            //When this card is destroyed, it deals the target next to it 5 melee damage
            Card crane = PlayCard("HarborCrane");
            DealDamage(ra, crane, 1, DamageType.Melee);
            AssertNextToCard(crane, ra.CharacterCard);

            QuickHPStorage(baron, ra, bunker, haka);
            DestroyCard(crane, baron.CharacterCard);
            QuickHPCheck(0, -5, 0, 0);

        }

        [Test()]
        public void TestHarkinParishJr_EntersPlay()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card hakaOngoing = PutInHand("Dominion");
            Card raOngoing = PutInHand("FlameBarrier");
            Card bunkerOngoing1 = PutInHand("AmmoDrop");
            Card bunkerOngoing2 = PutInHand("TurretMode");

            DecisionSelectCards = new Card[] { hakaOngoing, raOngoing, bunkerOngoing1 };
            QuickHandStorage(ra, bunker, haka);
            //When this card enters play, the hero with the highest HP must discard a card. Each other player must discard a card that shares a keyword with that card.
            PlayCard("HarkinParishJr");
            QuickHandCheck(-1, -1, -1);
            AssertInTrash(hakaOngoing);
            AssertInTrash(raOngoing);
            AssertInTrash(bunkerOngoing1);
            AssertInHand(bunkerOngoing2);

        }

        [Test()]
        public void TestHarkinParishJr_EntersPlay_TiedForHighest()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(haka, ra.CharacterCard.HitPoints.Value);
            SetHitPoints(bunker, 10);
            Card hakaOngoing = PutInHand("Dominion");
            Card raOngoing = PutInHand("FlameBarrier");
            Card bunkerOngoing1 = PutInHand("AmmoDrop");
            Card bunkerOngoing2 = PutInHand("TurretMode");

            DecisionSelectCards = new Card[] { haka.CharacterCard, hakaOngoing, raOngoing, bunkerOngoing1 };
            QuickHandStorage(ra, bunker, haka);
            //When this card enters play, the hero with the highest HP must discard a card. Each other player must discard a card that shares a keyword with that card.
            PlayCard("HarkinParishJr");
            QuickHandCheck(-1, -1, -1);
            AssertInTrash(hakaOngoing);
            AssertInTrash(raOngoing);
            AssertInTrash(bunkerOngoing1);
            AssertInHand(bunkerOngoing2);

        }

        [Test()]
        public void TestHarkinParishJr_EndOfTurn()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            //At the end of the environment turn, this card deals the hero target with the highest HP {H} melee damage.
            GoToPlayCardPhase(catchwater);
            PlayCard("HarkinParishJr");
            QuickHPStorage(baron, ra, bunker, haka);
            PrintSeparator("Checking End Of Turn Effect");
            GoToEndOfTurn(catchwater);
            QuickHPCheck(0, 0, 0, -3);


        }
    }
}
