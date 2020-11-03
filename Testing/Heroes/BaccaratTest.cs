using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;

using Cauldron;
using Cauldron.Baccarat;

namespace MyModTest
{
    [TestFixture()]
    public class BaccaratTest : BaseTest
    {
        #region helpers

        protected HeroTurnTakerController Baccarat { get { return FindHero("Baccarat"); } }

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(Baccarat.CharacterCard, 1);
            DealDamage(villain, Baccarat, 2, DamageType.Melee);
        }

        #endregion helpers

        [Test()]
        public void TestBaccaratWorks()
        {
            //SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");

            //Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            //Assert.IsNotNull(Baccarat);
            Assert.IsNotNull(Baccarat);
            //Assert.IsInstanceOf(typeof(BaccaratCharacterCardController), baccarat.CharacterCardController);

            //Assert.AreEqual(27, baccarat.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestInnatePowerOption1()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");

            StartGame();

            var mdp = GetCardInPlay("MobileDefensePlatform");

            // Base power option 1
            QuickHandStorage(Baccarat.ToHero());
            DecisionSelectTarget = mdp;
            QuickHPStorage(mdp);

            UsePower(Baccarat.CharacterCard);

            QuickHandCheck(3);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestPunchingBag()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");

            StartGame();

            GoToUsePowerPhase(Baccarat);

            // Punching Bag does 1 damage!
            QuickHPStorage(Baccarat);
            PlayCard("PunchingBag");
            QuickHPCheck(-1);
        }
    }
}
