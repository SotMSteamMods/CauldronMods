using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Titan;

namespace CauldronTests
{
    [TestFixture()]
    public class DriftTests : BaseTest
    {
        protected HeroTurnTakerController drift { get { return FindHero("Drift"); } }

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(drift.CharacterCard, 1);
            DealDamage(villain, drift, 2, DamageType.Melee);
        }

        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                AssertCardHasKeyword(card, keyword, false);
            }
        }

        [Test()]
        [Order(0)]
        public void TestDriftLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(drift);
            Assert.IsInstanceOf(typeof(TitanCharacterCardController), drift.CharacterCardController);

            foreach (var card in drift.HeroTurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(32, drift.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestDriftDecklist()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            AssertHasKeyword("ongoing", new[]
            {
                "CombatPragmatism",
                "Immolate",
                "PaybackTime",
                "StubbornGoliath",
                "Titanform",
                "Unbreakable",
                "VulcansJudgment"
            });

            AssertHasKeyword("equipment", new[]
            {
                "TheChaplain"
            });

            AssertHasKeyword("limited", new[]
            {
                "StubbornGoliath",
                "TheChaplain",
                "PaybackTime"
            });

            AssertHasKeyword("one-shot", new[]
            {
                "ForbiddenArchives",
                "HaplessShield",
                "JuggernautStrike",
                "Misbehavior",
                "MoltenVeins",
                "Ms5DemolitionCharge",
                "ObsidianGrasp",
                "Reversal"
            });
        }

        [Test()]
        public void Test()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            var a = drift.TurnTaker.CharacterCards;
            UsePower(drift);
        }
    }
}
