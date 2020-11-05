using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Necro;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight.Tests
{
    [TestFixture()]
    public class TheKnightTests : BaseTest
    {
        #region HelperFunctions
        protected string HeroNamespace = "Cauldron.TheKnight";

        protected HeroTurnTakerController HeroController { get { return FindHero("TheKnight"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(HeroController.CharacterCard, 1);
            DealDamage(villain, HeroController, 2, DamageType.Melee);
        }
        
        #endregion

        [Test()]
        public void TestCharacterLoad()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(HeroController);
            Assert.IsInstanceOf(typeof(TheKnightCharacterCardController), HeroController.CharacterCardController);

            Assert.AreEqual(32, HeroController.CharacterCard.HitPoints);
        }

        [Test()]
        public void InnatePower()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Megalopolis");
            StartGame();

            GoToUsePowerPhase(HeroController);
            DecisionSelectFunction = 0;
            DecisionSelectTarget = base.FindCardInPlay("BaronBlade");
            QuickHPStorage(DecisionSelectTarget);
            UsePower(HeroController.CharacterCard);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestIncap1()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(HeroController);

            //"One player may play a card now."
            //QuickHandStorage(legacy);
            //QuickHPStorage(legacy);
            ////using incap ability on legacy
            //DecisionSelectTarget = legacy.CharacterCard;
            ////set to true so legacy will deal himself the damage
            //DecisionsYesNo = new bool[] { true };

            //GoToUseIncapacitatedAbilityPhase(HeroController);
            //UseIncapacitatedAbility(HeroController, 0);

            ////verify damage was dealt and cards were drawn
            //QuickHPCheck(-2);
            //QuickHandCheck(2);
        }

        [Test()]
        public void TestIncap2()
        {
            SetupGameController("BaronBlade", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();
            SetHitPoints(legacy.CharacterCard, 20);

            SetupIncap(baron);
            AssertIncapacitated(HeroController);

            //"Reduce damage dealt to 1 target by 1 until the start of your next turn.",

            ////Grab the first three cards to discard
            ////One hero may discard up to 3 cards, then regain 2 HP for each card discarded.
            //QuickHandStorage(legacy);
            //QuickHPStorage(legacy);
            ////using incap ability on legacy
            //DecisionSelectTarget = legacy.CharacterCard;

            //GoToUseIncapacitatedAbilityPhase(HeroController);
            //UseIncapacitatedAbility(HeroController, 1);

            ////verify damage was dealt and cards were drawn
            //QuickHPCheck(6);
            //QuickHandCheck(-3);
        }

           //once figure out a way to choose only 1 card or 2 cards to discard, add test cases for that

        [Test()]
        public void TestIncap3()
        {
            SetupGameController("Omnitron", HeroNamespace, "Legacy", "Megalopolis");
            StartGame();
            SetHitPoints(legacy.CharacterCard, 20);
            SetupIncap(omnitron);
            AssertIncapacitated(HeroController);

            //"Increase damage dealt by 1 target by 1 until the start of your next turn."

            ////Destroy all non-character cards in play to reduce variance
            //GoToStartOfTurn(HeroController);
            //DestroyCards((Card c) => c.IsInPlayAndHasGameText && !c.IsCharacter);



            ////Select a hero target. Increase damage dealt by that target by 3 and increase damage dealt to that target by 2 until the start of your next turn.
            ////using incap ability on legacy
            //DecisionSelectTarget = legacy.CharacterCard;
            //GoToUseIncapacitatedAbilityPhase(HeroController);
            //DestroyCards((Card c) => c.IsInPlayAndHasGameText && !c.IsCharacter);
            //UseIncapacitatedAbility(HeroController, 2);

            //GoToPlayCardPhase(legacy);

            ////try legacy dealing damage to omnitron, should be +3
            //QuickHPStorage(omnitron);
            //DealDamage(legacy, omnitron, 2, DamageType.Melee);
            //QuickHPCheck(-5);

            ////try omnitron dealing damage to legacy, should be +2
            //QuickHPStorage(legacy);
            //DealDamage(omnitron, legacy, 2, DamageType.Projectile);
            //QuickHPCheck(-4);

            ////go to necro's next turn, should be normal damage
            //GoToUseIncapacitatedAbilityPhase(HeroController);

            //QuickHPStorage(omnitron);
            //DealDamage(legacy, omnitron, 2, DamageType.Melee);
            //QuickHPCheck(-2);

            //QuickHPStorage(legacy);
            //DealDamage(omnitron, legacy, 2, DamageType.Projectile);
            //QuickHPCheck(-2);

        }


    }
}
