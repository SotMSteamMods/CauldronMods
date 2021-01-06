using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class VaultFiveTests : BaseTest
    {

        #region VaultFiveHelperFunctions

        protected TurnTakerController vault5 { get { return FindEnvironment(); } }
        protected bool IsArtifact(Card card)
        {
            return card.DoKeywordsContain("artifact");
        }

        #endregion

        [Test()]
        public void TestVaultFiveWorks()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.VaultFive");
            StartGame();
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

        }

        [Test()]
        public void TestArtifactsRemovedOnIncap()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.VaultFive");
            StartGame();

            DecisionSelectTurnTaker = haka.TurnTaker;
            Card artifact = PlayCard("ByrgsNail");
            AssertInDeck(haka, artifact);
            //incap haka
            DealDamage(baron, haka, 99, DamageType.Radiant);
            AssertIncapacitated(haka);
            AssertOutOfGame(artifact);
        }

    }
}
