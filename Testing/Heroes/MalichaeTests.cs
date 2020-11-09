using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Malichae;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class MalichaeTests : BaseTest
    {
        #region MalichaeTestsHelperFunctions
        protected HeroTurnTakerController Malichae { get { return FindHero("Malichae"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(Malichae.CharacterCard, 1);
            DealDamage(villain, Malichae, 2, DamageType.Melee);
        }

        #endregion

        [Test()]
        public void TestMalichaeLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Malichae);
            Assert.IsInstanceOf(typeof(MalichaeCharacterCardController), Malichae.CharacterCardController);

            Assert.AreEqual(27, Malichae.CharacterCard.HitPoints);
        }

        [Test]
        public void InnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToUsePowerPhase(Malichae);

            QuickHandStorage(Malichae, ra, fanatic);
            var discard = GetCardFromHand(Malichae);
            DecisionDiscardCard = discard;
            UsePower(Malichae);

            QuickHandCheck(1, 0, 0);
            AssertInTrash(Malichae, discard);
        }


    }
}
