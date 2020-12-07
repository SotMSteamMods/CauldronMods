using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Cricket;

namespace CauldronTests
{
    [TestFixture()]
    public class CricketTests : BaseTest
    {
        protected HeroTurnTakerController cricket { get { return FindHero("Cricket"); } }

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(cricket.CharacterCard, 1);
            DealDamage(villain, cricket, 2, DamageType.Melee);
        }

        [Test()]
        public void TestLoadCrickett()
        {
            SetupGameController("BaronBlade", "Cauldron.Cricket", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(cricket);
            Assert.IsInstanceOf(typeof(CricketCharacterCardController), cricket.CharacterCardController);

            Assert.AreEqual(27, cricket.CharacterCard.HitPoints);
        }

        [Test()]
        public void Test()
        {
            SetupGameController("Apostate", "Cauldron.Cricket/WastelandRoninCricketCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            QuickHPStorage(apostate);
            UsePower(cricket);
            QuickHPCheck(-1);

            GoToPlayCardPhase(cricket);
            QuickHPStorage(apostate);
            DealDamage(cricket, apostate, 2, DamageType.Melee);
            QuickHPCheck(-3);

        }
    }
}
