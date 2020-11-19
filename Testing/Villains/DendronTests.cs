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

            QuickHPStorage(GetStainedWolfInPlay(), GetPaintedViperInPlay());

            // Act
            DealDamage(ra, GetStainedWolfInPlay(), 3, DamageType.Fire);
            DealDamage(ra, GetPaintedViperInPlay(), 3, DamageType.Fire);


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

            QuickHPStorage(GetStainedWolfInPlay(), GetPaintedViperInPlay(), ra.CharacterCard);

            // Act
            DealDamage(ra, GetStainedWolfInPlay(), 3, DamageType.Fire);
            DealDamage(ra, GetPaintedViperInPlay(), 3, DamageType.Fire);

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
            SetHitPoints(GetStainedWolfInPlay(), 1);
            SetHitPoints(GetPaintedViperInPlay(), 1);
            QuickHPStorage(Dendron.CharacterCard, GetStainedWolfInPlay(), GetPaintedViperInPlay());

            // Act
            PlayCard(livingInk);
            GoToEndOfTurn(Dendron);

            // Assert
            QuickHPCheck(3, 3, 3);
        }


        [Test]
        public void TestObsidianSkin()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();
            QuickHPStorage(Dendron);

            Card obsidianSkin = GetCard(ObsidianSkinCardController.Identifier);

            GoToPlayCardPhase(Dendron);
            PlayCard(obsidianSkin);

            // Act
            DealDamage(ra, Dendron, 4, DamageType.Fire);

            // Assert
            QuickHPCheck(-3); // -1 reduction from Obsidian Skin

        }

        [Test]
        public void TestPaintedViper()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();
            QuickHPStorage(ra);

            // Act
            PlayCard(GetPaintedViperInPlay());

            GoToEndOfTurn(Dendron);

            // Assert
            QuickHPCheck(-1); // Painted Viper deals hero with lowest HP H - 2 toxic (1 damage in this case)
        }

        [Test]
        public void TestRestoration()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            StartGame();
            SetHitPoints(Dendron, 30);
            SetHitPoints(GetPaintedViperInPlay(), 1);
            SetHitPoints(GetStainedWolfInPlay(), 1);


            QuickHPStorage(Dendron.CharacterCard, GetPaintedViperInPlay(), GetStainedWolfInPlay());

            Card restoration = GetCard(RestorationCardController.Identifier);


            // Act
            GoToPlayCardPhase(Dendron);
            PlayCard(restoration);

            // Assert
            QuickHPCheck(10, 7, 7);

        }

        [Test]
        public void TestShadedOwl()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            StartGame();
            Card shadedOwl = GetCard(ShadedOwlCardController.Identifier);
            QuickHPStorage(legacy, ra, haka);

            // Act
            GoToPlayCardPhase(Dendron);
            PlayCard(shadedOwl);
            GoToEndOfTurn(Dendron);

            // Assert

            // Legacy: -2 from Shaded Owl (+1 from Shaded Owl)
            // Ra: -2 from Painted Viper (+1 from Shaded Owl), -2 from Shaded Owl (+1 from Shaded Owl)
            // Haka: -3 from Stained Wolf (+1 from Shaded Owl), -2 from Shaded Owl (+1 from Shaded Owl)
            QuickHPCheck(-2, -4, -5);
        }

        [Test]
        public void TestStainedWolf()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            StartGame();
            QuickHPStorage(legacy, ra, haka);

            // Act
            GoToEndOfTurn(Dendron);

            // Assert
            QuickHPCheck(0, -1, -2); // 0 for Legacy, -1 Ra from Painted Viper (lowest hp hero), -2 Haka from Stained Wolf (highest hp hero)


        }

        [Test]
        public void TestTintedStagEmptyTrash()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();
            Card tintedStag = GetCard(TintedStagCardController.Identifier);
            QuickShuffleStorage(Dendron.TurnTaker.Trash);
            QuickHPStorage(legacy, ra, haka);

            // Act
            GoToPlayCardPhase(Dendron);
            PlayCard(tintedStag);
            GoToEndOfTurn(Dendron);

            // Assert
            QuickShuffleCheck(1);
            QuickHPCheck(0, -1, -2);

        }

        [Test]
        public void TestTintedStagNoTattoosInTrash()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            // Put some non Tattoo cards in trash
            PutInTrash(Dendron, GetCard(RestorationCardController.Identifier));
            PutInTrash(Dendron, GetCard(InkScarCardController.Identifier));


            StartGame();
            Card tintedStag = GetCard(TintedStagCardController.Identifier);
            QuickShuffleStorage(Dendron.TurnTaker.Trash);
            QuickHPStorage(legacy, ra, haka);

            // Act
            GoToPlayCardPhase(Dendron);
            PlayCard(tintedStag);
            GoToEndOfTurn(Dendron);

            // Assert
            QuickShuffleCheck(1);
            QuickHPCheck(0, -1, -2);
            AssertNumberOfCardsInPlay(Dendron, 4); // Dendron, Stained Wolf, Painted Viper, Tinted Stag

        }

        [Test]
        public void TestTintedStagTattooInTrash()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            // Put one Tattoo card in trash
            PutInTrash(Dendron, GetCard(StainedWolfCardController.Identifier));
            PutInTrash(Dendron, GetCard(RestorationCardController.Identifier));


            StartGame();
            Card tintedStag = GetCard(TintedStagCardController.Identifier);
            QuickShuffleStorage(Dendron.TurnTaker.Trash);
            QuickHPStorage(legacy, ra, haka);

            // Act
            GoToPlayCardPhase(Dendron);
            PlayCard(tintedStag);
            GoToEndOfTurn(Dendron);

            // Assert
            QuickShuffleCheck(1);
            QuickHPCheck(-2, -1, -2); // Legacy: -2 fr Stained Wolf played by Stag, Ra: -1 fr Painted Viper, Haka: -2 fr initial Stained Wolf
            AssertNumberOfCardsInPlay(Dendron, 5); // Dendron, Stained Wolf x 2, Painted Viper, Tinted Stag

        }

        [Test]
        public void TestTintedStagMultipleTattoosInTrash()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            // Put multiple Tattoo card in trash
            PutInTrash(Dendron, GetCard(StainedWolfCardController.Identifier));
            PutInTrash(Dendron, GetCard(PaintedViperCardController.Identifier));
            PutInTrash(Dendron, GetCard(ObsidianSkinCardController.Identifier));
            PutInTrash(Dendron, GetCard(AdornedOakCardController.Identifier));


            StartGame();
            Card tintedStag = GetCard(TintedStagCardController.Identifier);
            QuickShuffleStorage(Dendron.TurnTaker.Trash);
            QuickHPStorage(legacy, ra, haka);

            // Act
            GoToPlayCardPhase(Dendron);
            PlayCard(tintedStag);
            GoToEndOfTurn(Dendron);

            // Assert
            QuickShuffleCheck(1);
            AssertNumberOfCardsInPlay(Dendron, 5);// Dendron, Stained Wolf, Painted Viper, Tinted Stag, Tattoo from trash
        }

        [Test]
        public void TestUrsaMajor()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");


            StartGame();
            Card ursaMajor = GetCard(UrsaMajorCardController.Identifier);
            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, ursaMajor);
            DecisionSelectTarget = legacy.CharacterCard;
            
            // Act
            GoToPlayCardPhase(Dendron);
            PlayCard(ursaMajor);
            GoToEndOfTurn(Dendron);

            DealDamage(ra, ursaMajor, 3, DamageType.Fire);

            // Assert
            
            // Legacy: -2 fr Ursa Major, Ra: -1 fr Painted Viper, Haka: -2 fr Stained Wolf, Ursa Major: -2 due to 1 damage reduction on it
            QuickHPCheck(-2, -1, -2, -2); 

        }


        private Card GetStainedWolfInPlay()
        {
            return GetCardInPlay(StainedWolfCardController.Identifier);
        }

        private Card GetPaintedViperInPlay()
        {
            return GetCardInPlay(PaintedViperCardController.Identifier);
        }

    }
}
