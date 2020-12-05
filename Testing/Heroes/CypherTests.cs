using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cauldron.Cypher;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture]
    public class CypherTests : BaseTest
    {
        protected HeroTurnTakerController Cypher => FindHero("Cypher");

        private const string DeckNamespace = "Cauldron.Cypher";

        [Test]
        public void TestCypherLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", DeckNamespace, "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(Cypher);
            Assert.IsInstanceOf(typeof(CypherCharacterCardController), Cypher.CharacterCardController);

            Assert.AreEqual(26, Cypher.CharacterCard.HitPoints);
        }

    }
}
