using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Gargoyle;

namespace CauldronTests
{
    [TestFixture()]
    public class GargoyleTests : BaseTest
    {
        #region Gargoyle Utilities
        private HeroTurnTakerController gargoyle => FindHero("Gargoyle");
        private Card mobileDefensePlatform => GetCardInPlay("MobileDefensePlatform");
        private string[] gameDecks => new string[] { "BaronBlade", "Cauldron.Gargoyle", "Unity", "Bunker", "TheScholar", "Megalopolis" };

        private void StartTestGame()
        {
            SetupGameController(gameDecks);
            StartGame();

            DestroyCard(mobileDefensePlatform);
        }

        private void SetupIncapTest()
        {
            StartTestGame();
            SetupIncap(baron);
            AssertIncapacitated(gargoyle);
            GoToUseIncapacitatedAbilityPhase(gargoyle);
        }

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(gargoyle.CharacterCard, 1);
            DealDamage(villain, gargoyle, 2, DamageType.Melee);
        }

        #endregion Gargoyle Utilities

        [Test()]
        public void TestLoadGargoyle()
        {
            SetupGameController(gameDecks);

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(gargoyle);
            Assert.IsInstanceOf(typeof(GargoyleCharacterCardController), gargoyle.CharacterCardController);

            Assert.AreEqual(27, gargoyle.CharacterCard.HitPoints);
        }

        #region Test Innate Power
        /*
         * Power:
         * "Select a target. Reduce the next damage it deals by 1. Increase the next damage Gargoyle deals by 1."
         */
        [Test()]
        public void TestGargoyleInnatePower()
        {
            StartTestGame();

            GoToUsePowerPhase(gargoyle);

            // Select a target. 
            DecisionSelectTarget = baron.CharacterCard;
            UsePower(gargoyle.CharacterCard);
                        
            // Reduce the next damage it deals by 1.
            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(baron.CharacterCard, gargoyle.CharacterCard, 1, DamageType.Melee);
            QuickHPCheckZero();

            // Increase the next damage Gargoyle deals by 1.
            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(gargoyle.CharacterCard, baron.CharacterCard, 1, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0);
        }
        #endregion Test Innate Power

        #region Test Incap Powers
        /* 
         * Incap 1
         * "Increase the next damage dealt by a hero target by 2."
        */
        [Test()]
        public void TestGargoyleIncap1HeroCharacter()
        {
            SetupIncapTest();
            UseIncapacitatedAbility(gargoyle, 0);

            // Make sure it only affects hero damage
            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(bunker.CharacterCard, baron, 1, DamageType.Melee);
            QuickHPCheck(-3, 0, 0, 0);

            // Should only be the next damage,  this packet of damage should not be increased
            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(bunker.CharacterCard, baron, 1, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0);
        }
        [Test()]
        public void TestGargoyleIncap1HeroTarget()
        {
            Card mrChomps;

            SetupIncapTest();
            UseIncapacitatedAbility(gargoyle, 0);

            mrChomps = PlayCard(unity, "RaptorBot");

            // Make sure it only affects hero damage
            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(mrChomps, baron, 1, DamageType.Melee);
            QuickHPCheck(-3, 0, 0, 0);

            // Should only be the next damage,  this packet of damage should not be increased
            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(mrChomps, baron, 1, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0);
        }
        [Test()]
        public void TestGargoyleIncap1Villain()
        {
            SetupIncapTest();
            UseIncapacitatedAbility(gargoyle, 0);

            // Make sure it isn't affecting the villians damage
            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(baron.CharacterCard, bunker, 1, DamageType.Melee);
            QuickHPCheck(0, 0, -1, 0);
        }
        [Test()]
        public void TestGargoyleIncap1Environment()
        {
            TurnTakerController megalopolis;
            Card plummetingMonorail;

            SetupIncapTest();
            UseIncapacitatedAbility(gargoyle, 0);

            megalopolis = base.env;

            plummetingMonorail = PlayCard(megalopolis, "PlummetingMonorail");
            Assert.IsNotNull(plummetingMonorail);

            // Make sure it isn't affecting the evironment target's damage
            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(plummetingMonorail, bunker, 1, DamageType.Melee);
            QuickHPCheck(0, 0, -1, 0);
        }

        /* 
         * Incap 2
         * "Reduce the next damage dealt to a hero target by 2."
        */
        [Test()]
        public void TestGargoyleIncap2HeroCharacter()
        {
            SetupIncapTest();
            UseIncapacitatedAbility(gargoyle, 1);

            // Make sure it only affects heroes
            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(baron.CharacterCard, bunker, 2, DamageType.Melee);
            QuickHPCheckZero();

            // Should only be the next damage,  this packet of damage should not be increased
            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(baron.CharacterCard, bunker, 2, DamageType.Melee);
            QuickHPCheck(0, 0, -2, 0);

        }
        [Test()]
        public void TestGargoyleIncap2HeroTarget()
        {
            Card mrChomps;

            SetupIncapTest();
            UseIncapacitatedAbility(gargoyle, 1);

            mrChomps = PlayCard(unity, "RaptorBot");

            // Make sure it only affects heroes
            QuickHPStorage(mrChomps, baron.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            DealDamage(baron.CharacterCard, mrChomps, 2, DamageType.Melee);
            QuickHPCheckZero();

            // Should only be the next damage,  this packet of damage should not be increased
            QuickHPStorage(mrChomps, baron.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            DealDamage(baron.CharacterCard, mrChomps, 1, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0, 0);
        }
        [Test()]
        public void TestGargoyleIncap2Villain()
        {
            SetupIncapTest();
            UseIncapacitatedAbility(gargoyle, 1);

            // Make sure it isn't affecting damage to the villian
            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(bunker.CharacterCard, baron, 1, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0);
        }
        [Test()]
        public void TestGargoyleIncap2Environment()
        {
            TurnTakerController megalopolis;
            Card plummetingMonorail;

            SetupIncapTest();
            UseIncapacitatedAbility(gargoyle, 1);

            megalopolis = base.env;

            plummetingMonorail = PlayCard(megalopolis, "PlummetingMonorail");
            Assert.IsNotNull(plummetingMonorail);

            // Make sure it isn't affecting damage to the evironment target
            QuickHPStorage(plummetingMonorail, baron.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            DealDamage(bunker.CharacterCard, plummetingMonorail, 1, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0, 0);
        }

        /* 
         * Incap 3
         * "One player may draw a card now."
        */
        [Test()]
        public void TestGargoyleIncap3()
        {
            SetupIncapTest();
            AssertIncapLetsHeroDrawCard(gargoyle, 2, unity, 1);
        }

        #endregion Test Incap Powers

        /*
         * Absorb And Unleash
         * {Gargoyle} deals 1 hero target 2 toxic damage.
         * {Gargoyle} deals up to X targets 3 toxic damage each, where X is the amount of damage that was dealt to that hero target.
         */
        [Test()]
        public void TestAbsorbAndUnleash()
        {
            Card absorbAndUnleash;

            StartTestGame();

            PutOnDeck(gargoyle, gargoyle.HeroTurnTaker.Hand.Cards);
            absorbAndUnleash = PutInHand("AbsorbAndUnleash");

            DecisionSelectCards = new Card[] { gargoyle.CharacterCard, baron.CharacterCard, scholar.CharacterCard, null };
            DecisionYesNo = true;
            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            PlayCard(absorbAndUnleash);

            // Gargoyle should have been hit for 2. Baron and Scholar should have been hit for 3
            QuickHPCheck(-3, -2, 0, 0, -3);
        }

        [Test]
        public void TestAbsorbAndUnleashSkippedFinalTarget()
        {
            Card absorbAndUnleash;

            StartTestGame();

            PutOnDeck(gargoyle, gargoyle.HeroTurnTaker.Hand.Cards);
            absorbAndUnleash = PutInHand("AbsorbAndUnleash");

            DecisionSelectCards = new Card[] { gargoyle.CharacterCard, baron.CharacterCard, null };
            DecisionYesNo = true;
            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            PlayCard(absorbAndUnleash);

            // Gargoyle should have been hit for 2. Baron and Scholar should have been hit for 3
            QuickHPCheck(-3, -2, 0, 0, 0);
        }

    }
}
