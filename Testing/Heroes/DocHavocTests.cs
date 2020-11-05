using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cauldron.DocHavoc;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;

namespace MyModTest
{
    [TestFixture()]
    public class DocHavocTests : BaseTest
    {
        protected HeroTurnTakerController DocHavoc => FindHero("DocHavoc");



        [Test()]
        public void TestDocHavocLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(DocHavoc);
            Assert.IsInstanceOf(typeof(DocHavocCharacterCardController), DocHavoc.CharacterCardController);

            Assert.AreEqual(30, DocHavoc.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestDocsFlask()
        {
            // Arrange
            SetupGameController("BaronBlade", "Cauldron.DocHavoc", "Ra", "Megalopolis");
            StartGame();

            // Act
            PutInHand(DocsFlaskCardController.Identifier);
            Card docsFlask = GetCardFromHand(DocsFlaskCardController.Identifier);
            GoToPlayCardPhase(DocHavoc);
            PlayCard(docsFlask);

            DecisionSelectCard = ra.CharacterCard;

            // Reduce Ra's HP by 1
            SetHitPoints(ra.CharacterCard, 29);
            QuickHPStorage(ra);
            QuickHPUpdate();

            // Procs flask to heal Ra +1 HP
            GoToStartOfTurn(DocHavoc);

            // Assert
            AssertTriggersWhere((Func<ITrigger, bool>)(t => t.Types.Contains(TriggerType.GainHP)));
            QuickHPCheck(1);
        }

    }
}
