using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.UnitTest;

using Cauldron.Dendron;

using NUnit.Framework;
using Handelabra;

namespace CauldronTests
{
    [TestFixture]
    public class DendronVariantTests : CauldronBaseTest
    {

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

            Assert.IsNotNull(dendron);
            Assert.IsInstanceOf(typeof(WindcolorDendronCharacterCardController), dendron.CharacterCardController);

            Assert.AreEqual(40, dendron.CharacterCard.HitPoints);
        }

        [Test]
        public void TestWindcolorDendronStartGame()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");

            StartGame();

            AssertNotFlipped(dendron);

            var cards = FindCardsWhere(card => card.Identifier == StainedWolfCardController.Identifier || card.Identifier == PaintedViperCardController.Identifier);

            foreach (var card in cards)
            {
                AssertAtLocation(card, dendron.CharacterCard.UnderLocation);
                AssertDoesNotHaveGameText(card);
            }
        }

        [Test]
        public void TestWindcolorDendronChallenge()
        {
            SetupGameController(new string[] { DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis" }, challenge: true);

            StartGame();

            AssertNumberOfCardsAtLocation(dendron.CharacterCard.UnderLocation, 6);

            IEnumerable<Card> underCards = dendron.CharacterCard.UnderLocation.Cards;
            
            foreach(Card under in underCards)
            {
                AssertHasKeywordEvenIfUnderOrFaceDown(under, "tattoo");
            }

            if(underCards.Count((Card c) => c.Identifier == "StainedWolf" || c.Identifier == "PaintedViper") == 6)
            {
                Assert.Ignore("All cards under Dendron are Painted Vipers or Stained Wolves. Possible due to randomness, try rerunning the test.");
            }

            Card tattoo = underCards.First();
            PlayCard(tattoo);

            QuickHPStorage(dendron);
            DealDamage(tattoo, dendron, 5, DamageType.Infernal);
            QuickHPCheckZero();

            FlipCard(dendron);

            QuickHPStorage(dendron);
            DealDamage(tattoo, dendron, 5, DamageType.Infernal);
            QuickHPCheckZero();

        }

        [Test]
        public void TestWindcolorDendronFlipWhenNoCardsUnder()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            var c1 = PutOnDeck("UrsaMajor");
            var c2 = PutOnDeck("ShadedOwl");

            var cards = FindCardsWhere(card => card.Identifier == StainedWolfCardController.Identifier || card.Identifier == PaintedViperCardController.Identifier);
            PutInTrash(cards);

            AssertNotFlipped(dendron);

            GoToPlayCardPhase(dendron);
            PlayTopCard(dendron);

            GoToEndOfTurn(dendron);

            AssertFlipped(dendron);

            //second play is from backside
            AssertIsInPlay(c1, c2);
        }

        [Test]
        public void TestWindcolorDendronEndOfTurnPlayFromUnder()
        {
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            PutOnDeck("UrsaMajor");

            AssertNotFlipped(dendron);

            GoToPlayCardPhase(dendron);
            PlayTopCard(dendron);

            GoToEndOfTurn(dendron);

            int count = FindCardsWhere(card => card.Identifier == StainedWolfCardController.Identifier || card.Identifier == PaintedViperCardController.Identifier).Count(c => c.IsInPlayAndNotUnderCard && c.Location == dendron.TurnTaker.PlayArea);
            Assert.AreEqual(2, count, "2 Stained Wolfs or Painted Vipers should be played");
        }

        [Test]
        public void TestWindcolorFlippedDendronStartOfTurn()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Tachyon", "Megalopolis");

            var card = PlayCard("BloodThornAura");

            QuickHPStorage(dendron.CharacterCard, legacy.CharacterCard, ra.CharacterCard, haka.CharacterCard, tachyon.CharacterCard);
            StartGame();
            GoToPlayCardPhase(dendron);
            AddCannotPlayCardsStatusEffect(dendron, false, true);
            var cards = FindCardsWhere(c => c.Identifier == StainedWolfCardController.Identifier || c.Identifier == PaintedViperCardController.Identifier);
            PutInTrash(cards);
            FlipCard(dendron.CharacterCard);
            GoToStartOfTurn(dendron);
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

            FlipCard(dendron.CharacterCard);

            var c1 = PlayCard("UrsaMajor");
            AssertInPlayArea(dendron, c1);

            DestroyCard(c1);
            AssertAtLocation(c1, dendron.CharacterCard.UnderLocation);

            var c2 = PlayCard("TintedStag");
            DealDamage(legacy, c2, 10, DamageType.Cold);
            AssertAtLocation(c2, dendron.CharacterCard.UnderLocation);
       
        }

        [Test]
        public void TestWindcolorFlippedDendronTattoosUnderAreNotInPlay()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Cauldron.TangoOne", "Megalopolis");
            StartGame();


            Card viper = dendron.CharacterCard.UnderLocation.Cards.First(c => c.Identifier == "PaintedViper");
            Card owl = PlayCard("ShadedOwl");
            Card ursa = PlayCard("UrsaMajor");
            GoToPlayCardPhase(dendron);

            MoveCards(tango, c => c.DoKeywordsContain("critical"), tango.HeroTurnTaker.Hand, numberOfCards: 2);
            IEnumerable<Card> criticalCardsInHand = tango.HeroTurnTaker.Hand.Cards.Where(c => c.DoKeywordsContain("critical")).Take(2);
            Card rifle = PlayCard("SniperRifle");

            List<Card> decisionCards = new List<Card>();
            decisionCards.AddRange(criticalCardsInHand);
            decisionCards.Add(owl);
            SkipDecisionsBeforeAssertion(1);
            AssertNextDecisionChoices(notIncluded: new List<Card>(){ viper });
            UsePower(rifle, 0);
        }

        [Test]
        public void TestWindcolorFlippedDendronFlipBack()
        {
            // Arrange
            SetupGameController(DeckNamespace, "Legacy", "Ra", "Haka", "Tachyon", "Megalopolis");
            FlipCard(dendron.CharacterCard);
            MoveCards(dendron, FindCardsWhere(c => c.DoKeywordsContain("tattoo") && c.Location == dendron.TurnTaker.Deck).Take(3), dendron.CharacterCard.UnderLocation);
            QuickHPStorage(dendron);
            StartGame();

            AssertNotFlipped(dendron);

            QuickHPCheck(-10);
        }

        [Test]
        public void TestAdvancedWindcolorFlippedDendronIncreasedDamage()
        {
            SetupGameController(new[] { DeckNamespace, "Legacy", "Ra", "Haka", "Tachyon", "Megalopolis" }, advanced: true);
            StartGame();

            FlipCard(dendron.CharacterCard);

            QuickHPStorage(legacy);
            DealDamage(dendron, legacy, 1, DamageType.Toxic);
            QuickHPCheck(-3);
        }

        [Test]
        public void TestAdvancedWindcolorFlippedDendronFlipBack()
        {
            // Arrange
            SetupGameController(new[] { DeckNamespace, "Legacy", "Ra", "Haka", "Tachyon", "Megalopolis" }, advanced: true);
            FlipCard(dendron.CharacterCard);
            MoveCards(dendron, FindCardsWhere(c => c.DoKeywordsContain("tattoo") && c.Location == dendron.TurnTaker.Deck).Take(3), dendron.CharacterCard.UnderLocation);
            QuickHPStorage(dendron);
            StartGame();

            AssertNotFlipped(dendron);

            //10 damage then 10 healing
            QuickHPCheck(0);
        }

    }
}
