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

        [Test()]
        public void TestDendronStartGame()
        {
            SetupGameController(DeckNamespace, "Legacy", "Megalopolis");

            StartGame();

            // Should be 3 cards in play total (Dendron, 1 Stained Wolf, 1 Painted Viper)
            AssertNumberOfCardsInPlay(Dendron, 3);

        }

        [Test]
        public void TestAdornedOak()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();

            Card adornedOwl = GetCard(AdornedOakCardController.Identifier);
            PlayCard(adornedOwl);

            QuickHPStorage(GetStainedWolf(), GetPaintedViper());

            // Act
            DealDamage(ra, GetStainedWolf(), 3, DamageType.Fire);
            DealDamage(ra, GetPaintedViper(), 3, DamageType.Fire);


            // Assert
            QuickHPCheck(-2, -2);
        }

        [Test]
        public void TestBloodThornAura()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();

            Card bloodThornAura = GetCard(BloodThornAuraCardController.Identifier);
            PlayCard(bloodThornAura);

            QuickHPStorage(GetStainedWolf(), GetPaintedViper(), ra.CharacterCard);

            // Act
            DealDamage(ra, GetStainedWolf(), 3, DamageType.Fire);
            DealDamage(ra, GetPaintedViper(), 3, DamageType.Fire);

            // Assert
            QuickHPCheck(-3, -3, -2);
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

        [Test]
        public void TestInkScar()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            PutInTrash(TintedStagCardController.Identifier);
            PutInTrash(LivingInkCardController.Identifier);

            Card bloodThornAura = GetCard(BloodThornAuraCardController.Identifier);
            PutInTrash(bloodThornAura);

            StartGame();
            QuickShuffleStorage(Dendron.TurnTaker.Deck);


            GoToPlayCardPhase(Dendron);
            Card inkScar = GetCard(InkScarCardController.Identifier);
            PlayCard(inkScar);

            // Act


            // Assert
            AssertInTrash(Dendron, bloodThornAura);
            QuickShuffleCheck(1);
        }

        [Test]
        public void TestLivingInk()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            Card bloodThornAura = GetCard(BloodThornAuraCardController.Identifier);
            PutInTrash(bloodThornAura);
            
            Card livingInk = GetCard(LivingInkCardController.Identifier);

            StartGame();

            SetHitPoints(Dendron.CharacterCard, 40);
            SetHitPoints(GetStainedWolf(), 1);
            SetHitPoints(GetPaintedViper(), 1);
            QuickHPStorage(Dendron.CharacterCard, GetStainedWolf(), GetPaintedViper());

            // Act
            PlayCard(livingInk);
            GoToEndOfTurn(Dendron);

            // Assert
            QuickHPCheck(3, 3, 3);
        }

        private Card GetStainedWolf()
        {
            return GetCardInPlay(StainedWolfCardController.Identifier);
        }

        private Card GetPaintedViper()
        {
            return GetCardInPlay(PaintedViperCardController.Identifier);
        }

    }
}
