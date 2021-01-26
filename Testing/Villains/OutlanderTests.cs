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
        protected const string Archangel = "Archangel";
        protected const string AnchoredFragment = "AnchoredFragment";
        protected const string Crusader = "Crusader";
        protected const string DimensionalInsinuation = "DimensionalInsinuation";
        protected const string DisarmingBlow = "DisarmingBlow";
        protected const string Dragonborn = "Dragonborn";
        protected const string KnightsHatred = "KnightsHatred";
        protected const string Magekiller = "Magekiller";
        protected const string OutOfTouch = "OutOfTouch";
        protected const string RiftbladeStrikes = "RiftbladeStrikes";
        protected const string TransdimensionalOnslaught = "TransdimensionalOnslaught";
        protected const string Warbrand = "Warbrand";

        [Test()]
        public void TestOutlander_Load()
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
        public void TestOutlander_Decklist()
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

        [Test]
        public void TestOutlander_Back()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Bunker", "TheScholar", "Unity", "Legacy", "Megalopolis" });
            StartGame();

            Card anchor = PlayCard(AnchoredFragment);

            DealDamage(haka, outlander, 100, DamageType.Melee);
            //When {Outlander} flips to this side, restore him to 20 HP...
            AssertHitPoints(outlander.CharacterCard, 20);

            //...destroy all copies of Anchored Fragment...
            AssertInTrash(anchor);

            //...and put a random Trace into play. 
            Assert.AreEqual(2, FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("trace")).Count());

            //Then, if there are fewer than {H} Trace cards in play, flip {Outlander}'s villain character cards.
            AssertNotFlipped(outlander);
            DestroyNonCharacterVillainCards();

            //Need 5 traces
            DealDamage(haka, outlander, 100, DamageType.Melee);
            Assert.AreEqual(3, FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("trace")).Count());
            AssertNotFlipped(outlander);
            DestroyNonCharacterVillainCards();

            //Need 5 traces
            DealDamage(haka, outlander, 100, DamageType.Melee);
            Assert.AreEqual(4, FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("trace")).Count());
            AssertNotFlipped(outlander);
            DestroyNonCharacterVillainCards();

            //5 traces in play
            DealDamage(haka, outlander, 100, DamageType.Melee);
            Assert.AreEqual(5, FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("trace")).Count());
            AssertFlipped(outlander);
            DestroyNonCharacterVillainCards();

            //Cards beneath this one are not considered in play. Trace cards are indestructible.
            Card arch = DestroyCard(Archangel);
            AssertIsInPlay(arch);

            //Reduce the first damage dealt to {Outlander} each turn by {H}.
            QuickHPStorage(outlander);
            DealDamage(haka, outlander, 6, DamageType.Melee);
            QuickHPCheck(-1);

            //Second Damage
            QuickHPStorage(outlander);
            DealDamage(bunker, outlander, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //New turn
            GoToStartOfTurn(haka);
            QuickHPStorage(outlander);
            DealDamage(unity, outlander, 6, DamageType.Melee);
            QuickHPCheck(-1);

            //Second Damage
            QuickHPStorage(outlander);
            DealDamage(legacy, outlander, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //Win Game if flipped and destroyed!
            DealDamage(haka, outlander, 100, DamageType.Melee);
            AssertGameOver(EndingResult.VillainDestroyedVictory);
        }

        [Test]
        public void TestOutlander_Flip_3H()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Bunker", "TheScholar", "Megalopolis" });
            StartGame();

            //Then, if there are fewer than {H} Trace cards in play, flip {Outlander}'s villain character cards.
            DealDamage(haka, outlander, 100, DamageType.Melee);
            Assert.AreEqual(2, FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("trace")).Count());
            AssertNotFlipped(outlander);
            DestroyNonCharacterVillainCards();

            //3 traces in play
            DealDamage(haka, outlander, 100, DamageType.Melee);
            Assert.AreEqual(3, FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("trace")).Count());
            AssertFlipped(outlander);
        }

        [Test]
        public void TestOutlander_Flip_4H()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Bunker", "TheScholar", "Unity", "Megalopolis" });
            StartGame();

            //Then, if there are fewer than {H} Trace cards in play, flip {Outlander}'s villain character cards.
            DealDamage(haka, outlander, 100, DamageType.Melee);
            Assert.AreEqual(2, FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("trace")).Count());
            AssertNotFlipped(outlander);
            DestroyNonCharacterVillainCards();

            //Need 4 traces
            DealDamage(haka, outlander, 100, DamageType.Melee);
            Assert.AreEqual(3, FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("trace")).Count());
            AssertNotFlipped(outlander);
            DestroyNonCharacterVillainCards();

            //4 traces in play
            DealDamage(haka, outlander, 100, DamageType.Melee);
            Assert.AreEqual(4, FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.DoKeywordsContain("trace")).Count());
            AssertFlipped(outlander);
        }

        [Test]
        public void TestOutlander_Back_Advanced()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Bunker", "TheScholar", "Unity", "Legacy", "Megalopolis" });
            StartGame();
        }
    }
}
