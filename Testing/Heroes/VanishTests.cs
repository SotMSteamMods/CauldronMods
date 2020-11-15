using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Vanish;

namespace CauldronTests
{
    [TestFixture()]
    public class VanishTests : BaseTest
    {
        #region HelperFunctions
        protected HeroTurnTakerController vanish { get { return FindHero("Vanish"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(vanish.CharacterCard, 1);
            DealDamage(villain, vanish, 2, DamageType.Melee);
        }

        #endregion HelperFunctions

        [Test()]
        public void TestLoadVanish()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(vanish);
            Assert.IsInstanceOf(typeof(VanishCharacterCardController), vanish.CharacterCardController);

            Assert.AreEqual(26, vanish.CharacterCard.HitPoints);
        }

    }
}
