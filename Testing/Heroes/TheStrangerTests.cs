using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.TheStranger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class TheStranger : BaseTest
    {
        #region TheStrangerHelperFunctions
        protected HeroTurnTakerController stranger { get { return FindHero("TheStranger"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(stranger.CharacterCard, 1);
            DealDamage(villain, stranger, 2, DamageType.Melee);
        }
       

        #endregion

        [Test()]
        public void TestStrangerLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(stranger);
            Assert.IsInstanceOf(typeof(TheStrangerCharacterCardController), stranger.CharacterCardController);

            Assert.AreEqual(26, stranger.CharacterCard.HitPoints);
        }


    }
}
