using NUnit.Framework;
using Cauldron.Outlander;
using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    class OutlanderTests : CauldronBaseTest
    {
        [Test()]
        public void TestOutlanderLoad()
        {
            SetupGameController("Cauldron.Outlander", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(outlander);
            Assert.IsInstanceOf(typeof(OutlanderCharacterCardController), outlander.CharacterCardController);

            foreach (Card card in outlander.TurnTaker.GetAllCards())
            {
                CardController cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(20, outlander.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestOutlanderDecklist()
        {
            SetupGameController("Cauldron.Outlander", "Haka", "Bunker", "TheScholar", "Megalopolis");

            AssertHasKeyword("trace", new string[] { "Archangel", "Crusader", "Dragonborn", "Magekiller", "Warbrand" });

            AssertHasKeyword("one-shot", new string[] { "DisarmingBlow", "RiftbladeStrikes", "TransdimensionalOnslaught" });

            AssertHasKeyword("ongoing", new string[] { "AnchoredFragment", "DimensionalInsinuation", "KnightsHatred", "OutOfTouch" });
        }
    }
}
