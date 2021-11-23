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
    public class TheRamTests : CauldronBaseTest
    {
        #region RamHelperFunctions

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

        private readonly string MessageTerminator = "There should have been no other messages.";
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

            QuickShuffleStorage(ram.TurnTaker.Deck);
            StartGame();

            QuickShuffleCheck(1);
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
        public void TestRamEndOfTurnDamage()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "TheWraith", "Unity", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");

            QuickHPStorage(legacy, haka, wraith, unity);

            GoToEndOfTurn(ram);
            QuickHPCheck(0, -3, 0, 0);

            //should be the same on both sides
            PlayCard("UpClose");
            PlayCard("UpClose");
            PlayCard("UpClose");
            PlayCard("UpClose");
            DestroyCard("GrapplingClaw");

            GoToEndOfTurn(ram);
            QuickHPCheck(-3, 0, 0, 0);
        }
        [Test]
        public void TestRamDamageModifiers()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "TheWraith", "Unity", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");


            PlayCard("UpClose");

            QuickHPStorage(ram);

            DealDamage(legacy, ram, 2, DTM);
            QuickHPCheck(-2);
            DealDamage(haka, ram, 2, DTM);
            QuickHPCheck(-1);

            FlipCard(ram);

            DealDamage(legacy, ram, 2, DTM);
            QuickHPCheck(-3);
            DealDamage(haka, ram, 2, DTM);
            QuickHPCheck(-3);
        }
        [Test]
        public void TestRamFlipTriggers()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "TheWraith", "Unity", "Megalopolis");

            StartGame();
            Card claw = GetCardInPlay("GrapplingClaw");
            DestroyCard(claw);

            Card fallback = PlayCard("FallBack");

            PlayCard("UpClose");
            PlayCard("UpClose");
            PlayCard("UpClose");

            AssertNotFlipped(ram);
            PlayCard("UpClose");

            AssertFlipped(ram);
            AssertNotInPlay(fallback);
            AssertIsInPlay(claw);

            DestroyCard("UpClose");
            AssertNotFlipped(ram);
        }

        [Test]
        public void TestRam_Challenge()
        {
            SetupGameController(new string[] { "Cauldron.TheRam", "Legacy", "Haka", "TheWraith", "Unity", "Megalopolis" }, challenge: true);

            StartGame();
            DiscardTopCards(ram.TurnTaker.Deck, 14);
            Card claw = GetCardInPlay("GrapplingClaw");
            DestroyCard(claw);

            Card fallback = PlayCard("FallBack");

            PlayCard("UpClose");
            PlayCard("UpClose");
            PlayCard("UpClose");

            AssertNotFlipped(ram);
            PlayCard("UpClose");

            AssertFlipped(ram);
            AssertNotInPlay(fallback);
            AssertIsInPlay(claw);

            //"When {TheRam} is reduced to 40 or fewer HP, search the villain deck and trash for a copy of Fall Back and put it into play. Put all cards other than Close Up from the villain trash into the villain deck, then shuffle the villain deck."
            QuickShuffleStorage(ram);
            DealDamage(legacy, ram, 42, DamageType.Melee, isIrreducible: true);
            AssertIsInPlay("FallBack");
            QuickShuffleCheck(1);
            AssertNumberOfCardsInTrash(ram, 5, (Card c) => c.Identifier == "UpClose");
            AssertNumberOfCardsInTrash(ram, 0, (Card c) => c.Identifier != "UpClose" && c.Identifier != "GrapplingClaw");
        }
        [Test]
        public void TestRam_ChallengeOncePerGame()
        {
            SetupGameController(new string[] { "Cauldron.TheRam", "Legacy", "Haka", "TheWraith", "Unity", "Megalopolis" }, challenge: true);
            StartGame();
            DiscardTopCards(ram.TurnTaker.Deck, 14);

            //only to 40 or less
            DealDamage(legacy, ram, 30, DamageType.Melee);
            AssertNumberOfCardsInPlay(ram, 2); //character, grappling claw

            SaveAndLoad();

            DealDamage(legacy, ram, 30, DamageType.Melee);
            AssertNumberOfCardsInPlay(ram, 3); //character, grappling claw, fall back
            DestroyCard("FallBack");

            DealDamage(legacy, ram, 5, DamageType.Melee);
            AssertNumberOfCardsInPlay(ram, 2); //character, grappling claw

            SaveAndLoad();

            //check once-per-game preserves through save-load
            DealDamage(legacy, ram, 5, DamageType.Melee);
            AssertNumberOfCardsInPlay(ram, 2);

            AssertNumberOfCardsInTrash(ram, 6); //the Up Closes and one Fall Back
        }
        [Test]
        public void TestRam_ChallengeLeavesGrapplingClaw()
        {
            SetupGameController(new string[] { "Cauldron.TheRam", "Legacy", "Haka", "TheWraith", "Unity", "Megalopolis" }, challenge: true);
            StartGame();
            DiscardTopCards(ram.TurnTaker.Deck, 14);

            Card claw = FindCardInPlay("GrapplingClaw");
            DestroyCard(claw);

            PrintSpecialStringsForCard(ram.CharacterCard);

            DealDamage(legacy, ram, 45, DamageType.Melee);
            AssertAtLocation(claw, ram.TurnTaker.Trash);

            PrintSpecialStringsForCard(ram.CharacterCard);
        }
        [Test]
        public void TestRamGetUpCloseTrigger()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "TheWraith", "Unity", "Megalopolis");

            StartGame();
            Card claw = GetCardInPlay("GrapplingClaw");
            DestroyCard(claw);

            DecisionYesNo = true;

            GoToStartOfTurn(legacy);
            AssertIsInPlay("UpClose");
            Assert.True(IsUpClose(legacy), "Legacy was not Up Close, though he should have been.");
        }
        [Test]
        public void TestRamGetUpCloseTriggerOptional()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "TheWraith", "Unity", "Megalopolis");

            StartGame();
            Card claw = GetCardInPlay("GrapplingClaw");
            DestroyCard(claw);

            DecisionYesNo = false;

            GoToStartOfTurn(legacy);
            AssertNotInPlay("UpClose");
            Assert.False(IsUpClose(legacy), "Legacy was Up Close, though he should not have been.");

            DecisionYesNo = true;
            GoToStartOfTurn(haka);
            AssertIsInPlay("UpClose");
            Assert.True(IsUpClose(haka), "Haka was not Up Close, though he should have been.");
        }
        [Test]
        public void TestRamGetUpCloseTriggerOnlyIfNotUpClose()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "TheWraith", "Unity", "Megalopolis");

            StartGame();
            Card claw = GetCardInPlay("GrapplingClaw");
            DestroyCard(claw);

            PlayCard("UpClose");
            AssertNoDecision();
            GoToEndOfTurn(legacy);
        }
        [Test]
        public void TestRamGetUpCloseTriggerOnlyIfUpCloseInTrash()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "TheScholar", "Unity", "Megalopolis");

            StartGame();
            PutOnDeck(ram, ram.TurnTaker.Trash.Cards);
            PutInTrash("UpClose");

            AssertNoDecision();

            //Grappling Claw will automatically hook someone else, and Legacy will not get the chance to move up close.
            GoToEndOfTurn(legacy);
            AssertNumberOfCardsInTrash(ram, 0);
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

            GoToEndOfTurn(ram);

            AssertNumberOfCardsInPlay(ram, 3);
            Assert.True(IsUpClose(cosmic));
            QuickHPCheck(0, -3, -2);
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

            GoToEndOfTurn(ram);
            AssertNumberOfCardsInPlay(ram, 3);
            Assert.IsTrue(IsUpClose(sentinels));
            AssertNotUsablePower(sentinels, mainstay);
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

            GoToEndOfTurn(ram);

            QuickHPCheck(-2);
            AssertNumberOfCardsInPlay(ram, 2);
            CheckFinalMessage();
        }
        [Test]
        public void TestBarrierBusterNoEnvironmentCards()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "CaptainCosmic", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");

            //should hit non-character targets
            Card crest = PlayCard("CosmicCrest");

            QuickHPStorage(legacy.CharacterCard, haka.CharacterCard, cosmic.CharacterCard, crest);
            PlayCard("BarrierBuster");
            QuickHPCheck(-3, -3, -3, -3);
        }
        [Test]
        public void TestBarrierBuster2EnvironmentCards()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "CaptainCosmic", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");

            //should hit non-character targets
            Card crest = PlayCard("CosmicCrest");

            PlayCards("PoliceBackup", "HostageSituation");

            QuickHPStorage(legacy.CharacterCard, haka.CharacterCard, cosmic.CharacterCard);
            PlayCard("BarrierBuster");
            QuickHPCheck(-5, -5, -5);
            AssertInTrash(crest);
            AssertNumberOfCardsInPlay(FindEnvironment(), 0);
        }
        [Test]
        public void TestBarrierBusterOnlyCountsActuallyDestroyed()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "Unity", "TheTempleOfZhuLong");

            StartGame();
            DestroyCard("GrapplingClaw");

            PlayCards("ShinobiAssassin");

            QuickHPStorage(legacy.CharacterCard, haka.CharacterCard, unity.CharacterCard);
            PlayCard("BarrierBuster");
            //Assassin does not actually get destroyed, so only 3 damage
            QuickHPCheck(-3, -3, -3);
        }
        [Test]
        public void TestFallBackDamageAndDestroy()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "Unity", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");
            PlayCard("UpClose");
            PlayCard("UpClose");
            Assert.IsTrue(IsUpClose(legacy));

            QuickHPStorage(legacy.CharacterCard, haka.CharacterCard, unity.CharacterCard);
            PlayCard("FallBack");
            QuickHPCheck(-2, -2, -0);
            Assert.IsFalse(IsUpClose(legacy));
            AssertNumberOfCardsInPlay(ram, 2); //character card and fall back
        }
        [Test]
        public void TestFallBackDamageImmunity()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "Unity", "Ra", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");

            PlayCard("FallBack");
            QuickHPStorage(ram);
            Card bot = PlayCard("PlatformBot");

            int expectedDamage = 0;
            for (int i = 0; i < 4; i++)
            {
                DealDamage(legacy, ram, 1, DamageType.Melee);
                DealDamage(haka, ram, 2, DamageType.Melee);
                DealDamage(bot, ram, 3, DamageType.Melee);

                QuickHPCheck(-expectedDamage);

                PlayCard("UpClose");
                expectedDamage += i + 1;
            }
        }
        [Test]
        public void TestFallingMeteorBasic()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "Ra", "Unity", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");

            PlayCard("UpClose");

            SetHitPoints(haka, 20);

            QuickHPStorage(legacy, haka, unity, ra);
            PlayCard("FallingMeteor");
            Assert.IsTrue(IsUpClose(unity));
            QuickHPCheck(-4, -4, -4, -4);
        }
        [Test]
        public void TestFallingMeteorMayChooseDeckOrTrashFirst()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "Ra", "Unity", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");

            PlayCard("UpClose", 0);
            PutOnDeck(ram, GetCard("UpClose", 1));

            SetHitPoints(haka, 20);

            QuickHPStorage(legacy, haka, unity, ra);
            AssertNextDecisionSelectionType(SelectionType.SearchLocation);
            PlayCard("FallingMeteor");

            Assert.IsTrue(IsUpClose(unity));
            QuickHPCheck(-4, -4, -4, -4);
        }
        [Test]
        public void TestFallingMeteorCannotPutMultiplesOnSentinels()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "Ra", "TheSentinels", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");

            SetHitPoints(new TurnTakerController[] { legacy, haka, ra }, 10);

            //should be forced to pick one of the Sentinels first, then default to Legacy afterwards
            PlayCard("FallingMeteor");
            Assert.IsTrue((IsUpClose(sentinels)), "The Sentinels were not up close.");
            Assert.IsTrue((IsUpClose(legacy)), "Legacy was not up close.");
        }
        [Test]
        public void TestFallingMeteorEveryoneAlreadyUpClose()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "Ra", "TheSentinels", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");
            PutOnDeck(ram, ram.TurnTaker.Trash.Cards);

            PlayCard("UpClose");
            PlayCard("UpClose");
            PlayCard("UpClose");

            PlayCard("FallingMeteor");

            AssertInTrash("UpClose");
            //Falling Meteor and one Up Close
            AssertNumberOfCardsInTrash(ram, 2);
        }
        [Test]
        public void TestFallingMeteorDecrementsAllowedTargets()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "Ra", "TheVisionary", "VoidGuardWrithe", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");

            DecisionAutoDecideIfAble = true;
            //First to pull up close, second to pull up close, third is automatic so the next choice hits auto. 
            AssertMaxNumberOfDecisions(3);

            PlayCard("FallingMeteor");
        }
        [Test]
        public void TestForcefieldNodeImmunityAndDR()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "Ra", "TheVisionary", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");
            PlayCard("UpClose");
            PlayCard("UpClose");
            PlayCard("DecoyProjection");

            //legacy and haka should be up close, ra and visionary not
            Card node = PlayCard("ForcefieldNode");
            QuickHPStorage(ram.CharacterCard, node);

            DealDamage(legacy, node, 1, DTM);
            DealDamage(legacy, ram, 4, DTM);
            DealDamage(ra, node, 1, DTM);
            DealDamage(ra, ram, 4, DTM);

            QuickHPCheck(-3, -1);
        }
        [Test]
        public void TestForcefieldNodeDamage()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "Ra", "TheVisionary", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");
            PlayCard("UpClose");

            QuickHPStorage(legacy, haka, ra);

            PlayCard("ForcefieldNode");

            PlayCard("UpClose");
            //legacy was already up close, ra stayed away the whole time, haka went through
            QuickHPCheck(0, -2, 0);

            DestroyCard("UpClose");
            QuickHPCheck(0, 0, 0);
        }
        [Test]
        public void TestPersonalDefenseSpines()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "Ra", "Unity", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");
            PlayCard("UpClose");

            Card swift = PlayCard("SwiftBot");
            Card crate = PlayCard("SupplyCrate");
            Card charge = PlayCard("MotivationalCharge");
            Card mana = PlayCard("SavageMana");
            Card staff = PlayCard("TheStaffOfRa");

            QuickHPStorage(legacy.CharacterCard, haka.CharacterCard, ra.CharacterCard, unity.CharacterCard, swift);

            //Legacy is up close, others are not
            PlayCard("PersonalDefenseSpines");

            QuickHPCheck(-6, -2, -2, -2, -2);
            AssertInTrash(charge);
            AssertIsInPlay(new Card[] { crate, mana, staff });
        }
        [Test]
        public void TestPowerNodeImmunity()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "Ra", "TheVisionary", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");
            PlayCard("UpClose");
            PlayCard("UpClose");
            PlayCard("DecoyProjection");

            //legacy and haka should be up close, ra and visionary not
            Card node = PlayCard("PowerNode");
            QuickHPStorage(node);

            DealDamage(legacy, node, 1, DTM);
            DealDamage(ra, node, 2, DTM);

            QuickHPCheck(-1);
        }
        [Test]
        public void TestPowerNodeEndOfTurnResponse()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "Ra", "TheVisionary", "WagnerMarsBase");

            StartGame();


            Card ff = PlayCard("ForcefieldNode");
            Card claw = GetCardInPlay("GrapplingClaw");
            Card mortar = PlayCard("RemoteMortar");
            Card power = PlayCard("PowerNode");

            PutOnDeck(ram, ram.TurnTaker.Trash.Cards);

            //stack play
            //go-to skips normal play, so we only need the one
            Card rocket = PutOnDeck("RocketPod");

            SetHitPoints(new Card[] { legacy.CharacterCard, haka.CharacterCard, ram.CharacterCard, ff, claw, mortar, power }, 5);

            //keep damage from messing things up
            PlayCard("MeteorStorm");
            QuickHPStorage(ram.CharacterCard, ff, claw, mortar, power, legacy.CharacterCard, haka.CharacterCard);

            GoToStartOfTurn(legacy);
            AssertIsInPlay(rocket);
            QuickHPCheck(0, 1, 1, 1, 1, 0, 0);
        }
        [Test]
        public void TestRechargeCircuits()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "Ra", "TheVisionary", "WagnerMarsBase");

            StartGame();
            Card mortar = PlayCard("RemoteMortar");
            DestroyCard("GrapplingClaw");
            Card node = PlayCard("ForcefieldNode");

            PlayCard("MeteorStorm");
            PlayCard("RechargeCircuits");

            SetHitPoints(new Card[] { ram.CharacterCard, mortar, node }, 1);

            QuickHPStorage(ram.CharacterCard, node, mortar);

            GoToStartOfTurn(legacy);

            QuickHPCheck(10, 2, 2);
            GoToStartOfTurn(ram);
            PlayCard("MeteorStorm");
            PlayCard("UpClose");

            GoToStartOfTurn(legacy);
            QuickHPCheck(0, 0, 0);
        }
        [Test]
        public void TestRemoteMortarImmunity()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "Ra", "TheVisionary", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");
            PlayCard("UpClose");
            PlayCard("UpClose");
            PlayCard("DecoyProjection");

            //legacy and haka should be up close, ra and visionary not
            Card mortar = PlayCard("RemoteMortar");
            QuickHPStorage(mortar);

            DealDamage(legacy, mortar, 1, DTM);
            DealDamage(ra, mortar, 2, DTM);

            QuickHPCheck(-2);
        }
        [Test]
        public void TestRemoteMortarDamageAndDiscard()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "TheWraith", "Unity", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");
            PlayCard("UpClose");
            PlayCard("UpClose");
            Card bot = PlayCard("SwiftBot");

            //legacy and haka should be up close, wraith and unity not

            QuickHPStorage(legacy.CharacterCard, haka.CharacterCard, wraith.CharacterCard, unity.CharacterCard, bot);
            PlayCard("ThroatJab"); //let's not have the Ram mess up our damage

            PlayCard("RemoteMortar");
            QuickHandStorage(legacy, haka, wraith, unity);

            GoToEndOfTurn(ram);

            QuickHPCheck(-3, -3, 0, 0, 0);
            QuickHandCheck(-1, -1, 0, 0);
        }
        [Test]
        public void TestRocketPodImmunity()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "TheWraith", "TheVisionary", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");
            PlayCard("UpClose");
            PlayCard("UpClose");
            PlayCard("DecoyProjection");

            //legacy and haka should be up close, ra and visionary not
            Card rocket = PlayCard("RocketPod");
            QuickHPStorage(rocket);

            DealDamage(legacy, rocket, 1, DTM);
            DealDamage(wraith, rocket, 2, DTM);

            QuickHPCheck(-1);
        }
        [Test]
        public void TestRocketPodDamage()
        {
            SetupGameController("Cauldron.TheRam", "Legacy", "Haka", "TheWraith", "Unity", "Megalopolis");

            StartGame();
            DestroyCard("GrapplingClaw");
            PlayCard("UpClose");
            PlayCard("UpClose");
            Card bot = PlayCard("SwiftBot");

            //legacy and haka should be up close, wraith and unity not

            QuickHPStorage(legacy.CharacterCard, haka.CharacterCard, wraith.CharacterCard, unity.CharacterCard, bot);
            PlayCard("ThroatJab"); //let's not have the Ram mess up our damage

            PlayCard("RocketPod");

            GoToEndOfTurn(ram);

            QuickHPCheck(0, 0, -2, -2, -2);
        }
    }
}