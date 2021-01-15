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

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(drift.CharacterCard, 1);
            DealDamage(villain, drift, 2, DamageType.Melee);
        }

        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                AssertCardHasKeyword(card, keyword, false);
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
        [Sequential]
        public void TestShiftTrackSetup([Values(1, 2, 3, 4)] int decision)
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
