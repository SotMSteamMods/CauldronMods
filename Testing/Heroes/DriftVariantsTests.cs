using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Drift;

namespace CauldronTests
{
    [TestFixture()]
    public class DriftVariantsTests : CauldronBaseTest
    {
        protected const string AttenuationField = "AttenuationField";
        protected const string BorrowedTime = "BorrowedTime";
        protected const string DanceOfTheDragons = "DanceOfTheDragons";
        protected const string DestroyersAdagio = "DestroyersAdagio";
        protected const string DriftStep = "DriftStep";
        protected const string FutureFocus = "FutureFocus";
        protected const string ImposedSynchronization = "ImposedSynchronization";
        protected const string KnightsHeritage = "KnightsHeritage";
        protected const string MakeEverySecondCount = "MakeEverySecondCount";
        protected const string OutOfSync = "OutOfSync";
        protected const string PastFocus = "PastFocus";
        protected const string ResourcefulDaydreamer = "ResourcefulDaydreamer";
        protected const string Sabershard = "Sabershard";
        protected const string ThrowingShard = "ThrowingShard";
        protected const string TransitionShock = "TransitionShock";

        protected const string ShiftTrack = "ShiftTrack";

        [Test()]
        [Order(0)]
        public void TestDrift_1609_Load()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DriftingShadowDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(drift);
            Assert.IsInstanceOf(typeof(DriftingShadowDriftCharacterCardController), drift.CharacterCardController);

            foreach (var card in drift.HeroTurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(26, drift.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestDriftCharacter_1609_InnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DriftingShadowDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            SetHitPoints(drift, 17);
            //At the start of your next turn, shift {DriftL} or {DriftR}, then draw a card or use a power.

            DecisionSelectFunction = 1;
            GoToUsePowerPhase(drift);
            int trackPosition = CurrentShiftPosition();

            QuickHPStorage(drift);
            QuickHandStorage(drift);
            UsePower(drift);
            QuickHPCheck(0);
            QuickHandCheck(0);
            AssertTrackPosition(trackPosition);

            GoToEndOfTurn(baron);
            QuickHPCheck(0);
            QuickHandCheck(0);
            AssertTrackPosition(trackPosition);

            GoToStartOfTurn(drift);
            QuickHPCheck(0);
            QuickHandCheck(0);
            AssertTrackPosition(trackPosition + 1);

            trackPosition = CurrentShiftPosition();

            DecisionSelectFunction = 0;
            GoToStartOfTurn(drift);
            QuickHPCheck(0);
            QuickHandCheck(1);
            AssertTrackPosition(trackPosition - 1);
        }

        [Test()]
        public void TestDriftCharacter_1609_Incap0()
        {
            SetupGameController("Apostate", "Cauldron.Drift/DriftingShadowDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard(drift);
            //One hero may play a card now.

            Card moko = PutInHand("TaMoko");
            DecisionSelectCard = moko;
            UseIncapacitatedAbility(drift, 0);
            AssertIsInPlay(moko);
        }

        [Test()]
        public void TestDriftCharacter_1609_Incap1()
        {
            SetupGameController("Apostate", "Cauldron.Drift/DriftingShadowDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard(drift);
            //One hero may use a power now.

            //Haka deals 2 damage
            DecisionSelectTurnTaker = haka.TurnTaker;
            QuickHPStorage(apostate);
            UseIncapacitatedAbility(drift, 1);
            QuickHPCheck(-2);

            //Bunker draws 1
            DecisionSelectTurnTaker = bunker.TurnTaker;
            QuickHandStorage(bunker);
            UseIncapacitatedAbility(drift, 1);
            QuickHandCheck(1);

            SetHitPoints(scholar, 17);
            //Scholar heals 1
            DecisionSelectTurnTaker = scholar.TurnTaker;
            QuickHPStorage(scholar);
            UseIncapacitatedAbility(drift, 1);
            QuickHPCheck(1);
        }

        [Test()]
        public void TestDriftCharacter_1609_Incap2()
        {
            SetupGameController("Apostate", "Cauldron.Drift/DriftingShadowDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card moko = PlayCard("TaMoko");
            Card flak = PlayCard("FlakCannon");
            Card iron = PlayCard("FleshToIron");

            DestroyCard(drift);
            //Move up to 3 non-character hero cards from play to their owner' hands.
            UseIncapacitatedAbility(drift, 2);
            AssertInHand(moko, flak, iron);
        }

        [Test, Sequential]
        public void TestShiftTrackSetup_1609([Values(1, 2, 3, 4)] int decision)
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DriftingShadowDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card track = FindCardsWhere((Card c) => c.Identifier == "Base" + ShiftTrack + decision, false).FirstOrDefault();
            DecisionSelectCard = track;
            StartGame(false);

            Assert.AreEqual(decision, CurrentShiftPosition());
            AssertIsInPlay(track);
        }

        [Test()]
        [Order(0)]
        public void TestDrift_1789_Load()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/AllInGoodTimeDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(drift);
            Assert.IsInstanceOf(typeof(AllInGoodTimeDriftCharacterCardController), drift.CharacterCardController);

            foreach (var card in drift.HeroTurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(28, drift.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestDriftCharacter_1789_InnatePower_Play()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/AllInGoodTimeDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            StackDeck(TransitionShock, BorrowedTime, DestroyersAdagio, DanceOfTheDragons);

            //Discard cards from the top of your deck until you discard an ongoing. Play or draw it.

            UsePower(drift);
            FindCardInPlay(TransitionShock);
            AssertNumberOfCardsInTrash(drift, 3);
        }

        [Test()]
        public void TestDriftCharacter_1789_InnatePower_NoOngoings()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/AllInGoodTimeDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            // move all ongoings to the trash
            MoveCards(drift, c => c.Location == drift.TurnTaker.Deck && c.IsOngoing, drift.TurnTaker.Trash);

            //Discard cards from the top of your deck until you discard an ongoing. Play or draw it.

            int cardsInTrash = drift.TurnTaker.Trash.NumberOfCards;
            int cardsInDeck = drift.TurnTaker.Deck.NumberOfCards;

            AssertNextDecisionSelectionTypeIsNot(SelectionType.SelectFunction);
            UsePower(drift);
            AssertNumberOfCardsInDeck(drift, 0);
            AssertNumberOfCardsInTrash(drift, cardsInTrash + cardsInDeck);
        }

        [Test()]
        public void TestDriftCharacter_1789_InnatePower_Draw()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/AllInGoodTimeDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            StackDeck(TransitionShock, BorrowedTime, DestroyersAdagio, DanceOfTheDragons);

            //Discard cards from the top of your deck until you discard an ongoing. Play or draw it.
            DecisionSelectFunction = 1;
            QuickHandStorage(drift);
            UsePower(drift);
            QuickHandCheck(1);
            AssertNumberOfCardsInTrash(drift, 3);
        }

        [Test()]
        public void TestDriftCharacter_1789_Incap0()
        {
            SetupGameController("Apostate", "Cauldron.Drift/AllInGoodTimeDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard(drift);
            //One hero may play a card now.

            Card moko = PutInHand("TaMoko");
            DecisionSelectCard = moko;
            UseIncapacitatedAbility(drift, 0);
            AssertIsInPlay(moko);
        }

        [Test()]
        public void TestDriftCharacter_1789_Incap1()
        {
            SetupGameController("Apostate", "Cauldron.Drift/AllInGoodTimeDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard(drift);
            //Destroy 1 ongoing card.

            Card moko = PlayCard("TaMoko");
            UseIncapacitatedAbility(drift, 1);
            AssertInTrash(moko);
        }

        [Test()]
        public void TestDriftCharacter_1789_Incap2()
        {
            SetupGameController("Apostate", "Cauldron.Drift/AllInGoodTimeDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard(drift);
            //Select a target. Increase the next damage it deals by 2.

            QuickHPStorage(apostate, haka, bunker, scholar);
            UseIncapacitatedAbility(drift, 2);
            QuickHPCheckZero();

            //Only selected target
            QuickHPStorage(apostate, haka, bunker, scholar);
            DealDamage(haka, bunker, 2, DamageType.Melee);
            QuickHPCheck(0, 0, -2, 0);

            //+2 damage
            QuickHPStorage(apostate, haka, bunker, scholar);
            DealDamage(apostate, haka, 2, DamageType.Melee);
            QuickHPCheck(0, -4, 0, 0);

            //Only once
            QuickHPStorage(apostate, haka, bunker, scholar);
            DealDamage(apostate, haka, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0, 0);
        }

        [Test, Sequential]
        public void TestShiftTrackSetup_1789([Values(1, 2, 3, 4)] int decision)
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/AllInGoodTimeDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card track = FindCardsWhere((Card c) => c.Identifier == "Base" + ShiftTrack + decision, false).FirstOrDefault();
            DecisionSelectCard = track;
            StartGame(false);

            Assert.AreEqual(decision, CurrentShiftPosition());
            AssertIsInPlay(track);
        }

        [Test()]
        [Order(0)]
        public void TestDrift_Halberd_Load()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/TestSubjectHalberdDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(drift);
            Assert.IsInstanceOf(typeof(TestSubjectHalberdDriftCharacterCardController), drift.CharacterCardController);

            foreach (var card in drift.HeroTurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(26, drift.CharacterCard.HitPoints);
        }

        [Test, Sequential]
        public void TestDriftCharacter_Halberd_InnatePower_ShiftRight([Values(1, 2, 3)] int numDiscard)
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/TestSubjectHalberdDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //Discard 1, 2, or 3 cards. For each card discarded this way, shift {DriftL} or {DriftR}. Draw 2 cards.

            DecisionSelectWord = numDiscard.ToString();
            DecisionSelectFunction = 1;
            Log.Debug($"There are {drift.HeroTurnTaker.Hand.NumberOfCards} cards in Drift's hand.");
            int shiftPosition = CurrentShiftPosition();
            UsePower(drift);
            AssertTrackPosition(shiftPosition + numDiscard);
            AssertNumberOfCardsInHand(drift, 6 - numDiscard);
        }

        [Test, Sequential]
        public void TestDriftCharacter_Halberd_InnatePower_ShiftLeft([Values(1, 2, 3)] int numDiscard)
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/TestSubjectHalberdDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //Discard 1, 2, or 3 cards. For each card discarded this way, shift {DriftL} or {DriftR}. Draw 2 cards.
            Log.Debug($"There are {drift.HeroTurnTaker.Hand.NumberOfCards} cards in Drift's hand.");

            //Get to Position 4
            DecisionSelectWord = 3.ToString(); ;
            GoToShiftPosition(4);

            //Setup
            DecisionSelectWord = numDiscard.ToString();
            DecisionSelectFunction = 0;

            int shiftPosition = CurrentShiftPosition();
            UsePower(drift); 
            AssertNumberOfCardsInHand(drift, 6 - numDiscard);
            AssertTrackPosition(shiftPosition - numDiscard);
        }

        [Test]
        public void TestDriftCharacter_Halberd_InnatePower_ShiftMix()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/TestSubjectHalberdDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //Discard 1, 2, or 3 cards. For each card discarded this way, shift {DriftL} or {DriftR}. Draw 2 cards.

            //Setup
            DecisionSelectWord = 3.ToString();
            //Right, Left, Right
            DecisionSelectFunctions = new int?[] { 1, 0, 1 };

            int shiftPosition = CurrentShiftPosition();
            QuickHandStorage(drift);
            UsePower(drift);
            QuickHandCheck(-1);
            AssertTrackPosition(shiftPosition + 1);
        }

        [Test()]
        public void TestDriftCharacter_Halberd_Incap0()
        {
            SetupGameController("Apostate", "Cauldron.Drift/TestSubjectHalberdDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard(drift);
            //One player may draw a card now.

            QuickHandStorage(haka);
            UseIncapacitatedAbility(drift, 0);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestDriftCharacter_Halberd_Incap1()
        {
            SetupGameController("Apostate", "Cauldron.Drift/TestSubjectHalberdDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard(drift);
            //One hero may use a power now.

            //Haka deals 2 damage
            DecisionSelectTurnTaker = haka.TurnTaker;
            QuickHPStorage(apostate);
            UseIncapacitatedAbility(drift, 1);
            QuickHPCheck(-2);

            //Bunker draws 1
            DecisionSelectTurnTaker = bunker.TurnTaker;
            QuickHandStorage(bunker);
            UseIncapacitatedAbility(drift, 1);
            QuickHandCheck(1);

            SetHitPoints(scholar, 17);
            //Scholar heals 1
            DecisionSelectTurnTaker = scholar.TurnTaker;
            QuickHPStorage(scholar);
            UseIncapacitatedAbility(drift, 1);
            QuickHPCheck(1);
        }

        [Test()]
        public void TestDriftCharacter_Halberd_Incap2()
        {
            SetupGameController("Apostate", "Cauldron.Drift/TestSubjectHalberdDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard(drift);
            //Select a target. Prevent the next damage dealt to it.

            QuickHPStorage(apostate, haka, bunker, scholar);
            UseIncapacitatedAbility(drift, 2);
            QuickHPCheckZero();

            //Only selected target
            QuickHPStorage(apostate, haka, bunker, scholar);
            DealDamage(haka, bunker, 2, DamageType.Melee);
            QuickHPCheck(0, 0, -2, 0);

            //+2 damage
            QuickHPStorage(apostate, haka, bunker, scholar);
            DealDamage(haka, apostate, 2, DamageType.Melee);
            QuickHPCheckZero();

            //Only once
            QuickHPStorage(apostate, haka, bunker, scholar);
            DealDamage(haka, apostate, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0);
        }

        [Test, Sequential]
        public void TestShiftTrackSetup_Halberd_Other([Values(1, 2, 3, 4)] int decision)
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/TestSubjectHalberdDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card track = FindCardsWhere((Card c) => c.Identifier == "Base" + ShiftTrack + decision, false).FirstOrDefault();
            DecisionSelectCard = track;
            StartGame(false);

            Assert.AreEqual(decision, CurrentShiftPosition());
            AssertIsInPlay(track);
        }

        [Test()]
        [Order(0)]
        public void TestDrift_Breach_Load()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/ThroughTheBreachDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(drift);
            Assert.IsInstanceOf(typeof(ThroughTheBreachDriftCharacterCardController), drift.CharacterCardController);

            foreach (var card in drift.HeroTurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(25, drift.CharacterCard.HitPoints);
        }

        [Test]
        public void TestDriftCharacter_Breach_InnatePower()
        {
            SetupGameController("Apostate", "Cauldron.Drift/ThroughTheBreachDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DecisionYesNo = true;
            //Add the top 2 cards of your deck to your shift track, or discard the card from your current shift track space.
            StackDeck(FutureFocus);
            Card step = PutInTrash(DriftStep);
            Card[] top2 = GetTopCardsOfDeck(drift, 2).ToArray();
            UsePower(drift);
            AssertUnderCard(GetPositionalBreachShiftTrack(1), top2[0]);
            AssertUnderCard(GetPositionalBreachShiftTrack(2), top2[1]);

            //When you discard a card from the track, you may play it or {Drift} may deal 1 target 3 radiant damage.
            QuickHPStorage(apostate);
            DecisionSelectFunctions = new int?[] { 1, 0, 1, 0, 1, 1 };
            UsePower(drift);
            AssertIsInPlay(top2[0]);
            QuickHPCheck(0);

            //To shift to position 2
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            PlayCard(step);

            PrintSpecialStringsForCard(drift.CharacterCard);
            UsePower(drift);
            PrintSpecialStringsForCard(drift.CharacterCard);

            QuickHPUpdate();
            UsePower(drift);
            AssertInTrash(top2[1]);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestDriftCharacter_Breach_Incap0()
        {
            SetupGameController("Apostate", "Cauldron.Drift/ThroughTheBreachDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard(drift);
            //One player may draw a card now.

            QuickHandStorage(haka);
            UseIncapacitatedAbility(drift, 0);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestDriftCharacter_Breach_Incap1()
        {
            SetupGameController("Apostate", "Cauldron.Drift/ThroughTheBreachDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard(drift);
            //One player may draw a card now.

            QuickHPStorage(apostate);
            QuickHandStorage(bunker);
            UseIncapacitatedAbility(drift, 1);
            QuickHandCheck(1);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestDriftCharacter_Breach_Incap2()
        {
            SetupGameController("Apostate", "Cauldron.Drift/ThroughTheBreachDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card moko = PlayCard("TaMoko");
            Card popo = PlayCard("PoliceBackup");
            Card sword = FindCardInPlay("Condemnation");

            DestroyCard(drift);

            //Move 1 environment card from play to the of its deck.
            UseIncapacitatedAbility(drift, 2);
            AssertIsInPlay(moko, sword);
            AssertOnTopOfDeck(popo);
        }
    }
}
