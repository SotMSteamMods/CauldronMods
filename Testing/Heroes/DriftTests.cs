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
    public class DriftTests : BaseTest
    {
        protected HeroTurnTakerController drift { get { return FindHero("Drift"); } }

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

        public int CurrentShiftPosition()
        {
            return this.GetShiftPool().CurrentValue;
        }

        public TokenPool GetShiftPool()
        {
            return this.GetShiftTrack().FindTokenPool("ShiftPool");
        }

        public Card GetShiftTrack()
        {
            return base.FindCardsWhere((Card c) => c.SharedIdentifier == ShiftTrack && c.IsInPlayAndHasGameText, false).FirstOrDefault();
        }

        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                AssertCardHasKeyword(card, keyword, false);
            }
        }

        private void AssertTrackPosition(int expectedPosition)
        {
            Assert.AreEqual(expectedPosition, CurrentShiftPosition(), "Expected position: " + expectedPosition + ", was: " + CurrentShiftPosition());
        }

        private void GoToShiftPosition(int position)
        {
            DecisionSelectFunction = 2;
            for (int i = 1; i < position; i++)
            {
                UsePower(drift);
            }
        }

        [Test()]
        [Order(0)]
        public void TestDriftLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(drift);
            Assert.IsInstanceOf(typeof(DriftCharacterCardController), drift.CharacterCardController);

            foreach (var card in drift.HeroTurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(26, drift.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestDriftDecklist()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            AssertHasKeyword("focus", new[]
            {
                FutureFocus,
                PastFocus
            });

            AssertHasKeyword("one-shot", new[]
            {
                AttenuationField,
                BorrowedTime,
                DanceOfTheDragons,
                DestroyersAdagio,
                DriftStep,
                ImposedSynchronization,
                ResourcefulDaydreamer
            });

            AssertHasKeyword("ongoing", new[]
            {
                FutureFocus,
                KnightsHeritage,
                MakeEverySecondCount,
                OutOfSync,
                PastFocus,
                Sabershard,
                ThrowingShard,
                TransitionShock
            });

            AssertHasKeyword("limited", new[]
            {
                MakeEverySecondCount,
                OutOfSync,
                TransitionShock
            });
        }

        [Test()]
        public void TestDriftCharacter_InnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            SetHitPoints(drift, 17);
            //Shift {DriftLL}, {DriftL}, {DriftR}, {DriftRR}. Drift regains 1 HP.

            //Shift Right Twice
            DecisionSelectFunction = 3;
            int shiftPosition = CurrentShiftPosition();
            QuickHPStorage(drift);
            UsePower(drift);
            QuickHPCheck(1);
            AssertTrackPosition(shiftPosition + 2);

            //Shift Right
            DecisionSelectFunction = 2;
            shiftPosition = CurrentShiftPosition();
            QuickHPStorage(drift);
            UsePower(drift);
            QuickHPCheck(1);
            AssertTrackPosition(shiftPosition + 1);

            //Shift Left
            DecisionSelectFunction = 1;
            shiftPosition = CurrentShiftPosition();
            QuickHPStorage(drift);
            UsePower(drift);
            QuickHPCheck(1);
            AssertTrackPosition(shiftPosition - 1);

            //Shift Left Twice
            DecisionSelectFunction = 0;
            shiftPosition = CurrentShiftPosition();
            QuickHPStorage(drift);
            UsePower(drift);
            QuickHPCheck(1);
            AssertTrackPosition(shiftPosition - 2);
        }

        [Test()]
        public void TestDriftCharacter_Incap0()
        {
            SetupGameController("Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
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
            SetupGameController("Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
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
            SetupGameController("Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
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

        [Test, Sequential]
        public void TestShiftTrackSetup([Values(1, 2, 3, 4)] int decision)
        {
            SetupGameController("BaronBlade", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card track = FindCardsWhere((Card c) => c.Identifier == ShiftTrack + decision, false).FirstOrDefault();
            DecisionSelectCard = track;
            StartGame();

            Assert.AreEqual(decision, CurrentShiftPosition());
            AssertIsInPlay(track);
        }

        [Test()]
        public void TestAttenuationField_Past()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card atten = PutInHand(AttenuationField);
            Card mono = PlayCard("PlummetingMonorail");
            Card field = PlayCard("BacklashField");

            //Draw a card.
            QuickHandStorage(drift);
            PlayCard(atten);
            //Play -1, Draw +1
            QuickHandCheck(0);

            //{DriftPast} Destroy 1 environment card.
            //{DriftFuture} Destroy 1 ongoing card.
            AssertInTrash(mono);
            AssertIsInPlay(field);
        }

        [Test()]
        public void TestAttenuationField_Future()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card atten = PutInHand(AttenuationField);
            Card mono = PlayCard("PlummetingMonorail");
            Card field = PlayCard("BacklashField");
            GoToShiftPosition(3);

            //Draw a card.
            QuickHandStorage(drift);
            PlayCard(atten);
            //Play -1, Draw +1
            QuickHandCheck(0);

            //{DriftPast} Destroy 1 environment card.
            //{DriftFuture} Destroy 1 ongoing card.
            AssertInTrash(field);
            AssertIsInPlay(mono);
        }

        [Test, Combinatorial]
        public void TestBorrowedTime([Values(0, 1)] int decision, [Values(0, 1, 2, 3)] int shiftAmount)
        {
            SetupGameController("BaronBlade", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            if (decision == 0)
            {
                //Set Shift Track position to far right
                DecisionSelectFunction = 3;
                UsePower(drift);
                UsePower(drift);
            }

            SetHitPoints(baron, 17);
            SetHitPoints(drift, 17);
            SetHitPoints(haka, 17);

            DecisionSelectFunction = decision;
            DecisionSelectNumber = shiftAmount;
            int?[] hpChange = { 0, 0, 0 };
            for (int i = 0; i < shiftAmount; i++)
            {
                if (decision == 0)
                {
                    hpChange[i] = 2;
                }
                else
                {
                    hpChange[i] = -3;
                }
            }

            //Select {DriftL} or {DriftR}. Shift that direction up to 3 times. X is the number of times you shifted this way.
            //If you shifted at least {DriftL} this way, X targets regain 2 HP each. If you shifted {DriftR} this way, {Drift} deals X targets 3 radiant damage each.
            QuickHPStorage(baron, drift, haka);
            PlayCard(BorrowedTime);
            QuickHPCheck(hpChange);
        }

        [Test]
        public void TestDanceOfTheDragons_Future()
        {
            SetupGameController("Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            SetHitPoints(drift, 17);

            //Shift to future
            GoToShiftPosition(4);

            Card dance = PutInHand(DanceOfTheDragons);

            //targets to check
            Card sword = GetCardInPlay("Condemnation");
            GoToEndOfTurn(env);
            //anyone who entered play since last end of turn gets extra damage
            Card gauntlet = PlayCard("GauntletOfPerdition");

            //Values to compare
            int trackPosition = CurrentShiftPosition();
            QuickHandStorage(drift);
            QuickHPStorage(drift.CharacterCard, apostate.CharacterCard, sword, gauntlet);

            //{DriftFuture} {Drift} deals up to 3 targets 2 radiant damage each. Increase damage dealt this way by 1 to targets that entered play since the end of your last turn. Shift {DriftL}.
            //{DriftPast} Draw a card. {Drift} regains 1 HP. Shift {DriftRR}
            PlayCard(dance);

            //Shifted left one
            AssertTrackPosition(trackPosition - 1);
            //Play 1
            QuickHandCheck(-1);
            //2 damage to Apostate and Sword
            //Sword has 1 DR
            //3 to gauntlet
            QuickHPCheck(0, -2, -1, -3);
        }

        [Test]
        public void TestDanceOfTheDragons_Past()
        {
            SetupGameController("Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            SetHitPoints(drift, 17);

            Card dance = PutInHand(DanceOfTheDragons);

            //targets to check
            Card sword = GetCardInPlay("Condemnation");
            GoToEndOfTurn(env);
            //anyone who entered play since last end of turn gets extra damage
            Card gauntlet = PlayCard("GauntletOfPerdition");

            //Values to compare
            int trackPosition = CurrentShiftPosition();
            QuickHandStorage(drift);
            QuickHPStorage(drift.CharacterCard, apostate.CharacterCard, sword, gauntlet);

            //{DriftFuture} {Drift} deals up to 3 targets 2 radiant damage each. Increase damage dealt this way by 1 to targets that entered play since the end of your last turn. Shift {DriftL}.
            //{DriftPast} Draw a card. {Drift} regains 1 HP. Shift {DriftRR}
            PlayCard(dance);

            //Shifted right two
            AssertTrackPosition(trackPosition + 2);
            //Play 1, draw 1
            QuickHandCheck(0);
            //Drift heals 1
            QuickHPCheck(1, 0, 0, 0);
        }

        [Test]
        public void TestDanceOfTheDragons_Both()
        {
            //By being in position 3, after shifting left once, track ends up in a past position allowing for the second half to trigger
            SetupGameController("Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            SetHitPoints(drift, 17);

            //Shift to future
            GoToShiftPosition(3);

            Card dance = PutInHand(DanceOfTheDragons);

            //targets to check
            Card sword = GetCardInPlay("Condemnation");
            GoToEndOfTurn(env);
            //anyone who entered play since last end of turn gets extra damage
            Card gauntlet = PlayCard("GauntletOfPerdition");

            //Values to compare
            int trackPosition = CurrentShiftPosition();
            QuickHandStorage(drift);
            QuickHPStorage(drift.CharacterCard, apostate.CharacterCard, sword, gauntlet);

            //{DriftFuture} {Drift} deals up to 3 targets 2 radiant damage each. Increase damage dealt this way by 1 to targets that entered play since the end of your last turn. Shift {DriftL}.
            //{DriftPast} Draw a card. {Drift} regains 1 HP. Shift {DriftRR}
            PlayCard(dance);

            //Shifted left one
            AssertTrackPosition(trackPosition + 1);
            //Play 1
            QuickHandCheck(0);
            //2 damage to Apostate and Sword
            //Sword has 1 DR
            //3 to gauntlet
            QuickHPCheck(1, -2, -1, -3);
        }

        [Test]
        public void TestDestroyersAdagio_Past_PlayFromTrash()
        {
            SetupGameController("Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card fFocus = PutInTrash(FutureFocus);
            //{DriftPast} You may play an onoing card from your trash, or one player may play a card now. Shift {DriftR}
            //{DriftFuture} {Drift} deals 1 target 2 radiant damage. Shift {DriftLL}
            int trackPosition = CurrentShiftPosition();
            QuickHPStorage(apostate);
            PlayCard(DestroyersAdagio);
            AssertIsInPlay(fFocus);
            QuickHPCheck(0);
            AssertTrackPosition(trackPosition + 1);
        }

        [Test]
        public void TestDestroyersAdagio_Past_OtherPlays()
        {
            SetupGameController("Apostate", "Haka", "Cauldron.Drift", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DecisionSelectFunction = 1;
            Card mere = PutInHand("Mere");
            DecisionSelectCard = mere;
            //{DriftPast} You may play an onoing card from your trash, or one player may play a card now. Shift {DriftR}
            //{DriftFuture} {Drift} deals 1 target 2 radiant damage. Shift {DriftLL}
            int trackPosition = CurrentShiftPosition();
            QuickHPStorage(apostate);
            PlayCard(DestroyersAdagio);
            AssertIsInPlay(mere);
            QuickHPCheck(0);
            AssertTrackPosition(trackPosition + 1);
        }

        [Test]
        public void TestDestroyersAdagio_Future()
        {
            SetupGameController("Apostate", "Haka", "Cauldron.Drift", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            GoToShiftPosition(4);
            Card fFocus = PutInTrash(FutureFocus);
            //{DriftPast} You may play an onoing card from your trash, or one player may play a card now. Shift {DriftR}
            //{DriftFuture} {Drift} deals 1 target 2 radiant damage. Shift {DriftLL}
            int trackPosition = CurrentShiftPosition();
            QuickHPStorage(apostate);
            PlayCard(DestroyersAdagio);
            AssertInTrash(fFocus);
            QuickHPCheck(-2);
            AssertTrackPosition(trackPosition - 2);
        }

        [Test]
        public void TestDestroyersAdagio_Both()
        {
            SetupGameController("Apostate", "Haka", "Cauldron.Drift", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            GoToShiftPosition(2);
            DecisionSelectFunction = 0;
            Card fFocus = PutInTrash(FutureFocus);
            //{DriftPast} You may play an onoing card from your trash, or one player may play a card now. Shift {DriftR}
            //{DriftFuture} {Drift} deals 1 target 2 radiant damage. Shift {DriftLL}
            int trackPosition = CurrentShiftPosition();
            QuickHPStorage(apostate);
            PlayCard(DestroyersAdagio);
            AssertIsInPlay(fFocus);
            QuickHPCheck(-2);
            AssertTrackPosition(trackPosition - 1);
        }

        [Test]
        public void TestDriftStep_ShiftL()
        {
            SetupGameController("Apostate", "Haka", "Cauldron.Drift", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            GoToShiftPosition(2);
            Card step = PutInHand(DriftStep);
            Card fFocus = PutInHand(FutureFocus);
            DecisionSelectCard = fFocus;
            DecisionSelectFunction = 0;
            SetHitPoints(drift, 17);

            //Shift {DriftL} or {DriftR}.
            //If you shifted {DriftL} this way, {Drift} regains 1 HP. If you shifted {DriftR} this way, {Drift} deals 1 target 1 radiant damage.
            //Draw a card. You may play a card.
            int trackPosition = CurrentShiftPosition();
            QuickHPStorage(drift, apostate);
            QuickHandStorage(drift);
            PlayCard(step);
            //Play 2, Draw 1
            QuickHandCheck(-1);
            QuickHPCheck(1, 0);
            AssertTrackPosition(trackPosition - 1);
        }

        [Test]
        public void TestDriftStep_ShiftL_NoShift()
        {
            SetupGameController("Apostate", "Haka", "Cauldron.Drift", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card step = PutInHand(DriftStep);
            Card fFocus = PutInHand(FutureFocus);
            DecisionSelectCard = fFocus;
            SetHitPoints(drift, 17);

            //Shift {DriftL} or {DriftR}.
            //If you shifted {DriftL} this way, {Drift} regains 1 HP. If you shifted {DriftR} this way, {Drift} deals 1 target 1 radiant damage.
            //Draw a card. You may play a card.
            int trackPosition = CurrentShiftPosition();
            QuickHPStorage(drift, apostate);
            QuickHandStorage(drift);
            PlayCard(step);
            //Play 2, Draw 1
            QuickHandCheck(-1);
            QuickHPCheck(0, 0);
            AssertTrackPosition(trackPosition);
        }

        [Test]
        public void TestDriftStep_ShiftR()
        {
            SetupGameController("Apostate", "Haka", "Cauldron.Drift", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card step = PutInHand(DriftStep);
            Card fFocus = PutInHand(FutureFocus);
            DecisionSelectCard = fFocus;
            DecisionSelectFunction = 1;
            SetHitPoints(drift, 17);

            //Shift {DriftL} or {DriftR}.
            //If you shifted {DriftL} this way, {Drift} regains 1 HP. If you shifted {DriftR} this way, {Drift} deals 1 target 1 radiant damage.
            //Draw a card. You may play a card.
            int trackPosition = CurrentShiftPosition();
            QuickHPStorage(drift, apostate);
            QuickHandStorage(drift);
            PlayCard(step);
            //Play 2, Draw 1
            QuickHandCheck(-1);
            QuickHPCheck(0, -1);
            AssertTrackPosition(trackPosition + 1);
        }

        [Test]
        public void TestDriftStep_ShiftR_NoShift()
        {
            SetupGameController("Apostate", "Haka", "Cauldron.Drift", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            GoToShiftPosition(4);
            Card step = PutInHand(DriftStep);
            Card fFocus = PutInHand(FutureFocus);
            DecisionSelectCard = fFocus;
            DecisionSelectFunction = 1;
            SetHitPoints(drift, 17);

            //Shift {DriftL} or {DriftR}.
            //If you shifted {DriftL} this way, {Drift} regains 1 HP. If you shifted {DriftR} this way, {Drift} deals 1 target 1 radiant damage.
            //Draw a card. You may play a card.
            int trackPosition = CurrentShiftPosition();
            QuickHPStorage(drift, apostate);
            QuickHandStorage(drift);
            PlayCard(step);
            //Play 2, Draw 1
            QuickHandCheck(-1);
            QuickHPCheck(0, 0);
            AssertTrackPosition(trackPosition);
        }
    }
}
