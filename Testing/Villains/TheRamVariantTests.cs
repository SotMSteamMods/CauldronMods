using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Reflection;
using Handelabra;
using System.Collections.Generic;
using Cauldron.TheRam;

namespace CauldronTests
{
    [TestFixture()]
    public class TheRamVariantTests : BaseTest
    {
        #region RamHelperFunctions
        protected TurnTakerController ram { get { return FindVillain("TheRam"); } }

        protected Card winters { get { return GetCard("AdmiralWintersCharacter"); } }

        protected bool IsUpClose(TurnTakerController ttc)
        {
            return IsUpClose(ttc.TurnTaker);
        }
        protected bool IsUpClose(Card card)
        {
            return card.IsTarget && IsUpClose(card.Owner);
        }
        protected bool IsUpClose(TurnTaker tt)
        {
            return tt.HasCardsWhere((Card c) => c.NextToLocation != null && c.NextToLocation.Cards.Any((Card nextTo) => nextTo.Identifier == "UpClose"));
        }

        private string MessageTerminator = "There should have been no other messages.";
        protected void CheckFinalMessage()
        {
            GameController.ExhaustCoroutine(GameController.SendMessageAction(MessageTerminator, Priority.High, null));
        }

        protected DamageType DTM
        {
            get { return DamageType.Melee; }
        }
        #endregion
        [Test]
        public void TestStandardRamSetupUnchanged()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Megalopolis");

            QuickShuffleStorage(ram.TurnTaker.Deck);
            StartGame();

            QuickShuffleCheck(1);
            AssertNumberOfCardsInPlay(ram, 2);
            AssertIsInPlay("GrapplingClaw");
            AssertNumberOfCardsInTrash(ram, 5, (Card c) => c.Identifier == "UpClose");
            AssertNotFlipped(ram.CharacterCard);
            AssertHitPoints(ram, 80);
            AssertNotInPlay(winters);
        }
        [Test]
        public void TestPastRamSetup()
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "Legacy", "Megalopolis");

            QuickShuffleStorage(ram.TurnTaker.Deck);


            Assert.IsTrue(ram.CharacterCardController is PastTheRamCharacterCardController);
            Assert.IsTrue(FindCardController(winters) is AdmiralWintersCharacterCardController);
            StartGame();

            QuickShuffleCheck(1);
            AssertNumberOfCardsInPlay(ram, 4);
            AssertIsInPlay(winters);
            AssertIsInPlay("RemoteMortar", 2, 2);
            AssertNumberOfCardsInTrash(ram, 5, (Card c) => c.Identifier == "UpClose");
            AssertNotFlipped(ram.CharacterCard);
            AssertHitPoints(ram, 75);
            AssertHitPoints(winters, 20);
        }
    }
}