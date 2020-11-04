using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.LadyOfTheWood;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyModTest
{
    [TestFixture()]
    public class LadyOfTheWoodTests : BaseTest
    {
        #region LadyOfTheWoodHelperFunctions
        protected HeroTurnTakerController ladyOfTheWood { get { return FindHero("LadyOfTheWood"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(ladyOfTheWood.CharacterCard, 1);
            DealDamage(villain, ladyOfTheWood, 2, DamageType.Melee);
        }
        #endregion

        [Test()]
        public void TestLadyOfTheWoodLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(ladyOfTheWood);
            Assert.IsInstanceOf(typeof(LadyOfTheWoodCharacterCardController), ladyOfTheWood.CharacterCardController);

            Assert.AreEqual(22, ladyOfTheWood.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestLadyOfTheWoodInnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //Select a damage type. {LadyOfTheWood} deals 1 target 1 damage of the chosen type.
            GoToUsePowerPhase(ladyOfTheWood);
            DecisionSelectDamageType = DamageType.Cold;
            DecisionSelectTarget = ra.CharacterCard;
            QuickHPStorage(ra);
            UsePower(ladyOfTheWood.CharacterCard);
            QuickHPCheck(-1);

        }

        [Test()]
        public void TestLadyOfTheWoodIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            //Select a damage type. Until the start of your next turn, increase damage dealt of the chosen type by 1.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);

            DecisionSelectDamageType = DamageType.Fire;
            UseIncapacitatedAbility(ladyOfTheWood, 0);

            QuickHPStorage(haka);
            DealDamage(ra, haka, 5, DamageType.Fire);

            //Damage dealt should be 5 +1 => 6
            QuickHPCheck(-6);

            GoToStartOfTurn(ladyOfTheWood);

            QuickHPStorage(haka);
            DealDamage(ra, haka, 5, DamageType.Fire);

            //start of turn has passed, trigger should be gone, 5 damage
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestLadyOfTheWoodIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            //give hake room to gain health
            SetHitPoints(haka, 20);

            //1 hero may discard a card to regain 3 HP.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);
            DecisionSelectTurnTaker = haka.HeroTurnTaker;
            QuickHandStorage(haka);
            QuickHPStorage(haka);
            UseIncapacitatedAbility(ladyOfTheWood, 1);
            //should have 1 less card in hand and should have gained 3 HP
            QuickHandCheck(-1);
            QuickHPCheck(3);
            
        }

        [Test()]
        public void TestLadyOfTheWoodIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            PutIntoPlay("PoliceBackup");

            //Destroy an environment card.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);

            int numEnvironmentCardsInPlayBefore = GetNumberOfCardsInPlay(FindEnvironment());
            UseIncapacitatedAbility(ladyOfTheWood, 2);
            int numEnvironmentCardsInPlayAfter = GetNumberOfCardsInPlay(FindEnvironment());

            //1 environment card should have been destroyed, so 1 less card in play
            Assert.AreEqual(numEnvironmentCardsInPlayBefore - 1, numEnvironmentCardsInPlayAfter, "The number of environment cards in play does not match");

        }

        [Test()]
        public void TestCalmBeforeTheStormDiscard3()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            PutInHand("CalmBeforeTheStorm");
            Card calm = GetCardFromHand("CalmBeforeTheStorm");
            //Discard any number of cards.
            // Increase the next damage dealt by LadyOfTheWood by X, where X is 2 plus the number of cards discarded this way.
            GoToPlayCardPhase(ladyOfTheWood);
            QuickHandStorage(ladyOfTheWood);

            PlayCard(calm);
            //should be -5, 1 for calm before storm, and 4 others discarded
            QuickHandCheck(-5);

            QuickHPStorage(haka);
            DealDamage(ladyOfTheWood, haka, 4, DamageType.Melee);
            //since 4 cards discarded, should be +6 for  10 damage
            QuickHPCheck(-10);



        }


    }
}
