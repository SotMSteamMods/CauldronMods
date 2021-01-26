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

        [Test]
        public void TestOutlander_Start()
        {
            SetupGameController("Cauldron.Outlander", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //Search the villain deck for all Trace cards and put them beneath this card. Put 1 random Trace card from beneath this one into play. Shuffle the villain deck.
            AssertNumberOfCardsUnderCard(outlander.CharacterCard, 4);

            Assert.AreEqual(1, FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("trace")).Count());
        }

        [Test]
        public void TestOutlander_Front()
        {
            SetupGameController("Cauldron.Outlander", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //Cards beneath this one are not considered in play. 
            AssertNumberOfCardsInPlay(outlander, 2);

            //Trace cards are indestructible.
            Card trace = FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("trace")).FirstOrDefault();
            DestroyCard(trace);
            AssertIsInPlay(trace);

            //When {Outlander} would be destroyed instead flip his villain character cards.
            DealDamage(haka, outlander, 100, DamageType.Melee);
            AssertNotGameOver();

            //Can't test flip because Outlander flips right back
        }

        [Test]
        public void TestOutlander_Front_Advanced()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Bunker", "TheScholar", "Megalopolis" }, true);
            StartGame();

            //Whenever {Outlander} flips to this side, he becomes immune to damage until the start of the next villain turn.
            DealDamage(haka, outlander, 100, DamageType.Melee);

            QuickHPStorage(outlander);
            DealDamage(haka, outlander, 3, DamageType.Melee);
            QuickHPCheck(0);

            GoToStartOfTurn(env);
            QuickHPStorage(outlander);
            DealDamage(haka, outlander, 3, DamageType.Melee);
            QuickHPCheck(0);

            GoToStartOfTurn(outlander);
            QuickHPStorage(outlander);
            DealDamage(haka, outlander, 3, DamageType.Melee);
            QuickHPCheck(-3);
        }
    }
}
