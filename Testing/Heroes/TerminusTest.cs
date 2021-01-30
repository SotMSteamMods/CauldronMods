using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Terminus;

namespace CauldronTests
{
    [TestFixture()]
    public class TerminusTest : CauldronBaseTest
    {
        #region Terminus Utilities
        private string[] gameDecks => new string[] { "BaronBlade", "Cauldron.Terminus", "Legacy", "Bunker", "TheScholar", "Megalopolis" };

        private void StartTestGame()
        {
            StartTestGame(gameDecks);
        }

        private void StartTestGame(params String[] decksInPlay)
        {
            SetupGameController(decksInPlay);
            StartGame();

            base.DestroyNonCharacterVillainCards();
        }

        private void SetupIncapTest()
        {
            StartTestGame();
            SetupIncap(baron);
            AssertIncapacitated(terminus);
            GoToUseIncapacitatedAbilityPhase(terminus);
        }

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(terminus.CharacterCard, 1);
            DealDamage(villain, terminus, 2, DamageType.Melee);
        }

        #endregion Gargoyle Utilities

        [Test()]
        public void TestLoadTerminus()
        {
            SetupGameController(gameDecks);

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(terminus);
            Assert.IsInstanceOf(typeof(TerminusCharacterCardController), terminus.CharacterCardController);

            Assert.AreEqual(31, terminus.CharacterCard.HitPoints);
        }

        #region Test Innate Power
        /*
         * Power:
         * {Terminus} deals herself 2 cold damage and 1 target 3 cold damage.
         */
        [Test()]
        public void TestTerminusInnatePower()
        {
            StartTestGame();

            GoToUsePowerPhase(terminus);

            // Select a target. 
            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            UsePower(terminus.CharacterCard);
            QuickHPCheck(-3, -2, 0, 0, 0);
        }
        #endregion Test Innate Power

        #region Test Incap Powers
        /* 
         * Incap 1
         * One player may play a card now. 
        */
        [Test()]
        public void TestTerminusIncap1()
        {
            SetupIncapTest();
            AssertIncapLetsHeroPlayCard(terminus, 0, legacy, "MotivationalCharge");
        }

        /* 
         * Incap 2
         * Each hero may use a power printed on one of their non-character cards. Then destroy any card whose power was used this way. 
        */
        [Test()]
        public void TestTerminusIncap2()
        {
            Card motivationalCharge;
            Card bringWhatYouNeed;

            SetupIncapTest();
            motivationalCharge = PutIntoPlay("MotivationalCharge");
            bringWhatYouNeed = PutIntoPlay("BringWhatYouNeed");
            UseIncapacitatedAbility(terminus, 1);
            AssertNotInPlay(motivationalCharge);
            AssertNotInPlay(bringWhatYouNeed);
        }

        /* 
         * Incap 3
         * Each target regains 1 HP.
        */
        [Test()]
        public void TestTerminusIncap3()
        {
            SetupIncapTest();
            SetHitPoints(new Card[] { baron.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard }, 5);

            QuickHPStorage(baron, legacy, bunker, scholar);
            UseIncapacitatedAbility(terminus, 2);
            QuickHPCheck(1, 1, 1, 1);
        }

        #endregion Test Incap Powers

        #region Test Bone Chilling Touch

        /* 
         * A non-character target next to this card cannot have its current HP increased and cannot deal damage to {Terminus}.
         * powers
         * {Terminus} deals 1 target 2 cold damage. You may move this card next to that target.
         */
        [Test]
        public void TestBoneChillingTouch()
        {
            Card boneChillingTouch;
            Card bladeBattalion;

            StartTestGame();
            GoToUsePowerPhase(terminus);
            boneChillingTouch = PutIntoPlay("BoneChillingTouch");
            bladeBattalion = PutIntoPlay("BladeBattalion");

            SetHitPoints(baron, 20);
            DecisionSelectCards = new Card[] { bladeBattalion };
            DecisionsYesNo = new bool[] { true };

            QuickHPStorage(baron.CharacterCard, bladeBattalion, terminus.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            base.UsePower(boneChillingTouch);
            QuickHPCheck(0, -2, 0, 0, 0, 0);

            AssertNextToCard(boneChillingTouch, bladeBattalion);
            base.DealDamage(bladeBattalion, terminus, 5, DamageType.Melee);
            QuickHPCheckZero();

            base.GainHP(bladeBattalion, 2);
            QuickHPCheckZero();

            base.DealDamage(baron, terminus, 5, DamageType.Melee);
            QuickHPCheck(0, 0, -5, 0, 0, 0);

            base.GainHP(baron, 2);
            QuickHPCheck(2, 0, 0, 0, 0, 0);
        }

        #endregion Test Bone Chilling Touch

        #region Test Covenant Of Wrath
        /*
            * When this card would be destroyed, you may remove 3 tokens from your Wrath pool. If you removed 3 tokens this way, prevent that destruction.
            * Powers
            * {Terminus} deals 1 target 6 cold damage. Destroy this card.
            */
        [Test]
        public void TestCovenantOfWrath()
        {
            Card covenantOfWrath;

            StartTestGame();
            base.GameController.SkipToTurnTakerTurn(terminus);

            covenantOfWrath = PutIntoPlay("CovenantOfWrath");
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            UsePower(covenantOfWrath);
            QuickHPCheck(-6, 0, 0, 0, 0);

            AssertNotInPlay(covenantOfWrath);
        }
        [Test]
        public void TestCovenantOfWrathNotEnoughTokens()
        {
            Card covenantOfWrath;

            StartTestGame();
            base.GameController.SkipToTurnTakerTurn(terminus);

            base.AddTokensToPool(FindTokenPool("TerminusCharacter", "TerminusWrathPool"), 2);

            DecisionsYesNo = new bool[] { true };
            covenantOfWrath = PutIntoPlay("CovenantOfWrath");
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            UsePower(covenantOfWrath);
            QuickHPCheck(-6, 0, 0, 0, 0);

            AssertNotInPlay(covenantOfWrath);
        }
        [Test]
        public void TestCovenantOfWrathEnoughTokens()
        {
            Card covenantOfWrath;

            StartTestGame();
            base.GameController.SkipToTurnTakerTurn(terminus);

            base.AddTokensToPool(FindTokenPool("TerminusCharacter", "TerminusWrathPool"), 3);

            DecisionsYesNo = new bool[] { true };
            covenantOfWrath = PutIntoPlay("CovenantOfWrath");
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            UsePower(covenantOfWrath);
            QuickHPCheck(-6, 0, 0, 0, 0);

            AssertIsInPlay(covenantOfWrath);
        }

        #endregion Test Covenant Of Wrath

        #region Test Ethereal Armory
        /* 
         * Each player may play an ongoing or equipment card now.
         * At the start of your next turn, return each of those cards that is still in play to its player's hand.
         */
        [Test]
        public void TestEtherealArmory()
        {
            Card covenantOfWrath;
            Card motivationalCharge;
            Card ammoDrop;
            Card bringWhatYouNeed;

            StartTestGame();
            GoToPlayCardPhase(terminus);
            covenantOfWrath = PutInHand("CovenantOfWrath");
            motivationalCharge = PutInHand("MotivationalCharge");
            ammoDrop = PutInHand("AmmoDrop");
            bringWhatYouNeed = PutInHand("BringWhatYouNeed");

            QuickHandStorage(terminus, legacy, bunker, scholar);
            DecisionSelectCards = new Card[] { terminus.CharacterCard, covenantOfWrath, legacy.CharacterCard, motivationalCharge, bunker.CharacterCard, ammoDrop, scholar.CharacterCard, bringWhatYouNeed };
            PlayCard("EtherealArmory"); 
            QuickHandCheck(-1, -1, -1, -1);
            AssertIsInPlay(covenantOfWrath);
            AssertIsInPlay(motivationalCharge);
            AssertIsInPlay(ammoDrop);
            AssertIsInPlay(bringWhatYouNeed);

            base.GameController.SkipToTurnTakerTurn(terminus);
            base.GoToStartOfTurn(terminus);
            QuickHandCheck(1, 1, 1, 1);
            AssertNotInPlay(covenantOfWrath);
            AssertNotInPlay(motivationalCharge);
            AssertNotInPlay(ammoDrop);
            AssertNotInPlay(bringWhatYouNeed);

        }

        #endregion Test Ethereal Armory

        #region Test Flash Before Your Eyes
        /*
         * At the end of your turn, select a trash pile and put a card from it on top of its associated deck.
         * You cannot select that trash pile again for this effect until this card leaves play.
        */
        [Test]
        public void TestFlashBeforeYourEyes()
        {
            Card bladeBattalion;
            Card plummetingMonorail;
            Card motivationalCharge;

            StartTestGame();

            PutIntoPlay("FlashBeforeYourEyes");
            bladeBattalion = PutInTrash("BladeBattalion");
            plummetingMonorail = PutInTrash("PlummetingMonorail");
            motivationalCharge = PutInTrash("MotivationalCharge");

            DecisionSelectTurnTakers = new TurnTaker[] { baron.TurnTaker, baron.TurnTaker, base.env.TurnTaker, legacy.TurnTaker };
            DecisionSelectCards = new Card[] { bladeBattalion, plummetingMonorail, motivationalCharge };

            GoToEndOfTurn(terminus);
            AssertOnTopOfDeck(bladeBattalion);

            // You cannot select that trash pile again
            base.GameController.SkipToTurnTakerTurn(terminus);
            Assert.Catch<Exception>(new TestDelegate(() => GoToEndOfTurn(terminus)));

            base.GameController.SkipToTurnTakerTurn(terminus);
            GoToEndOfTurn(terminus);
            AssertOnTopOfDeck(plummetingMonorail);

            base.GameController.SkipToTurnTakerTurn(terminus);
            GoToEndOfTurn(terminus);
            AssertOnTopOfDeck(motivationalCharge);
        }

        #endregion Test Flash Before Your Eyes

        #region Test Full Moon Express

        /*
		 * Whenever {Terminus} is dealt damage by a non-hero target, she may deal that target 2 melee damage. If Stained Badge is not in play, 
		 * you may redirect damage dealt by non-hero targets to {Terminus}.
		 * At the start of your turn, remove this card from the game.
		 */
        [Test]
        public void TestFullMoonExpress()
        {
            Card fullMoonExpress;

            StartTestGame();
            fullMoonExpress = PutIntoPlay("FullMoonExpress");

            base.GameController.SkipToTurnPhase(new TurnPhase(baron.TurnTaker, Phase.End));
            DecisionsYesNo = new bool[] { true, true };

            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            DealDamage(baron, legacy, 2, DamageType.Melee);
            QuickHPCheck(-2, -2, 0, 0, 0);

            DealDamage(baron, terminus, 2, DamageType.Melee);
            QuickHPCheck(-2, -2, 0, 0, 0);

            GoToStartOfTurn(terminus);
            AssertOutOfGame(fullMoonExpress);
        }

        [Test]
        public void TestFullMoonExpressWithStainedBadge()
        {
            Card fullMoonExpress;

            StartTestGame();
            PutIntoPlay("StainedBadge");
            fullMoonExpress = PutIntoPlay("FullMoonExpress");

            base.GameController.SkipToTurnPhase(new TurnPhase(baron.TurnTaker, Phase.End));
            DecisionsYesNo = new bool[] { true };

            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            DealDamage(baron, legacy, 2, DamageType.Melee);
            QuickHPCheck(0, 0, -3, 0, 0);

            DealDamage(baron, terminus, 2, DamageType.Melee);
            QuickHPCheck(-2, -2, 0, 0, 0);

            GoToStartOfTurn(terminus);
            AssertOutOfGame(fullMoonExpress);
        }

        #endregion Test Full Moon Express

        #region Test Graven Shell
        /* 
         * This card is indestructible. If another Memento would enter play, instead remove it from the game and increase damage 
         * dealt by {Terminus} by 1.
         * Whenever {Terminus} destroys a card, add 2 tokens to your Wrath pool and she regains 1HP.
         */
        [Test]
        public void TestGravenShell()
        {
            Card bladeBattalion;
            Card gravenShell;
            Card railwaySpike;
            Card stainedBadge;

            StartTestGame();
            base.GameController.SkipToTurnTakerTurn(terminus);

            bladeBattalion = PutIntoPlay("BladeBattalion");
            gravenShell = PutIntoPlay("GravenShell");
            SetHitPoints(terminus, 20);

            QuickHPStorage(baron.CharacterCard, bladeBattalion, terminus.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            DealDamage(terminus, baron, 1, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0, 0, 0);

            railwaySpike = PlayCard(terminus, "RailwaySpike");
            DealDamage(terminus, baron, 1, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0, 0, 0);

            stainedBadge = PlayCard(terminus, "StainedBadge");
            DealDamage(terminus, baron, 1, DamageType.Melee);
            QuickHPCheck(-3, 0, 0, 0, 0, 0);

            DealDamage(terminus, bladeBattalion, 5, DamageType.Melee); //When destroyed, the card resets it's hit points for some reason
            QuickHPCheck(0, 0, 1, 0, 0, 0);

            AssertIsInPlay(gravenShell);
            AssertOutOfGame(railwaySpike);
            AssertOutOfGame(stainedBadge);
        }

        #endregion

        #region Test Railway Spike

        /* 
         * This card is indestructible. If another Memento would enter play, instead remove it from the game and draw 4 cards. 
         * Whenever {Terminus} is dealt damage, add 1 token or remove 3 tokens from your Wrath pool. If you removed 3 tokens 
         * this way, {Terminus} deals the source of that damage 3 cold damage. 
         */
        [Test]
        public void TestRailwaySpike()
        {
            Card gravenShell;
            Card railwaySpike;
            Card stainedBadge;
            TokenPool tokenPool;

            StartTestGame();
            base.GameController.SkipToTurnTakerTurn(terminus);

            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
            railwaySpike = PutIntoPlay("RailwaySpike");
            AddTokensToPool(tokenPool, 5);
            QuickHPStorage(baron.CharacterCard, terminus.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            QuickTokenPoolStorage(tokenPool);

            DecisionSelectFunctions = new int?[] { 0, 1 };
            DealDamage(baron, terminus, 1, DamageType.Melee);
            QuickHPCheck(0, -1, 0, 0, 0);
            QuickTokenPoolCheck(1);

            DealDamage(baron, terminus, 1, DamageType.Melee);
            QuickHPCheck(-3, -1, 0, 0, 0);
            QuickTokenPoolCheck(-3);

            QuickHandStorage(terminus, legacy, bunker, scholar);

            gravenShell = PlayCard(terminus, "GravenShell");
            QuickHandCheck(3, 0, 0, 0);

            stainedBadge = PlayCard(terminus, "StainedBadge");
            QuickHandCheck(3, 0, 0, 0);

            AssertIsInPlay(railwaySpike);
            AssertOutOfGame(gravenShell);
            AssertOutOfGame(stainedBadge);
        }

        #endregion

        #region Test Stained Badge

        /*
         * This card and {Terminus} are indestructible unless all other heroes are incapacitated. If another Memento would 
         * enter play, instead remove it from the game.
         * At the end of your turn, add 1 token to your Wrath pool if {Terminus} has 1 or more HP.
         */
        [Test]
        public void TestStainedBadge()
        {
            Card gravenShell;
            Card railwaySpike;
            Card stainedBadge;
            TokenPool tokenPool;

            StartTestGame();
            base.GameController.SkipToTurnTakerTurn(terminus);

            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
            stainedBadge = PutIntoPlay("StainedBadge");

            QuickTokenPoolStorage(tokenPool);
            GoToEndOfTurn(terminus);
            QuickTokenPoolCheck(1);

            SetHitPoints(terminus, 0);
            GoToEndOfTurn(terminus);
            QuickTokenPoolCheck(0);

            SetHitPoints(terminus, -1);
            GoToEndOfTurn(terminus);
            QuickTokenPoolCheck(0);

            DealDamage(baron, terminus, 2, DamageType.Melee);
            AssertNotIncapacitatedOrOutOfGame(terminus);
            
            gravenShell = PlayCard(terminus, "GravenShell");
            railwaySpike = PlayCard(terminus, "RailwaySpike");

            AssertIsInPlay(stainedBadge);
            AssertOutOfGame(gravenShell);
            AssertOutOfGame(railwaySpike);

            SetHitPoints(legacy.CharacterCard, 1);
            DealDamage(baron, legacy, 2, DamageType.Melee);
            AssertIncapacitated(legacy);
            AssertNotIncapacitatedOrOutOfGame(terminus);

            SetHitPoints(bunker.CharacterCard, 1);
            DealDamage(baron, bunker, 2, DamageType.Melee);
            AssertIncapacitated(bunker);
            AssertNotIncapacitatedOrOutOfGame(terminus);

            SetHitPoints(scholar.CharacterCard, 1);
            DealDamage(baron, scholar, 2, DamageType.Melee);
            AssertIncapacitated(scholar);
            AssertNotIncapacitatedOrOutOfGame(terminus);

            DealDamage(baron, terminus, 2, DamageType.Melee);
            AssertIncapacitated(terminus);
        }

        #endregion
    }
}
