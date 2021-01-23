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

            AssertIsInPlay(FutureDriftCharacter);

            DecisionYesNo = true;
            GoToPlayCardPhase(drift);

            AssertIsInPlay(PastDriftCharacter);
            GoToPlayCardPhase(haka);

            AssertIsInPlay(FutureDriftCharacter);
        }

        [Test()]
        public void TestDriftCharacter_Dual_Past_InnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DecisionYesNo = true;
            GoToPlayCardPhase(drift);

            //Reveal the top 2 cards of 1 hero deck. Replace or discard each of them in any order. Shift {DriftLL}.
            int shiftPosition = CurrentShiftPosition();
            UsePower(drift);
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
        public void TestDriftCharacter_Incap0()
        {
            SetupGameController("Apostate", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard(drift);
            //One hero may use a power now.

            //Haka deals 2 damage
            DecisionSelectTurnTaker = haka.TurnTaker;
            QuickHPStorage(apostate);
            UseIncapacitatedAbility(drift, 0);
            QuickHPCheck(-2);

            //Bunker draws 1
            DecisionSelectTurnTaker = bunker.TurnTaker;
            QuickHandStorage(bunker);
            UseIncapacitatedAbility(drift, 0);
            QuickHandCheck(1);

            SetHitPoints(scholar, 17);
            //Scholar heals 1
            DecisionSelectTurnTaker = scholar.TurnTaker;
            QuickHPStorage(scholar);
            UseIncapacitatedAbility(drift, 0);
            QuickHPCheck(1);
        }

        [Test()]
        public void TestDriftCharacter_Incap1()
        {
            SetupGameController("Apostate", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard(drift);
            //One target regains 1 HP and deals another target 1 radiant damage.

            DecisionSelectCards = new Card[] { haka.CharacterCard, apostate.CharacterCard };

            SetHitPoints(haka, 17);
            QuickHPStorage(haka, apostate);
            UseIncapacitatedAbility(drift, 1);
            QuickHPCheck(1, -1);
        }

        [Test()]
        public void TestDriftCharacter_Incap2()
        {
            SetupGameController("Apostate", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard(drift);
            GoToStartOfTurn(drift);
            //One player may discard a one-shot. If they do, they may draw 2 cards.
            var a = base.GameController.FindTurnTakersWhere(tt => !tt.IsIncapacitatedOrOutOfGame);

            Card elbow = PutInHand("ElbowSmash");
            QuickHandStorage(haka);
            UseIncapacitatedAbility(drift, 2);
            //Discard 1, Draw 2
            QuickHandCheck(1);

            DiscardAllCards(haka, bunker, scholar);
            QuickHandStorage(haka, bunker, scholar);
            UseIncapacitatedAbility(drift, 2);
            //With no discard, no draw
            QuickHandCheckZero();
        }

        [Test]
        public void TestShiftTrackSetup()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card track = FindCardsWhere((Card c) => c.Identifier == "Base" + ShiftTrack + 1, false).FirstOrDefault();
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
