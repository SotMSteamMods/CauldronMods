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
    public class DriftDualVariantTests : CauldronBaseTest
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
        protected const string PastDriftCharacter = "PastDriftCharacter";
        protected const string FutureDriftCharacter = "FutureDriftCharacter";

        [Test()]
        [Order(0)]
        public void TestDriftLoad_Dual()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(drift);
            Assert.IsInstanceOf(typeof(DualDriftCharacterCardController), drift.CharacterCardController);

            foreach (var card in drift.HeroTurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            //Assert.AreEqual(26, drift.CharacterCard.HitPoints);
            AssertMaximumHitPoints(GetCard("PastDriftCharacter"), 15);
            AssertMaximumHitPoints(GetCard("FutureDriftCharacter"), 16);
        }

        [Test()]
        public void TestDriftCharacter_StartWithFuture()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            AssertIsInPlay(FutureDriftCharacter);
        }

        [Test, Ignore("Decisions before start don't seem to work. Tested in game and it works.")]
        public void TestDriftCharacter_StartWithPast()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            DecisionSelectCard = GetCard(FutureDriftCharacter);
            StartGame();

            AssertIsInPlay(PastDriftCharacter);
        }

        [Test()]
        public void TestDriftCharacter_SwitchActiveHero()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //Start with Future
            AssertIsInPlay(FutureDriftCharacter);
            int position1 = CurrentShiftPosition();

            //Shift Right 1
            PlayCard(DestroyersAdagio);
            AssertTrackPosition(position1 + 1);
            int position2 = CurrentShiftPosition();

            //Trigger switch with phase change
            DecisionYesNo = true;
            GoToEndOfTurn(baron);

            //Assert that other character and starting position are active
            AssertIsInPlay(PastDriftCharacter);
            AssertTrackPosition(position1);

            //Switch back
            GoToPlayCardPhase(drift);

            //Assert in secondary position
            AssertIsInPlay(FutureDriftCharacter);
            AssertTrackPosition(position2);
        }

        [Test()]
        public void TestDriftCharacter_Dual_Past_InnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //Switch to Past and Shift to far Right
            DecisionYesNo = true;
            DecisionSelectFunction = 1;
            DecisionSelectNumber = 3;
            PlayCard(BorrowedTime);

            int shiftPosition = CurrentShiftPosition();
            Card[] top2 = GetTopCardsOfDeck(drift, 2).ToArray();
            DecisionMoveCardDestinations = new MoveCardDestination[]
            {
                new MoveCardDestination(drift.TurnTaker.Trash),
                new MoveCardDestination(drift.TurnTaker.Deck)
            };

            //Reveal the top 2 cards of 1 hero deck. Replace or discard each of them in any order. Shift {DriftLL}.

            UsePower(drift);
            AssertTrackPosition(shiftPosition - 2);
            AssertOnTopOfDeck(top2[1]);
            AssertInTrash(top2[0]);
        }

        [Test()]
        public void TestDriftCharacter_Dual_Future_InnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card fFocus = PutInHand(FutureFocus);
            DecisionSelectCard = fFocus;

            //Play an ongoing card. At the end of your next turn, return it from play to your hand. Shift {DriftRR}.
            int shiftPosition = CurrentShiftPosition();
            UsePower(drift);
            AssertIsInPlay(fFocus);
            AssertTrackPosition(shiftPosition + 2);

            GoToEndOfTurn(drift);
            AssertInHand(fFocus);
        }

        [Test()]
        public void TestDriftCharacter_Future_Incap0()
        {
            SetupGameController("Apostate", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DecisionYesNo = true;
            DestroyCard(drift);
            //One player may draw a card now.

            DecisionSelectTurnTaker = bunker.TurnTaker;
            QuickHandStorage(bunker);
            UseIncapacitatedAbility(drift, 0);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestDriftCharacter_Future_Incap1()
        {
            SetupGameController("Apostate", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard(drift);
            //Reveal the top card of a hero deck and replace it. If that card has a power on it. Play it and that hero uses that power.

            Card moko = PutOnDeck("TaMoko");
            Card battle = PutOnDeck("HakaOfBattle");
            Card mere = PutOnDeck("Mere");

            //Mere
            QuickHandStorage(haka);
            QuickHPStorage(apostate);
            UseIncapacitatedAbility(drift, 1);
            QuickHandCheck(1);
            QuickHPCheck(-2);
            AssertIsInPlay(mere);

            //TaMoko
            QuickHandStorage(haka);
            QuickHPStorage(apostate);
            UseIncapacitatedAbility(drift, 1);
            QuickHandCheck(0);
            QuickHPCheck(0);
            AssertOnTopOfDeck(moko);
        }

        [Test()]
        public void TestDriftCharacter_Future_Incap2()
        {
            SetupGameController("Apostate", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard(drift);
            SetHitPoints(apostate, 17);

            //One target regains 2 HP.

            QuickHPStorage(apostate);
            UseIncapacitatedAbility(drift, 2);
            //Discard 1, Draw 2
            QuickHPCheck(2);
        }

        [Test]
        public void TestShiftTrackSetup()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card track = FindCardsWhere((Card c) => c.Identifier == "Dual" + ShiftTrack + 1, false).FirstOrDefault();
            DecisionSelectCard = track;
            StartGame();

            Assert.AreEqual(1, CurrentShiftPosition());
            AssertIsInPlay(track);
        }

        [Test, Sequential, Ignore("Picking a ShiftTrack by Identifier always returns the first one. Testing in game confirms this works.")]
        public void TestShiftTrackSetup_Other([Values(2, 3, 4)] int decision)
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card track = FindCardsWhere((Card c) => c.Identifier == ShiftTrack + decision, false).FirstOrDefault();
            DecisionSelectCard = track;
            StartGame();

            Assert.AreEqual(decision, CurrentShiftPosition());
            AssertIsInPlay(track);
        }
    }
}
