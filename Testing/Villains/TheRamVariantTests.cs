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

        protected void CleanupStartingCards()
        {
            PutOnDeck(ram, FindCardsWhere((Card c) => c.IsVillain && c.IsInPlay && !c.IsCharacter));
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
        [Test]
        public void TestPastWintersRedirect()
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            PlayCard("UpClose");

            //The first time {AdmiralWinters} would be dealt damage each turn, redirect that damage to {TheRam}.",
            QuickHPStorage(ram.CharacterCard, winters, legacy.CharacterCard);
            DealDamage(legacy, winters, 4, DTM);
            QuickHPCheck(-4, 0, 0);
            DealDamage(legacy, winters, 4, DTM);
            QuickHPCheck(0, -4, 0);

            PlayCard("TakeDown");
            //once per turn
            GoToStartOfTurn(legacy);
            DealDamage(legacy, winters, 4, DTM);
            QuickHPCheck(-4, 0, 0);
        }
        [Test]
        public void TestPastWintersImmunity()
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            Card traffic = PlayCard("TrafficPileup");
            Card police = PlayCard("PoliceBackup");

            PlayCard("UpClose");

            QuickHPStorage(ram.CharacterCard, winters);
            DealDamage(legacy, winters, 3, DTM);
            DealDamage(legacy, winters, 3, DTM);
            QuickHPCheck(-3, -3);

            //"{AdmiralWinters} is immune to damage from targets that are not up close. 
            DealDamage(haka, winters, 4, DTM);
            DealDamage(traffic, winters, 4, DTM);
            QuickHPCheckZero();

            //only targets
            DealDamage(police, winters, 4, DTM);
            QuickHPCheck(0, -4);
        }
        [Test]
        public void TestPastWintersPreventsHeroWin()
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            DestroyCard(ram.CharacterCard);
            AssertNotGameOver();
        }
        [Test]
        public void TestPastWintersDamage([Values(0, 1, 2, 3)] int upCloseInPlay)
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "Legacy", "TheWraith", "Haka", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            PlayCards((Card c) => c.Identifier == "UpClose", upCloseInPlay);

            DecisionSelectTarget = ram.CharacterCard;
            PlayCard("ThroatJab");
            DecisionSelectTarget = null;
            PlayCard("TakeDown");
            List<Card> heroes = FindCardsWhere((Card c) => c.IsHeroCharacterCard).ToList();
            List<Card> damageList = heroes.Where((Card c) => !IsUpClose(c)).ToList();

            //"At the end of the villain turn, {AdmiralWinters} deals {H} projectile damage to each hero that is not Up Close. 
            GoToEndOfTurn();

            foreach (Card hero in heroes)
            {
                int expectedDamage = damageList.Contains(hero) ? 3 : 0;
                Assert.AreEqual(hero.MaximumHitPoints - expectedDamage, hero.HitPoints);
            }
        }
        [Test]
        public void TestPastWintersPlays([Values(0, 1, 2, 3, 4, 5)] int upCloseInPlay)
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "TheWraith", "Legacy", "Haka", "Tempest", "TheVisionary", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            Card notInPlay = PutOnDeck("FallBack");
            List<Card> safeToPlay = new List<Card> { GetCard("ForcefieldNode", 0), GetCard("ForcefieldNode", 1), GetCard("RocketPod", 0), GetCard("RocketPod", 1) };
            List<Card> toBeInPlay = new List<Card> { };
            for(int i = 0; i < upCloseInPlay - 1; i++)
            {
                toBeInPlay.Add(safeToPlay[i]);
            }
            PutOnDeck(ram, toBeInPlay);

            PlayCards((Card c) => c.Identifier == "UpClose", upCloseInPlay);

            //[...at the end of the villain turn], play the top X cards of the villain deck, where X is the number of copies of Up Close in play minus 1."

            GoToEndOfTurn();
            AssertIsInPlay(toBeInPlay);
            AssertNotInPlay(notInPlay);
        }
        [Test]
        public void TestPastRamStoresOneShots()
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "TheWraith", "Legacy", "Haka", "Tempest", "TheVisionary", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            QuickHPStorage(wraith, legacy, haka, tempest, visionary);
            Card buster = PlayCard("BarrierBuster");
            AssertUnderCard(ram.CharacterCard, buster);
            QuickHPCheckZero();

            //winters and ram, shouldn't count Barrier Buster
            AssertNumberOfCardsInPlay(ram, 2); 
        }
        [Test]
        public void TestPastRamFrontFlipCondition()
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "TheWraith", "Legacy", "Haka", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            PlayCard("TakeDown");

            GoToStartOfTurn(haka);
            Card buster = PlayCard("BarrierBuster");
            Card meteor = PlayCard("FallingMeteor");
            Card spines = PlayCard("PersonalDefenseSpines");
            AssertNotFlipped(ram);
            GoToStartOfTurn(ram);
            AssertFlipped(ram);
        }
        [Test]
        public void TestPastRamFrontFlipOnlyWithEnough()
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "TheWraith", "Legacy", "Haka", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            PlayCard("TakeDown");

            GoToStartOfTurn(haka);
            Card buster = PlayCard("BarrierBuster");
            Card meteor = PlayCard("FallingMeteor");
            AssertNotFlipped(ram);
            GoToStartOfTurn(ram);
            AssertNotFlipped(ram);
        }
        [Test]
        public void TestPastRamRFGWhenDestroyed()
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "TheWraith", "Legacy", "Haka", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            DestroyCard(ram.CharacterCard);
            AssertNotInTrash(ram.CharacterCard);
            AssertOutOfGame(ram.CharacterCard);
        }
        [Test]
        public void TestPastRamHealOnFlipH3()
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "TheWraith", "Legacy", "Haka", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            SetHitPoints(ram, 50);
            QuickHPStorage(ram);
            FlipCard(ram);
            QuickHPCheck(5);
        }
        [Test]
        public void TestPastRamHealOnFlipH4()
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "TheWraith", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            SetHitPoints(ram, 50);
            QuickHPStorage(ram);
            FlipCard(ram);
            QuickHPCheck(6);
        }
        [Test]
        public void TestPastRamPlaysStoredCardsOnFlip()
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "TheWraith", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            Card buster = PlayCard("BarrierBuster");
            Card meteor = PlayCard("FallingMeteor");

            QuickHPStorage(wraith);
            AssertNotInTrash(buster, meteor);
            AssertNextDecisionChoices(new Card[] { buster, meteor });
            FlipCard(ram);
            AssertInTrash(buster, meteor);

            //Wraith took: 3 melee damage from Barrier Buster, H = 4 + 1 projectile from Falling Meteor
            QuickHPCheck(-8);
        }
        [Test]
        public void TestPastRamFlippedDamageBoost()
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "TheWraith", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            FlipCard(ram);

            QuickHPStorage(legacy, haka, ram);
            DealDamage(ram, legacy, 1, DTM);
            DealDamage(ram, ram, 1, DTM);
            DealDamage(winters, haka, 1, DTM);
            QuickHPCheck(-1, -1, -1);

            DealDamage(ram, legacy, 1, DamageType.Projectile);
            DealDamage(ram, ram, 1, DamageType.Projectile);
            DealDamage(winters, haka, 1, DamageType.Projectile);
            QuickHPCheck(-2, -2, -2);
        }
        [Test]
        public void TestPastRamFlipToFrontCondition()
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "TheWraith", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            PlayCard("TakeDown");
            GoToStartOfTurn(haka);
            FlipCard(ram);
            AssertFlipped(ram);
            GoToStartOfTurn(ram);
            AssertNotFlipped(ram);
        }
        [Test]
        public void TestPastWintersFlipCondition()
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "TheWraith", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            DestroyCard(winters);
            AssertIsInPlay(winters);
            AssertFlipped(winters);
            AssertNotTarget(winters);
        }
        [Test]
        public void TestPastRamWinnableRamFirst()
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "TheWraith", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            DestroyCard(ram.CharacterCard);
            AssertNotGameOver();
            DestroyCard(winters);
            AssertGameOver();
        }
        [Test]
        public void TestPastRamWinnableWintersFirst()
        {
            SetupGameController("Cauldron.TheRam/PastTheRamCharacter", "TheWraith", "Legacy", "Haka", "Ra", "Megalopolis");
            StartGame();
            CleanupStartingCards();

            DestroyCard(winters);
            AssertNotGameOver();
            DestroyCard(ram.CharacterCard);
            AssertGameOver();
        }
    }
}