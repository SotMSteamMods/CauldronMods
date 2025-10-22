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
    public class DriftTests : CauldronBaseTest
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
        public void TestDriftIncap()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //SaveAndLoad();
            DealDamage(baron, haka, 50, 0);
            DealDamage(baron, bunker, 50, 0);
            DealDamage(baron, drift, 50, 0);
            AssertNumberOfCardsAtLocation(drift.TurnTaker.OutOfGame, 40);

            DealDamage(baron, scholar, 50, 0);
            AssertGameOver();
        }

        [Test()]
        public void TestDriftResurrect()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "TheTempleOfZhuLong");
            StartGame();

            DealDamage(baron, drift, 50, 0);
            AssertIncapacitated(drift);

            PlayCard("RitesOfRevival");
            GoToEndOfTurn(base.env);
            AssertNotIncapacitatedOrOutOfGame(drift);
            AssertNotFlipped(GetShiftTrack());
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
        public void TestDriftCharacter_InnatePowerWithCutoutSwap()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            AssertNumberOfUsablePowers(drift, 1);
            UsePower(drift);
            AssertNumberOfUsablePowers(drift, 0);
            Card shard = PlayCard("ThrowingShard");
            UsePower(shard, 0);
            DestroyCard(shard);
            AssertNumberOfUsablePowers(drift, 0);
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
        //[Ignore("Picking a ShiftTrack by Identifier always returns the first one. Testing in game confirms this works.")]
        public void TestShiftTrackSetup([Values(1, 2, 3, 4)] int decision)
        {
            SetupGameController("BaronBlade", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card track = FindCardsWhere((Card c) => c.Identifier == "Base" + ShiftTrack + decision, false).FirstOrDefault();
            DecisionSelectCard = track;
            StartGame(false);

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
            QuickHPCheckThroughFormChange(hpChange);
        }

        [Test]
        public void TestBorrowedTime_NoShiftPossible()
        {
            SetupGameController("Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            SetHitPoints(drift, 17);

            //Shift to future
            GoToShiftPosition(4);

            DecisionSelectFunction = 1;
            DecisionSelectNumber = 2;
            QuickHPStorage(apostate, drift, haka, bunker);
            PlayCard(BorrowedTime);
            QuickHPCheck(0, 0, 0, 0);

           
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
            //Shifted Left, so Drift deals 1
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
            //No movement, no action
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
            //Shifted Right, so Drift heals 1
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
            //No movement, no action
            QuickHPCheck(0, 0);
            AssertTrackPosition(trackPosition);
        }

        [Test]
        public void TestFutureFocus_NoShift_Yes()
        {
            SetupGameController("Apostate", "Haka", "Cauldron.Drift", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card pFocus = PlayCard(PastFocus);

            //When this card enters play, return all other focus cards to your hand.
            PlayCard(FutureFocus);
            AssertInHand(pFocus);

            DecisionYesNo = true;

            //When {Drift} is dealt damage, if you have not shifted this turn, you may shift {DriftRRR}. If you shifted {DriftRRR} this way, {Drift} deals 1 target 3 radiant damage.
            int trackPosition = CurrentShiftPosition();
            QuickHPStorage(apostate);
            DealDamage(apostate, drift, 2, DamageType.Melee);
            AssertTrackPosition(trackPosition + 3);
            QuickHPCheck(-3);
        }

        [Test]
        public void TestFutureFocus_NoShift_No()
        {
            SetupGameController("Apostate", "Haka", "Cauldron.Drift", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card pFocus = PlayCard(PastFocus);

            //When this card enters play, return all other focus cards to your hand.
            PlayCard(FutureFocus);
            AssertInHand(pFocus);

            DecisionYesNo = false;

            //When {Drift} is dealt damage, if you have not shifted this turn, you may shift {DriftRRR}. If you shifted {DriftRRR} this way, {Drift} deals 1 target 3 radiant damage.
            int trackPosition = CurrentShiftPosition();
            QuickHPStorage(apostate);
            DealDamage(apostate, drift, 2, DamageType.Melee);
            AssertTrackPosition(trackPosition);
            QuickHPCheck(0);
        }

        [Test]
        public void TestFutureFocus_YesShift()
        {
            SetupGameController("Apostate", "Haka", "Cauldron.Drift", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            GoToShiftPosition(1);
            Card pFocus = PlayCard(PastFocus);

            //When this card enters play, return all other focus cards to your hand.
            PlayCard(FutureFocus);
            AssertInHand(pFocus);

            DecisionYesNo = true;

            //When {Drift} is dealt damage, if you have not shifted this turn, you may shift {DriftRRR}. If you shifted {DriftRRR} this way, {Drift} deals 1 target 3 radiant damage.
            int trackPosition = 4;
            QuickHPStorage(apostate);
            DealDamage(apostate, drift, 2, DamageType.Melee);
            AssertTrackPosition(trackPosition);
            QuickHPCheck(-3);
        }

        [Test]
        public void TestFutureFocus_NotEnoughShift()
        {
            SetupGameController("Apostate", "Haka", "Cauldron.Drift", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            GoToShiftPosition(2);
            GoToStartOfTurn(haka);
            Card pFocus = PlayCard(PastFocus);

            //When this card enters play, return all other focus cards to your hand.
            PlayCard(FutureFocus);
            AssertInHand(pFocus);

            DecisionYesNo = true;

            //When {Drift} is dealt damage, if you have not shifted this turn, you may shift {DriftRRR}. If you shifted {DriftRRR} this way, {Drift} deals 1 target 3 radiant damage.
            QuickHPStorage(apostate);
            DealDamage(apostate, drift, 2, DamageType.Melee);
            AssertTrackPosition(4);
            QuickHPCheck(0);
        }
        [Test]
        public void TestFutureFocus_ShiftsNotCarriedOver()
        {
            SetupGameController("Apostate", "Haka", "Cauldron.Drift", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PlayCard(FutureFocus);
            DecisionYesNo = true;
            QuickHPStorage(apostate);

            DealDamage(apostate, drift, 1, DamageType.Melee);
            AssertTrackPosition(4);
            QuickHPCheck(-3);

            GoToStartOfTurn(drift);
            DealDamage(apostate, drift, 1, DamageType.Melee);
            QuickHPCheckZero();
        }

        [Test]
        public void TestImposedSynchronization_Future()
        {
            SetupGameController("Apostate", "Haka", "Cauldron.Drift", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            GoToShiftPosition(3);
            Card imp = PutInHand(ImposedSynchronization);
            DecisionSelectCard = haka.CharacterCard;

            int trackPosition = CurrentShiftPosition();
            QuickHandStorage(drift);
            PlayCard(imp);
            //Draw a card.
            //Play 1, Draw 1 = 0
            QuickHandCheck(0);
            //{DriftPast} Select a target. Reduce damage dealt to that target by 1 until the start of your next turn. Shift {DriftL}.
            AssertTrackPosition(trackPosition + 1);

            QuickHPStorage(haka);
            DealDamage(apostate, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //{DriftFuture} Select a target. Increase damage dealt by that target by 1 until the start of your next turn. Shift {DriftR}.
            QuickHPStorage(apostate);
            DealDamage(haka, apostate, 2, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test]
        public void TestImposedSynchronization_Past()
        {
            SetupGameController("Apostate", "Haka", "Cauldron.Drift", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card imp = PutInHand(ImposedSynchronization);
            DecisionSelectCard = haka.CharacterCard;
            GoToShiftPosition(2);
            int trackPosition = CurrentShiftPosition();
            QuickHandStorage(drift);
            PlayCard(imp);
            //Draw a card.
            //Play 1, Draw 1 = 0
            QuickHandCheck(0);

            //Shifted one right
            AssertTrackPosition(trackPosition - 1);

            //{DriftPast} Select a target. Reduce damage dealt to that target by 1 until the start of your next turn. Shift {DriftL}.
            QuickHPStorage(haka);
            DealDamage(apostate, haka, 2, DamageType.Melee);
            QuickHPCheck(-1);

            //{DriftFuture} Select a target. Increase damage dealt by that target by 1 until the start of your next turn. Shift {DriftR}.
            QuickHPStorage(apostate);
            DealDamage(haka, apostate, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test]
        public void TestKnightsHeritage()
        {
            SetupGameController(new string[] { "Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis" });
            StartGame();

            IEnumerable<Card> knights = FindCardsWhere(c => c.Identifier == KnightsHeritage).Take(2);
            Card fFocus = PutInHand(FutureFocus);
            Card second = PutInHand(MakeEverySecondCount);
            Card knight0 = PutInHand(knights.First());
            Card knight1 = PutInHand(knights.Last());
            DecisionSelectCards = new Card[] { fFocus, second };

            PlayCard(knight0);
            //...and play up to 2 ongoing cards from your hand.
            AssertIsInPlay(fFocus, second);
            DestroyCards(fFocus, second);

            //The first time {Drift} is dealt damage each turn, you may shift {DriftL} or {DriftR}.
            int trackPosition = CurrentShiftPosition();
            DecisionSelectFunction = 1;
            DealDamage(apostate, drift, 2, DamageType.Melee);
            AssertTrackPosition(trackPosition + 1);

            //Only first time
            trackPosition = CurrentShiftPosition();
            DealDamage(apostate, drift, 2, DamageType.Melee);
            AssertTrackPosition(trackPosition);

            GoToStartOfTurn(drift);
            //Shift left
            DecisionSelectFunction = 0;
            trackPosition = CurrentShiftPosition();
            DealDamage(apostate, drift, 2, DamageType.Melee);
            AssertTrackPosition(trackPosition - 1);

            DecisionDoNotSelectCard = SelectionType.PlayCard;
            //When this card enters play, destroy all other copies of Knight's Heritage...
            PlayCard(knight1);
            AssertInTrash(knight0);
        }
        [Test]
        public void TestKnightsHeritage_ShiftOptional()
        {
            SetupGameController(new string[] { "Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis" });
            StartGame();

            IEnumerable<Card> knights = FindCardsWhere(c => c.Identifier == KnightsHeritage).Take(2);
            Card fFocus = PutInHand(FutureFocus);
            Card second = PutInHand(MakeEverySecondCount);
            Card knight0 = PutInHand(knights.First());
            Card knight1 = PutInHand(knights.Last());
            DecisionSelectCards = new Card[] { fFocus, second };

            GoToShiftPosition(2);
            PlayCard(knight0);
            //...and play up to 2 ongoing cards from your hand.
            AssertIsInPlay(fFocus, second);
            DestroyCards(fFocus, second);

            //The first time {Drift} is dealt damage each turn, you may shift {DriftL} or {DriftR}.
            int trackPosition = CurrentShiftPosition();
            DecisionDoNotSelectFunction = true;
            DealDamage(apostate, drift, 2, DamageType.Melee);
            AssertTrackPosition(trackPosition);

            //Only first time
            trackPosition = CurrentShiftPosition();
            AssertNoDecision();
            DealDamage(apostate, drift, 2, DamageType.Melee);
            AssertTrackPosition(trackPosition);
        }
        [Test]
        public void TestKnightsHeritage_DamageTracksAcrossRedBlue()
        {
            SetupGameController(new string[] { "Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis" });
            StartGame();

            DiscardAllCards(drift);
            PlayCard("FutureFocus");
            PlayCard("KnightsHeritage");
            DecisionYesNo = true;
            DealDamage(apostate, drift, 1, DamageType.Melee);

            AssertTrackPosition(3);
        }
        [Test]
        public void TestMakeEverySecondCount()
        {
            SetupGameController("Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DecisionSelectCard = haka.CharacterCard;

            PlayCard(MakeEverySecondCount);
            //{DriftPast} Whenever you shift {DriftL}, select a hero target. Reduce the next damage dealt to that target by 2.
            //{DriftFuture} Whenever you shift {DriftR}, select a hero target. Increase the next damage dealt by that target by 2.

            //Past: Nothing by default
            QuickHPStorage(haka);
            DealDamage(haka, haka, 3, DamageType.Melee);
            QuickHPCheck(-3);

            //Past + Right = Nothing
            GoToShiftPosition(2);
            QuickHPStorage(haka);
            DealDamage(haka, haka, 3, DamageType.Melee);
            QuickHPCheck(-3);

            //Past + Left = Reduce Damage
            GoToShiftPosition(1);
            QuickHPStorage(haka);
            DealDamage(haka, haka, 3, DamageType.Melee);
            QuickHPCheck(-1);

            //Crossing the line should not trigger it
            GoToShiftPosition(3);
            QuickHPStorage(haka);
            DealDamage(haka, haka, 3, DamageType.Melee);
            QuickHPCheck(-3);

            //Future + Right = Increase Damage
            GoToShiftPosition(4);
            QuickHPStorage(haka);
            DealDamage(haka, haka, 3, DamageType.Melee);
            QuickHPCheck(-5);

            //Future + Left = Nothing
            GoToShiftPosition(3);
            QuickHPStorage(haka);
            DealDamage(haka, haka, 3, DamageType.Melee);
            QuickHPCheck(-3);

            //Crossing the line should not trigger it
            GoToShiftPosition(2);
            QuickHPStorage(haka);
            DealDamage(haka, haka, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }
        [Test]
        public void TestMakeEverySecondCount_OnlyShiftTrack()
        {
            SetupGameController("Apostate", "Cauldron.Drift", "Setback", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PlayCard("MakeEverySecondCount");

            PlayCard("FriendlyFire");
            DecisionYesNo = true;
            GoToShiftPosition(3);

            DealDamage(drift, apostate, 1, DamageType.Melee);
            AssertMaxNumberOfDecisions(1);
            DealDamage(drift, apostate, 1, DamageType.Melee);
        }

        [Test]
        public void TestOutOfSync()
        {
            SetupGameController("Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PlayCard(OutOfSync);

            //{DriftPast} Reduce damage dealt to {Drift} by 1.
            QuickHPStorage(drift, apostate);
            DealDamage(apostate, drift, 2, DamageType.Melee);
            DealDamage(drift, apostate, 2, DamageType.Melee);
            QuickHPCheck(-1, -2);

            GoToShiftPosition(3);
            //{DriftFuture} Increase damage dealt by {Drift} to other targets by 1.
            QuickHPStorage(drift, apostate);
            DealDamage(apostate, drift, 2, DamageType.Melee);
            DealDamage(drift, apostate, 2, DamageType.Melee);
            //Only increase to other targets
            DealDamage(drift, drift, 2, DamageType.Melee);
            QuickHPCheck(-4, -3);
        }

        [Test]
        public void TestPastFocus()
        {
            SetupGameController("Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card fFocus = PlayCard(FutureFocus);
            Card sync = PutInHand(OutOfSync);
            DecisionSelectCard = sync;
            DecisionYesNo = true;

            //When this card enters play, return all other focus cards to your hand.
            PlayCard(PastFocus);
            AssertInHand(fFocus);

            GoToShiftPosition(4);
            GoToStartOfTurn(drift);
            //When {Drift} is dealt damage, if you have not shifted this turn, you may shift {DriftLLL}. If you shifted {DriftLLL} this way, you may play a card.
            int trackPosition = CurrentShiftPosition();
            DealDamage(apostate, drift, 2, DamageType.Melee);
            AssertTrackPosition(1);
            AssertIsInPlay(sync);
        }

        [Test, Sequential]
        public void TestResourcefulDaydreamer([Values(1, 3)] int shiftPosition)
        {
            SetupGameController("Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            GoToShiftPosition(shiftPosition);
            SetHitPoints(drift, 17);
            GoToStartOfTurn(drift);

            Card discard = PutInHand(PastFocus);
            Card resourceful = PutInHand(ResourcefulDaydreamer);
            Card sync = PutInHand(OutOfSync);
            DecisionSelectCards = new Card[] { discard, sync };
            DecisionYesNo = true;

            QuickHPStorage(drift);
            QuickHandStorage(drift);
            PlayCard(resourceful);
            //Draw 2 cards. Discard a card.

            //Draw 2, Discard 1, Play 1 
            int handChange = 0;
            int hpChange = 0;

            //Past
            if (shiftPosition == 1)
            {
                //if in Past play another
                handChange = -1;
                //{DriftPast} You may play a card.
                AssertIsInPlay(sync);
            }
            //Future
            if (shiftPosition == 3)
            {
                //{DriftFuture} You may use a power.
                //Drift's only power heals for 1
                hpChange = 1;
                //If it's not played then it is in hand
                AssertInHand(sync);
            }

            QuickHandCheck(handChange);
            QuickHPCheck(hpChange);
        }

        [Test]
        public void TestSabershard_Past()
        {
            SetupGameController("Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card saber = PlayCard(Sabershard);

            QuickHPStorage(apostate);
            QuickHandStorage(drift);
            int trackPosition = CurrentShiftPosition();

            UsePower(saber, 0);

            //{DriftPast} Draw a card. Shift {DriftR}.
            //{DriftFuture} {Drift} 1 target 2 radiant damage. Shift {DriftL}.
            QuickHandCheck(1);
            AssertTrackPosition(trackPosition + 1);
            QuickHPCheck(0);

            AssertNextMessageContains("is not on a {Future} space, so nothing happens!");
            UsePower(saber, 1);
        }

        [Test]
        public void TestSabershard_Future()
        {
            SetupGameController("Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            GoToShiftPosition(4);
            Card saber = PlayCard(Sabershard);

            QuickHPStorage(apostate);
            QuickHandStorage(drift);
            int trackPosition = CurrentShiftPosition();

            UsePower(saber, 1);

            //{DriftPast} Draw a card. Shift {DriftR}.
            //{DriftFuture} {Drift} 1 target 2 radiant damage. Shift {DriftL}.
            QuickHandCheck(0);
            AssertTrackPosition(trackPosition - 1);
            QuickHPCheck(-2);

            AssertNextMessageContains("is not on a {Past} space, so nothing happens!");
            UsePower(saber, 0);
        }

        [Test]
        public void TestThrowingShard_Past()
        {
            SetupGameController("Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            SetHitPoints(apostate, 17);

            Card shard = PlayCard(ThrowingShard);
            Card traffic = PlayCard("TrafficPileup");

            QuickHPStorage(apostate.CharacterCard, traffic);
            int trackPosition = CurrentShiftPosition();

            AssertNextMessageContains("is not on a {Future} space, so nothing happens!");
            UsePower(shard, 1);

            UsePower(shard, 0);

            //{DriftPast} 1 target regains 2 HP. Shift {DriftRR}.
            //{DriftFuture} {Drift} deals each non-hero target 1 radiant damage. Shift {DriftLL}.

            QuickHPCheck(2, 0);
            AssertTrackPosition(trackPosition + 2);

        }

        [Test]
        public void TestThrowingShard_Future()
        {
            SetupGameController("Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            GoToShiftPosition(4);
            SetHitPoints(apostate, 17);

            Card shard = PlayCard(ThrowingShard);
            Card traffic = PlayCard("TrafficPileup");

            QuickHPStorage(apostate.CharacterCard, traffic);
            int trackPosition = CurrentShiftPosition();

            AssertNextMessageContains("is not on a {Past} space, so nothing happens!");
            UsePower(shard, 0);

            UsePower(shard, 1);

            //{DriftPast} 1 target regains 2 HP. Shift {DriftRR}.
            //{DriftFuture} {Drift} deals each non-hero target 1 radiant damage. Shift {DriftLL}.
            QuickHPCheck(-1, -1);
            AssertTrackPosition(trackPosition - 2);

        }

        [Test]
        public void TestTransitionShock()
        {
            SetupGameController("Apostate", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "TimeCataclysm");
            StartGame();

            //To prevent healing on Drift's power
            PlayCard("OppressiveSmog");

            PlayCard(TransitionShock);

            QuickHPStorage(apostate, drift);
            DecisionYesNo = true;
            //Whenever you shift from {DriftPast} to {DriftFuture} or from {DriftFuture} to {DrifitFuture}, {Drift} may deal 1 other target and herself 1 psychic damage.
            GoToShiftPosition(2);
            QuickHPCheckZeroThroughFormChange();

            AssertNextDecisionChoices(notIncluded: drift.CharacterCard.ToEnumerable());
            GoToShiftPosition(3);
            QuickHPCheckThroughFormChange(-1, -1);

            GoToShiftPosition(4);
            QuickHPCheckZeroThroughFormChange();

            GoToShiftPosition(3);
            QuickHPCheckZeroThroughFormChange();

            AssertNextDecisionChoices(notIncluded: drift.CharacterCard.ToEnumerable());
            GoToShiftPosition(2);
            QuickHPCheckThroughFormChange(-1, -1);

            GoToShiftPosition(1);
            QuickHPCheckZeroThroughFormChange();

            DecisionYesNo = false;
            GoToShiftPosition(3);
            QuickHPCheckZeroThroughFormChange();
        }
        [Test]
        public void TestTransitionShock_OnlyShiftTrack()
        {
            SetupGameController("Apostate", "Cauldron.Drift", "Setback", "Bunker", "TheScholar", "TimeCataclysm");
            StartGame();

            PlayCard(TransitionShock);
            Card lookingUp = PlayCard("LookingUp");

            AssertMaxNumberOfDecisions(1);
            UsePower(lookingUp);
        }

        [Test()]
        public void TestDriftAndProgeny()
        {
            SetupGameController("Progeny", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            SetHitPoints(drift, 8);
            StartGame();

            GoToShiftPosition(4);
            DealDamage(progeny, drift, 8, DamageType.Radiant);
            AssertIncapacitated(drift);

            GoToStartOfTurn(progeny);

            AssertNotFlipped(progeny);

        }

        [Test()]
        public void TestDriftComingInWithOblivaeon()
        {
            SetupGameController(new string[] { "OblivAeon", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            DealDamage(oblivaeon, legacy, 100, DamageType.Fire, isIrreducible: true, ignoreBattleZone: true);
            GoToAfterEndOfTurn(oblivaeon);
            DecisionSelectFromBoxIdentifiers = new string[] { "AllInGoodTimeDriftCharacter" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Cauldron.Drift";
            RunActiveTurnPhase();

            GoToPlayCardPhase(drift);
            DealDamage(drift, haka, 1, DamageType.Melee);
        }

        [Test()]
        public void TestDriftAsRepresentativeOfEarth()
        {
            SetupGameController(new string[] { "BaronBlade", "Legacy", "Haka", "Tachyon", "TheCelestialTribunal" });
            StartGame();

            DecisionSelectFromBoxIdentifiers = new string[] { "Cauldron.DriftCharacter" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Cauldron.Drift";

            Card representative = PlayCard("RepresentativeOfEarth");

            PrintJournal();

            SaveAndLoad();

        }
    }
}
