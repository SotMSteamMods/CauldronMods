using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Cauldron.MagnificentMara;

namespace CauldronTests
{
    [TestFixture()]
    public class MagnificentMaraTests : BaseTest
    {
        #region MaraHelperFunctions
        protected HeroTurnTakerController mara { get { return FindHero("MagnificentMara"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(mara.CharacterCard, 1);
            DealDamage(villain, mara, 2, DamageType.Melee);
        }

        #endregion maraHelperFunctions

        [Test]
        public void TestMaraLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.MagnificentMara", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(mara);
            Assert.IsInstanceOf(typeof(MagnificentMaraCharacterCardController), mara.CharacterCardController);

            Assert.AreEqual(27, mara.CharacterCard.HitPoints);
        }
    }
}
