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
            return card != null && card.DoKeywordsContain("transport");
        }
        private void AddReduceDamageTrigger(TurnTakerController ttc,bool hero, bool villain, int amount)
        {
            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(amount);
            reduceDamageStatusEffect.TargetCriteria.IsHero = hero;
            reduceDamageStatusEffect.TargetCriteria.IsVillain = villain;
            reduceDamageStatusEffect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(reduceDamageStatusEffect, true, new CardSource(FindCardController(ttc.TurnTaker.Deck.TopCard))));
        }

        private void AddCantGainHPDamageTrigger(TurnTakerController ttc, bool hero, bool villain)
        {
            CannotGainHPStatusEffect effect = new CannotGainHPStatusEffect();
            effect.TargetCriteria.IsHero = hero;
            effect.TargetCriteria.IsVillain = villain;
            effect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(effect, true, new CardSource(FindCardController(ttc.TurnTaker.Deck.TopCard))));
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
        public void TestHarborCrane_SelfDamage()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();

            //Increase damage dealt by the target next to this card by 1.
            Card crane = PlayCard("HarborCrane");
            DealDamage(crane, crane, 1, DamageType.Melee);
            AssertNotNextToCard(crane, crane);

            //move crane next to ra
            DealDamage(ra, crane, 1, DamageType.Melee);
            AssertNextToCard(crane, ra.CharacterCard);

            DealDamage(crane, crane, 1, DamageType.Melee);
            AssertNextToCard(crane, ra.CharacterCard);
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
            Card hakaOngoing1 = PutInHand("Dominion");
            Card hakaOngoing2 = PutInHand("SavageMana");

            Card raOngoing1 = PutInHand("FlameBarrier");
            Card raOngoing2 = PutInHand("BlazingTornado");

            Card bunkerOngoing1 = PutInHand("AmmoDrop");
            Card bunkerOngoing2 = PutInHand("TurretMode");

            DecisionSelectCards = new Card[] { hakaOngoing1, raOngoing1, bunkerOngoing1 };
            QuickHandStorage(ra, bunker, haka);
            //When this card enters play, the hero with the highest HP must discard a card. Each other player must discard a card that shares a keyword with that card.
            PlayCard("HarkinParishJr");
            QuickHandCheck(-1, -1, -1);
            AssertInTrash(hakaOngoing1);
            AssertInHand(hakaOngoing2);
            AssertInTrash(raOngoing1);
            AssertInHand(raOngoing2);
            AssertInTrash(bunkerOngoing1);
            AssertInHand(bunkerOngoing2);

        }

        [Test()]
        public void TestHarkinParishJr_EntersPlay_TiedForHighest()
        {
            SetupGameController(new[] { "BaronBlade", "Ra", "Bunker", "Haka", "SkyScraper", "Cauldron.CatchwaterHarbor" });
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(haka, sky.CharacterCard.HitPoints.Value);
            SetHitPoints(bunker, 10);
            Card hakaOngoing = PutInHand("Dominion");
            PutInHand("SavageMana");
            Card raOngoing = PutInHand("FlameBarrier");
            PutInHand("ImbuedFire");
            Card ssOngoing = PutInHand("ThorathianMonolith");
            PutInHand("Proportionist");
            Card bunkerOngoing1 = PutInHand("AmmoDrop");
            Card bunkerOngoing2 = PutInHand("TurretMode");
            PutInHand("UpgradeMode");

            IEnumerable<Card> offToSideSky = sky.TurnTaker.OffToTheSide.Cards.Where(c => c.IsCharacter);
            AssertNextDecisionChoices(notIncluded: offToSideSky);
            DecisionSelectCards = new Card[] { haka.CharacterCard, hakaOngoing, raOngoing, bunkerOngoing1, ssOngoing };
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

        [Test()]
        public void TestLeftBehind_Next()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(bunker, 10);
            //Play this card next to the hero with the second lowest HP.
            Card left = PlayCard("LeftBehind");
            AssertNextToCard(left, ra.CharacterCard);

        }
        [Test()]
        public void TestLeftBehind_Next_Oblivaeon_0Heroes()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "Cauldron.CatchwaterHarbor", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Play this card next to the hero with the second lowest HP.
            //since there are no heroes in this battlezone, it should go to the trash
            Card left = PlayCard("LeftBehind");
            AssertInTrash(left);

        }
        [Test()]
        public void TestLeftBehind_Next_Oblivaeon_1Hero()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "Cauldron.CatchwaterHarbor", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();

            SwitchBattleZone(ra);

            //Play this card next to the hero with the second lowest HP.
            //since there is 1 hero here, it should go to the trash
            Card left = PlayCard("LeftBehind");
            AssertInTrash(left);

        }


        [Test()]
        public void TestLeftBehind_GameOverChange_OtherTargetKillsVillain()
        {
            SetupGameController("CitizenDawn", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(bunker, 10);
            //Play this card next to the hero with the second lowest HP.
            Card left = PlayCard("LeftBehind");
            AssertNextToCard(left, ra.CharacterCard);
            DealDamage(bunker, dawn, 10000, DamageType.Projectile);
            AssertGameOver(EndingResult.EnvironmentDefeat);

        }

        [Test()]
        public void TestLeftBehind_GameOverChange_OtherTargetKillsOmnitron()
        {
            SetupGameController("Omnitron", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(bunker, 10);
            //Play this card next to the hero with the second lowest HP.
            Card left = PlayCard("LeftBehind");
            AssertNextToCard(left, ra.CharacterCard);
            DealDamage(bunker, omnitron, 10000, DamageType.Projectile);
            AssertGameOver(EndingResult.EnvironmentDefeat);

        }

        [Test()]
        public void TestLeftBehind_GameOverChange_OtherTargetKillsKaargra()
        {
            SetupGameController("KaargraWarfang", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            FlipCard(warfang.CharacterCard);
            SetHitPoints(bunker, 10);
            //Play this card next to the hero with the second lowest HP.
            Card left = PlayCard("LeftBehind");
            AssertNextToCard(left, ra.CharacterCard);
            DealDamage(bunker, warfang, 10000, DamageType.Projectile);
            AssertGameOver(EndingResult.EnvironmentDefeat);

        }

        [Test()]
        public void TestLeftBehind_GameOverChange_OtherTargetKillsDreamer()
        {
            SetupGameController("TheDreamer", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(bunker, 10);
            SetHitPoints(ra, 11);
            //Play this card next to the hero with the second lowest HP.
            Card left = PlayCard("LeftBehind");
            AssertNextToCard(left, ra.CharacterCard);
            DealDamage(bunker, dreamer, 10000, DamageType.Projectile);
            AssertGameOver(EndingResult.EnvironmentDefeat);

        }

        [Test()]
        public void TestLeftBehind_GameOverChange_TargetKillsDreamer()
        {
            SetupGameController("TheDreamer", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(bunker, 10);
            SetHitPoints(ra, 11);
            //Play this card next to the hero with the second lowest HP.
            Card left = PlayCard("LeftBehind");
            AssertNextToCard(left, ra.CharacterCard);
            DealDamage(ra, dreamer, 10000, DamageType.Projectile);
            AssertGameOver(EndingResult.AlternateDefeat);

        }

        [Test()]
        public void TestLeftBehind_GameOverChange_TargetKillsKaargra()
        {
            SetupGameController("KaargraWarfang", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            FlipCard(warfang.CharacterCard);
            SetHitPoints(bunker, 10);
            //Play this card next to the hero with the second lowest HP.
            Card left = PlayCard("LeftBehind");
            AssertNextToCard(left, ra.CharacterCard);
            DealDamage(ra, warfang, 10000, DamageType.Projectile);
            AssertNotGameOver();

        }

        [Test()]
        public void TestLeftBehind_GameOverChange_TargetKillsVillain()
        {
            SetupGameController("CitizenDawn", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(bunker, 10);
            //Play this card next to the hero with the second lowest HP.
            Card left = PlayCard("LeftBehind");
            AssertNextToCard(left, ra.CharacterCard);
            DealDamage(ra, dawn, 10000, DamageType.Projectile);
            AssertGameOver(EndingResult.VillainDestroyedVictory);

        }

        [Test()]
        public void TestLeftBehind_GameOverChange_NextToIncapped()
        {
            SetupGameController("CitizenDawn", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(bunker, 10);
            //Play this card next to the hero with the second lowest HP.
            Card left = PlayCard("LeftBehind");
            AssertNextToCard(left, ra.CharacterCard);
            DealDamage(dawn, ra, 10000, DamageType.Radiant);
            AssertGameOver(EndingResult.EnvironmentDefeat);

        }

        [Test()]
        public void TestOminousLoop()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            //The first time a One-shot card enters the trash each turn, move it beneath this card. Cards beneath this one are not considered in play
            Card loop = PlayCard("OminousLoop");
            Card os1 = PlayCard("SlashAndBurn");
            AssertUnderCard(loop, os1);
            Card os2 = PlayCard("DeviousDisruption");
            AssertInTrash(os2);
            GoToNextTurn();
            Card ongoing = PutOnDeck("FlameBarrier");
            DiscardTopCards(ra.TurnTaker.Deck, 1);
            AssertInTrash(ongoing);
            Card os3 = PutOnDeck("FireBlast");
            DiscardTopCards(ra.TurnTaker.Deck, 1);
            AssertUnderCard(loop, os3);
            //At the start of the environment turn, destroy this card and put all cards beneath it into play in the order they were placed there
            QuickHPStorage(baron, ra, bunker, haka);
            GoToStartOfTurn(catchwater);
            AssertInTrash(loop);
            AssertInTrash(os1);
            AssertInTrash(os2);
            AssertInTrash(os3);
            QuickHPCheck(-5, -5, -3, 0);
            AssertNumberOfCardsInRevealed(catchwater, 0);

        }

        [Test()]
        public void TestRadioPlaza()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            //Damage dealt to hero targets is irreducible.
            Card radio = PlayCard("RadioPlaza");
            QuickHPStorage(baron, ra, bunker, haka);
            AddReduceDamageTrigger(bunker, true, false, 1);
            AddReduceDamageTrigger(bunker, false, true, 1);

            DealDamage(baron, c => c.IsTarget, 2, DamageType.Fire);

            QuickHPCheck(-1, -2, -2, -2);

            GoToStartOfTurn(catchwater);
            AssertInTrash(radio);

        }

        [Test()]
        public void TestRadioPlazaRevealsCards()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            
            Card radio = PlayCard("RadioPlaza");

            // Check that everything is face up
            AssertFaceUp(baron.TurnTaker.Deck.TopCard);
            AssertFaceUp(ra.TurnTaker.Deck.TopCard);
            AssertFaceUp(bunker.TurnTaker.Deck.TopCard);
            AssertFaceUp(haka.TurnTaker.Deck.TopCard);
            AssertFaceUp(catchwater.TurnTaker.Deck.TopCard);

            Assert.IsTrue(baron.TurnTaker.Deck.TopCard.IsPositionKnown);
            Assert.IsTrue(ra.TurnTaker.Deck.TopCard.IsPositionKnown);
            Assert.IsTrue(bunker.TurnTaker.Deck.TopCard.IsPositionKnown);
            Assert.IsTrue(haka.TurnTaker.Deck.TopCard.IsPositionKnown);
            Assert.IsTrue(catchwater.TurnTaker.Deck.TopCard.IsPositionKnown);

            // Empty a deck and check that doesn't break everything
            do
            {
                AssertFaceUp(ra.TurnTaker.Deck.TopCard);
                Assert.IsTrue(ra.TurnTaker.Deck.TopCard.IsPositionKnown);

                DrawCard(ra);
            }
            while (!ra.TurnTaker.Deck.IsEmpty);

            AssertNumberOfCardsInDeck(ra, 0);
        }

        [Test()]
        public void TestRadioPlazaTriggersAmbuscadeTraps()
        {
            SetupGameController("Ambuscade", "Legacy", "Parse", "Tempest", "Cauldron.CatchwaterHarbor");
            StartGame();
            GoToPlayCardPhase(ambuscade);

            var cs = StackDeck(ambuscade, new[] { "RiggedToDetonate", "RiggedToDetonate", "PersonalCloakingDevice" }).ToArray();
            QuickHPStorage(legacy);

            // Playing the Radio Plaza should trigger the traps
            Card radio = PlayCard("RadioPlaza");

            AssertOnTopOfDeck(cs[0]);
            AssertFaceUp(ambuscade.TurnTaker.Deck.TopCard);
            Assert.IsTrue(ambuscade.TurnTaker.Deck.TopCard.IsPositionKnown);

            // Both traps damage Legacy
            QuickHPCheck(-6);
        }

        [Test()]
        public void TestRadioPlazaPlayedBeforeJohnnyVariantPower()
        {
            SetupGameController("BaronBlade", "JohnnyRocket/MaximumSpeedJohnnyRocketCharacter", "Ra", "Haka", "Cauldron.CatchwaterHarbor");
            TurnTakerController johnny = FindHero("JohnnyRocket");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            MoveAllCards(johnny, johnny.TurnTaker.Deck, johnny.TurnTaker.Trash);
            // these cards have 3 copies each, and cause Johnny to deal exactly 1 damage
            foreach (Card cardToMove in FindCardsWhere((Card c) => c.Identifier == "SonicShattering" || c.Identifier == "FlurryOfBlows"))
            {
                MoveCard(johnny, cardToMove, johnny.TurnTaker.Deck);
            }
            QuickHPStorage(mdp);

            DecisionSelectCards = new Card[] { null, mdp };
            Card radio = PlayCard("RadioPlaza");
            UsePower(johnny.CharacterCard);

            // power draws 1 card and the other 5 are put into play
            AssertNumberOfCardsInDeck(johnny, 0);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestRadioPlazaPlayedAfterJohnnyVariantPower()
        {
            SetupGameController("BaronBlade", "JohnnyRocket/MaximumSpeedJohnnyRocketCharacter", "Ra", "Haka", "Cauldron.CatchwaterHarbor");
            TurnTakerController johnny = FindHero("JohnnyRocket");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            MoveAllCards(johnny, johnny.TurnTaker.Deck, johnny.TurnTaker.Trash);
            foreach (Card cardToMove in FindCardsWhere((Card c) => c.Identifier == "SonicShattering" || c.Identifier == "FlurryOfBlows"))
            {
                MoveCard(johnny, cardToMove, johnny.TurnTaker.Deck);
            }
            QuickHPStorage(mdp);

            DecisionSelectCards = new Card[] { null, mdp };
            UsePower(johnny.CharacterCard);
            Card radio = PlayCard("RadioPlaza");

            AssertNumberOfCardsInDeck(johnny, 0);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestRadioPlaza_Oblivaeon()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.CatchwaterHarbor", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();

            SwitchBattleZone(ra);
            Card radio = PlayCard("RadioPlaza");
            PrintSpecialStringsForCard(radio);
            AssertNumberOfCardSpecialStrings(radio, 6);

            // Check that everything is face up except ra
            AssertFaceUp(oblivaeon.TurnTaker.Deck.TopCard);
            Assert.IsFalse(ra.TurnTaker.Deck.TopCard.IsFaceUp);
            AssertFaceUp(legacy.TurnTaker.Deck.TopCard);
            AssertFaceUp(haka.TurnTaker.Deck.TopCard);
            AssertFaceUp(tachyon.TurnTaker.Deck.TopCard);
            AssertFaceUp(luminary.TurnTaker.Deck.TopCard);
            AssertFaceUp(catchwater.TurnTaker.Deck.TopCard);

            Assert.IsTrue(oblivaeon.TurnTaker.Deck.TopCard.IsPositionKnown);
            Assert.IsFalse(ra.TurnTaker.Deck.TopCard.IsPositionKnown);
            Assert.IsTrue(legacy.TurnTaker.Deck.TopCard.IsPositionKnown);
            Assert.IsTrue(haka.TurnTaker.Deck.TopCard.IsPositionKnown);
            Assert.IsTrue(tachyon.TurnTaker.Deck.TopCard.IsPositionKnown);
            Assert.IsTrue(luminary.TurnTaker.Deck.TopCard.IsPositionKnown);
            Assert.IsTrue(catchwater.TurnTaker.Deck.TopCard.IsPositionKnown);
        }

        [Test()]
        public void TestSmoothCriminal()
        {
            SetupGameController(new string[] { "BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor" });
            StartGame();
            DestroyNonCharacterVillainCards();

            StackAfterShuffle(catchwater.TurnTaker.Deck, new string[] { "RadioPlaza", "LeftBehind", "OminousLoop" });


            PlayCard("SSEscape");
            PlayCard("ToOverbrook");


            int num = GetNumberOfCardsInPlay((Card c) => IsTransport(c));
            //Reduce damage dealt to Gangsters by 1.
            Card smooth = PlayCard("SmoothCriminal");
            Card harkin = PlayCard("HarkinParishJr");
            StackAfterShuffle(catchwater.TurnTaker.Deck, new string[] { "LeftBehind", "OminousLoop" });
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, bunker.CharacterCard, haka.CharacterCard, smooth, harkin);
            DealDamage(baron, c => c.IsTarget, 2, DamageType.Fire);
            QuickHPCheck(-2, -2, -2, -2, -1, -1);

            //At the end of the environment turn, this card deals each hero target X projectile damage, where X is 1 plus the number of Transports in play.
            GoToPlayCardPhase(catchwater);
            QuickHPUpdate();
            AddCantGainHPDamageTrigger(catchwater, true, false);
            AddCantGainHPDamageTrigger(catchwater, false, true);
            GoToEndOfTurn(catchwater);
            QuickHPCheck(0, -1 - num, -1 - num, -4 - num, 0, 0);

        }

        [Test()]
        public void TestTheCervantesClub_Xis1_NoOneShot()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(ra, 10);
            GoToPlayCardPhase(catchwater);
            //At the end of the environment turn, 1 hero character regains X HP and discards the top X cards of their deck, where X is 1, 2, or 3.
            //If any One-shots were discarded this way, that player discards 2 cards, then draws a card.
            PlayCard("TheCervantesClub");


            Card raTop = PutOnDeck("FlameBarrier");
            DecisionSelectCard = ra.CharacterCard;
            DecisionSelectFunction = 0;
            QuickHandStorage(ra);
            QuickHPStorage(ra);
            GoToEndOfTurn(catchwater);
            QuickHPCheck(1);
            AssertInTrash(raTop);
            QuickHandCheckZero();

        }

        [Test()]
        public void TestTheCervantesClub_Xis1_OneShot()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(ra, 10);
            GoToPlayCardPhase(catchwater);
            //At the end of the environment turn, 1 hero character regains X HP and discards the top X cards of their deck, where X is 1, 2, or 3.
            //If any One-shots were discarded this way, that player discards 2 cards, then draws a card.
            PlayCard("TheCervantesClub");


            Card raTop = PutOnDeck("FireBlast");
            DecisionSelectCards = new Card[] { ra.CharacterCard, ra.HeroTurnTaker.Hand.TopCard, ra.HeroTurnTaker.Hand.GetTopCards(2).ElementAt(1) };
            DecisionSelectFunction = 0;
            QuickHandStorage(ra);
            QuickHPStorage(ra);
            GoToEndOfTurn(catchwater);
            QuickHPCheck(1);
            AssertInTrash(raTop);
            QuickHandCheck(-1);

        }

        [Test()]
        public void TestTheCervantesClub_Xis2_OneShot()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(ra, 10);
            GoToPlayCardPhase(catchwater);
            //At the end of the environment turn, 1 hero character regains X HP and discards the top X cards of their deck, where X is 1, 2, or 3.
            //If any One-shots were discarded this way, that player discards 2 cards, then draws a card.
            PlayCard("TheCervantesClub");


            Card raTop = PutOnDeck("FireBlast");
            Card raTop2 = ra.TurnTaker.Deck.GetTopCards(2).ElementAt(1);
            DecisionSelectCards = new Card[] { ra.CharacterCard, ra.HeroTurnTaker.Hand.TopCard, ra.HeroTurnTaker.Hand.GetTopCards(2).ElementAt(1) };
            DecisionSelectFunction = 1;
            QuickHandStorage(ra);
            QuickHPStorage(ra);
            GoToEndOfTurn(catchwater);
            QuickHPCheck(2);
            AssertInTrash(raTop);
            AssertInTrash(raTop2);
            QuickHandCheck(-1);

        }

        [Test()]
        public void TestTheCervantesClub_ExcludesOffToSideSky()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "SkyScraper", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(ra, 10);
            GoToPlayCardPhase(catchwater);
            //At the end of the environment turn, 1 hero character regains X HP and discards the top X cards of their deck, where X is 1, 2, or 3.
            //If any One-shots were discarded this way, that player discards 2 cards, then draws a card.
            PlayCard("TheCervantesClub");


            Card raTop = PutOnDeck("FireBlast");
            Card raTop2 = ra.TurnTaker.Deck.GetTopCards(2).ElementAt(1);
            IEnumerable<Card> offToSideSky = sky.TurnTaker.OffToTheSide.Cards.Where(c => c.IsCharacter);
            AssertNextDecisionChoices(notIncluded: offToSideSky);
            DecisionSelectCards = new Card[] { ra.CharacterCard, ra.HeroTurnTaker.Hand.TopCard, ra.HeroTurnTaker.Hand.GetTopCards(2).ElementAt(1) };
            DecisionSelectFunction = 1;
            QuickHandStorage(ra);
            QuickHPStorage(ra);
            GoToEndOfTurn(catchwater);
            QuickHPCheck(2);
            AssertInTrash(raTop);
            AssertInTrash(raTop2);
            QuickHandCheck(-1);

        }

        [Test()]
        public void TestTheCervantesClub_Xis2_NoOneShot()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(ra, 10);
            GoToPlayCardPhase(catchwater);
            //At the end of the environment turn, 1 hero character regains X HP and discards the top X cards of their deck, where X is 1, 2, or 3.
            //If any One-shots were discarded this way, that player discards 2 cards, then draws a card.
            PlayCard("TheCervantesClub");


            Card raTop = PutOnDeck("FlameBarrier");
            Card raTop2 = PutOnDeck("TheStaffOfRa");
            DecisionSelectCard = ra.CharacterCard;
            DecisionSelectFunction = 1;
            QuickHandStorage(ra);
            QuickHPStorage(ra);
            GoToEndOfTurn(catchwater);
            QuickHPCheck(2);
            AssertInTrash(raTop);
            AssertInTrash(raTop2);
            QuickHandCheckZero();

        }

        [Test()]
        public void TestTheCervantesClub_Xis3_OneShot()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(ra, 10);
            GoToPlayCardPhase(catchwater);
            //At the end of the environment turn, 1 hero character regains X HP and discards the top X cards of their deck, where X is 1, 2, or 3.
            //If any One-shots were discarded this way, that player discards 2 cards, then draws a card.
            PlayCard("TheCervantesClub");


            Card raTop = PutOnDeck("FireBlast");
            Card raTop2 = ra.TurnTaker.Deck.GetTopCards(2).ElementAt(1);
            Card raTop3 = ra.TurnTaker.Deck.GetTopCards(3).ElementAt(2);

            DecisionSelectCards = new Card[] { ra.CharacterCard, ra.HeroTurnTaker.Hand.TopCard, ra.HeroTurnTaker.Hand.GetTopCards(2).ElementAt(1) };
            DecisionSelectFunction = 2;
            QuickHandStorage(ra);
            QuickHPStorage(ra);
            GoToEndOfTurn(catchwater);
            QuickHPCheck(3);
            AssertInTrash(raTop);
            AssertInTrash(raTop2);
            AssertInTrash(raTop3);

            QuickHandCheck(-1);

        }

        [Test()]
        public void TestTheCervantesClub_Xis3_NoOneShot()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            SetHitPoints(ra, 10);
            GoToPlayCardPhase(catchwater);
            //At the end of the environment turn, 1 hero character regains X HP and discards the top X cards of their deck, where X is 1, 2, or 3.
            //If any One-shots were discarded this way, that player discards 2 cards, then draws a card.
            PlayCard("TheCervantesClub");


            Card raTop = PutOnDeck("FlameBarrier");
            Card raTop2 = PutOnDeck("TheStaffOfRa");
            Card raTop3 = PutOnDeck("BlazingTornado");

            DecisionSelectCard = ra.CharacterCard;
            DecisionSelectFunction = 2;
            QuickHandStorage(ra);
            QuickHPStorage(ra);
            GoToEndOfTurn(catchwater);
            QuickHPCheck(3);
            AssertInTrash(raTop);
            AssertInTrash(raTop2);
            AssertInTrash(raTop3);
            QuickHandCheckZero();

        }


        [Test()]
        public void TestThisJustIn_Discard()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            // When this card enters play, 1 player may discard 3 cards.
            //If no cards are discarded this way, play the top card of the villain deck.
            GoToPlayCardPhase(catchwater);
            DecisionSelectTurnTaker = bunker.TurnTaker;
            Card[] discardCards = bunker.HeroTurnTaker.Hand.GetTopCards(3).ToArray();
            DecisionSelectCards = discardCards;
            DecisionYesNo = true;
            Card mdp = PutOnDeck("MobileDefensePlatform");
            Card thisJustIn = PlayCard("ThisJustIn");
            AssertInTrash(discardCards);
            AssertOnTopOfDeck(mdp);

            // At the end of the environment turn, destroy this card.
            GoToEndOfTurn(catchwater);
            AssertInTrash(thisJustIn);

        }

        [Test()]
        public void TestThisJustIn_NoDiscard()
        {
            SetupGameController("BaronBlade", "Ra", "Bunker", "Haka", "Cauldron.CatchwaterHarbor");
            StartGame();
            DestroyNonCharacterVillainCards();
            // When this card enters play, 1 player may discard 3 cards.
            //If no cards are discarded this way, play the top card of the villain deck.
            GoToPlayCardPhase(catchwater);
            DecisionSelectTurnTaker = bunker.TurnTaker;
            Card[] discardCards = bunker.HeroTurnTaker.Hand.GetTopCards(3).ToArray();
            DecisionYesNo = false;
            Card mdp = PutOnDeck("MobileDefensePlatform");
            Card thisJustIn = PlayCard("ThisJustIn");
            AssertInHand(discardCards);
            AssertIsInPlay(mdp);

            // At the end of the environment turn, destroy this card.
            GoToEndOfTurn(catchwater);
            AssertInTrash(thisJustIn);

        }
    }

}
