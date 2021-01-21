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
        public void TestGargoyleInnatePower()
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
        public void TestGargoyleIncap2HeroTarget()
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
        public void TestGargoyleIncap2VillainTarget()
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
        public void TestGargoyleIncap2EnvironmentTarget()
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
        public void TestGargoyleIncap3()
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
    }
}
