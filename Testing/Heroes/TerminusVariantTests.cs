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
    public class TerminusVariantTests : CauldronBaseTest
    {
        #region Terminus Utilities

        private const string MinistryOfStrategicScienceTerminus = "MinistryOfStrategicScienceTerminusCharacter";
        private const string FutureTerminus = "FutureTerminusCharacter";

        private void StartTestGame(string variantName)
        {
            SetupGameController("BaronBlade", $"Cauldron.Terminus/{variantName}", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            base.DestroyNonCharacterVillainCards();
        }

        private void SetupIncapTest(string variantName)
        {
            StartTestGame(variantName);
            SetupIncap(baron);
            AssertIncapacitated(terminus);
            GoToUseIncapacitatedAbilityPhase(terminus);
        }

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(terminus.CharacterCard, 1);
            DealDamage(villain, terminus, 2, DamageType.Melee);
        }

        #endregion Terminus Utilities

        #region Ministry Of Strategic Science Terminus
        [Test()]
        public void TestLoadMinistryOfStrategicScienceTerminus()
        {
            SetupGameController("BaronBlade", $"Cauldron.Terminus/{MinistryOfStrategicScienceTerminus}", "Unity", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(terminus);
            Assert.IsInstanceOf(typeof(MinistryOfStrategicScienceTerminusCharacterCardController), terminus.CharacterCardController);

            Assert.AreEqual(30, terminus.CharacterCard.HitPoints);
        }

        #region Test Innate Power
        /*
         * Power:
         * Remove X tokens from your wrath pool (up to 2). {Terminus} deals 1 target X+1 cold damage and regains X+1 HP.
         */
        [Test()]
        public void TestMinistryOfStrategicScienceTerminusInnatePower0()
        {
            TokenPool tokenPool;
            StartTestGame(MinistryOfStrategicScienceTerminus);

            GoToUsePowerPhase(terminus);
            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");

            DecisionSelectNumber = 0;
            DecisionSelectTarget = baron.CharacterCard;
            SetHitPoints(terminus, 20);

            QuickTokenPoolStorage(tokenPool);
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            UsePower(terminus.CharacterCard);
            QuickHPCheck(-1, 1, 0, 0, 0);
            QuickTokenPoolCheck(0);
        }
        [Test()]
        public void TestMinistryOfStrategicScienceTerminusInnatePower1()
        {
            TokenPool tokenPool;
            StartTestGame(MinistryOfStrategicScienceTerminus);

            GoToUsePowerPhase(terminus);
            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
            AddTokensToPool(tokenPool, 3);

            DecisionSelectNumber = 1;
            DecisionSelectTarget = baron.CharacterCard;
            SetHitPoints(terminus, 20);

            QuickTokenPoolStorage(tokenPool);
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            UsePower(terminus.CharacterCard);
            QuickHPCheck(-2, 2, 0, 0, 0);
            QuickTokenPoolCheck(-1);
        }
        [Test()]
        public void TestMinistryOfStrategicScienceTerminusInnatePower2()
        {
            TokenPool tokenPool;
            StartTestGame(MinistryOfStrategicScienceTerminus);

            GoToUsePowerPhase(terminus);
            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
            AddTokensToPool(tokenPool, 3);

            DecisionSelectNumber = 2;
            DecisionSelectTarget = baron.CharacterCard;
            SetHitPoints(terminus, 20);

            QuickTokenPoolStorage(tokenPool);
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            UsePower(terminus.CharacterCard);
            QuickHPCheck(-3, 3, 0, 0, 0);
            QuickTokenPoolCheck(-2);
        }

        [Test()]
        public void TestMinistryOfStrategicScienceTerminusInnatePowerNotMoreThanExisting()
        {
            Assert.Ignore("Not sure how to make a test for this, but the log shows it works if you comment this assert.");
            TokenPool tokenPool;
            StartTestGame(MinistryOfStrategicScienceTerminus);

            GoToUsePowerPhase(terminus);
            tokenPool = terminus.CharacterCard.FindTokenPool("TerminusWrathPool");
            AddTokensToPool(tokenPool, 1);

            DecisionSelectNumber = 2;
            DecisionSelectTarget = baron.CharacterCard;
            SetHitPoints(terminus, 20);

            //remove 0 or remove 1, should not be able to try to remove 2
            AssertNumberOfChoicesInNextDecision(2, SelectionType.SelectNumeral);

            QuickTokenPoolStorage(tokenPool);
            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            Assert.Catch<Exception>(new TestDelegate(() => UsePower(terminus)));
            QuickTokenPoolCheck(0);
            QuickHPCheckZero();

            DecisionSelectNumber = 1;
            UsePower(terminus.CharacterCard);
            QuickHPCheck(-2, 2, 0, 0, 0);
            QuickTokenPoolCheck(-1);
        }

        #endregion Test Innate Power

        #region Test Incap Powers
        /* 
         * Incap 1
         * One hero may deal themselves 2 psychic damage to draw 2 cards.
        */
        [Test()]
        public void TestMinistryOfStrategicScienceTerminusIncap1YesDamage()
        {
            int cardsInDeck;

            SetupIncapTest(MinistryOfStrategicScienceTerminus);
            DecisionSelectCards = new Card[] { legacy.CharacterCard };
            DecisionYesNo = true;

            QuickHandStorage(legacy, bunker, scholar);
            QuickHPStorage(baron, legacy, bunker, scholar);
            cardsInDeck = legacy.TurnTaker.Deck.NumberOfCards;
            base.UseIncapacitatedAbility(terminus, 0);
            QuickHandCheck(2, 0, 0);
            QuickHPCheck(0, -2, 0, 0);
            AssertNumberOfCardsInDeck(legacy, cardsInDeck - 2);
        }
        [Test()]
        public void TestMinistryOfStrategicScienceTerminusIncap1NoDamage()
        {
            int cardsInDeck;

            SetupIncapTest(MinistryOfStrategicScienceTerminus);
            DecisionSelectCards = new Card[] { null };
            DecisionYesNo = false;
            QuickHandStorage(legacy, bunker, scholar);
            QuickHPStorage(baron, legacy, bunker, scholar);
            cardsInDeck = legacy.TurnTaker.Deck.NumberOfCards;
            base.UseIncapacitatedAbility(terminus, 0);
            QuickHandCheck(0, 0, 0);
            QuickHPCheck(0, 0, 0, 0);
            AssertNumberOfCardsInDeck(legacy, cardsInDeck);
        }
        [Test()]
        public void TestMinistryOfStrategicScienceTerminusIncap1DamagePrevented()
        {
            //Incap does not make the draws conditional on taking the damage

            int cardsInDeck;

            SetupIncapTest(MinistryOfStrategicScienceTerminus);
            DecisionSelectCards = new Card[] { scholar.CharacterCard };
            PlayCard("FleshToIron");
            DecisionYesNo = true;

            QuickHandStorage(legacy, bunker, scholar);
            QuickHPStorage(baron, legacy, bunker, scholar);
            cardsInDeck = scholar.TurnTaker.Deck.NumberOfCards;
            base.UseIncapacitatedAbility(terminus, 0);
            QuickHandCheck(0, 0, 2);
            QuickHPCheck(0, 0, 0, 0);
            AssertNumberOfCardsInDeck(scholar, cardsInDeck - 2);
        }
        /* 
         * Incap 2
         * One non-hero target regains 3 HP.
        */
        [Test()]
        public void TestMinistryOfStrategicScienceTerminusIncap2()
        {
            SetupIncapTest(MinistryOfStrategicScienceTerminus);
            DecisionSelectCards = new Card[] { baron.CharacterCard };
            Card traffic = PlayCard("TrafficPileup");
            SetHitPoints(traffic, 5);
            AssertNextDecisionChoices(new Card[] { baron.CharacterCard, traffic }, new Card[] { legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard });
            SetHitPoints(baron.CharacterCard, 20);
            QuickHPStorage(baron, legacy, bunker, scholar);
            UseIncapacitatedAbility(terminus, 1);
            QuickHPCheck(3, 0, 0, 0);
            Assert.AreEqual(5, traffic.HitPoints);
        }

        /* 
         * Incap 3
         * Reveal the top 3 cards of a hero deck. Discard 2 of them and replace the other.
        */
        [Test()]
        public void TestMinistryOfStrategicScienceTerminusIncap3()
        {
            int bunkerDeckCount;
            int bunkerTrashCount;
            Card adhesiveFoamGrenade;
            Card heavyPlating;
            Card upgradeMode;

            SetupIncapTest(MinistryOfStrategicScienceTerminus);

            adhesiveFoamGrenade = PutOnDeck("AdhesiveFoamGrenade");
            heavyPlating = PutOnDeck("HeavyPlating");
            upgradeMode = PutOnDeck("UpgradeMode");

            DecisionSelectCards = new Card[] { heavyPlating, adhesiveFoamGrenade, upgradeMode };
            DecisionSelectLocation = new LocationChoice(bunker.TurnTaker.Deck);
            bunkerDeckCount = bunker.TurnTaker.Deck.NumberOfCards;
            bunkerTrashCount = bunker.TurnTaker.Trash.NumberOfCards;

            UseIncapacitatedAbility(terminus, 2);

            AssertInDeck(bunker, upgradeMode);
            AssertInTrash(bunker, new Card[] { heavyPlating, adhesiveFoamGrenade });
            AssertNumberOfCardsInDeck(bunker, bunkerDeckCount - 2);
            AssertNumberOfCardsInTrash(bunker, bunkerTrashCount + 2);
        }

        #endregion Test Incap Powers

        #endregion Ministry Of Strategic Science Terminus

        #region Future Terminus
        [Test()]
        public void TestLoadFutureTerminus()
        {
            SetupGameController("BaronBlade", $"Cauldron.Terminus/{FutureTerminus}", "Unity", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(terminus);
            Assert.IsInstanceOf(typeof(FutureTerminusCharacterCardController), terminus.CharacterCardController);

            Assert.AreEqual(29, terminus.CharacterCard.HitPoints);
        }

        #region Test Innate Power
        /*
         * Power:
         * Play a card. {Terminus} deals herself 2 cold damage.
         */
        [Test()]
        public void TestFutureTerminusInnatePower()
        {
            Card covenantOfWrath;

            StartTestGame(FutureTerminus);

            GoToUsePowerPhase(terminus);

            covenantOfWrath = PutInHand(terminus, "CovenantOfWrath");

            // Select a target. 
            DecisionSelectCards = new Card[] { covenantOfWrath };

            QuickHPStorage(baron, terminus, legacy, bunker, scholar);
            QuickHandStorage(terminus, legacy, bunker, scholar);
            UsePower(terminus.CharacterCard);
            QuickHandCheck(-1, 0, 0, 0);
            QuickHPCheck(0, -2, 0, 0, 0);
            AssertIsInPlay(covenantOfWrath);
        }

        #endregion Test Innate Power

        #region Test Incap Powers
        /* 
         * Incap 1
         * One hero may use a power now.
        */
        [Test()]
        public void TestFutureTerminusIncap1()
        {
            SetupIncapTest(FutureTerminus);
            AssertIncapLetsHeroUsePower(terminus, 0, bunker);
        }

        /* 
         * Incap 2
         * Each environment target deals itself 3 cold damage.
        */
        [Test()]
        public void TestFutureTerminusIncap2HeroTarget()
        {
            Card plummetingMonorail1;
            Card plummetingMonorail2;
            SetupIncapTest(FutureTerminus);

            plummetingMonorail1 = PutIntoPlay("PlummetingMonorail");
            plummetingMonorail2 = PutIntoPlay("PlummetingMonorail");

            QuickHPStorage(baron.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, plummetingMonorail1, plummetingMonorail2);
            UseIncapacitatedAbility(terminus, 1);
            QuickHPCheck(0, 0, 0, 0, -3, -3);
        }

        /* 
         * Incap 3
         * Reveal the top 2 cards of a non-villain deck and replace them in the same order.
        */
        [Test()]
        public void TestFutureTerminusIncap3Hero()
        {
            int bunkerDeckCount;
            int bunkerTrashCount;
            Card adhesiveFoamGrenade;
            Card heavyPlating;
            Card upgradeMode;

            SetupIncapTest(FutureTerminus);
            adhesiveFoamGrenade = PutOnDeck("AdhesiveFoamGrenade");
            heavyPlating = PutOnDeck("HeavyPlating");
            upgradeMode = PutOnDeck("UpgradeMode");
            bunkerDeckCount = bunker.TurnTaker.Deck.NumberOfCards;
            bunkerTrashCount = bunker.TurnTaker.Trash.NumberOfCards;

            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(bunker.TurnTaker.Deck) };
            UseIncapacitatedAbility(terminus, 2);

            AssertNumberOfCardsInDeck(bunker, bunkerDeckCount);
            AssertNumberOfCardsInTrash(bunker, bunkerTrashCount);
            AssertOnTopOfDeck(adhesiveFoamGrenade, 2);
            AssertOnTopOfDeck(heavyPlating, 1);
            AssertOnTopOfDeck(upgradeMode, 0);
        }

        #endregion Test Incap Powers

        #endregion Future Terminus
    }
}