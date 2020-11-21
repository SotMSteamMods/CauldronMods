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
    public class TheRamTests : BaseTest
    {
        #region RamHelperFunctions
        protected TurnTakerController ram { get { return FindVillain("TheRam"); } }

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
        #endregion

        [Test]
        public void TestRamLoads()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(ram);
            Assert.IsInstanceOf(typeof(TheRamCharacterCardController), ram.CharacterCardController);

            Assert.AreEqual(80, ram.CharacterCard.HitPoints);
        }
        [Test]
        public void TestRamSetsUp()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Megalopolis");

            StartGame();

            AssertNumberOfCardsInPlay(ram, 2);
            AssertIsInPlay("GrapplingClaw");
            AssertNumberOfCardsInTrash(ram, 5, (Card c) => c.Identifier == "UpClose");
            AssertNotFlipped(ram.CharacterCard);
        }
        [Test]
        public void TestUpClosePlaysByValidHeroIfNoneSpecified()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "TheVisionary", "Megalopolis");

            StartGame();

            Card decoy = PlayCard("DecoyProjection");

            AssertNextDecisionChoices(new List<Card> { legacy.CharacterCard, haka.CharacterCard, visionary.CharacterCard }, new List<Card> { decoy });
            Card close = PlayCard("UpClose");

            AssertIsInPlay(close);
            AssertNextToCard(close, legacy.CharacterCard);

            AssertNextDecisionChoices(new List<Card> { haka.CharacterCard, visionary.CharacterCard }, new List<Card> { legacy.CharacterCard, decoy });
            Card close2 = PlayCard("UpClose");

            AssertIsInPlay(close2);
            AssertNextToCard(close2, haka.CharacterCard);
        }
        [Test]
        public void TestUpClosePlaysBySpecifiedHeroIfValid()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "Ra", "TheVisionary", "Megalopolis");

            StartGame();
            Card decoy = PlayCard("DecoyProjection");

            Card close = GetCard("UpClose");

            IEnumerator play;

            play = (GetCardController(close) as UpCloseCardController).PlayBySpecifiedHero(haka.CharacterCard, true, null);
            GameController.ExhaustCoroutine(play);
            AssertNextToCard(close, haka.CharacterCard);

            Card close2 = GetCard("UpClose");
            play = (GetCardController(close2) as UpCloseCardController).PlayBySpecifiedHero(haka.CharacterCard, true, null);
            GameController.ExhaustCoroutine(play);
            AssertNextToCard(close2, legacy.CharacterCard);

            Card close3 = GetCard("UpClose");
            play = (GetCardController(close3) as UpCloseCardController).PlayBySpecifiedHero(decoy, true, null);
            GameController.ExhaustCoroutine(play);
            AssertNextToCard(close3, ra.CharacterCard);  
        }
        [Test]
        public void TestUpCloseDestroysSelfWhenNoValidHero()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "TheVisionary", "Megalopolis");

            StartGame();

            PlayCard("UpClose");
            PlayCard("UpClose");
            Card livingClose = PlayCard("UpClose");

            AssertNextMessages("All heroes were already Up Close, so the new one destroys itself.", "This fails the test unless the UpClose message went off.");
            Card doomedClose = GetCardFromTrash(ram, "UpClose");
            PlayCard(doomedClose);

            AssertInTrash(doomedClose);
            DestroyCardJournalEntry doomedDies = GameController.Game.Journal.DestroyCardEntries().Where((DestroyCardJournalEntry dc) => dc.Card == doomedClose).FirstOrDefault();
            Assert.IsNotNull(doomedDies);
            DestroyCardJournalEntry livingDies = GameController.Game.Journal.DestroyCardEntries().Where((DestroyCardJournalEntry dc) => dc.Card == livingClose).FirstOrDefault();
            Assert.IsNull(livingDies);
            GameController.ExhaustCoroutine(GameController.SendMessageAction("This fails the test unless the UpClose message went off.", Priority.High, null));
        }
        [Test]
        public void TestUpCloseGrantsPowerThatDestroysIt()
        {    
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "CaptainCosmic", "Megalopolis");

            StartGame();

            UsePower(haka);

            AssertNumberOfUsablePowers(haka, 0);
            Card livingClose = PlayCard("UpClose");

            DecisionSelectCard = haka.CharacterCard;
            Card deadClose = PlayCard("UpClose");
            PlayCard("UpClose");
            
            AssertIsInPlay(deadClose);

            AssertNumberOfUsablePowers(haka, 1);
            UsePower(haka, 1);
            AssertIsInPlay(livingClose);
            AssertInTrash(deadClose);

            Assert.False(IsUpClose(haka));
        }
        [Test]
        public void TestUpCloseIncludesOwnedTargetsNotNextTo()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "TheVisionary", "Megalopolis");

            StartGame();
            Card decoy = PlayCard("DecoyProjection");

            PlayCard("UpClose");
            PlayCard("UpClose");
            PlayCard("UpClose");

            Assert.True(IsUpClose(decoy));
        }
        [Test]
        public void TestGrapplingClawSimple()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "CaptainCosmic", "Megalopolis");

            StartGame();

            QuickHPStorage(legacy, haka, cosmic);
            Assert.True(GetCard("GrapplingClaw").IsTarget);
            Assert.AreEqual(8, GetCard("GrapplingClaw").MaximumHitPoints);
            AssertMaxNumberOfDecisions(0);

            GoToStartOfTurn(legacy);

            AssertNumberOfCardsInPlay(ram, 3);
            Assert.True(IsUpClose(cosmic));
            QuickHPCheck(0, 0, -2);
        }
        [Test]
        public void TestGrapplingClawIgnoresUpCloseAndNonCharacter()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "CaptainCosmic", "Megalopolis");

            StartGame();

            PlayCard("CosmicCrest");
            SetHitPoints(haka, 15);
            SetHitPoints(legacy, 10);

            PlayCard("UpClose");

            Assert.False(IsUpClose(haka));
            GoToStartOfTurn(legacy);
            Assert.True(IsUpClose(haka));
        }
        [Test]
        public void TestGrapplingClawAllHeroesUpClose()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "CaptainCosmic", "Megalopolis");

            StartGame();

            PlayCard("UpClose");
            PlayCard("UpClose");
            PlayCard("UpClose");

            AssertNextMessages("Grappling Claw could not find a hero that was not Up Close.", MessageTerminator);

            GoToStartOfTurn(legacy);

            CheckFinalMessage();
        }
        [Test]
        public void TestGrapplingClawIncapsHero()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "CaptainCosmic", "Megalopolis");

            StartGame();

            SetHitPoints(haka, 1);
            AssertNextMessages("Haka was incapacitated before they could be pulled Up Close.", MessageTerminator);

            GoToStartOfTurn(legacy);

            AssertNumberOfCardsInPlay(ram, 2);
            CheckFinalMessage();
        }
        [Test]
        public void TestGrapplingClawIncapsSubCharacterOfMultiHero()
        {
           SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "TheSentinels", "Megalopolis");

           StartGame();

           SetHitPoints(mainstay, 1);
           AssertNextMessages("The Sentinels was pulled Up Close!", MessageTerminator);

           GoToStartOfTurn(legacy);

           AssertNumberOfCardsInPlay(ram, 3);
           Assert.IsTrue(IsUpClose(sentinels));
           AssertUsablePower(sentinels, mainstay);
           CheckFinalMessage();
        }
        [Test]
        public void TestGrapplingClawNoUpCloseInTrash()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "CaptainCosmic", "Megalopolis");

            StartGame();
            MoveAllCards(ram, ram.TurnTaker.Trash, ram.TurnTaker.Deck);
            SetHitPoints(haka, 10);
            QuickHPStorage(haka);
            AssertNextMessages("There were no copies of Up Close in the villain trash.", MessageTerminator);

            GoToStartOfTurn(legacy);

            QuickHPCheck(-2);
            AssertNumberOfCardsInPlay(ram, 2);
            CheckFinalMessage();
        }
    }
}