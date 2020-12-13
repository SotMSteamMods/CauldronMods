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
    public class ImpactTests : BaseTest
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
        public void TestImpactLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(impact);
            Assert.IsInstanceOf(typeof(ImpactCharacterCardController), impact.CharacterCardController);

            Assert.AreEqual(29, impact.CharacterCard.HitPoints);
        }
        [Test]
        public void TestImpactDecklist()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            AssertHasKeyword("ongoing", new[]
            {
                "DecayingOrbit",
                "GravitationalLensing",
                "GraviticOrb",
                "HurledObstruction",
                "InescapablePull",
                "LocalMicrogravity",
                "Meditate",
                "RepulsionField",
                "SlingshotTrajectory",
                "SpatialFinesse"
            });

            AssertHasKeyword("limited", new[]
            {
                "GravitationalLensing",
                "LocalMicrogravity",
                "RepulsionField"
            });

            AssertHasKeyword("one-shot", new[]
            {
                "AcceleratedCollision",
                "CrushingRift",
                "EscapeVelocity",
                "MassDriver"
            });
        }
        [Test]
        public void TestImpactPowerSimple()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            QuickHPStorage(baron);
            UsePower(impact);
            QuickHPCheck(-1);
        }
        [Test]
        public void TestImpactPowerDestroyOngoing()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            DecisionYesNo = true;
            QuickHPStorage(baron);
            Card moko = PlayCard("TaMoko");
            UsePower(impact);
            QuickHPCheck(-3);
            AssertInTrash(moko);
            UsePower(impact);
            QuickHPCheck(-1);
        }
        [Test]
        public void TestImpactPowerDestroyOngoingIsOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            DecisionYesNo = false;
            QuickHPStorage(baron);
            Card moko = PlayCard("TaMoko");
            UsePower(impact);
            QuickHPCheck(-1);
            AssertIsInPlay(moko);
            UsePower(impact);
            QuickHPCheck(-1);
        }
    }
}