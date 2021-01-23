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
    public class GargoyleVariantTests : CauldronBaseTest
    {
        #region Gargoyle Utilities

        private const string WastelandRoninGargoyle = "WastelandRoninGargoyleCharacter";
        private const string FutureGargoyle = "FutureGargoyleCharacter";
        private const string DragonRangerGargoyle = "DragonRangerGargoyleCharacter";
        private const string InfiltratorGargoyle = "InfiltratorGargoyleCharacter";

        private void StartTestGame(string variantName)
        {
            StartTestGame(variantName, "BaronBlade", "Unity", "Bunker", "TheScholar", "Megalopolis");
        }

        private void StartTestGame(string variantName, params String[] decksInPlay)
        {
            SetupGameController("BaronBlade", $"Cauldron.Gargoyle/{variantName}", "Unity", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            base.DestroyNonCharacterVillainCards();
        }

        private void SetupIncapTest(string variantName)
        {
            StartTestGame(variantName);
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

        #region Wasteland Ronin Gargoyle
        [Test()]
        public void TestLoadWastelandRoninGargoyle()
        {
            SetupGameController("BaronBlade", $"Cauldron.Gargoyle/{WastelandRoninGargoyle}", "Unity", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(gargoyle);
            Assert.IsInstanceOf(typeof(WastelandRoninGargoyleCharacterCardController), gargoyle.CharacterCardController);

            Assert.AreEqual(27, gargoyle.CharacterCard.HitPoints);
        }

        #region Test Innate Power
        /*
         * Power:
         * {Gargoyle} deals 1 target 2 toxic damage.
         */
        [Test()]
        public void TestWastelandRoninGargoyleInnatePower()
        {
            StartTestGame(WastelandRoninGargoyle);

            GoToUsePowerPhase(gargoyle);

            // Select a target. 
            DecisionSelectTarget = baron.CharacterCard;
            
            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            UsePower(gargoyle.CharacterCard);
            QuickHPCheck(-2, 0, 0, 0, 0);
        }

        #endregion Test Innate Power

        #region Test Incap Powers
        /* 
         * Incap 1
         * "Increase the next damage dealt by a hero target by 2."
        */
        [Test()]
        public void TestWastelandRoninGargoyleIncap1()
        {
            SetupIncapTest(WastelandRoninGargoyle);
            AssertIncapLetsHeroPlayCard(gargoyle, 0, unity, "BeeBot");
        }
        /* 
         * Incap 2
         * // Select a target. Prevent the next damage that would be dealt to that target and by that target.
        */
        [Test()]
        public void TestWastelandRoninGargoyleIncap2HeroTarget()
        {
            SetupIncapTest(WastelandRoninGargoyle);
            DecisionSelectCards = new Card[] { unity.CharacterCard };
            UseIncapacitatedAbility(gargoyle, 1);

            // Prevent the next damage they take
            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(baron, unity, 2, DamageType.Melee);
            QuickHPCheckZero();

            // Only the next damage
            DealDamage(baron, unity, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0, 0);

            // Prevent the next damage they deal
            DealDamage(unity, baron, 2, DamageType.Melee);
            QuickHPCheckZero();

            // Only the next damage
            DealDamage(unity, baron, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0);
        }
        [Test()]
        public void TestWastelandRoninGargoyleIncap2VillainTarget()
        {
            SetupIncapTest(WastelandRoninGargoyle);
            DecisionSelectCards = new Card[] { baron.CharacterCard };
            UseIncapacitatedAbility(gargoyle, 1);

            // Prevent the next damage they take
            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(unity, baron, 2, DamageType.Melee);
            QuickHPCheckZero();

            // Only the next damage
            DealDamage(unity, baron, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0);

            // Prevent the next damage they deal
            DealDamage(baron, unity, 2, DamageType.Melee);
            QuickHPCheckZero();

            // Only the next damage
            DealDamage(baron, unity, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0, 0);
        }

        [Test()]
        public void TestWastelandRoninGargoyleIncap2EnvironmentTarget()
        {
            Card plummetingMonorail;

            SetupIncapTest(WastelandRoninGargoyle);
            plummetingMonorail = PutIntoPlay("PlummetingMonorail");
            DecisionSelectCards = new Card[] { plummetingMonorail };
            UseIncapacitatedAbility(gargoyle, 1);

            // Prevent the next damage they take
            QuickHPStorage(baron.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, plummetingMonorail);
            DealDamage(unity, plummetingMonorail, 2, DamageType.Melee);
            QuickHPCheckZero();

            // Only the next damage
            DealDamage(unity, plummetingMonorail, 2, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, -2);

            // Prevent the next damage they deal
            DealDamage(plummetingMonorail, unity, 2, DamageType.Melee);
            QuickHPCheckZero();

            // Only the next damage
            DealDamage(plummetingMonorail, unity, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0, 0, 0);
        }

        /* 
         * Incap 3
         * One hero target deals itself 2 toxic damage. Another regains 2 HP.
        */
        [Test()]
        public void TestWastelandRoninGargoyleIncap3()
        {
            SetupIncapTest(WastelandRoninGargoyle);

            DecisionSelectCards = new Card[] { unity.CharacterCard, bunker.CharacterCard };
            base.SetHitPoints(bunker.CharacterCard, 15);

            QuickHPStorage(baron, unity, bunker, scholar);
            UseIncapacitatedAbility(gargoyle, 2);
            QuickHPCheck(0, -2, 2, 0);
        }

        #endregion Test Incap Powers

        #endregion Wasteland Ronin Gargoyle

        #region Future Gargoyle
        [Test()]
        public void TestLoadFutureGargoyle()
        {
            SetupGameController("BaronBlade", $"Cauldron.Gargoyle/{FutureGargoyle}", "Unity", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(gargoyle);
            Assert.IsInstanceOf(typeof(FutureGargoyleCharacterCardController), gargoyle.CharacterCardController);

            Assert.AreEqual(28, gargoyle.CharacterCard.HitPoints);
        }

        #region Test Innate Power
        /*
         * Power:
         * You may destroy 1 hero ongoing or equipment card. If you do, 1 player plays a card. Draw a card.
         */
        [Test()]
        public void TestFutureGargoyleInnatePower()
        {
            Card leechField;
            Card modularWorkBench;

            StartTestGame(FutureGargoyle);

            GoToUsePowerPhase(gargoyle);

            leechField = PutIntoPlay("LeechField");
            modularWorkBench = PutInHand(unity, "ModularWorkbench");

            // Select a target. 
            DecisionsYesNo = new bool[] { true };
            DecisionSelectCards = new Card[] { leechField, modularWorkBench };
            DecisionSelectTurnTakers = new TurnTaker[] { unity.HeroTurnTaker };

            QuickHandStorage(gargoyle, unity, bunker, scholar);
            UsePower(gargoyle.CharacterCard);
            QuickHandCheck(1, -1, 0, 0);
        }
        [Test]
        public void TestFutureGargoyleInnatePowerNoDestroy()
        {
            Card leechField;
            Card modularWorkBench;

            StartTestGame(FutureGargoyle);

            GoToUsePowerPhase(gargoyle);

            leechField = PutIntoPlay("LeechField");
            modularWorkBench = PutInHand(unity, "ModularWorkbench");

            // Select a target. 
            DecisionsYesNo = new bool[] { true };
            DecisionSelectCards = new Card[] { null, modularWorkBench };
            DecisionSelectTurnTakers = new TurnTaker[] { unity.HeroTurnTaker };
            AssertMaxNumberOfDecisions(1);

            QuickHandStorage(gargoyle, unity, bunker, scholar);
            UsePower(gargoyle.CharacterCard);
            QuickHandCheck(1, 0, 0, 0);
            AssertIsInPlay(leechField);
        }

        #endregion Test Innate Power

        #region Test Incap Powers
        /* 
         * Incap 1
         * Increase the next damage dealt to a non-hero target by 2.
        */
        [Test()]
        public void TestFutureGargoyleIncap1Villain()
        {
            SetupIncapTest(FutureGargoyle);

            UseIncapacitatedAbility(gargoyle, 0);

            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(bunker, baron, 1, DamageType.Melee);
            QuickHPCheck(-3, 0, 0, 0);
            DealDamage(bunker, baron, 1, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0);
        }
        [Test()]
        public void TestFutureGargoyleIncap1Hero()
        {
            SetupIncapTest(FutureGargoyle);

            UseIncapacitatedAbility(gargoyle, 0);

            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(baron, unity, 1, DamageType.Melee);
            QuickHPCheck(0, -1, 0, 0);
        }
        /* 
         * Incap 2
         * Reduce the next damage dealt by a non-hero target by 2.
        */
        [Test()]
        public void TestFutureGargoyleIncap2HeroTarget()
        {
            SetupIncapTest(FutureGargoyle);

            UseIncapacitatedAbility(gargoyle, 1);

            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(unity, baron, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0);
        }
        [Test()]
        public void TestFutureGargoyleIncap2VillainTarget()
        {
            SetupIncapTest(FutureGargoyle);

            UseIncapacitatedAbility(gargoyle, 1);

            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(baron, unity, 2, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0);

            DealDamage(baron, unity, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0, 0);
        }
        [Test()]
        public void TestFutureGargoyleIncap2EnvironmentTarget()
        {
            Card plummetingMonorail;
            SetupIncapTest(FutureGargoyle);

            plummetingMonorail = PutIntoPlay("PlummetingMonorail");
            UseIncapacitatedAbility(gargoyle, 1);

            QuickHPStorage(baron.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, plummetingMonorail);
            DealDamage(plummetingMonorail, unity, 2, DamageType.Melee);
            QuickHPCheck(0, 0, 0, 0, 0);

            DealDamage(plummetingMonorail, unity, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0, 0, 0);
        }

        /* 
         * Incap 3
         * Shuffle 1 non-villain trash into its deck.
        */
        [Test()]
        public void TestFutureGargoyleIncap3Hero()
        {
            SetupIncapTest(FutureGargoyle);

            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(unity.TurnTaker.Trash) };
            PutInTrash("BeeBot", "ModularWorkbench");

            UseIncapacitatedAbility(gargoyle, 2);
            AssertNumberOfCardsInTrash(unity, 0);
        }

        [Test()]
        public void TestFutureGargoyleIncap3Environment()
        {
            SetupIncapTest(FutureGargoyle);

            DecisionSelectLocations = new LocationChoice[] { new LocationChoice(env.TurnTaker.Trash) };
            PutInTrash("PlummetingMonorail");

            UseIncapacitatedAbility(gargoyle, 2);
            AssertNumberOfCardsInTrash(env, 0);
        }

        #endregion Test Incap Powers

        #endregion Future Gargoyle

        #region Dragon Ranger Gargoyle
        [Test()]
        public void TestLoadDragonRangerGargoyle()
        {
            SetupGameController("BaronBlade", $"Cauldron.Gargoyle/{DragonRangerGargoyle}", "Unity", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(gargoyle);
            Assert.IsInstanceOf(typeof(DragonRangerGargoyleCharacterCardController), gargoyle.CharacterCardController);

            Assert.AreEqual(29, gargoyle.CharacterCard.HitPoints);
        }

        #region Test Innate Power
        /*
         * Power:
         * {Gargoyle} deals 3 targets 1 toxic damage each. Increase the next damage he deals by the number of heroes damaged this way.
         */
        [Test()]
        public void TestDragonRangerGargoyleInnatePower()
        {
            StartTestGame(DragonRangerGargoyle);

            GoToUsePowerPhase(gargoyle);

            // Select a target. 
            DecisionSelectTargets = new Card[] { baron.CharacterCard, unity.CharacterCard, bunker.CharacterCard };

            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            UsePower(gargoyle.CharacterCard);            
            QuickHPCheck(-1, 0, -1, -1, 0);

            DealDamage(gargoyle, baron, 2, DamageType.Toxic);
            QuickHPCheck(-4, 0, 0, 0, 0);

            DealDamage(gargoyle, baron, 2, DamageType.Toxic);
            QuickHPCheck(-2, 0, 0, 0, 0);
        }
        [Test]
        public void TestDragonRangerGargoyleInnatePowerMustDealDamageForBenefit()
        {
            StartTestGame(DragonRangerGargoyle);

            GoToUsePowerPhase(gargoyle);

            PlayCard("HeavyPlating");
            // Select a target. 
            DecisionSelectTargets = new Card[] { baron.CharacterCard, unity.CharacterCard, bunker.CharacterCard };

            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            UsePower(gargoyle.CharacterCard);
            QuickHPCheck(-1, 0, -1, 0, 0);

            DealDamage(gargoyle, baron, 2, DamageType.Toxic);
            QuickHPCheck(-3, 0, 0, 0, 0);

            DealDamage(gargoyle, baron, 2, DamageType.Toxic);
            QuickHPCheck(-2, 0, 0, 0, 0);
        }
        [Test]
        public void TestDragonRangerGargoyleInnatePowerDistinctHeroes()
        {
            StartTestGame(DragonRangerGargoyle);

            GoToUsePowerPhase(gargoyle);

            PlayCard("AlchemicalRedirection");
            // Select a target. 
            DecisionSelectTargets = new Card[] { baron.CharacterCard, unity.CharacterCard, bunker.CharacterCard };

            QuickHPStorage(baron, gargoyle, unity, bunker, scholar);
            UsePower(gargoyle.CharacterCard);
            QuickHPCheck(-1, 0, 0, 0, -2);

            DealDamage(gargoyle, baron, 2, DamageType.Toxic);
            QuickHPCheck(-3, 0, 0, 0, 0);

            DealDamage(gargoyle, baron, 2, DamageType.Toxic);
            QuickHPCheck(-2, 0, 0, 0, 0);
        }
        #endregion Test Innate Power

        #region Test Incap Powers
        /* 
         * Incap
         * One player may draw a card now.
         * One hero may use a power now.
         * Select a target. Prevent the next damage it would deal.
        */
        [Test()]
        public void TestDragonRangerGargoyleIncapPower1()
        {
            SetupIncapTest(DragonRangerGargoyle);

            AssertIncapLetsHeroDrawCard(gargoyle, 0, unity, 1);
        }
        [Test()]
        public void TestDragonRangerGargoyleIncapPower2()
        {
            SetupIncapTest(DragonRangerGargoyle);

            AssertIncapLetsHeroUsePower(gargoyle, 1, bunker);
        }
        /*
         * Incap 3
         * Select a target. Prevent the next damage it would deal.
         */
        [Test()]
        public void TestDragonRangerGargoyleIncapPower3Hero()
        {
            SetupIncapTest(DragonRangerGargoyle);
            DecisionSelectCards = new Card[] { unity.CharacterCard };
            UseIncapacitatedAbility(gargoyle, 2);

            // Prevent the next damage they deal
            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(unity, baron, 2, DamageType.Melee);
            QuickHPCheckZero();

            // Only the next damage
            DealDamage(unity, baron, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0, 0);
        }
        [Test()]
        public void TestDragonRangerGargoyleIncapPower3Villain()
        {
            SetupIncapTest(DragonRangerGargoyle);
            DecisionSelectCards = new Card[] { baron.CharacterCard };
            UseIncapacitatedAbility(gargoyle, 2);

            // Prevent the next damage they deal
            QuickHPStorage(baron, unity, bunker, scholar);
            DealDamage(baron, unity, 2, DamageType.Melee);
            QuickHPCheckZero();

            // Only the next damage
            DealDamage(baron, unity, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0, 0);
        }
        [Test()]
        public void TestDragonRangerGargoyleIncapPower3Environment()
        {
            Card plummetingMonorail;

            SetupIncapTest(DragonRangerGargoyle);
            plummetingMonorail = PutIntoPlay("PlummetingMonorail");
            DecisionSelectCards = new Card[] { plummetingMonorail };
            UseIncapacitatedAbility(gargoyle, 2);

            // Prevent the next damage they deal
            QuickHPStorage(plummetingMonorail, baron.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            DealDamage(plummetingMonorail, baron, 2, DamageType.Melee);
            QuickHPCheckZero();

            // Only the next damage
            DealDamage(plummetingMonorail, baron, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0, 0, 0);
        }
        #endregion Test Incap Powers

        #endregion Dragon Ranger Gargoyle

        #region Infiltrator Gargoyle
        [Test()]
        public void TestLoadInfiltratorGargoyle()
        {
            SetupGameController("BaronBlade", $"Cauldron.Gargoyle/{InfiltratorGargoyle}", "Unity", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(gargoyle);
            Assert.IsInstanceOf(typeof(InfiltratorGargoyleCharacterCardController), gargoyle.CharacterCardController);

            Assert.AreEqual(27, gargoyle.CharacterCard.HitPoints);
        }

        #region Test Innate Power
        /*
         * Power:
         * {Gargoyle} deals 2 targets 1 toxic damage each. For each hero damaged this way, draw or play a card.
         */
        [Test()]
        public void TestInfiltratorGargoyleInnatePower0()
        {
            Card bioenergyPulse;
            Card leechField;
            Card plummetingMonorail;

            StartTestGame(InfiltratorGargoyle);

            GoToUsePowerPhase(gargoyle);
            bioenergyPulse = PutInHand("BioenergyPulse");
            leechField = PutInHand("LeechField");
            plummetingMonorail = PutIntoPlay("PlummetingMonorail");

            DecisionSelectTargets = new Card[] { baron.CharacterCard, plummetingMonorail };

            QuickHPStorage(baron.CharacterCard, plummetingMonorail, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            UsePower(gargoyle);
            QuickHPCheck(-1, -1, 0, 0, 0, 0);
        }
        [Test()]
        public void TestInfiltratorGargoyleInnatePower1Play()
        {
            Card bioenergyPulse;
            Card leechField;
            Card plummetingMonorail;

            StartTestGame(InfiltratorGargoyle);

            GoToUsePowerPhase(gargoyle);
            bioenergyPulse = PutInHand("BioenergyPulse");
            leechField = PutInHand("LeechField");
            plummetingMonorail = PutIntoPlay("PlummetingMonorail");

            //DecisionSelectTargets = new Card[] {  };
            DecisionSelectCards = new Card[] { baron.CharacterCard, gargoyle.CharacterCard, bioenergyPulse };
            DecisionSelectFunctions = new int?[] { 0 }; // Play a card
            QuickHPStorage(baron.CharacterCard, plummetingMonorail, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            QuickHandStorage(gargoyle, unity, bunker, scholar);
            UsePower(gargoyle);
            QuickHPCheck(-1, 0, -1, 0, 0, 0);
            QuickHandCheck(-1, 0, 0, 0);
        }
        [Test()]
        public void TestInfiltratorGargoyleInnatePower1Draw()
        {
            Card bioenergyPulse;
            Card leechField;
            Card plummetingMonorail;

            StartTestGame(InfiltratorGargoyle);

            GoToUsePowerPhase(gargoyle);
            bioenergyPulse = PutInHand("BioenergyPulse");
            leechField = PutInHand("LeechField");
            plummetingMonorail = PutIntoPlay("PlummetingMonorail");

            DecisionSelectTargets = new Card[] { baron.CharacterCard, gargoyle.CharacterCard };
            DecisionSelectFunctions = new int?[] { 1 }; // Draw a card

            QuickHPStorage(baron.CharacterCard, plummetingMonorail, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            QuickHandStorage(gargoyle, unity, bunker, scholar);
            UsePower(gargoyle);
            QuickHPCheck(-1, 0, -1, 0, 0, 0);
            QuickHandCheck(1, 0, 0, 0);
        }
        [Test()]
        public void TestInfiltratorGargoyleInnatePower2Play()
        {
            Card bioenergyPulse;
            Card leechField;
            Card plummetingMonorail;

            StartTestGame(InfiltratorGargoyle);

            GoToUsePowerPhase(gargoyle);
            bioenergyPulse = PutInHand("BioenergyPulse");
            leechField = PutInHand("LeechField");
            plummetingMonorail = PutIntoPlay("PlummetingMonorail");

            //DecisionSelectTargets = new Card[] {  };
            DecisionSelectCards = new Card[] { unity.CharacterCard, gargoyle.CharacterCard, bioenergyPulse, leechField };
            DecisionSelectFunctions = new int?[] { 0, 0 }; // Play a card
            QuickHPStorage(baron.CharacterCard, plummetingMonorail, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            QuickHandStorage(gargoyle, unity, bunker, scholar);
            UsePower(gargoyle);
            QuickHPCheck(0, 0, -1, -1, 0, 0);
            QuickHandCheck(-2, 0, 0, 0);
        }
        [Test()]
        public void TestInfiltratorGargoyleInnatePower2Draw()
        {
            Card bioenergyPulse;
            Card leechField;
            Card plummetingMonorail;

            StartTestGame(InfiltratorGargoyle);

            GoToUsePowerPhase(gargoyle);
            bioenergyPulse = PutInHand("BioenergyPulse");
            leechField = PutInHand("LeechField");
            plummetingMonorail = PutIntoPlay("PlummetingMonorail");

            DecisionSelectTargets = new Card[] { unity.CharacterCard, gargoyle.CharacterCard };
            DecisionSelectFunctions = new int?[] { 1, 1 }; // Draw a card

            QuickHPStorage(baron.CharacterCard, plummetingMonorail, gargoyle.CharacterCard, unity.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            QuickHandStorage(gargoyle, unity, bunker, scholar);
            UsePower(gargoyle);
            QuickHPCheck(0, 0, -1, -1, 0, 0);
            QuickHandCheck(2, 0, 0, 0);
        }
        #endregion Test Innate Power

        #region Test Incap Powers
        /*
         * Incap 1:
         * One player may play a card now.
         */
        [Test()]
        public void TestInfiltratorGargoyleIncapPower1()
        {
            SetupIncapTest(InfiltratorGargoyle);
            AssertIncapLetsHeroPlayCard(gargoyle, 0, unity, "BeeBot");
        }
        /*
         * Incap 2:
         * Discard the top card of 1 deck.
         */
        [Test()]
        public void TestInfiltratorGargoyleIncapPower2Villain()
        {
            int totalCardsInDeck;
            int totalCardsInTrash;

            SetupIncapTest(InfiltratorGargoyle);

            totalCardsInDeck = baron.TurnTaker.Deck.Cards.Count();
            totalCardsInTrash = baron.TurnTaker.Trash.Cards.Count();

            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);
            UseIncapacitatedAbility(gargoyle, 1);

            AssertNumberOfCardsInDeck(baron, totalCardsInDeck - 1); ;
            AssertNumberOfCardsInTrash(baron, totalCardsInTrash + 1); ;
        }
        [Test()]
        public void TestInfiltratorGargoyleIncapPower2Hero()
        {
            int totalCardsInDeck;
            int totalCardsInTrash;

            SetupIncapTest(InfiltratorGargoyle);

            totalCardsInDeck = unity.TurnTaker.Deck.Cards.Count();
            totalCardsInTrash = unity.TurnTaker.Trash.Cards.Count();

            DecisionSelectLocation = new LocationChoice(unity.TurnTaker.Deck);
            UseIncapacitatedAbility(gargoyle, 1);

            AssertNumberOfCardsInDeck(unity, totalCardsInDeck - 1); ;
            AssertNumberOfCardsInTrash(unity, totalCardsInTrash + 1); ;
        }
        [Test()]
        public void TestInfiltratorGargoyleIncapPower2Environment()
        {
            int totalCardsInDeck;
            int totalCardsInTrash;

            SetupIncapTest(InfiltratorGargoyle);

            totalCardsInDeck = env.TurnTaker.Deck.Cards.Count();
            totalCardsInTrash = env.TurnTaker.Trash.Cards.Count();

            DecisionSelectLocation = new LocationChoice(env.TurnTaker.Deck);
            UseIncapacitatedAbility(gargoyle, 1);

            AssertNumberOfCardsInDeck(env, totalCardsInDeck - 1); ;
            AssertNumberOfCardsInTrash(env, totalCardsInTrash + 1); ;
        }
        /*
         * Incap 3:
         * 1 target deals itself 1 toxic damage. Another regains 1 HP.
         */
        [Test()]
        public void TestInfiltratorGargoyleIncapPower3()
        {
            SetupIncapTest(InfiltratorGargoyle);

            SetHitPoints(unity, 15);
            DecisionSelectTargets = new Card[] { baron.CharacterCard, unity.CharacterCard };
            QuickHPStorage(baron, unity, bunker, scholar);
            UseIncapacitatedAbility(gargoyle, 2);
            QuickHPCheck(-1, 1, 0, 0);
        }
        #endregion Test Incap Powers

        #endregion Infiltrator Gargoyle
    }
}
