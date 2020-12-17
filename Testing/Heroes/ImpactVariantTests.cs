using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Impact;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class ImpactVariantTests : BaseTest
    {
        #region ImpactHelperFunctions
        protected HeroTurnTakerController impact { get { return FindHero("Impact"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(impact.CharacterCard, 1);
            DealDamage(villain, impact, 2, DamageType.Melee);
        }
        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                AssertCardHasKeyword(card, keyword, false);
            }
        }

        #endregion
        [Test]
        public void TestRenegadeLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact/RenegadeImpactCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(impact);
            Assert.IsInstanceOf(typeof(RenegadeImpactCharacterCardController), impact.CharacterCardController);

            Assert.AreEqual(27, impact.CharacterCard.HitPoints);
        }
        [Test]
        public void TestWastelandRoninLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact/WastelandRoninImpactCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(impact);
            Assert.IsInstanceOf(typeof(WastelandRoninImpactCharacterCardController), impact.CharacterCardController);

            Assert.AreEqual(26, impact.CharacterCard.HitPoints);
        }
    }
}