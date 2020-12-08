using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.TheStranger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Handelabra.Sentinels.Engine.Controller.ChronoRanger;

namespace CauldronTests
{
    [TestFixture()]
    public class TheStrangerVariantTests : BaseTest
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
        public void TestRunecarvedStrangerLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger/RunecarvedTheStrangerCharacter", "Haka", "Ra", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(stranger);
            Assert.IsInstanceOf(typeof(RunecarvedTheStrangerCharacterCardController), stranger.CharacterCardController);

            Assert.AreEqual(28, stranger.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestPastStrangerLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger/PastTheStrangerCharacter", "Haka", "Ra", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(stranger);
            Assert.IsInstanceOf(typeof(PastTheStrangerCharacterCardController), stranger.CharacterCardController);

            Assert.AreEqual(29, stranger.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestWastelandRoninStrangerLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger/WastelandRoninTheStrangerCharacter", "Haka", "Ra", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(stranger);
            Assert.IsInstanceOf(typeof(WastelandRoninTheStrangerCharacterCardController), stranger.CharacterCardController);

            Assert.AreEqual(24, stranger.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestCornStrangerLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger/CornTheStrangerCharacter", "Haka", "Ra", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(stranger);
            Assert.IsInstanceOf(typeof(CornTheStrangerCharacterCardController), stranger.CharacterCardController);

            Assert.AreEqual(27, stranger.CharacterCard.HitPoints);
        }

    }
}
