using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Cypher;

namespace CauldronTests
{
    [TestFixture]
    public class CypherVariantTests : CauldronBaseTest
    {

        private const string DeckNamespace = "Cauldron.Cypher";

        protected bool IsAugment(Card card)
        {
            if (card != null)
            {
                if (card.HasGameText && base.GameController.DoesCardContainKeyword(card, "augment"))
                    return true;

                if (card.IsInPlay && card.IsFaceDownNonCharacter && GameController.GetCardPropertyJournalEntryBoolean(card, CypherBaseCardController.NanocloudKey) == true)
                    return true;
            }
            return false;
        }

        private bool AreAugmented(List<Card> heroes)
        {
            foreach (Card hero in heroes)
            {
                if (!hero.IsHeroCharacterCard || !hero.IsInPlayAndHasGameText || hero.IsIncapacitatedOrOutOfGame
                    || !hero.NextToLocation.HasCards || !hero.GetAllNextToCards(false).Any(IsAugment))
                {
                    return false;
                }
            }

            return true;
        }

        private bool AreNotAugmented(List<Card> heroes)
        {
            return !AreAugmented(heroes);
        }

        private bool HasAugment(Card hero, Card augment)
        {
            return HasAugments(hero, new List<Card>() { augment });
        }

        private bool HasAugments(Card hero, List<Card> augments)
        {
            return FindCardsWhere(card => card == hero && card.Location.IsNextToCard).All(augments.Contains);
        }

        private void PutAugmentsIntoPlay(Dictionary<Card, List<Card>> augDictionary)
        {
            foreach (KeyValuePair<Card, List<Card>> kvp in augDictionary)
            {
                DecisionSelectCard = kvp.Key;
                foreach (Card aug in kvp.Value)
                {
                    PlayCard(aug);
                }
            }
        }

        [Test]
        public void TestFirstResponseCypherLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", DeckNamespace + "/FirstResponseCypherCharacter", "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(cypher);
            Assert.IsInstanceOf(typeof(FirstResponseCypherCharacterCardController), cypher.CharacterCardController);

            Assert.AreEqual(23, cypher.CharacterCard.HitPoints);
        }


        [Test]
        public void TestFirstResponseCypherInnatePower()
        {
            SetupGameController("BaronBlade", DeckNamespace + "/FirstResponseCypherCharacter", "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card muscleAug = GetCard(MuscleAugCardController.Identifier);
            Card dermalAug = GetCard(DermalAugCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { muscleAug }},
                { tachyon.CharacterCard, new List<Card>() { dermalAug }},
            }); ;

            GoToUsePowerPhase(cypher);
            QuickHPStorage(ra);
            DecisionSelectTarget = ra.CharacterCard;
            UsePower(cypher);
            QuickHPCheck(-2);
        }

        [Test]
        public void TestFirstResponseCypherIncapacitate1()
        {
            SetupGameController("BaronBlade", DeckNamespace + "/FirstResponseCypherCharacter", "Ra", "Tachyon", "Megalopolis");
            StartGame();

            //incapacitate
            DealDamage(baron, cypher, 99, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(cypher);
            AssertIncapLetsHeroDrawCard(cypher, 0, ra, 1);
        }

        [Test]
        public void TestFirstResponseCypherIncapacitate2Discard()
        {
            SetupGameController("BaronBlade", DeckNamespace + "/FirstResponseCypherCharacter", "Ra", "Tachyon", "Megalopolis");
            StartGame();

            //incapacitate
            DealDamage(baron, cypher, 99, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(cypher);

            var ttcs = new[] { baron, ra, tachyon, env };
            var beforeTrashs = ttcs.ToDictionary(ttc => ttc, ttc => ttc.TurnTaker.Trash.NumberOfCards);
            var bottomCards = ttcs.ToDictionary(ttc => ttc, ttc => ttc.TurnTaker.Deck.BottomCard);

            DecisionSelectTurnTakers = ttcs.Select(ttc => ttc.TurnTaker).ToArray();
            DecisionMoveCardDestinations = ttcs.Select(ttc => new MoveCardDestination(ttc.TurnTaker.Trash)).ToArray();

            UseIncapacitatedAbility(cypher, 1);

            foreach (var ttc in ttcs)
            {
                AssertNumberOfCardsInTrash(ttc, beforeTrashs[ttc] + 1);
                AssertInTrash(ttc, bottomCards[ttc]);
                AssertNumberOfCardsInRevealed(ttc, 0);
            }

            AssertNumberOfCardsInRevealed(cypher, 0);
        }

        [Test]
        public void TestFirstResponseCypherIncapacitate2Return()
        {
            SetupGameController("BaronBlade", DeckNamespace + "/FirstResponseCypherCharacter", "Ra", "Tachyon", "Megalopolis");
            StartGame();

            //incapacitate
            DealDamage(baron, cypher, 99, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(cypher);

            var ttcs = new[] { baron, ra, tachyon, env };
            var beforeTrashs = ttcs.ToDictionary(ttc => ttc, ttc => ttc.TurnTaker.Trash.NumberOfCards);
            var bottomCards = ttcs.ToDictionary(ttc => ttc, ttc => ttc.TurnTaker.Deck.BottomCard);

            DecisionSelectTurnTakers = ttcs.Select(ttc => ttc.TurnTaker).ToArray();
            DecisionMoveCardDestinations = ttcs.Select(ttc => new MoveCardDestination(ttc.TurnTaker.Deck, true)).ToArray();

            UseIncapacitatedAbility(cypher, 1);

            foreach (var ttc in ttcs)
            {
                AssertNumberOfCardsInTrash(ttc, beforeTrashs[ttc]); //unchanged
                AssertInDeck(ttc, bottomCards[ttc]);
                AssertOnBottomOfDeck(ttc, bottomCards[ttc]);
                AssertNumberOfCardsInRevealed(ttc, 0);
            }

            AssertNumberOfCardsInRevealed(cypher, 0);
        }

        [Test]
        public void TestFirstResponseCypherIncapacitate3()
        {
            SetupGameController("BaronBlade", DeckNamespace + "/FirstResponseCypherCharacter", "Ra", "Tachyon", "Megalopolis");
            StartGame();

            //incapacitate
            DealDamage(baron, cypher, 99, DamageType.Melee);

            QuickHPStorage(tachyon);

            GoToUseIncapacitatedAbilityPhase(cypher);

            AssertNumberOfStatusEffectsInPlay(0);
            UseIncapacitatedAbility(cypher, 2);
            AssertNumberOfStatusEffectsInPlay(1);

            QuickHPStorage(tachyon);
            DealDamage(ra, tachyon, 1, DamageType.Cold);
            QuickHPCheck(-2);

            AssertNumberOfStatusEffectsInPlay(0);
        }


        [Test]
        public void TestSwarmingProtocolCypherLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", DeckNamespace + "/SwarmingProtocolCypherCharacter", "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(cypher);
            Assert.IsInstanceOf(typeof(SwarmingProtocolCypherCharacterCardController), cypher.CharacterCardController);

            Assert.AreEqual(22, cypher.CharacterCard.HitPoints);
        }


        [Test]
        public void TestSwarmingProtocolCypherInnatePower()
        {
            SetupGameController("BaronBlade", DeckNamespace + "/SwarmingProtocolCypherCharacter", "Ra", "Tachyon", "Megalopolis");
            StartGame();

            var top = cypher.HeroTurnTaker.Deck.TopCard;
            var hand = cypher.HeroTurnTaker.Hand.TopCard;
            DecisionSelectCards = new[] { hand, ra.CharacterCard };

            QuickHandStorage(cypher);
            UsePower(cypher.CharacterCard);
            QuickHandCheck(0);

            AssertInHand(cypher, top);
            AssertNextToCard(hand, ra.CharacterCard);
            Assert.IsTrue(hand.IsFaceDownNonCharacter, $"{hand.Title} should be facedown");
        }

        [Test]
        public void TestSwarmingProtocolCypherIncapacitate1()
        {
            SetupGameController("BaronBlade", DeckNamespace + "/SwarmingProtocolCypherCharacter", "Ra", "Tachyon", "Megalopolis");
            StartGame();

            PutInHand("WrathfulGaze");

            //incapacitate
            DealDamage(baron, cypher, 99, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(cypher);

            AssertIncapLetsHeroPlayCard(cypher, 0, ra, "WrathfulGaze");
        }

        [Test]
        public void TestSwarmingProtocolCypherIncapacitate2()
        {
            SetupGameController("BaronBlade", DeckNamespace + "/SwarmingProtocolCypherCharacter", "Ra", "Tachyon", "Megalopolis");
            StartGame();

            //incapacitate
            DealDamage(baron, cypher, 99, DamageType.Melee);

            GoToUseIncapacitatedAbilityPhase(cypher);

            DecisionSelectLocation = new LocationChoice(baron.TurnTaker.Deck);
            var before = baron.TurnTaker.Trash.NumberOfCards;

            UseIncapacitatedAbility(cypher, 1);

            AssertNumberOfCardsInTrash(baron, before + 3);
        }

        [Test]
        public void TestSwarmingProtocolCypherIncapacitate3()
        {
            SetupGameController("BaronBlade", DeckNamespace + "/SwarmingProtocolCypherCharacter", "Ra", "Tachyon", "Megalopolis");
            StartGame();

            //incapacitate
            DealDamage(baron, cypher, 99, DamageType.Melee);

            var card = PlayCard("RooftopCombat");

            GoToUseIncapacitatedAbilityPhase(cypher);

            AssertIsInPlay(card);

            DecisionSelectCard = card;
            UseIncapacitatedAbility(cypher, 2);

            AssertInTrash(card);
        }

        private Card SetupSwarmingAugment(TurnTakerController target, Card hand = null)
        {
            if (hand is null)
                hand = GetRandomCardFromHand(cypher);

            DecisionSelectCards = new[] { hand, target.CharacterCard };
            UsePower(cypher.CharacterCard);
            ResetDecisions();
            AssertIsInPlay(hand);
            AssertNextToCard(hand, target.CharacterCard);

            return hand;
        }

        [Test]
        public void TestSwarmingProtocolNeuralInterface()
        {
            // You may move 1 Augment in play next to a new hero. Draw 2 cards. Discard a card

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace + "/SwarmingProtocolCypherCharacter", "Ra", "Tachyon", "Megalopolis");
            StartGame();

            var fauxAug = SetupSwarmingAugment(ra);
            Card neuralInterface = GetCard(NeuralInterfaceCardController.Identifier);
            Assert.AreNotEqual(fauxAug, neuralInterface);
            // Act
            GoToPlayCardPhase(cypher);

            PlayCard(neuralInterface);
            GoToUsePowerPhase(cypher);

            PrintSeparator();

            QuickHandStorage(cypher);

            UsePower(neuralInterface);

            // Assert
            Assert.True(AreAugmented(new List<Card>() { cypher.CharacterCard }), "Cypher should be augmented.");
            Assert.True(HasAugment(cypher.CharacterCard, fauxAug), "Cypher should have a nanocloud aug");
            Assert.True(AreNotAugmented(new List<Card>() { ra.CharacterCard }), "Ra should not be augmented");
            QuickHandCheck(1);
            AssertFlipped(fauxAug);
            
        }


        [Test]
        public void TestSwarmingProtocolCyberintegration()
        {
            SetupGameController("BaronBlade", DeckNamespace + "/SwarmingProtocolCypherCharacter", "Ra", "Tachyon", "Megalopolis");
            StartGame();

            SetHitPoints(new[] { cypher.CharacterCard, ra.CharacterCard, tachyon.CharacterCard }, 18);
            Card hand = PutInHand("InitiatedUpgrade");
            var fauxAug = SetupSwarmingAugment(ra,hand);
            Card dermalAug = GetCard(DermalAugCardController.Identifier);
            Card retinalAug = GetCard(RetinalAugCardController.Identifier);

            PutAugmentsIntoPlay(new Dictionary<Card, List<Card>>()
            {
                { ra.CharacterCard, new List<Card>() { retinalAug}},
                { tachyon.CharacterCard, new List<Card>() { dermalAug}}
            });

            QuickHPStorage(cypher, ra, tachyon);

            DecisionSelectCards = new Card[] { fauxAug, retinalAug, null };
            PlayCard("Cyberintegration");
            QuickHPCheck(0, 6, 0);
        }

        [Test]
        public void TestSwarmingProtocolCyborgBlaster()
        {
            // You may move 1 Augment in play next to a new hero.
            // One augmented hero deals 1 target 2 lightning damage.

            // Arrange
            SetupGameController("BaronBlade", DeckNamespace + "/SwarmingProtocolCypherCharacter", "Ra", "Tachyon", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            var fauxAug = SetupSwarmingAugment(ra);

            Card cyborgBlaster = GetCard(CyborgBlasterCardController.Identifier);
            QuickHPStorage(mdp);

            DecisionSelectCards = new[] { fauxAug, tachyon.CharacterCard, mdp };

            PlayCard(cyborgBlaster);

            Assert.True(AreAugmented(new List<Card>() { tachyon.CharacterCard }));
            Assert.True(AreNotAugmented(new List<Card>() { ra.CharacterCard }));
            Assert.True(HasAugment(tachyon.CharacterCard, fauxAug));
            QuickHPCheck(-2); // +2 from Cyborg Blaster,
        }
    }
}
