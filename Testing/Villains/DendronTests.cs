using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.UnitTest;

using Cauldron.Dendron;

using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture]
    public class DendronTests : BaseTest
    {
        protected TurnTakerController Dendron => FindVillain("Dendron");

        private const string DeckNamespace = "Cauldron.Dendron";

        [Test()]
        public void TestDendronLoads()
        {
            SetupGameController(DeckNamespace, "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Dendron);
            Assert.IsInstanceOf(typeof(DendronCharacterCardController), Dendron.CharacterCardController);

            Assert.AreEqual(50, Dendron.CharacterCard.HitPoints);
        }

        [Test]
        public void TestChokingInscription()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();

            Card chokingInscription = GetCard(ChokingInscriptionCardController.Identifier);
            Card flameBarrier = GetCard("FlameBarrier");
            PlayCard(flameBarrier);

            PlayCard(chokingInscription);
            Assert.True(false, "Finish this test");
        }

        [Test]
        public void TestAdornedOwl()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();

            Card chokingInscription = GetCard(AdornedOakCardController.Identifier);
            Card stainedWolf = GetCard(StainedWolfCardController.Identifier);
            Card obsidianSkin = GetCard(ObsidianSkinCardController.Identifier);
            PlayCard(stainedWolf);
            PlayCard(chokingInscription);
            PlayCard(obsidianSkin);

            DealDamage(ra, stainedWolf, 4, DamageType.Fire);
            DealDamage(ra, Dendron, 4, DamageType.Fire);

            Assert.True(false, "Finish this test");
        }

        [Test]
        public void TestDarkDesign()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();

            GoToStartOfTurn(legacy);
            DealDamage(legacy, Dendron, 4, DamageType.Melee);
            
            DealDamage(ra, Dendron, 4, DamageType.Fire);



            GoToStartOfTurn(Dendron);
            Card darkDesign = GetCard(DarkDesignCardController.Identifier);
            PlayCard(darkDesign);

            Assert.True(false, "Finish this test");

        }

    }
}
