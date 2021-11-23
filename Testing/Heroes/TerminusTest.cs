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
        private TokenPool WrathPool => FindTokenPool("TerminusCharacter", "TerminusWrathPool");

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

        private void AssertTokensInWrathPool(int expected)
        {
            Assert.AreEqual(expected, WrathPool.CurrentValue, $"Expected {expected} tokens in Terminus's wrath pool, but there were {WrathPool.CurrentValue}");
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
        [Test]
        public void TestBoneChillingTouchExtraEffectsNotOnCharacter()
        {
            Card boneChillingTouch;
            Card bladeBattalion;

            StartTestGame();
            GoToUsePowerPhase(terminus);
            boneChillingTouch = PutIntoPlay("BoneChillingTouch");
            bladeBattalion = PutIntoPlay("BladeBattalion");

            SetHitPoints(baron, 20);
            SetHitPoints(bladeBattalion, 2);
            DecisionSelectCards = new Card[] { baron.CharacterCard };
            DecisionsYesNo = new bool[] { true };

            QuickHPStorage(baron.CharacterCard, bladeBattalion, terminus.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            base.UsePower(boneChillingTouch);
            QuickHPCheck(-2, 0, 0, 0, 0, 0);

            AssertNextToCard(boneChillingTouch, baron.CharacterCard);
            base.DealDamage(bladeBattalion, terminus, 5, DamageType.Melee);
            QuickHPCheck(0, 0, -5, 0, 0, 0);

            base.GainHP(bladeBattalion, 2);
            QuickHPCheck(0, 2, 0, 0, 0, 0);

            base.DealDamage(baron, terminus, 5, DamageType.Melee);
            QuickHPCheck(0, 0, -5, 0, 0, 0);

            base.GainHP(baron, 2);
            QuickHPCheck(2, 0, 0, 0, 0, 0);
        }
        [Test]
        public void TestBoneChillingTouchGrantedPower()
        {
            StartTestGame();

            Card touch = PlayCard("BoneChillingTouch");
            AssertNumberOfUsablePowers(terminus, 3);

            Assert.AreEqual(GameController.GetAllPowersForCardController(terminus.CharacterCardController).Count(), 2);
            Assert.AreEqual(GameController.GetAllPowersForCardController(GameController.FindCardController(touch)).Count(), 1);
            UsePower(touch);
            AssertNumberOfUsablePowers(terminus, 1);
            GoToStartOfTurn(terminus);
            UsePower(terminus, 1);
            AssertNumberOfUsablePowers(terminus, 1);
            
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
            AssertTokensInWrathPool(0);
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
            AssertTokensInWrathPool(0);
        }
        [Test]
        public void TestCovenantOfWrathEnoughTokensOptional()
        {
            Card covenantOfWrath;

            StartTestGame();
            base.GameController.SkipToTurnTakerTurn(terminus);

            base.AddTokensToPool(FindTokenPool("TerminusCharacter", "TerminusWrathPool"), 3);

            DecisionsYesNo = new bool[] { false };
            covenantOfWrath = PutIntoPlay("CovenantOfWrath");
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            UsePower(covenantOfWrath);
            QuickHPCheck(-6, 0, 0, 0, 0);

            AssertInTrash(covenantOfWrath);
            AssertTokensInWrathPool(3);
        }
        [Test]
        public void TestCovenantOfWrathOtherDestroy()
        {
            Card covenantOfWrath;

            StartTestGame();
            base.GameController.SkipToTurnTakerTurn(terminus);

            base.AddTokensToPool(FindTokenPool("TerminusCharacter", "TerminusWrathPool"), 3);
            DecisionsYesNo = new bool[] { true };
            covenantOfWrath = PutIntoPlay("CovenantOfWrath");

            DestroyCard(covenantOfWrath);
            AssertTokensInWrathPool(0);
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
            Card armory = PutInTrash("EtherealArmory");

            QuickHandStorage(terminus, legacy, bunker, scholar);
            DecisionSelectCards = new Card[] { covenantOfWrath, motivationalCharge, ammoDrop, bringWhatYouNeed };
            PlayCard(armory);
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

        [Test]
        public void TestEtherealArmory_MysticalEnhancement()
        {
            SetupGameController("BaronBlade", "Ra", "Cauldron.MagnificentMara", "Cauldron.Terminus", "Megalopolis");
            StartGame();

            Card enhancement = PutInHand("MysticalEnhancement");
            Card jailbreaker = PutInHand("Jailbreaker");
            DiscardAllCards(ra);

            DecisionSelectTurnTakers = new TurnTaker[] { terminus.TurnTaker, mara.TurnTaker };
            DecisionSelectCards = new Card[] { jailbreaker, enhancement, jailbreaker };
            PlayCard("EtherealArmory");
            AssertInPlayArea(terminus, jailbreaker);
            AssertNextToCard(enhancement, jailbreaker);

            GoToStartOfTurn(terminus);
            AssertInHand(jailbreaker, enhancement);



        }
        [Test]
        public void TestEtherealArmoryNotReturnFromNotInPlay()
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
            Card armory = PutInTrash("EtherealArmory");

            QuickHandStorage(terminus, legacy, bunker, scholar);
            DecisionSelectCards = new Card[] { covenantOfWrath, motivationalCharge, ammoDrop, bringWhatYouNeed };
            PlayCard(armory);
            QuickHandCheck(-1, -1, -1, -1);
            AssertIsInPlay(covenantOfWrath);
            AssertIsInPlay(motivationalCharge);
            AssertIsInPlay(ammoDrop);
            AssertIsInPlay(bringWhatYouNeed);

            DestroyCard(ammoDrop);
            PutOnDeck(scholar, bringWhatYouNeed);

            base.GameController.SkipToTurnTakerTurn(terminus);
            base.GoToStartOfTurn(terminus);
            QuickHandCheck(1, 1, 0, 0);
            AssertInHand(covenantOfWrath);
            AssertInHand(motivationalCharge);
            AssertInTrash(ammoDrop);
            AssertOnTopOfDeck(bringWhatYouNeed);
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

            Card flash = GetCard("FlashBeforeYourEyes");
            PlayCard(flash);
            bladeBattalion = PutInTrash("BladeBattalion");
            plummetingMonorail = PutInTrash("PlummetingMonorail");
            motivationalCharge = PutInTrash("MotivationalCharge");

            PrintSpecialStringsForCard(flash);

            DecisionSelectTurnTakers = new TurnTaker[] { baron.TurnTaker, baron.TurnTaker, base.env.TurnTaker, legacy.TurnTaker };
            DecisionSelectCards = new Card[] { bladeBattalion, plummetingMonorail, motivationalCharge };

            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(baron.TurnTaker.Trash), new LocationChoice(baron.TurnTaker.Trash), new LocationChoice(base.env.TurnTaker.Trash), new LocationChoice(legacy.TurnTaker.Trash) };
            GoToEndOfTurn(terminus);
            AssertOnTopOfDeck(bladeBattalion);

            PrintSpecialStringsForCard(flash);

            // You cannot select that trash pile again
            base.GameController.SkipToTurnTakerTurn(terminus);
            Assert.Catch<Exception>(new TestDelegate(() => GoToEndOfTurn(terminus)));

            PrintSpecialStringsForCard(flash);

            base.GameController.SkipToTurnTakerTurn(terminus);
            GoToEndOfTurn(terminus);
            AssertOnTopOfDeck(plummetingMonorail);

            PrintSpecialStringsForCard(flash);

            base.GameController.SkipToTurnTakerTurn(terminus);
            GoToEndOfTurn(terminus);
            AssertOnTopOfDeck(motivationalCharge);

            PrintSpecialStringsForCard(flash);

        }

        [Test]
        public void TestFlashBeforeYourEyes_Oblivaeon()
        {

            SetupGameController(new string[] { "OblivAeon", "Cauldron.Terminus", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            Card flash = GetCard("FlashBeforeYourEyes");
            PlayCard(flash);

            PrintSpecialStringsForCard(flash);


            GoToEndOfTurn(terminus);

            PrintSpecialStringsForCard(flash);

            // You cannot select that trash pile again
            base.GameController.SkipToTurnTakerTurn(terminus);

            PrintSpecialStringsForCard(flash);

            base.GameController.SkipToTurnTakerTurn(terminus);
            GoToEndOfTurn(terminus);

            PrintSpecialStringsForCard(flash);

            base.GameController.SkipToTurnTakerTurn(terminus);
            GoToEndOfTurn(terminus);

            PrintSpecialStringsForCard(flash);

        }

        [Test]
        public void TestFlashBeforeYourEyesSelectEmptyTrash()
        {
            StartTestGame();

            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(terminus.TurnTaker.Trash), new LocationChoice(baron.TurnTaker.Trash) };
            Card flash = PutIntoPlay("FlashBeforeYourEyes");
            AssertNumberOfCardsAtLocation(baron.TurnTaker.Trash, 1);
            AssertNumberOfCardsAtLocation(terminus.TurnTaker.Trash, 0);
            GoToEndOfTurn(terminus);

            var allowedTrashes = new LocationChoice[] { new LocationChoice(baron.TurnTaker.Trash), new LocationChoice(legacy.TurnTaker.Trash), new LocationChoice(scholar.TurnTaker.Trash), new LocationChoice(base.env.TurnTaker.Trash) };
            var forbiddenTrashes = new LocationChoice[] { new LocationChoice(terminus.TurnTaker.Trash) };
            AssertNextDecisionChoices(allowedTrashes, forbiddenTrashes);

            GoToEndOfTurn(terminus);
            AssertNumberOfCardsAtLocation(baron.TurnTaker.Trash, 0);

        }
        [Test]
        public void TestFlashBeforeYourEyesResetAfterLeavePlay()
        {
            StartTestGame();

            Card mdp = GetCardFromTrash(baron, "MobileDefensePlatform");
            Card batt = PutInTrash("BladeBattalion");

            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Trash);

            Card flash = PutIntoPlay("FlashBeforeYourEyes");
            GoToEndOfTurn(terminus);
            AssertNumberOfCardsAtLocation(baron.TurnTaker.Trash, 1);
            GoToStartOfTurn(terminus);
            DestroyCard(flash);
            PlayCard(flash);
            GoToEndOfTurn(terminus);
            AssertNumberOfCardsAtLocation(baron.TurnTaker.Trash, 0);
        }
        [Test]
        public void TestFlashBeforeYourEyesNoTrashesLeft()
        {
            StartTestGame();

            PutInTrash("EtherealArmory", "TheLegacyRing", "AmmoDrop", "FleshToIron", "TrafficPileup");
            Card flash = PlayCard("FlashBeforeYourEyes");

            for(int i = 0; i < 6; i++)
            {
                GoToEndOfTurn(terminus);
            }

            AssertNumberOfCardsAtLocation(baron.TurnTaker.Trash, 0);
            AssertNumberOfCardsAtLocation(terminus.TurnTaker.Trash, 0);
            AssertNumberOfCardsAtLocation(legacy.TurnTaker.Trash, 0);
            AssertNumberOfCardsAtLocation(bunker.TurnTaker.Trash, 0);
            AssertNumberOfCardsAtLocation(scholar.TurnTaker.Trash, 0);
            AssertNumberOfCardsAtLocation(base.env.TurnTaker.Trash, 0);

            GoToEndOfTurn(terminus);
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
            DecisionsYesNo = new bool[] { true, true, true, false };

            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            DealDamage(baron, legacy, 2, DamageType.Melee);
            QuickHPCheck(-2, -2, 0, 0, 0);

            DealDamage(baron, terminus, 2, DamageType.Melee);
            QuickHPCheck(-2, -2, 0, 0, 0);

            DealDamage(baron, legacy, 2, DamageType.Melee);
            QuickHPCheck(0, 0, -3, 0, 0);

            GoToStartOfTurn(terminus);
            AssertOutOfGame(fullMoonExpress);
        }
        [Test]
        public void TestFullMoonExpressDamageOptional()
        {
            Card fullMoonExpress;

            StartTestGame();
            fullMoonExpress = PutIntoPlay("FullMoonExpress");

            DecisionYesNo = false;
            QuickHPStorage(baron, terminus);
            DealDamage(baron, terminus, 1, DamageType.Melee);
            QuickHPCheck(0, -1);
        }

        [Test]
        public void TestFullMoonExpressWithStainedBadge()
        {
            Card fullMoonExpress;

            StartTestGame();
            PutIntoPlay("StainedBadge");
            fullMoonExpress = PutIntoPlay("FullMoonExpress");

            base.GameController.SkipToTurnPhase(new TurnPhase(baron.TurnTaker, Phase.End));
            DecisionsYesNo = new bool[] { true, true };

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
            AssertTokensInWrathPool(0);

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

            AssertTokensInWrathPool(2);
            AssertIsInPlay(gravenShell);
            AssertOutOfGame(railwaySpike);
            AssertOutOfGame(stainedBadge);
        }

        [Test]
        public void TestGravenShellRemovesMementosFromNonCardPlay()
        {
            StartTestGame();

            Card shell = PlayCard("GravenShell");
            Card badge = GetCard("StainedBadge");

            GameController.ExhaustCoroutine(GameController.MoveCard(terminus, badge, terminus.TurnTaker.PlayArea, isPutIntoPlay: true));
            AssertIsInPlay(shell);
            AssertOutOfGame(badge);
        }

        [Test]
        public void TestGravenShell_MarkOfDestructionBug()
        {
            SetupGameController("BaronBlade", "Cauldron.Terminus", "Cauldron.TheStranger", "Legacy/YoungLegacyCharacter", "Cauldron.WindmillCity");
            StartGame();

            SetHitPoints(terminus, 10);

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            Card gravenShell = PlayCard("GravenShell");
            DecisionSelectCard = mdp;
            DecisionAutoDecideIfAble = true;
            Card markOfDestruction = PlayCard("MarkOfDestruction");

            QuickHPStorage(terminus);
            DealDamage(terminus, markOfDestruction, 10, DamageType.Infernal);
            AssertInTrash(markOfDestruction);
            AssertInTrash(mdp);
            QuickHPCheck(1);
            AssertTokensInWrathPool(2);

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
            gravenShell = PutInHand("GravenShell");
            stainedBadge = PutInHand("StainedBadge");
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

            gravenShell = PlayCard(terminus, gravenShell);
            QuickHandCheck(3, 0, 0, 0);

            stainedBadge = PlayCard(terminus, stainedBadge);
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

            // Check tokens added functionality
            QuickTokenPoolStorage(tokenPool);
            GoToEndOfTurn(terminus);
            QuickTokenPoolCheck(1);

            SetHitPoints(terminus, 0);
            GoToEndOfTurn(terminus);
            QuickTokenPoolCheck(0);

            SetHitPoints(terminus, -1);
            GoToEndOfTurn(terminus);
            QuickTokenPoolCheck(0);

            // Test terminus indestructible
            DealDamage(baron, terminus, 2, DamageType.Melee);
            AssertNotIncapacitatedOrOutOfGame(terminus);

            // Test removes mementos
            gravenShell = PlayCard(terminus, "GravenShell");
            railwaySpike = PlayCard(terminus, "RailwaySpike");

            AssertIsInPlay(stainedBadge);
            AssertOutOfGame(gravenShell);
            AssertOutOfGame(railwaySpike);

            // Test terminus stays indestructible until the last hero falls
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
            AssertIncapacitated(terminus);
            AssertGameOver(EndingResult.HeroesDestroyedDefeat);
        }

        [Test]
        public void TestStainedBadgeRepresentativOfEarth()
        {
            Card representativeOfEarth;
            Card stainedBadge;
            Card representativeTerminus;

            StartTestGame("BaronBlade", "Cauldron.Terminus", "Legacy", "Bunker", "TheScholar", "TheCelestialTribunal");
            base.GameController.SkipToTurnTakerTurn(terminus);

            DecisionSelectFromBoxTurnTakerIdentifier = "Cauldron.Terminus";
            DecisionSelectFromBoxIdentifiers = new string[] { "Cauldron.MinistryOfStrategicScienceTerminusCharacter" };
            
            representativeOfEarth = PutIntoPlay("RepresentativeOfEarth");
            stainedBadge = PutIntoPlay("StainedBadge");

            representativeTerminus = representativeOfEarth.GetAllNextToCards(true).FirstOrDefault();

            // Test ends game
            SetHitPoints(representativeTerminus, 2);
            DealDamage(baron, representativeTerminus, 2, DamageType.Cold);
            AssertGameOver(EndingResult.EnvironmentDefeat);
        }

        [Test]
        public void TestStainedBadgeResurrectionRitual()
        {
            Card gravenShell;
            Card railwaySpike;
            Card resurrectionRitual;
            Card stainedBadge;
            int countUnder = 0;

            StartTestGame("BaronBlade", "Cauldron.Terminus", "Legacy", "Bunker", "TheScholar", "TheTempleOfZhuLong");
            base.GameController.SkipToTurnTakerTurn(terminus);

            resurrectionRitual = PutIntoPlay("ResurrectionRitual");
            stainedBadge = PutIntoPlay("StainedBadge");

            countUnder = resurrectionRitual.UnderLocation.Cards.Count();
            gravenShell = PlayCard(terminus, "GravenShell");
            AssertNumberOfCardsAtLocation(resurrectionRitual.UnderLocation, countUnder);

            railwaySpike = PlayCard(terminus, "RailwaySpike");
            AssertNumberOfCardsAtLocation(resurrectionRitual.UnderLocation, countUnder);


            SetHitPoints(terminus, -1);
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
            AssertIncapacitated(terminus);
            AssertNumberOfCardsAtLocation(resurrectionRitual.UnderLocation, countUnder);
            AssertGameOver(EndingResult.HeroesDestroyedDefeat);
        }

        [Test]
        public void TestStainedBadgeMultipleCharacterCards()
        {
            Card stainedBadge;
            var nightloreDict = new Dictionary<string, string> { };
            nightloreDict["Cauldron.Starlight"] = "NightloreCouncilStarlightCharacter";
            SetupGameController(new List<string> { "BaronBlade", "Cauldron.Starlight", "Cauldron.Terminus", "TheSentinels", "Megalopolis" }, false, nightloreDict);
            StartGame();
            base.GameController.SkipToTurnTakerTurn(terminus);

            stainedBadge = PutIntoPlay("StainedBadge");

            // Test terminus indestructible
            SetHitPoints(terminus, 2);
            DealDamage(baron, terminus, 2, DamageType.Melee);
            AssertNotIncapacitatedOrOutOfGame(terminus);

            // Test terminus stays indestructible until the last hero falls
            SetHitPoints(mainstay, 1);
            DealDamage(baron, mainstay, 2, DamageType.Melee);
            AssertNotIncapacitatedOrOutOfGame(sentinels);
            AssertNotIncapacitatedOrOutOfGame(terminus);

            SetHitPoints(writhe, 1);
            DealDamage(baron, writhe, 2, DamageType.Melee);
            AssertNotIncapacitatedOrOutOfGame(sentinels);
            AssertNotIncapacitatedOrOutOfGame(terminus);

            SetHitPoints(medico, 1);
            DealDamage(baron, medico, 2, DamageType.Melee);
            AssertNotIncapacitatedOrOutOfGame(sentinels);
            AssertNotIncapacitatedOrOutOfGame(terminus);

            SetHitPoints(idealist, 1);
            DealDamage(baron, idealist, 2, DamageType.Melee);
            AssertIncapacitated(sentinels);
            AssertNotIncapacitatedOrOutOfGame(terminus);

            SetHitPoints(terra, 1);
            DealDamage(baron, terra, 2, DamageType.Melee);
            AssertNotIncapacitatedOrOutOfGame(starlight);
            AssertNotIncapacitatedOrOutOfGame(terminus);

            SetHitPoints(asheron, 1);
            DealDamage(baron, asheron, 2, DamageType.Melee);
            AssertNotIncapacitatedOrOutOfGame(starlight);
            AssertNotIncapacitatedOrOutOfGame(terminus);

            SetHitPoints(cryos, 1);
            DealDamage(baron, cryos, 2, DamageType.Melee);
            AssertIncapacitated(starlight);

            AssertIncapacitated(terminus);
            AssertGameOver(EndingResult.HeroesDestroyedDefeat);
        }
        [Test]
        public void TestStainedBadgeIsolated()
        {
            SetupGameController("MissInformation", "Cauldron.Terminus", "Legacy", "Tempest", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("StainedBadge");
            DealDamage(miss, terminus, 50, DamageType.Melee);
            AssertNotIncapacitatedOrOutOfGame(terminus);

            PlayCard("IsolatedHero");
            AssertIncapacitated(terminus);
        }
        [Test]
        public void TestStainedBadgeNotIndestructibleWhenAlone()
        {
            SetupGameController("BaronBlade", "Cauldron.Terminus", "Legacy", "Tempest", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card badge = PlayCard("StainedBadge");
            DestroyCard(badge);
            AssertIsInPlay(badge);
            
            DealDamage(baron, legacy, 50, DamageType.Melee);
            DealDamage(baron, tempest, 50, DamageType.Melee);
            AssertNotIncapacitatedOrOutOfGame(terminus);

            DestroyCard(badge);
            AssertInTrash(badge);
        }
        #endregion

        #region Test Guilty Verdict

        /* 
         * Whenever a hero target is dealt 3 or more damage, add 1 token or remove 3 tokens from your Wrath pool. 
         * If you removed 3 tokens this way, increase damage dealt by that hero target by 1 until the start of 
         * the next environment turn.
         */
        [Test]
        public void TestGuiltyVerdict()
        {
            TokenPool tokenPool;
            StartTestGame();

            PutIntoPlay("GuiltyVerdict");
            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
            AddTokensToPool(tokenPool, 2);

            base.GameController.SkipToTurnTakerTurn(legacy);

            DecisionSelectFunctions = new int?[] { 0, 1 };
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            QuickTokenPoolStorage(tokenPool);

            DealDamage(baron, legacy, 3, DamageType.Melee);
            DealDamage(legacy, baron, 3, DamageType.Melee);
            QuickHPCheck(-4, 0, -4, 0, 0);
            QuickTokenPoolCheck(1);

            DealDamage(baron, legacy, 3, DamageType.Melee);
            DealDamage(legacy, baron, 3, DamageType.Melee);
            QuickHPCheck(-5, 0, -4, 0, 0);
            QuickTokenPoolCheck(-3);

            base.GoToStartOfTurn(base.env);
            DealDamage(legacy, baron, 3, DamageType.Melee);
            QuickHPCheck(-4, 0, 0, 0, 0);
            QuickTokenPoolCheck(0);
        }

        [Test]
        public void TestGuiltyVerdict_DontAddTokensOnFlippingNonHeroesIntoHeroes()
        {
            TokenPool tokenPool;
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Terminus", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            PutIntoPlay("GuiltyVerdict");
            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
            AddTokensToPool(tokenPool, 2);

            Card red = MoveCard(oblivaeon, "TheRedMenace", oblivaeon.TurnTaker.FindSubDeck("MissionDeck"));

            GoToBeforeStartOfTurn(terminus);
            RunActiveTurnPhase();

            SetHitPoints(red, 2);

            AssertTokenPoolCount(tokenPool, 2);
            DealDamage(haka, red, 4, DamageType.Melee);
            AssertFlipped(red);
            AssertTokenPoolCount(tokenPool, 2);
        }
        [Test]
        public void TestGuiltyVerdict_NoPrimingCarryover()
        {
            SetupGameController("BaronBlade", "Cauldron.Terminus", "Legacy", "Bunker", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            PlayCard("GuiltyVerdict");

            //should only have 1 choice, who to hit with power
            //Guilty Verdict chould not trigger
            AssertMaxNumberOfDecisions(1);
            UsePower(terminus);
        }
        #endregion Test Guilty Verdict

        #region Test Immortal Coils
        /*
         * Power
         * {Terminus} deals 1 target 3 cold damage. Add 1 or remove 3 tokens from your Wrath pool. If you removed 3 tokens this way, 
         * reduce damage dealt by that target by 1 until the start of your next turn.
         */
        [Test]
        public void TestImmortalCoils()
        {
            Card immortalCoils;
            TokenPool tokenPool;
            StartTestGame();

            immortalCoils = PutIntoPlay("ImmortalCoils");
            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
            AddTokensToPool(tokenPool, 2);
            base.GameController.SkipToTurnTakerTurn(terminus);

            DecisionSelectCards = new Card[] { baron.CharacterCard, baron.CharacterCard };
            DecisionSelectFunctions = new int?[] { 0, 1, 0 };
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            QuickTokenPoolStorage(tokenPool);

            UsePower(immortalCoils);
            DealDamage(baron, legacy, 1, DamageType.Melee);
            QuickHPCheck(-3, 0, -2, 0, 0);
            QuickTokenPoolCheck(1);

            UsePower(immortalCoils);
            DealDamage(baron, legacy, 1, DamageType.Melee);
            QuickHPCheck(-3, 0, -1, 0, 0);
            QuickTokenPoolCheck(-3);

            base.GameController.SkipToTurnPhase(new TurnPhase(baron.TurnTaker, Phase.End));

            base.GoToStartOfTurn(terminus);
            DealDamage(baron, legacy, 1, DamageType.Melee);
            QuickHPCheck(0, 0, -2, 0, 0);
            QuickTokenPoolCheck(0);

        }
        #endregion Test Immortal Coils

        #region Test Jailbreaker

        /*
         * When this card enters play, add 3 tokens to your Wrath pool and destroy all other copies of Jailbreaker.
         * Powers 
         * {Terminus} deals herself 1 cold damage and any number of other targets 2 projectile damage each.
         */
        [Test]
        public void TestJailbreaker()
        {
            Card jailbreaker1;
            Card jailbreaker2;
            TokenPool tokenPool;

            StartTestGame();
            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");

            QuickTokenPoolStorage(tokenPool);
            jailbreaker1 = PutIntoPlay("Jailbreaker");
            QuickTokenPoolCheck(3);
            jailbreaker2 = PutIntoPlay("Jailbreaker");
            QuickTokenPoolCheck(3);
            AssertNotInPlay(jailbreaker1);
            AssertIsInPlay(jailbreaker2);

            DecisionSelectCards = new Card[] { baron.CharacterCard, legacy.CharacterCard, null };
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            UsePower(jailbreaker2);
            QuickHPCheck(-2, -1, -2, 0, 0);
        }
        #endregion Test Jailbreaker

        #region Test No Rest For The Wicked

        /*
         * You may put a target from the villain trash into play. If that target has more than 5HP, 
         * reduce its current HP to 5. That target deals up to 2 other targets 5 infernal damage each. 
         * If no target enters play this way, add 5 tokens to your Wrath pool.
         */
        [Test]
        public void TestNoRestForTheWicked()
        {
            Card noRestForTheWicked;
            Card elementalRedistributor;
            TokenPool tokenPool;

            StartTestGame();
            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
            elementalRedistributor = PutInTrash("ElementalRedistributor");
            PutOnDeck(baron, FindCardsWhere((card) => card.Identifier == "MobileDefensePlatform" && card.IsInTrash));
            QuickTokenPoolStorage(tokenPool);



            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(baron.TurnTaker.Trash), new LocationChoice(baron.TurnTaker.Trash) };
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            noRestForTheWicked = PlayCard("NoRestForTheWicked");
            QuickHPCheck(-5, -5, 0, 0, 0);
            QuickTokenPoolCheck(0);
            AssertHitPoints(elementalRedistributor, 5);
            noRestForTheWicked = PlayCard("NoRestForTheWicked");
            QuickHPCheck(0, 0, 0, 0, 0);
            QuickTokenPoolCheck(5);
        }

        [Test]
        public void TestNoRestForTheWicked_MazeOfMirrors()
        {
            SetupGameController("BaronBlade", "Cauldron.Terminus", "Luminary", "Unity", "MMFFCC");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card elementalRedistributor = PutInTrash("ElementalRedistributor");

            Card raptorBot = PutInTrash("RaptorBot");
            Card platformBot = PutInTrash("PlatformBot");
            Card backlashGenerator = PutInTrash("BacklashGenerator");
            Card sabreBattleDrone = PutInTrash("SabreBattleDrone");

            Card mazeOfMirrors = PlayCard("MazeOfMirrors");

            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(unity.TurnTaker.Trash) };
            DecisionSelectCards = new Card[] { raptorBot, null };
            Card noRestForTheWicked = PlayCard("NoRestForTheWicked");
            AssertInPlayArea(unity, raptorBot);
            AssertInTrash(baron, elementalRedistributor);
            AssertInTrash(unity, platformBot);
            AssertInTrash(luminary, backlashGenerator);
            AssertInTrash(luminary, sabreBattleDrone);

        }

        [Test]
        public void TestNoRestForTheWicked_Oblivaeon()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Terminus", "Legacy", "Haka", "Cauldron.BlackwoodForest", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            TokenPool tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");

            Card aeonWarrior = GetCard("AeonWarrior");
            Location aeonTrash = oblivaeon.TurnTaker.FindSubTrash("AeonMenDeck");
            MoveCard(oblivaeon, aeonWarrior, aeonTrash, cardSource: oblivaeon.CharacterCardController.GetCardSource());

            QuickTokenPoolStorage(tokenPool);
            Card noRestForTheWicked = PlayCard("NoRestForTheWicked");
            QuickTokenPoolCheck(5);
            AssertAtLocation(aeonWarrior, aeonTrash);

            //check that it can still se aeon trash when needed
            Card vassal = GetCard("AeonVassal");
            PlayCard(oblivaeon, vassal, overridePlayLocation: terminus.BattleZone.FindScion().PlayArea);
            PlayCard(noRestForTheWicked);
            AssertIsInPlay(aeonWarrior);
        }
        #endregion Test No Rest For The Wicked

        #region Test Return Fire

        /*
         * Select a non-hero target. That target deals {Terminus} 1 projectile damage, then {Terminus} deals it 2 projectile damage. 
         * You may draw a card. 
         * You may play a card.
         */
        [Test]
        public void TestReturnFire()
        {
            Card returnFire;
            Card covenantOfWrath;
            TokenPool tokenPool;

            StartTestGame();
            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
            returnFire = PutInHand("ReturnFire");
            covenantOfWrath = PutInHand("CovenantOfWrath");
            var topCard = GetTopCardOfDeck(terminus);

            DecisionSelectTarget = baron.CharacterCard;
            DecisionSelectCardToPlay = covenantOfWrath;
            DecisionsYesNo = new bool[] { true, true };
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            QuickHandStorage(terminus, legacy, bunker, scholar);
            PlayCard(returnFire);
            AssertInTrash(returnFire);
            AssertInHand(topCard);
            AssertIsInPlay(covenantOfWrath);
            QuickHPCheck(-2, -1, 0, 0, 0);
            QuickHandCheck(-1, 0, 0, 0);
        }
        #endregion Test Return Fire

        #region Test Searing Gaze
        /* 
         * {Terminus} deals 6 cold damage to a target that dealt her damage since your last turn.
         * If a non-character target is destroyed this way, you may remove it and this card from 
         * the game.
         */
        [Test]
        public void TestSearingGaze()
        {
            Card searingGaze;
            Card bladeBattalion;
            Card elementalRedistributor;

            StartTestGame();
            bladeBattalion = PutIntoPlay("BladeBattalion");
            elementalRedistributor = PutIntoPlay("ElementalRedistributor");
            GoToEndOfTurn(base.env);
            GoToStartOfTurn(terminus);
            QuickHPStorage(baron.CharacterCard, elementalRedistributor, terminus.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            DealDamage(bladeBattalion, terminus, 5, DamageType.Cold);
            DealDamage(elementalRedistributor, terminus, 5, DamageType.Cold);
            SelectYesNoForNextDecision(true, true);
            DecisionSelectTargets = new Card[] { bladeBattalion, elementalRedistributor };
            searingGaze = PutIntoPlay("SearingGaze");
            AssertOutOfGame(bladeBattalion);
            AssertOutOfGame(searingGaze);
            searingGaze = PutIntoPlay("SearingGaze");
            QuickHPCheck(0, -6, -10, 0, 0, 0);
            AssertIsInPlay(elementalRedistributor);
            AssertInTrash(searingGaze);
        }
        #endregion Test Searing Gaze

        #region Test Soul Ignition

        /* 
         * Add 3 tokens to your Wrath pool. 
         * You may use a power now. 
         * You may discard a card. If you do, add 3 tokens to your Wrath pool or use a power now.
         */
        [Test]
        public void TestSoulIgntionGainTokens()
        {
            Card soulIgnition;
            Card jailbreaker;
            Card covenantOfWrath;
            TokenPool tokenPool;

            StartTestGame();
            soulIgnition = PutInHand("SoulIgnition");
            jailbreaker = PutIntoPlay("Jailbreaker");
            covenantOfWrath = PutIntoPlay("CovenantOfWrath");

            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");

            DecisionsYesNo = new bool[] { true, true };
            DecisionSelectFunctions = new int?[] { 0 };
            DecisionSelectPowers = new Card[] { jailbreaker, covenantOfWrath };
            QuickTokenPoolStorage(tokenPool);
            QuickHandStorage(terminus, legacy, bunker, scholar);
            PlayCard(soulIgnition);
            QuickHandCheck(-2, 0, 0, 0);
            QuickTokenPoolCheck(6);
        }

        [Test]
        public void TestSoulIgntionGainTokensUseExtraPower()
        {
            Card soulIgnition;
            Card jailbreaker;
            Card covenantOfWrath;
            TokenPool tokenPool;

            StartTestGame();
            soulIgnition = PutInHand("SoulIgnition");
            jailbreaker = PutIntoPlay("Jailbreaker");
            covenantOfWrath = PutIntoPlay("CovenantOfWrath");

            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");

            DecisionsYesNo = new bool[] { true, true };
            DecisionSelectFunctions = new int?[] { 1 };
            DecisionSelectPowers = new Card[] { jailbreaker, covenantOfWrath };
            QuickTokenPoolStorage(tokenPool);
            QuickHandStorage(terminus, legacy, bunker, scholar);
            PlayCard(soulIgnition);
            QuickHandCheck(-2, 0, 0, 0);
            QuickTokenPoolCheck(0);
        }

        #endregion Test Soul Ignition

        #region Test Stoke the Furnace
        /* 
         * Draw 2 cards. 
         * Increase damage dealt by {Terminus} by 1 until the start of your next turn. 
         * Add or remove 3 tokens from your Wrath pool. If you removed 3 tokens this way, you may play a card.
         */
        [Test]
        public void TestStokeTheFurnaceAddTokens()
        {
            Card stokeTheFurnace;
            TokenPool tokenPool;

            StartTestGame();
            stokeTheFurnace = PutInHand("StokeTheFurnace");

            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");

            DecisionSelectFunctions = new int?[] { 0 };
            DecisionsYesNo = new bool[] { true };
            QuickHandStorage(terminus, legacy, bunker, scholar);
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            QuickTokenPoolStorage(tokenPool);

            DealDamage(terminus, baron, 1, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0, 0);

            PlayCard(stokeTheFurnace);
            DealDamage(terminus, baron, 1, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0, 0);
            QuickHandCheck(1, 0, 0, 0);
            QuickTokenPoolCheck(3);

            base.GameController.SkipToTurnTakerTurn(legacy);
            base.GameController.SkipToTurnTakerTurn(baron);
            base.GoToStartOfTurn(terminus);
            DealDamage(terminus, baron, 1, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0, 0);
        }
        [Test]
        public void TestStokeTheFurnacePlayCard()
        {
            Card covenantOfWrath;
            Card stokeTheFurnace;
            TokenPool tokenPool;

            StartTestGame();
            stokeTheFurnace = PutInHand("StokeTheFurnace");
            covenantOfWrath = PutInHand("CovenantOfWrath");

            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
            AddTokensToPool(tokenPool, 3);

            DecisionSelectFunctions = new int?[] { 1 };
            DecisionsYesNo = new bool[] { true };
            DecisionSelectCards = new Card[] { covenantOfWrath  };
            QuickHandStorage(terminus, legacy, bunker, scholar);
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            QuickTokenPoolStorage(tokenPool);

            DealDamage(terminus, baron, 1, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0, 0);

            PlayCard(stokeTheFurnace);
            DealDamage(terminus, baron, 1, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0, 0);
            QuickHandCheck(0, 0, 0, 0);
            QuickTokenPoolCheck(-3);

            base.GameController.SkipToTurnTakerTurn(legacy);
            base.GameController.SkipToTurnTakerTurn(baron);
            base.GoToStartOfTurn(terminus);
            DealDamage(terminus, baron, 1, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0, 0);
        }
        #endregion Test Stoke the Furnace

        #region Test The Chain Conductor
        /* 
         * Reveal cards from the top of your deck until you reveal a Memento or Equipment card. Put it into play or into 
         * your hand. Shuffle the rest of the revealed cards into your deck. 
         * If no card entered play this way, add 3 tokens to your Wrath pool and {Terminus} deals 1 target 2 cold damage.
         */
        [Test]
        public void TestTheChainConductorPutInPlay()
        {
            Card boneChillingTouch;
            Card stokeTheFurnace;
            Card stainedBadge;
            Card theChainCoductor;
            int cardsInDeck;
            TokenPool tokenPool;

            StartTestGame();
            stainedBadge = PutOnDeck("StainedBadge");
            boneChillingTouch = PutOnDeck("BoneChillingTouch");
            stokeTheFurnace = PutOnDeck("StokeTheFurnace");
            theChainCoductor = PutInHand("TheChainConductor");

            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
            cardsInDeck = terminus.TurnTaker.Deck.NumberOfCards;
            DecisionMoveCardDestination = new MoveCardDestination(terminus.HeroTurnTaker.PlayArea);
            QuickHPStorage(baron);
            QuickTokenPoolStorage(tokenPool);
            QuickHandStorage(terminus, legacy, bunker, scholar);
            PlayCard(theChainCoductor);
            AssertNumberOfCardsInDeck(terminus, cardsInDeck - 1);
            QuickTokenPoolCheck(0);
            QuickHandCheck(-1, 0, 0, 0);
            QuickHPCheckZero();
        }

        [Test]
        public void TestTheChainConductorPutInHand()
        {
            int cardsInDeck;
            Card boneChillingTouch;
            Card stokeTheFurnace;
            Card stainedBadge;
            Card theChainCoductor;
            TokenPool tokenPool;

            StartTestGame();
            stainedBadge = PutOnDeck("StainedBadge");
            boneChillingTouch = PutOnDeck("BoneChillingTouch");
            stokeTheFurnace = PutOnDeck("StokeTheFurnace");
            theChainCoductor = PutInHand("TheChainConductor");

            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
            cardsInDeck = terminus.TurnTaker.Deck.NumberOfCards;
            DecisionMoveCardDestination = new MoveCardDestination(terminus.HeroTurnTaker.Hand);
            QuickHPStorage(baron);
            QuickTokenPoolStorage(tokenPool);
            QuickHandStorage(terminus, legacy, bunker, scholar);
            PlayCard(theChainCoductor);
            base.GoToEndOfTurn(terminus);
            AssertNumberOfCardsInDeck(terminus, cardsInDeck - 1);
            QuickTokenPoolCheck(3);
            QuickHandCheckZero();
            QuickHPCheck(-2);
        }
        [Test]
        public void TestTheChainConductorMementoExilesPlay()
        {
            int cardsInDeck;
            Card boneChillingTouch;
            Card stokeTheFurnace;
            Card stainedBadge;
            Card theChainCoductor;
            TokenPool tokenPool;

            StartTestGame();
            PlayCard("GravenShell");
            stainedBadge = PutOnDeck("StainedBadge");
            boneChillingTouch = PutOnDeck("BoneChillingTouch");
            stokeTheFurnace = PutOnDeck("StokeTheFurnace");
            theChainCoductor = PutInHand("TheChainConductor");

            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
            cardsInDeck = terminus.TurnTaker.Deck.NumberOfCards;
            DecisionMoveCardDestination = new MoveCardDestination(terminus.HeroTurnTaker.PlayArea);
            QuickHPStorage(baron);
            QuickTokenPoolStorage(tokenPool);
            QuickHandStorage(terminus, legacy, bunker, scholar);
            PlayCard(theChainCoductor);
            base.GoToEndOfTurn(terminus);
            AssertNumberOfCardsInDeck(terminus, cardsInDeck - 1);
            QuickTokenPoolCheck(3);
            QuickHandCheck(-1, 0, 0, 0);
            AssertOutOfGame(stainedBadge);
            QuickHPCheck(-3);
        }
        #endregion Test The Chain Conductor

        #region Test The Light At The End
        /* 
         * When this card enters play, put a one-shot from your trash into play.
         * At the end of your turn, you may remove 3 tokens from your Wrath pool. 
         * If you do, {Terminus} regains 2HP or destroys 1 ongoing card.
         */
        [Test]
        public void TestTheLightAtTheEndDontSpendTokens()
        {
            Card etherealArmory;
            Card theLightAtTheEnd;
            Card covenantOfWrath;
            Card motivationalCharge;
            Card ammoDrop;
            Card bringWhatYouNeed;
            TokenPool tokenPool;

            StartTestGame();
            base.GoToPlayCardPhase(terminus);
            etherealArmory = PutInTrash("EtherealArmory");
            covenantOfWrath = PutInHand("CovenantOfWrath");
            theLightAtTheEnd = GetCard("TheLightAtTheEnd");
            motivationalCharge = PutInHand("MotivationalCharge");
            ammoDrop = PutInHand("AmmoDrop");
            bringWhatYouNeed = PutInHand("BringWhatYouNeed");

            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
            AddTokensToPool(tokenPool, 3);
            SetHitPoints(terminus, 20);
            DecisionSelectFunctions = new int?[] { 0 };
            DecisionSelectCards = new Card[] { covenantOfWrath, motivationalCharge, ammoDrop, bringWhatYouNeed, covenantOfWrath };

            QuickTokenPoolStorage(tokenPool);
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            PlayCard(theLightAtTheEnd);
            base.GoToEndOfTurn(terminus);
            QuickHPCheckZero();
            QuickTokenPoolCheck(0);
            AssertIsInPlay(covenantOfWrath);
        }

        [Test]
        public void TestTheLightAtTheEndGainHP()
        {
            Card etherealArmory;
            Card theLightAtTheEnd;
            Card covenantOfWrath;
            Card motivationalCharge;
            Card ammoDrop;
            Card bringWhatYouNeed;
            TokenPool tokenPool;

            StartTestGame();
            base.GoToPlayCardPhase(terminus);
            etherealArmory = PutInTrash("EtherealArmory");
            covenantOfWrath = PutInHand("CovenantOfWrath");
            theLightAtTheEnd = GetCard("TheLightAtTheEnd");
            motivationalCharge = PutInHand("MotivationalCharge");
            ammoDrop = PutInHand("AmmoDrop");
            bringWhatYouNeed = PutInHand("BringWhatYouNeed");

            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
            AddTokensToPool(tokenPool, 3);
            SetHitPoints(terminus, 20);
            DecisionsYesNo = new bool[] { true };
            DecisionSelectFunctions = new int?[] { 0 };
            DecisionSelectCards = new Card[] { covenantOfWrath, motivationalCharge, ammoDrop, bringWhatYouNeed, covenantOfWrath };

            QuickTokenPoolStorage(tokenPool);
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            PlayCard(theLightAtTheEnd);
            base.GoToEndOfTurn(terminus);
            QuickHPCheck(0, 2, 0, 0, 0);
            QuickTokenPoolCheck(-3);
            AssertIsInPlay(covenantOfWrath);
        }

        [Test]
        public void TestTheLightAtTheEndRemoveOngoing()
        {
            Card etherealArmory;
            Card theLightAtTheEnd;
            Card covenantOfWrath;
            Card motivationalCharge;
            Card ammoDrop;
            Card bringWhatYouNeed;
            TokenPool tokenPool;

            StartTestGame();
            base.GoToPlayCardPhase(terminus);
            etherealArmory = PutInTrash("EtherealArmory");
            covenantOfWrath = PutInHand("CovenantOfWrath");
            theLightAtTheEnd = GetCard("TheLightAtTheEnd");
            motivationalCharge = PutInHand("MotivationalCharge");
            ammoDrop = PutInHand("AmmoDrop");
            bringWhatYouNeed = PutInHand("BringWhatYouNeed");

            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
            AddTokensToPool(tokenPool, 3);
            SetHitPoints(terminus, 20);
            DecisionsYesNo = new bool[] { true, false };
            DecisionSelectFunctions = new int?[] { 1 };
            DecisionSelectCards = new Card[] { covenantOfWrath, motivationalCharge, ammoDrop, bringWhatYouNeed, covenantOfWrath };

            QuickTokenPoolStorage(tokenPool);
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            PlayCard(theLightAtTheEnd);
            base.GoToEndOfTurn(terminus);
            QuickHPCheckZero();
            QuickTokenPoolCheck(-3);
            AssertNotInPlay(covenantOfWrath);
        }
        #endregion Test The Chain Conductor

        #region Test Unusual Suspects
        /* 
         * Add 2 tokens to your Wrath pool.
         * {Terminus} deals up to 2 targets 2 cold damage each. If {Terminus} deals damage this way to
         * a target that shares a keyword with a card in any trash pile, increase that damage by 2.
         */
        [Test]
        public void TestUnusualSuspects()
        {
            Card bladeBattalion1;
            Card bladeBattalion2;
            TokenPool tokenPool;

            StartTestGame();
            base.GoToPlayCardPhase(terminus);
            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");

            bladeBattalion1 = PutIntoPlay("BladeBattalion");
            bladeBattalion2 = PutIntoPlay("BladeBattalion");
            DealDamage(terminus, bladeBattalion1, 6, DamageType.Melee);

            DecisionSelectCards = new Card[] { baron.CharacterCard, bladeBattalion2 };
            QuickTokenPoolStorage(tokenPool);
            QuickHPStorage(baron.CharacterCard, terminus.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion2);
            PlayCard("UnusualSuspects");
            QuickTokenPoolCheck(2);
            QuickHPCheck(-2, 0, 0, 0, 0, -4);

        }

        [Test]
        public void UnusualSuspectsOblivAeon()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Terminus", "Legacy", "Haka", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            DiscardTopCards(oblivaeon.TurnTaker.FindSubDeck("AeonMenDeck"), 3, oblivaeon.CharacterCardController.GetCardSource());

            Card aeonWarrior = GetCard("AeonWarrior");
            PlayCard(oblivaeon, aeonWarrior, overridePlayLocation: terminus.BattleZone.FindScion().PlayArea);

            DecisionSelectTargets = new Card[] { aeonWarrior, null };
            QuickHPStorage(aeonWarrior);
            PlayCard("UnusualSuspects");
            QuickHPCheck(-4);


        }
        [Test]
        public void UnusualSuspectsOblivAeon_OtherBattleZone()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Terminus", "Legacy", "Haka", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            DiscardTopCards(oblivaeon.TurnTaker.FindSubDeck("AeonMenDeck"), 3, oblivaeon.CharacterCardController.GetCardSource());

            SwitchBattleZone(terminus);
            Card aeonWarrior = GetCard("AeonWarrior");
            PlayCard(oblivaeon, aeonWarrior, overridePlayLocation: terminus.BattleZone.FindScion().PlayArea);

            DecisionSelectTargets = new Card[] { aeonWarrior, null };
            QuickHPStorage(aeonWarrior);
            PlayCard("UnusualSuspects");
            QuickHPCheck(-4);


        }
        #endregion Test Unusual Suspects

        #region Test Borrowed Token Pools
        [Test]
        public void TestGuiseBorrowsImmortalCoil()
        {
            SetupGameController("BaronBlade", "Cauldron.Terminus", "Guise", "Legacy", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card coil = PlayCard("ImmortalCoils");
            PlayCard("LemmeSeeThat");


            UsePower(coil);
            AssertTokensInWrathPool(1);
        }


        [Test]
        public void TestRepOfEarthNonTerminus()
        {
            Assert.Ignore("Representative of Earth does not work in test mode.");
            SetupGameController("BaronBlade", "Haka", "Guise", "Legacy", "TheCelestialTribunal");
            StartGame();
            DestroyNonCharacterVillainCards();

            DecisionSelectFromBoxTurnTakerIdentifier = "Cauldron.Terminus";
            DecisionSelectFromBoxIdentifiers = new string[] { "Cauldron.MinistryOfStrategicScienceTeminus" };

            DecisionSelectTurnTaker = haka.TurnTaker;
            SetHitPoints(haka, 20);
            QuickHPStorage(baron, haka);
            PlayCard("CalledToJudgement");
            QuickHPCheck(-1, 1);
        }
        [Test]
        public void TestRepOfEarthOtherTerminus()
        {
            Assert.Ignore("Representative of Earth does not work in test mode.");
            SetupGameController("BaronBlade", "Cauldron.Terminus", "Guise", "Legacy", "TheCelestialTribunal");
            StartGame();
            DestroyNonCharacterVillainCards();

            DecisionSelectFromBoxTurnTakerIdentifier = "Cauldron.Terminus";
            DecisionSelectFromBoxIdentifiers = new string[] { "Cauldron.MinistryOfStrategicScienceTeminus" };

            DecisionSelectTurnTaker = terminus.TurnTaker;
            AddTokensToPool(WrathPool, 3);

            DecisionSelectNumber = 2;

            PlayCard("CalledToJudgement");

            AssertTokensInWrathPool(1);
        }
        #endregion 
    }
}
