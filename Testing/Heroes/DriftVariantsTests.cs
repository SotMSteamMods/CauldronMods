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
        public void TestDriftCharacter_Incap0()
        {
            SetupGameController("Apostate", "Cauldron.Drift/DriftingShadowDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
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
            SetupGameController("Apostate", "Cauldron.Drift/DriftingShadowDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
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
            SetupGameController("Apostate", "Cauldron.Drift/DriftingShadowDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
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
            SetupGameController("BaronBlade", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card track = FindCardsWhere((Card c) => c.Identifier == "Base" + ShiftTrack + 1, false).FirstOrDefault();
            DecisionSelectCard = track;
            StartGame();

            Assert.AreEqual(1, CurrentShiftPosition());
            AssertIsInPlay(track);
        }

        [Test, Sequential, Ignore("Picking a ShiftTrack by Identifier always returns the first one. Testing in game confirms this works.")]
        public void TestShiftTrackSetup_Other([Values(2, 3, 4)] int decision)
        {
            SetupGameController("BaronBlade", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card track = FindCardsWhere((Card c) => c.Identifier == ShiftTrack + decision, false).FirstOrDefault();
            DecisionSelectCard = track;
            StartGame();

            Assert.AreEqual(decision, CurrentShiftPosition());
            AssertIsInPlay(track);
        }
    }
}
