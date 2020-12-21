using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.UnitTest;

using Cauldron.Dendron;

using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture]
    public class DendronVariantTests : BaseTest
    {
        protected TurnTakerController Dendron => FindVillain("Dendron");

        private const string DeckNamespace = "Cauldron.Dendron/WindcolorDendronCharacter";

        private Card GetStainedWolfInPlay()
        {
            return GetCardInPlay(StainedWolfCardController.Identifier);
        }

        private Card GetPaintedViperInPlay()
        {
            return GetCardInPlay(PaintedViperCardController.Identifier);
        }


        [Test]
        public void TestWindcolorDendronLoads()
        {
            SetupGameController(DeckNamespace, "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Dendron);
            Assert.IsInstanceOf(typeof(WindcolorDendronCharacterCardController), Dendron.CharacterCardController);

            Assert.AreEqual(40, Dendron.CharacterCard.HitPoints);
        }

        [Test]
        public void TestWindcolorDendronStartGame()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();

            AssertNotFlipped(Dendron);

            var cards = FindCardsWhere(card => card.Identifier == StainedWolfCardController.Identifier);
            foreach (var card in cards)
            {
                AssertAtLocation(card, Dendron.CharacterCard.UnderLocation);
                AssertDoesNotHaveGameText(card);
            }
        }

        [Test]
        public void TestWindcolorDendronFlipWhenNoCardsUnder()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            var c1 = PutOnDeck("PaintedViper");
            var c2 = PutOnDeck("PaintedViper");

            var cards = FindCardsWhere(card => card.Identifier == StainedWolfCardController.Identifier);
            PutInTrash(cards);

            AssertNotFlipped(Dendron);

            GoToPlayCardPhase(Dendron);
            PlayTopCard(Dendron);

            GoToEndOfTurn(Dendron);

            AssertFlipped(Dendron);

            //second play is from backside
            AssertIsInPlay(c1, c2);
        }

        [Test]
        public void TestWindcolorDendronEndOfTurnPlayFromUnder()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            PutOnDeck("PaintedViper");

            AssertNotFlipped(Dendron);

            GoToPlayCardPhase(Dendron);
            PlayTopCard(Dendron);

            GoToEndOfTurn(Dendron);

            int count = FindCardsWhere(card => card.Identifier == StainedWolfCardController.Identifier).Count(c => c.IsInPlayAndNotUnderCard && c.Location == Dendron.TurnTaker.PlayArea);
            Assert.AreEqual(2, count, "2 Stained Wolfs should be played");
        }

        [Test]
        public void TestWindcolorFlippedDendronStartOfTurn()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Tachyon", "Megalopolis");
            FlipCard(Dendron.CharacterCard);
            var card = PlayCard("BloodThornAura");

            QuickHPStorage(Dendron.CharacterCard, legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, tachyon.CharacterCard);
            StartGame();
            QuickHPCheck(0, -2, -2, -2, -2);

            AssertInTrash(card);
            //blood thron aura should be destroyed, hero's dealt 2 radiant damage.
        }

        [Test]
        public void TestWindcolorFlippedDendronTattoosMoveUnderneth()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Tachyon", "Megalopolis");
            StartGame();

            FlipCard(Dendron.CharacterCard);

            var c1 = PlayCard("PaintedViper");
            AssertInPlayArea(Dendron, c1);

            DestroyCard(c1);
            AssertAtLocation(c1, Dendron.CharacterCard.UnderLocation);

            var c2 = PlayCard("TintedStag");
            DealDamage(legacy, c2, 10, DamageType.Cold);
            AssertAtLocation(c2, Dendron.CharacterCard.UnderLocation);
        }

        [Test]
        public void TestWindcolorFlippedDendronFlipBack()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Tachyon", "Megalopolis");
            FlipCard(Dendron.CharacterCard);
            MoveCards(Dendron, FindCardsWhere(c => c.DoKeywordsContain("tattoo") && c.Location == Dendron.TurnTaker.Deck).Take(3), Dendron.CharacterCard.UnderLocation);
            QuickHPStorage(Dendron);
            StartGame();

            AssertNotFlipped(Dendron);

            QuickHPCheck(-10);
        }

        [Test]
        public void TestAdvancedWindcolorFlippedDendronIncreasedDamage()
        {
            SetupGameController(new[] { DeckNamespace, "Legacy", "Ra", "Haka", "Tachyon", "Megalopolis" }, advanced: true);
            StartGame();

            FlipCard(Dendron.CharacterCard);

            QuickHPStorage(legacy);
            DealDamage(Dendron, legacy, 1, DamageType.Toxic);
            QuickHPCheck(-3);
        }

        [Test]
        public void TestAdvancedWindcolorFlippedDendronFlipBack()
        {
            // Arrange
            SetupGameController(new[] { DeckNamespace, "Legacy", "Ra", "Haka", "Tachyon", "Megalopolis" }, advanced: true);
            FlipCard(Dendron.CharacterCard);
            MoveCards(Dendron, FindCardsWhere(c => c.DoKeywordsContain("tattoo") && c.Location == Dendron.TurnTaker.Deck).Take(3), Dendron.CharacterCard.UnderLocation);
            QuickHPStorage(Dendron);
            StartGame();

            AssertNotFlipped(Dendron);

            //10 damage then 10 healing
            QuickHPCheck(0);
        }

    }
}
