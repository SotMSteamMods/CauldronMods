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
    public class GargoyleTests : CauldronBaseTest
    {
        #region Gargoyle Utilities
        private string[] gameDecks => new string[] { "BaronBlade", "Cauldron.Gargoyle", "Unity", "Bunker", "TheScholar", "Megalopolis" };

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
            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            DealDamage(baron.CharacterCard, gargoyle.CharacterCard, 1, DamageType.Melee);
            QuickHPCheckZero();
            // Only once
            DealDamage(baron, gargoyle, 1, DamageType.Melee);
            QuickHPCheck(0, -1, 0, 0, 0);
            
            // Increase the next damage Gargoyle deals by 1.
            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            DealDamage(gargoyle.CharacterCard, baron.CharacterCard, 1, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0, 0);
            // Only once
            DealDamage(gargoyle, baron, 1, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0, 0);
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

        #region Test Absorb and Unleash
        /*
         * Absorb And Unleash
         * {Gargoyle} may deal 1 target 0 toxic damage.
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

            DecisionSelectCards = new Card[] { null, gargoyle.CharacterCard, baron.CharacterCard, scholar.CharacterCard, null };
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

            DecisionSelectCards = new Card[] { null, gargoyle.CharacterCard, baron.CharacterCard, null };
            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            PlayCard(absorbAndUnleash);

            // Gargoyle should have been hit for 2. Baron should have been hit for 3
            QuickHPCheck(-3, -2, 0, 0, 0);
        }

        [Test]
        public void TestAbsorbAndUnleashFirstDamage()
        {
            StartTestGame("BaronBlade", "Cauldron.Gargoyle", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            //Boost damage so the 0-damage hit is visible
            UsePower(legacy);

            DecisionSelectCards = new Card[] { baron.CharacterCard, gargoyle.CharacterCard, baron.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard };
            QuickHPStorage(baron, gargoyle, legacy, bunker, scholar);
            PlayCard("AbsorbAndUnleash");

            //Gargoyle hits Baron for 0 + 1, himself for 2 + 1, Baron, Legacy, and Bunker for 3 + 1 each, and runs out before Scholar
            QuickHPCheck(-5, -3, -4, -4, 0);
        }

        [Test]
        public void TestAbsorbAndUnleashNoHeroDamage()
        {
            StartTestGame("BaronBlade", "Cauldron.Gargoyle", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            PlayCard("HeroicInterception");
            UsePower(legacy);
            DecisionSelectCards = new Card[] { baron.CharacterCard, gargoyle.CharacterCard, baron.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, scholar.CharacterCard };
            QuickHPStorage(baron, gargoyle, legacy, bunker, scholar);
            PlayCard("AbsorbAndUnleash");

            //Gargoyle hits Baron for 1, the damage to himself is prevented, and he doesn't get to hit again.
            QuickHPCheck(-1, 0, 0, 0, 0);
        }

        [Test]
        public void TestAbsorbAndUnleash_RedirectedHeroDamage()
        {
            StartTestGame("BaronBlade", "Cauldron.Gargoyle", "Legacy", "Bunker", "Tachyon", "Megalopolis");

            UsePower(legacy);
            DecisionSelectCards = new Card[] { baron.CharacterCard, tachyon.CharacterCard, baron.CharacterCard, baron.CharacterCard, legacy.CharacterCard, bunker.CharacterCard, tachyon.CharacterCard };
            QuickHPStorage(baron, gargoyle, legacy, bunker, tachyon);

            DecisionYesNo = true;
            PlayCard("SynapticInterruption");
            PlayCard("AbsorbAndUnleash");

            //Gargoyle hits Baron for 1, the damage to tachyon is redirected, so X is 0
            QuickHPCheck(-4, 0, 0, 0, 0);
        }

        #endregion Test Absorb and Unleash

        #region Test Agile Technique
        /*
         * Power
         * {Gargoyle} deals up to 2 targets 2 toxic damage each.
         * If a hero was damaged this way, {Gargoyle} deals a third target 2 melee damage.
         */
        [Test]
        public void TestAgileTechnique()
        {
            Card agileTechnique;
            Card bladeBattalion;

            StartTestGame();
            PutOnDeck(gargoyle, gargoyle.HeroTurnTaker.Hand.Cards);

            GoToUsePowerPhase(gargoyle);

            agileTechnique = PutIntoPlay("AgileTechnique");
            bladeBattalion = PutIntoPlay("BladeBattalion");

            DecisionSelectCards = new Card[] { gargoyle.CharacterCard, baron.CharacterCard, bladeBattalion };

            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion);
            UsePower(agileTechnique);

            // Baron and Gargoyle should be hit for 2, and Blade Battallion for 2 + 1
            QuickHPCheck(-2, -2, 0, 0, 0, -3);
        }
        [Test]
        public void TestAgileTechniqueNoHero()
        {
            Card agileTechnique;
            Card bladeBattalion;

            StartTestGame();
            PutOnDeck(gargoyle, gargoyle.HeroTurnTaker.Hand.Cards);

            GoToUsePowerPhase(gargoyle);

            agileTechnique = PutIntoPlay("AgileTechnique");
            bladeBattalion = PutIntoPlay("BladeBattalion");

            DecisionSelectCards = new Card[] { baron.CharacterCard, bladeBattalion };

            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion);
            UsePower(agileTechnique);

            // Baron, Blade Battallion should have been hit for 2
            QuickHPCheck(-2, 0, 0, 0, 0, -2);
        }
        [Test]
        public void TestAgileTechniqueSelectedOneVillian()
        {
            Card agileTechnique;
            Card bladeBattalion;

            StartTestGame();
            PutOnDeck(gargoyle, gargoyle.HeroTurnTaker.Hand.Cards);

            GoToUsePowerPhase(gargoyle);

            agileTechnique = PutIntoPlay("AgileTechnique");
            bladeBattalion = PutIntoPlay("BladeBattalion");

            DecisionSelectCards = new Card[] { baron.CharacterCard, null };

            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion);
            UsePower(agileTechnique);

            // Baron, Blade Battallion should have been hit for 2
            QuickHPCheck(-2, 0, 0, 0, 0, 0);
        }
        [Test]
        public void TestAgileTechniqueSelectedOneHero()
        {
            Card agileTechnique;
            Card bladeBattalion;

            StartTestGame();
            PutOnDeck(gargoyle, gargoyle.HeroTurnTaker.Hand.Cards);

            GoToUsePowerPhase(gargoyle);

            agileTechnique = PutIntoPlay("AgileTechnique");
            bladeBattalion = PutIntoPlay("BladeBattalion");

            DecisionSelectCards = new Card[] { gargoyle.CharacterCard, null, baron.CharacterCard };

            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion);
            UsePower(agileTechnique);

            // Gargoyle should be hit for 2, Baron for 2 + 1
            QuickHPCheck(-3, -2, 0, 0, 0, 0);
        }
        [Test]
        public void TestAgileTechniqueImmuneHeroTarget()
        {
            StartTestGame("BaronBlade", "Cauldron.Gargoyle", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            PlayCard("HeroicInterception");

            Card battalion = PlayCard("BladeBattalion");

            var ofInterest = new List<Card> { scholar.CharacterCard, baron.CharacterCard, battalion };
            DecisionSelectCards = ofInterest;
            QuickHPStorage(ofInterest.ToArray());

            Card agile = PlayCard("AgileTechnique");
            UsePower(agile);

            QuickHPCheck(0, -2, 0);
        }
        [Test]
        public void TestAgileTechniqueNonCharacterHeroTarget()
        {
            StartTestGame("BaronBlade", "Cauldron.Gargoyle", "Unity", "Bunker", "TheScholar", "Megalopolis");

            Card bot = PlayCard("SwiftBot");

            Card battalion = PlayCard("BladeBattalion");

            var ofInterest = new List<Card> { bot, baron.CharacterCard, battalion };
            DecisionSelectCards = ofInterest;
            QuickHPStorage(ofInterest.ToArray());

            Card agile = PlayCard("AgileTechnique");
            UsePower(agile);

            QuickHPCheck(-2, -2, 0);
        }
        [Test]
        public void TestAgileTechniqueFinalDamageBasedOnHeroDamage()
        {
            StartTestGame("BaronBlade", "Cauldron.Gargoyle", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            PlayCard("Fortitude");

            Card battalion = PlayCard("BladeBattalion");

            var ofInterest = new List<Card> { legacy.CharacterCard, baron.CharacterCard, battalion };
            DecisionSelectCards = ofInterest;
            QuickHPStorage(ofInterest.ToArray());

            Card agile = PlayCard("AgileTechnique");
            UsePower(agile);

            QuickHPCheck(-1, -2, -2);
        }
        [Test]
        public void TestAgileTechniqueDamageMultipleHeroes()
        {
            StartTestGame("BaronBlade", "Cauldron.Gargoyle", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            PlayCard("Fortitude");

            Card battalion = PlayCard("BladeBattalion");

            var ofInterest = new List<Card> { legacy.CharacterCard, gargoyle.CharacterCard, baron.CharacterCard };
            DecisionSelectCards = ofInterest;
            QuickHPStorage(ofInterest.ToArray());

            Card agile = PlayCard("AgileTechnique");
            UsePower(agile);

            QuickHPCheck(-1, -2, -2);
        }
        [Test]
        public void TestAgileTechniqueNoFunctionDecisionIfSameDamage()
        {
            StartTestGame("BaronBlade", "Cauldron.Gargoyle", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Card battalion = PlayCard("BladeBattalion");

            var ofInterest = new List<Card> { legacy.CharacterCard, gargoyle.CharacterCard, baron.CharacterCard };
            DecisionSelectCards = ofInterest;
            QuickHPStorage(ofInterest.ToArray());

            Card agile = PlayCard("AgileTechnique");
            UsePower(agile);

            //who to hit #1, who to hit #2, no function decision, who to hit #3
            AssertMaxNumberOfDecisions(3);
            QuickHPCheck(-2, -2, -3);
        }
        #endregion Test Agile Technique

        #region Test Bioenergy Pulse
        /*
         * Whenever {Gargoyle} deals himself damage, he may also deal that to 1 other target.
         * Power
         * {Gargoyle} deals each non-hero target 1 toxic damage.
         * Increase the next damage {Gargoyle} deals by 1.
         */
        [Test]
        public void TestBioenergyPulseTrigger()
        {
            Card agileTechnique;
            Card bioenergyPulse;
            Card bladeBattalion;
            Card plummetingMonorail;

            StartTestGame();
            PutOnDeck(gargoyle, gargoyle.HeroTurnTaker.Hand.Cards);

            GoToUsePowerPhase(gargoyle);

            agileTechnique = PutIntoPlay("AgileTechnique");
            bioenergyPulse = PutIntoPlay("BioenergyPulse");
            bladeBattalion = PutIntoPlay("BladeBattalion");
            plummetingMonorail = PutIntoPlay("PlummetingMonorail");

            
            DecisionSelectCard = baron.CharacterCard;
            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion);

            DealDamage(gargoyle, gargoyle, 3, DamageType.Melee);
            //Gargoyle should hit himself for 3 and Blade for 3
            QuickHPCheck(-3, -3, 0, 0, 0, 0);

            PlayCard("RooftopCombat");
            //With +1 damage, he should hit himself for 3 + 1 and Blade for 4 + 1
            DealDamage(gargoyle, gargoyle, 3, DamageType.Melee);
            QuickHPCheck(-5, -4, 0, 0, 0, 0);
        }

        [Test]
        public void TestBioenergyPulseTriggerNoDamageToGargoyle()
        {
            Card agileTechnique;
            Card bioenergyPulse;
            Card bladeBattalion;
            Card plummetingMonorail;

            StartTestGame();
            PutOnDeck(gargoyle, gargoyle.HeroTurnTaker.Hand.Cards);

            GoToUsePowerPhase(gargoyle);

            agileTechnique = PutIntoPlay("AgileTechnique");
            bioenergyPulse = PutIntoPlay("BioenergyPulse");
            bladeBattalion = PutIntoPlay("BladeBattalion");
            plummetingMonorail = PutIntoPlay("PlummetingMonorail");

            DecisionSelectCards = new Card[] { unity.CharacterCard, baron.CharacterCard, bladeBattalion };

            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion);
            UsePower(agileTechnique);

            // Baron & Unity hit for 2, Blade Battallion should have been hit for 3
            QuickHPCheck(-2, 0, -2, 0, 0, -3);
        }

        [Test]
        public void TestBioenergyPulsePower()
        {
            Card bioenergyPulse;
            Card bladeBattalion;
            Card plummetingMonorail;

            StartTestGame();
            PutOnDeck(gargoyle, gargoyle.HeroTurnTaker.Hand.Cards);

            GoToUsePowerPhase(gargoyle);

            bioenergyPulse = PutIntoPlay("BioenergyPulse");
            bladeBattalion = PutIntoPlay("BladeBattalion");
            plummetingMonorail = PutIntoPlay("PlummetingMonorail");

            DecisionSelectCards = new Card[] { plummetingMonorail, baron.CharacterCard, plummetingMonorail, baron.CharacterCard };

            //{Gargoyle} deals each non-hero target 1 toxic damage.
            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion, plummetingMonorail);
            UsePower(bioenergyPulse);

            // Plummeting Monorail, Baron Blade, and Blade Battalion should have been hit for 1.
            QuickHPCheck(-1, 0, 0, 0, 0, -1, -1);

            // Increase the next damage {Gargoyle} deals by 1.
            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion, plummetingMonorail);
            UsePower(bioenergyPulse);

            // Plummeting Monorail should have been hit for 2. Baron Blade, and Blade Battalion should have been hit for 1.
            QuickHPCheck(-1, 0, 0, 0, 0, -1, -2);
        }
        [Test]
        public void TestBioenergyPulseTriggerWhenImmuneToDamage()
        {
            StartTestGame("BaronBlade", "Cauldron.Gargoyle", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            PlayCard("HeroicInterception");
            PlayCard("BioenergyPulse");

            DecisionSelectCard = baron.CharacterCard;
            QuickHPStorage(baron, gargoyle, legacy, bunker, scholar);

            DealDamage(gargoyle, gargoyle, 5, DamageType.Melee);
            QuickHPCheckZero();
        }
        #endregion Test Bioenergy Pulse

        #region Dreamcatcher
        /*                
         *  Draw 2 cards.
         *  Reduce the next damage dealt by {Gargoyle} by X, where X is up to 3.
         *  {Gargoyle} deals 1 target 1 toxic damage. If that target takes damage this way, {Gargoyle} deals X other targets 2 toxic damage each."
         */
        [Test]
        public void TestDreamcatcher([Values(0, 1, 2, 3)] int reduction)
        {
            Card dreamcatcher;

            StartTestGame("BaronBlade", "Cauldron.Gargoyle", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            PutOnDeck(gargoyle, gargoyle.HeroTurnTaker.Hand.Cards);
            dreamcatcher = PutInHand("Dreamcatcher");

            UsePower(legacy); // increase hero damage by 1
            PutIntoPlay("InspiringPresence"); // Increase hero damage by 1
            PutIntoPlay("CrampedQuartersCombat"); // increase all damage by 1 and make it melee

            GoToPlayCardPhase(gargoyle);

            QuickHandStorage(gargoyle, legacy, scholar, bunker);
            QuickHPStorage(baron, gargoyle, legacy, scholar, bunker);
            DecisionSelectFunctions = new int?[] { reduction }; // x = 3
            DecisionSelectTargets = new Card[] { gargoyle.CharacterCard, baron.CharacterCard, legacy.CharacterCard, scholar.CharacterCard };
            PlayCard(dreamcatcher);
            // Gargoyle should have gained 2 cards. So -1 card played +2 cards gained would be a net +1
            QuickHandCheck(1, 0, 0, 0);

            var selfDamage = -4 + reduction;
            var excessDamage = new List<int>();
            for(int i = 0; i < 3; i++)
            {
                if(i < reduction)
                {
                    excessDamage.Add(-5);
                }
                else
                {
                    excessDamage.Add(0);
                }
            }
            QuickHPCheck(excessDamage[0], selfDamage, excessDamage[1], excessDamage[2], 0);
        }
        [Test]
        public void TestDreamcatcherNoDamage()
        {
            Card dreamcatcher;

            StartTestGame("BaronBlade", "Cauldron.Gargoyle", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            PutOnDeck(gargoyle, gargoyle.HeroTurnTaker.Hand.Cards);
            dreamcatcher = PutInHand("Dreamcatcher");

            UsePower(legacy); // increase hero damage by 1

            GoToPlayCardPhase(gargoyle);

            QuickHandStorage(gargoyle, legacy, scholar, bunker);
            QuickHPStorage(baron, gargoyle, legacy, scholar, bunker);
            DecisionSelectFunctions = new int?[] { 3 }; // x = 3
            DecisionSelectTargets = new Card[] { gargoyle.CharacterCard, baron.CharacterCard, legacy.CharacterCard, scholar.CharacterCard };
            PlayCard(dreamcatcher);
            // Gargoyle should have gained 2 cards. So -1 card played +2 cards gained would be a net +1
            QuickHandCheck(1, 0, 0, 0);

            //self-damage was 1 + 1 - 3 <= 0, so no damage dealt and we don't get the extra hits 
            QuickHPCheckZero();
        }
        [Test]
        public void TestDreamcatcherDamageRedirected()
        {
            Card dreamcatcher;

            StartTestGame("BaronBlade", "Cauldron.Gargoyle", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            PutOnDeck(gargoyle, gargoyle.HeroTurnTaker.Hand.Cards);
            dreamcatcher = PutInHand("Dreamcatcher");

            UsePower(legacy); // increase hero damage by 1
            PutIntoPlay("InspiringPresence"); // Increase hero damage by 1
            PutIntoPlay("CrampedQuartersCombat"); // increase all damage by 1 and make it melee
            PutIntoPlay("AlchemicalRedirection"); //redirect self-damage to Scholar

            GoToPlayCardPhase(gargoyle);

            QuickHandStorage(gargoyle, legacy, scholar, bunker);
            QuickHPStorage(baron, gargoyle, legacy, scholar, bunker);
            DecisionSelectFunctions = new int?[] { 3 }; // x = 3
            DecisionSelectTargets = new Card[] { gargoyle.CharacterCard, baron.CharacterCard, legacy.CharacterCard, scholar.CharacterCard };
            PlayCard(dreamcatcher);
            // Gargoyle should have gained 2 cards. So -1 card played +2 cards gained would be a net +1
            QuickHandCheck(1, 0, 0, 0);
            QuickHPCheck(0, 0, 0, -1, 0);
        }
        #endregion

        #region Essence Theft

        /* 
         * {Gargoyle} may deal 3 targets 1 toxic damage each or 1 target 3 melee damage.
         * If any targets were dealt damage this way, {Gargoyle} regains 1HP.
         */
        [Test]
        public void TestEssenceTheftMultipleTarget()
        {
            Card bladeBattalion1;
            Card bladeBattalion2;
            Card bladeBattalion3;

            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            bladeBattalion1 = PutIntoPlay("BladeBattalion");
            bladeBattalion2 = PutIntoPlay("BladeBattalion");
            bladeBattalion3 = PutIntoPlay("BladeBattalion");

            // {Gargoyle} may deal 3 targets 1 toxic damage each
            SetHitPoints(gargoyle, gargoyle.CharacterCard.MaximumHitPoints.Value - 1);

            DecisionSelectFunctions = new int?[] { 0 };
            DecisionSelectTargets = new Card[] { baron.CharacterCard, bladeBattalion1, bladeBattalion2};
            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3);
            PlayCard("EssenceTheft");
            //If any targets were dealt damage this way, { Gargoyle} regains 1HP
            QuickHPCheck(-1, 1, 0, 0, 0, -1, -1, 0);
        }
        [Test]
        public void TestEssenceTheftMultipleTargetNoDamage()
        {
            Card bladeBattalion1;
            Card bladeBattalion2;
            Card bladeBattalion3;

            StartTestGame("BaronBlade", "Cauldron.Gargoyle", "Cauldron.Cricket", "Bunker", "TheScholar", "Megalopolis");

            GoToPlayCardPhase(gargoyle);

            bladeBattalion1 = PutIntoPlay("BladeBattalion");
            bladeBattalion2 = PutIntoPlay("BladeBattalion");
            bladeBattalion3 = PutIntoPlay("BladeBattalion");
            // Prevent all damage
            PutIntoPlay("SoundMasking");

            // {Gargoyle} may deal 3 targets 1 toxic damage each
            SetHitPoints(gargoyle, gargoyle.CharacterCard.MaximumHitPoints.Value - 1);

            DecisionSelectFunctions = new int?[] { 0 };
            DecisionSelectTargets = new Card[] { baron.CharacterCard, bladeBattalion1, bladeBattalion2 };
            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, cricket.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3);
            PlayCard("EssenceTheft");
            //If any targets were dealt damage this way, { Gargoyle} regains 1HP
            QuickHPCheck(0, 0, 0, 0, 0, 0, 0, 0);
        }

        [Test]
        public void TestEssenceTheftSingleTarget()
        {
            Card bladeBattalion1;
            Card bladeBattalion2;
            Card bladeBattalion3;

            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            bladeBattalion1 = PutIntoPlay("BladeBattalion");
            bladeBattalion2 = PutIntoPlay("BladeBattalion");
            bladeBattalion3 = PutIntoPlay("BladeBattalion");

            // {Gargoyle} may deal 1 targets 3 melee damage
            SetHitPoints(gargoyle, gargoyle.CharacterCard.MaximumHitPoints.Value - 1);

            DecisionSelectFunctions = new int?[] { 1 };
            DecisionSelectTargets = new Card[] { baron.CharacterCard };
            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3);
            PlayCard("EssenceTheft"); 
            //If any targets were dealt damage this way, { Gargoyle} regains 1HP
            QuickHPCheck(-3, 1, 0, 0, 0, 0, 0, 0);
        }

        [Test]
        public void TestEssenceTheftSingleTargetNoDamage()
        {
            Card bladeBattalion1;
            Card bladeBattalion2;
            Card bladeBattalion3;

            StartTestGame("BaronBlade", "Cauldron.Gargoyle", "Cauldron.Cricket", "Bunker", "TheScholar", "Megalopolis");

            GoToPlayCardPhase(gargoyle);

            bladeBattalion1 = PutIntoPlay("BladeBattalion");
            bladeBattalion2 = PutIntoPlay("BladeBattalion");
            bladeBattalion3 = PutIntoPlay("BladeBattalion");
            // Prevent all damage
            PutIntoPlay("SoundMasking");

            // {Gargoyle} may deal 1 targets 3 melee damage
            SetHitPoints(gargoyle, gargoyle.CharacterCard.MaximumHitPoints.Value - 1);

            DecisionSelectFunctions = new int?[] { 1 };
            DecisionSelectTargets = new Card[] { baron.CharacterCard };
            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, cricket.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3);
            PlayCard("EssenceTheft");
            //If any targets were dealt damage this way, { Gargoyle} regains 1HP
            QuickHPCheck(0, 0, 0, 0, 0, 0, 0, 0);
        }
        #endregion Essence Theft

        #region Grim Herald
        /*
         * {Gargoyle} deals 1 target 3 toxic damage. 
         * One other player may discard a card. If they do, you may play a card or draw a card now.
         */
        [Test]
        public void TestGrimHeraldPlayCard()
        {
            Card beeBot;
            Card markForExecution;
            Card grimHerald;

            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            beeBot = PutInHand(unity, "BeeBot");
            markForExecution = PutInHand(gargoyle, "MarkForExecution");
            grimHerald = PutInHand(gargoyle, "GrimHerald");

            DecisionSelectTurnTakers = new TurnTaker[] { unity.TurnTaker };
            DecisionSelectCards = new Card[] { baron.CharacterCard , beeBot, markForExecution };

            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            QuickHandStorage(gargoyle, unity, bunker, scholar);
            PlayCard(grimHerald);
            QuickHPCheck(-3, 0, 0, 0, 0);
            QuickHandCheck(-2, -1, 0, 0);
        }

        [Test]
        public void TestGrimHeraldDrawCard()
        {
            Card beeBot;
            Card markForExecution;
            Card grimHerald;

            StartTestGame();

            GoToPlayCardPhase(gargoyle);
            MoveAllCardsFromHandToDeck(scholar);

            beeBot = PutInHand(unity, "BeeBot");
            markForExecution = PutInHand(gargoyle, "MarkForExecution");
            grimHerald = PutInHand(gargoyle, "GrimHerald");

            DecisionSelectTurnTakers = new TurnTaker[] { unity.TurnTaker };
            DecisionSelectCards = new Card[] { baron.CharacterCard, beeBot, markForExecution };
            DecisionSelectFunctions = new int?[] { 1 };

            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            QuickHandStorage(gargoyle, unity, bunker, scholar);
            PlayCard(grimHerald);
            QuickHPCheck(-3, 0, 0, 0, 0);
            QuickHandCheck(0, -1, 0, 0);
        }
        [Test]
        public void TestGrimHeraldPlayOrDrawOptional()
        {
            Card beeBot;
            Card markForExecution;
            Card grimHerald;

            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            beeBot = PutInHand(unity, "BeeBot");
            markForExecution = PutInHand(gargoyle, "MarkForExecution");
            grimHerald = PutInHand(gargoyle, "GrimHerald");

            DecisionSelectTurnTakers = new TurnTaker[] { unity.TurnTaker };
            DecisionSelectCards = new Card[] { baron.CharacterCard, beeBot, markForExecution };
            DecisionDoNotSelectFunction = true;

            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            QuickHandStorage(gargoyle, unity, bunker, scholar);
            PlayCard(grimHerald);
            QuickHPCheck(-3, 0, 0, 0, 0);
            QuickHandCheck(-1, -1, 0, 0);
        }
        [Test]
        public void TestGrimHeraldExcludeHeroWithNoCardInHand()
        {
            Card grimHerald;
            StartTestGame();

            GoToPlayCardPhase(gargoyle);
            grimHerald = PutInHand(gargoyle, "GrimHerald");

            MoveAllCardsFromHandToDeck(unity);
            MoveAllCardsFromHandToDeck(scholar);
            MoveAllCardsFromHandToDeck(bunker);

            AssertMaxNumberOfDecisions(1);
            PlayCard(grimHerald);
        }

        #endregion Grim Herald

        #region Leech Field
        /*
         * Once per turn, when {Gargoyle} deals or is dealt damage, or when a non-hero target is dealt damage
         * you may reduce that damage by 1 and increase the next damage dealt by {Gargoyle} by 1.
         */
        [Test]
        public void TestLeechFieldGargoyleDamaged()
        {
            Card bladeBattalion1;
            Card bladeBattalion2;
            Card bladeBattalion3;

            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            bladeBattalion1 = PutIntoPlay("BladeBattalion");
            bladeBattalion2 = PutIntoPlay("BladeBattalion");
            bladeBattalion3 = PutIntoPlay("BladeBattalion");
            PutIntoPlay("LeechField");

            SelectYesNoForNextDecision(true,true);
            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3);
            DealDamage(baron.CharacterCard, gargoyle.CharacterCard, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, 0, 0, 0);
            DealDamage(gargoyle.CharacterCard, bladeBattalion1, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, -2, 0, 0);
        }

        [Test]
        public void TestLeechFieldGargoyleDealsDamage()
        {
            Card bladeBattalion1;
            Card bladeBattalion2;
            Card bladeBattalion3;

            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            bladeBattalion1 = PutIntoPlay("BladeBattalion");
            bladeBattalion2 = PutIntoPlay("BladeBattalion");
            bladeBattalion3 = PutIntoPlay("BladeBattalion");
            PutIntoPlay("LeechField");

            SelectYesNoForNextDecision(true, true);
            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3);
            DealDamage(gargoyle.CharacterCard, bladeBattalion1, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, 0, 0, 0);
            DealDamage(gargoyle.CharacterCard, bladeBattalion1, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, -2, 0, 0);
        }

        [Test]
        public void TestLeechFieldDamageInFlight()
        {
            SetupGameController("BaronBlade", "Cauldron.Gargoyle", "VoidGuardMainstay", "InsulaPrimalis");
            StartGame();

            DestroyNonCharacterVillainCards();

            PutIntoPlay("PreemptivePayback");

            GoToPlayCardPhase(gargoyle);

            var bladeBattalion = PutIntoPlay("BladeBattalion");
            PutIntoPlay("LeechField");

            DecisionSelectTarget = bladeBattalion;
            DecisionsYesNo = new bool[] { false, true, true };
            QuickHPStorage(bladeBattalion, voidMainstay.CharacterCard);
            DealDamage(gargoyle.CharacterCard, voidMainstay.CharacterCard, 1, DamageType.Melee);
            QuickHPCheck(-2, -2);
        }

        [Test]
        public void TestLeechFieldAppliesToPreemptive()
        {
            SetupGameController("BaronBlade", "Cauldron.Gargoyle/WastelandRoninGargoyleCharacter", "VoidGuardMainstay", "CaptainCosmic", "InsulaPrimalis");
            StartGame();

            DestroyNonCharacterVillainCards();

            PutIntoPlay("PreemptivePayback");

            GoToPlayCardPhase(gargoyle);

            var bladeBattalion = PutIntoPlay("BladeBattalion");

            DecisionSelectCard = gargoyle.CharacterCard;
            var siphon = PutIntoPlay("DynamicSiphon");

            PutIntoPlay("LeechField");

            DecisionSelectTargets = new Card[] { siphon, bladeBattalion };
            DecisionsYesNo = new bool[] { true, true };
            QuickHPStorage(bladeBattalion, voidMainstay.CharacterCard);
            DealDamage(gargoyle.CharacterCard, voidMainstay.CharacterCard, 2, DamageType.Melee);
            QuickHPCheck(-3, -1);
        }

        [Test]
        public void TestLeechFieldOtherHeroDamaged()
        {
            Card bladeBattalion1;
            Card bladeBattalion2;
            Card bladeBattalion3;

            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            bladeBattalion1 = PutIntoPlay("BladeBattalion");
            bladeBattalion2 = PutIntoPlay("BladeBattalion");
            bladeBattalion3 = PutIntoPlay("BladeBattalion");
            PutIntoPlay("LeechField");

            SelectYesNoForNextDecision(true, true);
            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3);
            DealDamage(baron.CharacterCard, unity.CharacterCard, 1, DamageType.Melee);
            QuickHPCheck(0, 0, -1, 0, 0, 0, 0, 0);
        }

        [Test]
        public void TestLeechFieldNonHeroDamaged()
        {
            Card bladeBattalion1;
            Card bladeBattalion2;
            Card bladeBattalion3;

            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            bladeBattalion1 = PutIntoPlay("BladeBattalion");
            bladeBattalion2 = PutIntoPlay("BladeBattalion");
            bladeBattalion3 = PutIntoPlay("BladeBattalion");
            PutIntoPlay("LeechField");

            SelectYesNoForNextDecision(true, true);
            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3);
            DealDamage(unity.CharacterCard, bladeBattalion1, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, 0, 0, 0);
            DealDamage(gargoyle.CharacterCard, bladeBattalion1, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, -2, 0, 0);
        }
        [Test]
        public void TestLeechFieldMaySkipDamage()
        {
            Card bladeBattalion1;
            Card bladeBattalion2;
            Card bladeBattalion3;

            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            bladeBattalion1 = PutIntoPlay("BladeBattalion");
            bladeBattalion2 = PutIntoPlay("BladeBattalion");
            bladeBattalion3 = PutIntoPlay("BladeBattalion");
            PutIntoPlay("LeechField");

            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3);
            DecisionYesNo = false;
            DealDamage(gargoyle.CharacterCard, bladeBattalion1, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, -1, 0, 0);
            DecisionYesNo = true;
            DealDamage(gargoyle.CharacterCard, bladeBattalion1, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, 0, 0, 0);
            DealDamage(gargoyle.CharacterCard, bladeBattalion1, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, -2, 0, 0);
        }
        [Test]
        public void TestLeechFieldNotImmediatelyUseCreatedBuff()
        {
            Card bladeBattalion1;
            Card bladeBattalion2;
            Card bladeBattalion3;

            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            bladeBattalion1 = PutIntoPlay("BladeBattalion");
            bladeBattalion2 = PutIntoPlay("BladeBattalion");
            bladeBattalion3 = PutIntoPlay("BladeBattalion");
            PutIntoPlay("LeechField");

            SelectYesNoForNextDecision(true, true);
            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3);
            DealDamage(gargoyle.CharacterCard, bladeBattalion1, 2, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, -1, 0, 0);
            DealDamage(gargoyle.CharacterCard, bladeBattalion1, 2, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, -3, 0, 0);
        }
        [Test]
        public void TestLeechFieldExpiresWhenUsed()
        {
            Card bladeBattalion1;
            Card bladeBattalion2;
            Card bladeBattalion3;

            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            bladeBattalion1 = PutIntoPlay("BladeBattalion");
            bladeBattalion2 = PutIntoPlay("BladeBattalion");
            bladeBattalion3 = PutIntoPlay("BladeBattalion");
            PutIntoPlay("LeechField");

            SelectYesNoForNextDecision(true, true);
            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3);
            DealDamage(unity.CharacterCard, bladeBattalion1, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, 0, 0, 0);
            DealDamage(gargoyle.CharacterCard, bladeBattalion1, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, -2, 0, 0);
            DealDamage(gargoyle.CharacterCard, bladeBattalion1, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, -1, 0, 0);
        }
        [Test]
        public void TestLeechFieldGargoyleDealsDamageExpiresWhenUsed()
        {
            Card bladeBattalion1;
            Card bladeBattalion2;
            Card bladeBattalion3;

            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            bladeBattalion1 = PutIntoPlay("BladeBattalion");
            bladeBattalion2 = PutIntoPlay("BladeBattalion");
            bladeBattalion3 = PutIntoPlay("BladeBattalion");
            PutIntoPlay("LeechField");

            SelectYesNoForNextDecision(true, true);
            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3);
            DealDamage(gargoyle.CharacterCard, bladeBattalion1, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, 0, 0, 0);
            DealDamage(gargoyle.CharacterCard, bladeBattalion1, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, -2, 0, 0);
            DealDamage(gargoyle.CharacterCard, bladeBattalion1, 1, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0, -1, 0, 0);
        }
        #endregion Leech Field

        #region Mark for Execution
        /*
         * Each hero gains the following power: 
         * Power
         * {Gargoyle} deals 1 target 2 toxic damage.
         * {Gargoyle} may deal a second target 1 melee damage.
         */
        [Test]
        public void TestMarkforExecutionPowerAdded()
        {
            int storedUnityPowersCount = 0;
            int storedGargoylePowersCount = 0;
            int storedBunkerPowersCount = 0;
            int storedScholarPowersCount = 0;

            StartTestGame();
            GoToPlayCardPhase(gargoyle);

            storedUnityPowersCount = unity.CharacterCard.NumberOfPowers;
            storedGargoylePowersCount = gargoyle.CharacterCard.NumberOfPowers;
            storedBunkerPowersCount = bunker.CharacterCard.NumberOfPowers;
            storedScholarPowersCount = scholar.CharacterCard.NumberOfPowers;
            PutIntoPlay("MarkForExecution");
            // Each hero gains the following power: 
            AssertNumberOfUsablePowers(unity.CharacterCard, storedUnityPowersCount + 1);
            AssertNumberOfUsablePowers(gargoyle.CharacterCard, storedGargoylePowersCount); // Since gargoyle can use the power on the card, it isn't added to his character
            AssertNumberOfUsablePowers(bunker.CharacterCard, storedBunkerPowersCount + 1);
            AssertNumberOfUsablePowers(scholar.CharacterCard, storedScholarPowersCount + 1);
        }


        [Test]
        public void TestIssue813_MarkforExecutionGrantsPowersToNonRealCards()
        {

            SetupGameController("BaronBlade", "Ra", "Cauldron.Gargoyle", "SkyScraper", "TheSentinels", "Cauldron.VaultFive");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(gargoyle);

            Card mark = PutIntoPlay("MarkForExecution");
            Card instructions = sentinels.HeroTurnTaker.GetAllCards(realCardsOnly: false).Where(c => !c.IsRealCard).FirstOrDefault();
            Assert.That(FindCardController(mark).AskIfContributesPowersToCardController(FindCardController(instructions)) == null, "Mark for Execution granted a power to a non-real card!");
            IEnumerable<Card> offToSideSky = sky.TurnTaker.OffToTheSide.Cards.Where(c => c.IsCharacter);
            foreach (Card notSky in offToSideSky)
            {
                Assert.That(FindCardController(mark).AskIfContributesPowersToCardController(FindCardController(notSky)) == null, "Mark for Execution granted a power to an off to the side card!");

            }

        }
       


        [Test]
        public void TestMarkforExecutionPowerUsed()
        {
            Card bladeBattalion;

            StartTestGame();

            bladeBattalion = PutIntoPlay("BladeBattalion");
            PutIntoPlay("MarkForExecution");
            GoToUsePowerPhase(unity);

            DecisionSelectCards = new Card[] { baron.CharacterCard, bladeBattalion };

            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion);
            UsePower(unity, 1);
            // { Gargoyle} deals 1 target 2 toxic damage.
            // { Gargoyle} may deal a second target 1 melee damage.
            QuickHPCheck(-2, 0, 0, 0, 0, -1);
        }

        [Test]
        public void TestMarkforExecutionPowerUsedNoSecondDamage()
        {
            Card bladeBattalion;

            StartTestGame();

            bladeBattalion = PutIntoPlay("BladeBattalion");
            PutIntoPlay("MarkForExecution");
            GoToUsePowerPhase(unity);

            DecisionSelectCards = new Card[] { baron.CharacterCard, null };

            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion);
            UsePower(unity, 1);
            // { Gargoyle} deals 1 target 2 toxic damage.
            // { Gargoyle} may deal a second target 1 melee damage.
            QuickHPCheck(-2, 0, 0, 0, 0, 0);
        }

        #endregion Mark for Execution

        #region Preservation Engine

        /*
         * When {Gargoyle} destroys a target by reducing its HP below 0, increase the next damage dealt by {Gargoyle} by X and he regains 1HP, 
         * where X is the amount of negative HP that target had.
         * Powers
         * Discard 2 cards. Draw 2 cards
         */
        [Test]
        public void TestPreservationEngineDamageBoost()
        {
            Card bladeBattalion;

            StartTestGame();

            GoToUsePowerPhase(gargoyle);

            PutIntoPlay("PreservationEngine");
            bladeBattalion = PutIntoPlay("BladeBattalion");
            SetHitPoints(gargoyle, gargoyle.CharacterCard.MaximumHitPoints.Value - 2);
            SetHitPoints(bladeBattalion, 1);
            // Intentionally leaving out blade battalion since it will be destroyed and hit points will just reset to max.
            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            DealDamage(gargoyle, bladeBattalion, 4, DamageType.Melee);
            QuickHPCheck(0, 1, 0, 0, 0);
            DealDamage(gargoyle, baron, 1, DamageType.Melee);
            QuickHPCheck(-4, 0, 0, 0, 0);
        }

        [Test]
        public void TestPreservationEngineDiscardAndDraw()
        {
            Card preservationEngine;
            StartTestGame();

            GoToUsePowerPhase(gargoyle);

            preservationEngine = PutIntoPlay("PreservationEngine");

            QuickHandStorage(gargoyle, unity, bunker, scholar);
            UsePower(preservationEngine);
            QuickHandCheck(0, 0, 0, 0);
            AssertNumberOfCardsInTrash(gargoyle, 2);
        }
        [Test]
        public void TestPreservationEngineRegainHPOncePerTurn()
        {
            Card bladeBattalion;
            Card bladeBattalion2;

            StartTestGame();

            GoToUsePowerPhase(gargoyle);

            PutIntoPlay("PreservationEngine");
            bladeBattalion = PutIntoPlay("BladeBattalion");
            bladeBattalion2 = PutIntoPlay("BladeBattalion");
            SetHitPoints(gargoyle, gargoyle.CharacterCard.MaximumHitPoints.Value - 2);
            SetHitPoints(bladeBattalion, 1);
            // Intentionally leaving out blade battalion since it will be destroyed and hit points will just reset to max.
            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            DealDamage(gargoyle, bladeBattalion, 4, DamageType.Melee);
            QuickHPCheck(0, 1, 0, 0, 0);
            DealDamage(gargoyle, bladeBattalion2, 4, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0);
        }

        #endregion Preservation Engine

        #region Something to Fear

        /*
         * Select a target. Reduce the next damage it deals by 1. Increase the next damage {Gargoyle} deals by 1.
         * Search your deck and trash for a Hunter card and put it into play or into your hand. If you searched your deck, shuffle it.
         */
        [Test]
        public void TestSomethingToFearFromDeckToHand()
        {
            Card bladeBattalion;
            Card somethingToFear;

            StartTestGame();

            bladeBattalion = PutIntoPlay("BladeBattalion");
            MoveAllCardsFromHandToDeck(gargoyle);
            somethingToFear = PutInHand("SomethingToFear");

            GoToPlayCardPhase(gargoyle);

            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(gargoyle.TurnTaker.Deck) };
            DecisionSelectCards = new Card[] { baron.CharacterCard, FindCard((card)=>card.Identifier == "ViolentAssist") };
            DecisionMoveCardDestinations = new MoveCardDestination[] { new MoveCardDestination(gargoyle.TurnTaker.ToHero().Hand) };

            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion);
            QuickHandStorage(gargoyle, unity, bunker, scholar); 
            QuickShuffleStorage(gargoyle.TurnTaker.ToHero().Deck, unity.TurnTaker.ToHero().Hand, bunker.TurnTaker.ToHero().Hand, scholar.TurnTaker.ToHero().Hand);
            PlayCard(somethingToFear);
            DealDamage(baron, gargoyle, 1, DamageType.Melee);
            DealDamage(gargoyle, baron, 1, DamageType.Melee);
            DealDamage(gargoyle, bladeBattalion, 1, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0, 0, -1);
            QuickHandCheck(0, 0, 0, 0);
            QuickShuffleCheck(1, 0, 0, 0);
        }

        [Test]
        public void TestSomethingToFearFromDeckToPlay()
        {
            Card bladeBattalion;
            Card somethingToFear;

            StartTestGame();

            bladeBattalion = PutIntoPlay("BladeBattalion");
            MoveAllCardsFromHandToDeck(gargoyle);
            somethingToFear = PutInHand("SomethingToFear");

            GoToPlayCardPhase(gargoyle);

            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(gargoyle.TurnTaker.Deck) };
            DecisionSelectCards = new Card[] { baron.CharacterCard, FindCard((card) => card.Identifier == "ViolentAssist") };
            DecisionMoveCardDestinations = new MoveCardDestination[] { new MoveCardDestination(gargoyle.TurnTaker.ToHero().PlayArea) };

            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion);
            QuickHandStorage(gargoyle, unity, bunker, scholar);
            QuickShuffleStorage(gargoyle.TurnTaker.ToHero().Deck, unity.TurnTaker.ToHero().Hand, bunker.TurnTaker.ToHero().Hand, scholar.TurnTaker.ToHero().Hand);
            PlayCard(somethingToFear);
            DealDamage(baron, gargoyle, 1, DamageType.Melee);
            DealDamage(gargoyle, baron, 1, DamageType.Melee);
            DealDamage(gargoyle, bladeBattalion, 1, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0, 0, -1);
            QuickHandCheck(-1, 0, 0, 0);
            QuickShuffleCheck(1, 0, 0, 0);
        }
        [Test]
        public void TestSomethingToFearFromTrashToHand()
        {
            Card bladeBattalion;
            Card somethingToFear;
            
            StartTestGame();

            bladeBattalion = PutIntoPlay("BladeBattalion");
            somethingToFear = PutInHand("SomethingToFear");
            PutInTrash(gargoyle, FindCardsWhere((card)=>card.Identifier== "ViolentAssist"));

            GoToPlayCardPhase(gargoyle);

            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(gargoyle.TurnTaker.Trash) };
            DecisionSelectCards = new Card[] { baron.CharacterCard, FindCard((card) => card.Identifier == "ViolentAssist") };
            DecisionMoveCardDestinations = new MoveCardDestination[] { new MoveCardDestination(gargoyle.TurnTaker.ToHero().Hand) };

            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion);
            QuickHandStorage(gargoyle, unity, bunker, scholar);
            QuickShuffleStorage(gargoyle.TurnTaker.ToHero().Deck, unity.TurnTaker.ToHero().Hand, bunker.TurnTaker.ToHero().Hand, scholar.TurnTaker.ToHero().Hand);
            PlayCard(somethingToFear);
            DealDamage(baron, gargoyle, 1, DamageType.Melee);
            DealDamage(gargoyle, baron, 1, DamageType.Melee);
            DealDamage(gargoyle, bladeBattalion, 1, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0, 0, -1);
            QuickHandCheck(0, 0, 0, 0);
            QuickShuffleCheck(0, 0, 0, 0);
        }

        [Test]
        public void TestSomethingToFearFromTrashToPlay()
        {
            Card bladeBattalion;
            Card somethingToFear;

            StartTestGame();

            bladeBattalion = PutIntoPlay("BladeBattalion");
            somethingToFear = PutInHand("SomethingToFear");
            PutInTrash(gargoyle, FindCardsWhere((card) => card.Identifier == "ViolentAssist"));

            GoToPlayCardPhase(gargoyle);

            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(gargoyle.TurnTaker.Trash) };
            DecisionSelectCards = new Card[] { baron.CharacterCard, FindCard((card) => card.Identifier == "ViolentAssist") };
            DecisionMoveCardDestinations = new MoveCardDestination[] { new MoveCardDestination(gargoyle.TurnTaker.ToHero().PlayArea) };

            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion);
            QuickHandStorage(gargoyle, unity, bunker, scholar);
            QuickShuffleStorage(gargoyle.TurnTaker.ToHero().Deck, unity.TurnTaker.ToHero().Hand, bunker.TurnTaker.ToHero().Hand, scholar.TurnTaker.ToHero().Hand);
            PlayCard(somethingToFear);
            DealDamage(baron, gargoyle, 1, DamageType.Melee);
            DealDamage(gargoyle, baron, 1, DamageType.Melee);
            DealDamage(gargoyle, bladeBattalion, 1, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0, 0, -1);
            QuickHandCheck(-1, 0, 0, 0);
            QuickShuffleCheck(0, 0, 0, 0);
        }
        #endregion Something to Fear

        #region Terrorize

        /*
         * At the end of each turn, each non-hero target damaged by {Gargoyle} during that turn deals itself 1 irreducible psychic damage.
         * At the start of the villain turn, {Gargoyle} may deal 1 target 0 psychic damage.
        */
        [Test]
        public void TestTerrorizeEndOfHeroTurn()
        {
            Card bladeBattalion1;
            Card bladeBattalion2;
            Card bladeBattalion3;
            Card plummetingMonorail;

            StartTestGame();
            GoToPlayCardPhase(gargoyle);
            bladeBattalion1 = PutIntoPlay("BladeBattalion");
            bladeBattalion2 = PutIntoPlay("BladeBattalion");
            bladeBattalion3 = PutIntoPlay("BladeBattalion");
            plummetingMonorail = PutIntoPlay("PlummetingMonorail");
            PutIntoPlay("Terrorize");

            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3, plummetingMonorail);
            DealDamage(gargoyle.CharacterCard, new Card[] { baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3, plummetingMonorail }, 1, DamageType.Melee);
            QuickHPCheck(-1, -1, -1, -1, -1, -1, -1, -1, -1);

            // At the end of each turn, each non-hero target damaged by {Gargoyle} during that turn deals itself 1 psychic damage.
            GoToEndOfTurn(gargoyle);
            QuickHPCheck(-1, 0, 0, 0, 0, -1, -1, -1, -1);

            GoToPlayCardPhase(unity); 
            DealDamage(gargoyle.CharacterCard, new Card[] { baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3, plummetingMonorail }, 1, DamageType.Melee);
            QuickHPCheck(-1, -1, -1, -1, -1, -1, -1, -1, -1);

            // At the end of each turn, each non-hero target damaged by {Gargoyle} during that turn deals itself 1 psychic damage.
            GoToEndOfTurn(unity);
            QuickHPCheck(-1, 0, 0, 0, 0, -1, -1, -1, -1);
        }

        [Test]
        public void TestTerrorizeEndOfEnvironmentTurn()
        {
            Card bladeBattalion1;
            Card bladeBattalion2;
            Card bladeBattalion3;
            Card plummetingMonorail;

            StartTestGame();
            GoToStartOfTurn(env);
            DestroyCards((card) => card.IsEnvironment);

            bladeBattalion1 = PutIntoPlay("BladeBattalion");
            bladeBattalion2 = PutIntoPlay("BladeBattalion");
            bladeBattalion3 = PutIntoPlay("BladeBattalion");
            plummetingMonorail = PutIntoPlay("PlummetingMonorail");
            PutIntoPlay("Terrorize");

            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3, plummetingMonorail);
            DealDamage(gargoyle.CharacterCard, new Card[] { baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3, plummetingMonorail }, 1, DamageType.Melee);
            QuickHPCheck(-1, -1, -1, -1, -1, -1, -1, -1, -1);

            // At the end of each turn, each non-hero target damaged by {Gargoyle} during that turn deals itself 1 psychic damage.

            GoToEndOfTurn(env);
            QuickHPCheck(-1, 0, 0, 0, 0, -1, -1, -1, -1);
        }

        [Test]
        public void TestTerrorizeStartOfVillainTurn()
        {
            StartTestGame();

            PutIntoPlay("Terrorize");

            GoToEndOfTurn(env);
            // At the start of the villain turn, {Gargoyle} may deal 1 target 0 psychic damage.
            base.UsePower(gargoyle);
            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            GoToStartOfTurn(baron);
            QuickHPCheck(-1, 0, 0, 0, 0);
        }

        [Test]
        public void TestTerrorizeEndOfVillainTurn()
        {
            Card bladeBattalion1;
            Card bladeBattalion2;
            Card bladeBattalion3;
            Card plummetingMonorail;

            StartTestGame();

            bladeBattalion1 = PutIntoPlay("BladeBattalion");
            bladeBattalion2 = PutIntoPlay("BladeBattalion");
            bladeBattalion3 = PutIntoPlay("BladeBattalion");
            plummetingMonorail = PutIntoPlay("PlummetingMonorail");
            PreventEndOfTurnEffects(baron, bladeBattalion1);
            PreventEndOfTurnEffects(baron, bladeBattalion2);
            PreventEndOfTurnEffects(baron, bladeBattalion3);

            PutIntoPlay("Terrorize");

            QuickHPStorage(baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3, plummetingMonorail);
            DealDamage(gargoyle.CharacterCard, new Card[] { baron.CharacterCard, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, bladeBattalion1, bladeBattalion2, bladeBattalion3, plummetingMonorail }, 1, DamageType.Melee);
            QuickHPCheck(-1, -1, -1, -1, -1, -1, -1, -1, -1);

            // At the end of each turn, each non-hero target damaged by {Gargoyle} during that turn deals itself 1 psychic damage.
            GoToEndOfTurn(baron);

            QuickHPCheck(-1, 0, 0, 0, 0, -1, -1, -1, -1);
        }
        [Test]
        public void TestTerrorizeEOTDamageIrreducible()
        {
            StartTestGame();
            PlayCard("LivingForceField");
            PlayCard("Terrorize");

            QuickHPStorage(baron);
            DealDamage(gargoyle, baron, 3, DamageType.Melee);
            QuickHPCheck(-2);
            GoToEndOfTurn();
            QuickHPCheck(-1);

        }
        #endregion Terrorize

        #region Ultimatum
        /*
         * Destroy up to 3 hero ongoing or equipment cards belonging to other players.
         * {Gargoyle} deals 1 target X toxic damage, where X is 3 times the number of cards destroyed this way.
         */
        [Test]
        public void TestUltimatumZeroDestroyed()
        {
            StartTestGame();

            GoToPlayCardPhase(gargoyle);
            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            PlayCard("Ultimatum");
            QuickHPCheck(0, 0, 0, 0, 0);
        }

        [Test]
        public void TestUltimatumOneDestroyed()
        {
            Card ammoDrop;
            Card modularWorkbench;
            Card leechField;

            StartTestGame();

            GoToPlayCardPhase(gargoyle);
            ammoDrop = PutIntoPlay("AmmoDrop");
            modularWorkbench = PutIntoPlay("ModularWorkbench");
            leechField = PutIntoPlay("LeechField");

            DecisionSelectCards = new Card[] { ammoDrop, null, baron.CharacterCard };
            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            PlayCard("Ultimatum");
            QuickHPCheck(-3, 0, 0, 0, 0);
        }
        [Test]
        public void TestUltimatumTwoDestroyed()
        {
            Card ammoDrop;
            Card modularWorkbench;
            Card fleshToIron;

            StartTestGame();

            GoToPlayCardPhase(gargoyle);
            ammoDrop = PutIntoPlay("AmmoDrop");
            modularWorkbench = PutIntoPlay("ModularWorkbench");
            fleshToIron = PutIntoPlay("FleshToIron");

            DecisionSelectCards = new Card[] { ammoDrop, modularWorkbench, null, baron.CharacterCard };
            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            PlayCard("Ultimatum");
            QuickHPCheck(-6, 0, 0, 0, 0);
        }
        [Test]
        public void TestUltimatumThreeDestroyed()
        {
            Card ammoDrop;
            Card modularWorkbench;
            Card fleshToIron;

            StartTestGame();

            GoToPlayCardPhase(gargoyle);
            ammoDrop = PutIntoPlay("AmmoDrop");
            modularWorkbench = PutIntoPlay("ModularWorkbench");
            fleshToIron = PutIntoPlay("FleshToIron");

            DecisionSelectCards = new Card[] { ammoDrop, modularWorkbench, fleshToIron, baron.CharacterCard };
            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            PlayCard("Ultimatum");
            QuickHPCheck(-9, 0, 0, 0, 0);
        }
        [Test]
        public void TestUltimatumNotDestroyOwnCards()
        {
            StartTestGame();

            Card assist = PlayCard("ViolentAssist");

            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            PlayCard("Ultimatum");
            AssertIsInPlay(assist);
            QuickHPCheckZero();
        }

        #endregion Ultimatum

        #region ViolentAssist
        /* 
         * Once per turn when {Gargoyle} would be dealt damage by another hero target, you may prevent that damage.
         * If you do, increase the next damage dealt by {Gargoyle} by X, where X is the amount of damage prevented this way.
         */
        [Test]
        public void TestViolentAssistYes()
        {
            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            DecisionsYesNo = new bool[] { true };
            DecisionSelectCards = new Card[] { baron.CharacterCard };
            PutIntoPlay("ViolentAssist");
            DealDamage(unity, gargoyle, 5, DamageType.Melee);
            DealDamage(gargoyle, baron, 1, DamageType.Melee);
            QuickHPCheck(-6, 0, 0, 0, 0);
            // should only be the next damage
            DealDamage(gargoyle, baron, 1, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0, 0);
        }

        [Test]
        public void TestIssue954_ViolentAssistAndEnhancedSenses()
        {
            SetupGameController("Cauldron.Anathema", "Ra", "Cauldron.Gargoyle", "Haka", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card senses = PlayCard("EnhancedSenses");
            Card violent = PlayCard("ViolentAssist");

            DecisionYesNo = true;
            QuickHPStorage(anathema, ra, gargoyle, haka);
            DealDamage(haka, gargoyle, 5, DamageType.Fire);
            QuickHPCheckZero();
           
        }

        [Test]
        public void TestViolentAssistNo()
        {
            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            DecisionsYesNo = new bool[] { false };
            DecisionSelectCards = new Card[] { baron.CharacterCard };
            PutIntoPlay("ViolentAssist");
            DealDamage(unity, gargoyle, 5, DamageType.Melee);
            DealDamage(gargoyle, baron, 1, DamageType.Melee);
            QuickHPCheck(-1, -5, 0, 0, 0);
        }

        [Test]
        public void TestViolentAssistGargoyleDealsDamage()
        {
            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            DecisionsYesNo = new bool[] { false };
            DecisionSelectCards = new Card[] { baron.CharacterCard };
            PutIntoPlay("ViolentAssist");
            DealDamage(gargoyle, gargoyle, 5, DamageType.Melee);
            DealDamage(gargoyle, baron, 1, DamageType.Melee);
            QuickHPCheck(-1, -5, 0, 0, 0);
        }

        [Test]
        public void TestViolentAssistCanSkipDamage()
        {
            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            DecisionsYesNo = new bool[] { false, true, true };
            DecisionSelectCards = new Card[] { baron.CharacterCard };
            PutIntoPlay("ViolentAssist");
            DealDamage(unity, gargoyle, 1, DamageType.Melee);
            QuickHPCheck(0, -1, 0, 0, 0);
            DealDamage(unity, gargoyle, 5, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0);
            DealDamage(gargoyle, baron, 1, DamageType.Melee);
            QuickHPCheck(-6, 0, 0, 0, 0);
            //once used, shouldn't get the option again
            DealDamage(unity, gargoyle, 1, DamageType.Melee);
            DealDamage(gargoyle, baron, 1, DamageType.Melee);
            QuickHPCheck(-1, -1, 0, 0, 0);
        }

        #endregion ViolentAssist

        #region Wither
        /*
         * Destroy 1 ongoing or environment card.
         * If a card is destroyed this way, increase the next damage dealt by {Gargoyle} by 2.
         */
        [Test]
        public void TestWitherWasCardDestroyed()
        {
            Card plummetingMonorail;
            Card leechField;

            // The destruction isn't optional, so at least 1 card should be destroyed.
            StartTestGame();

            GoToPlayCardPhase(gargoyle);
            plummetingMonorail = PutIntoPlay("PlummetingMonorail");
            leechField = PutIntoPlay("LeechField");

            PlayCard("Wither");

            Assert.IsTrue(GetNumberOfCardsInTrash(gargoyle) > 0 || GetNumberOfCardsInTrash(env) > 0, "No card was destroyed");
        }

        [Test]
        public void TestWitherDamageIncreased()
        {
            Card plummetingMonorail;
            Card leechField;

            // The destruction isn't optional, so at least 1 card should be destroyed.
            StartTestGame();

            GoToPlayCardPhase(gargoyle);
            plummetingMonorail = PutIntoPlay("PlummetingMonorail");
            leechField = PutIntoPlay("LeechField");

            PlayCard("Wither");

            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            DealDamage(gargoyle, baron, 1, DamageType.Melee);
            QuickHPCheck(-3, 0, 0, 0, 0);
            DealDamage(gargoyle, baron, 1, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0, 0);
        }

        #endregion Wither

        #region Your Strength is Mine
        /*
         * Play this card next to a target. You may destroy this card at any time.
         * When this card is destroyed, reduce the next damage dealt by that target by 2 and increase the next damage dealt by {Gargoyle} by 2.
        */
        [Test]
        public void TestYourStrengthIsMineVillainReducedYes()
        {
            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            DecisionsYesNo = new bool[] { true };
            PutIntoPlay("YourStrengthIsMine");
            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);

            // Your Strength is Mine should have been destroyed so Baron's next damage should be reduced by 2
            DealDamage(baron.CharacterCard, gargoyle.CharacterCard, 2, DamageType.Toxic);
            QuickHPCheck(0, 0, 0, 0, 0);
            // but only his next damage
            DealDamage(baron.CharacterCard, gargoyle.CharacterCard, 2, DamageType.Toxic);
            QuickHPCheck(0, -2, 0, 0, 0);
        }

        [Test]
        public void TestYourStrengthIsMineHeroIncreasedYes()
        {
            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            DecisionsYesNo = new bool[] { true };
            PutIntoPlay("YourStrengthIsMine");
            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);

            // Your Strength is Mine should have been destroyed so Baron's next damage should be reduced by 2
            DealDamage(gargoyle.CharacterCard, baron.CharacterCard, 2, DamageType.Toxic);
            QuickHPCheck(-4, 0, 0, 0, 0);
            // but only his next damage
            DealDamage(gargoyle.CharacterCard, baron.CharacterCard, 2, DamageType.Toxic);
            QuickHPCheck(-2, 0, 0, 0, 0);
        }

        [Test]
        public void TestYourStrengthIsMineHeroIncreasedNo()
        {
            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            DecisionsYesNo = new bool[] { false, false };
            PutIntoPlay("YourStrengthIsMine");
            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);

            // Your Strength is Mine should have been destroyed so Baron's next damage should be reduced by 2
            DealDamage(gargoyle.CharacterCard, baron.CharacterCard, 2, DamageType.Toxic);
            DealDamage(baron.CharacterCard, gargoyle.CharacterCard, 2, DamageType.Toxic);
            QuickHPCheck(-2, -2, 0, 0, 0);
        }
        [Test]
        public void TestYourStrengthIsMineOtherDestroySource()
        {
            StartTestGame();

            GoToPlayCardPhase(gargoyle);

            Card ysim = PutIntoPlay("YourStrengthIsMine");
            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);

            DestroyCard(ysim);
            AssertNumberOfStatusEffectsInPlay(2);

        }
        [Test]
        public void TestYourStrengthIsMineNonCharacterOtherDestroy()
        {
            StartTestGame();

            GoToPlayCardPhase(gargoyle);
            Card redist = PlayCard("ElementalRedistributor");
            DecisionSelectCard = redist;
            Card ysim = PutIntoPlay("YourStrengthIsMine");
            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);

            DestroyCard(ysim);
            AssertNumberOfStatusEffectsInPlay(2);
        }
        [Test]
        public void TestYourStrengthIsMineDestructionFailed()
        {
            SetupGameController("BaronBlade", "Cauldron.Gargoyle", "Legacy", "Ra", "TimeCataclysm");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card ysim = PlayCard("YourStrengthIsMine");
            Card point = PlayCard("FixedPoint");
            DecisionYesNo = true;

            QuickHPStorage(baron);
            DealDamage(gargoyle, baron, 2, DamageType.Melee);
            QuickHPCheck(-2);
            AssertIsInPlay(ysim);

            DestroyCard(point);
            DestroyCard(ysim);
            DealDamage(gargoyle, baron, 2, DamageType.Melee);
            QuickHPCheck(-4);
        }
        #endregion Your Strength is Mine
    }
}
