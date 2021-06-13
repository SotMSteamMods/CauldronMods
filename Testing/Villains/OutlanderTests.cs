using NUnit.Framework;
using Cauldron.Outlander;
using Handelabra.Sentinels.UnitTest;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

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

        private void SetupFlipped()
        {
            var inPlay = outlander.FindCardsWhere((Card c) => c.DoKeywordsContain("trace") && c.IsInPlayAndHasGameText).ToList();
            var outOfPlay = outlander.CharacterCard.UnderLocation.Cards.ToList();

            while (inPlay.Count < GameController.Game.H - 1)
            {
                var card = outOfPlay.First();
                outOfPlay.Remove(card);
                PlayCard(card, isPutIntoPlay: true);
                inPlay.Add(card);
            }
            FlipCard(outlander.CharacterCard);
            AssertFlipped(outlander.CharacterCard);
        }



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
        public void TestOutlander_Challenge()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Bunker", "TheScholar", "Megalopolis" }, challenge: true);
            StartGame();

            //Whenever a villain Ongoing is destroyed, other villain Ongoings are indestructible until the end of the villain turn.
            Card ongoing1 = PlayCard("DimensionalInsinuation");
            Card ongoing2 = PlayCard("AnchoredFragment");

            DestroyCard(ongoing1, bunker.CharacterCard);
            AssertInTrash(ongoing1);

            DestroyCard(ongoing2, bunker.CharacterCard);
            AssertInPlayArea(outlander, ongoing2);

            GoToStartOfTurn(haka);
            DestroyCard(ongoing2, bunker.CharacterCard);
            AssertInTrash(ongoing2);

        }

        [Test]
        public void TestOutlander_Challenge_EdgeCase()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "VoidGuardDrMedico", "Luminary", "TheScholar", "Megalopolis" }, challenge: true);
            StartGame();

            IEnumerable<Card> ongoings = FindCardsWhere(c => outlander.TurnTaker.Deck.HasCard(c) && c.IsOngoing).Take(4);
            PlayCards(ongoings);

            SetHitPoints(luminary.CharacterCard, 10);
            SetHitPoints(scholar.CharacterCard, 5);
            DecisionSelectCard = luminary.CharacterCard;
            DecisionAutoDecideIfAble = true;
            UsePower(voidMedico);

            Card donor = PlayCard("UniversalDonor");
            Card second = PlayCard("SecondOpinion");

            DiscardAllCards(luminary);
            MoveAllCards(luminary, luminary.TurnTaker.Deck, luminary.TurnTaker.Trash);

            Card terralunar = PlayCard("TerralunarTranslocator");
            DecisionSelectPower = terralunar;
            DecisionYesNo = true;



            Card crusader = PlayCard("Crusader");

            DecisionSelectCards = luminary.CharacterCards.Concat(ongoings).Concat(new List<Card>(){ null});
            GoToEndOfTurn(outlander);
            RunActiveTurnPhase();
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
        public void TestOutlander_FlipDestroy()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Bunker", "TheScholar", "Megalopolis" });
            StartGame();

            DestroyCard(outlander.CharacterCard);
            AssertNotGameOver();
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
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Bunker", "TheScholar", "Unity", "Legacy", "Megalopolis" }, advanced: true, randomSeed: 1138828501);
            outlander.DebugTraceToPlay = GetCard(Archangel);
            StartGame();

            SetupFlipped();

            Card moko = PlayCard("TaMoko");
            Card mere = PlayCard("Mere");
            Card endure = PlayCard("EnduringIntercession");
            Card tai = PlayCard("Taiaha");
            AssertIsInPlay(moko, mere, endure, tai);

            AddCannotDealDamageTrigger(outlander, outlander.CharacterCard);

            //At the end of the villain turn, destroy {H - 2} hero ongoing and/or equipment cards.
            DecisionSelectCards = new[] { moko, endure, mere };
            GoToEndOfTurn(outlander);
            AssertIsInPlay(tai);
            AssertInTrash(endure, moko, mere);
        }

        [Test]
        public void TestAnchoredFragment_NoDamage()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Bunker", "TheScholar", "Megalopolis" });
            outlander.DebugTraceToPlay = GetCard("Magekiller");
            StartGame();

            //When this card enters play, {Outlander} deals the hero target with the highest HP 1 melee damage.
            QuickHPStorage(haka);
            PlayCard(AnchoredFragment);
            QuickHPCheck(-1);

            //At the start of the villain turn, if {Outlander} was not dealt at least {H} times 2 damage in the last round, destroy {H} hero ongoing and/or equipment cards.
            Card moko = PlayCard("TaMoko");
            Card flak = PlayCard("FlakCannon");
            Card mere = PlayCard("Mere");

            GoToStartOfTurn(outlander);
            AssertInTrash(moko, mere, flak);
        }

        [Test]
        public void TestAnchoredFragment_NotEnoughDamage()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Bunker", "TheScholar", "Megalopolis" });
            outlander.DebugTraceToPlay = GetCard("Magekiller");
            StartGame();

            //When this card enters play, {Outlander} deals the hero target with the highest HP 1 melee damage.
            QuickHPStorage(haka);
            PlayCard(AnchoredFragment);
            QuickHPCheck(-1);

            DealDamage(haka, outlander, 5, DamageType.Melee);

            //At the start of the villain turn, if {Outlander} was not dealt at least {H} times 2 damage in the last round, destroy {H} hero ongoing and/or equipment cards.
            Card moko = PlayCard("TaMoko");
            Card flak = PlayCard("FlakCannon");
            Card mere = PlayCard("Mere");

            GoToStartOfTurn(outlander);
            AssertInTrash(moko, mere, flak);
        }

        [Test]
        public void TestAnchoredFragment_EnoughDamage()
        {
            //seed: 1588414095
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Bunker", "TheScholar", "Megalopolis" }, randomSeed: 1588414095);
            outlander.DebugTraceToPlay = GetCard("Magekiller");
            StartGame();

            //When this card enters play, {Outlander} deals the hero target with the highest HP 1 melee damage.
            QuickHPStorage(haka);
            PlayCard(AnchoredFragment);
            QuickHPCheck(-1);

            DealDamage(haka, outlander, 6, DamageType.Melee);

            //At the start of the villain turn, if {Outlander} was not dealt at least {H} times 2 damage in the last round, destroy {H} hero ongoing and/or equipment cards.
            Card moko = PlayCard("TaMoko");
            Card flak = PlayCard("FlakCannon");
            Card truth = PlayCard("TruthSeeker");
            Card mere = PlayCard("Mere");

            GoToStartOfTurn(outlander);
            AssertIsInPlay(mere, moko, flak, truth);
        }

        [Test]
        public void TestArchangel()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Bunker", "TheScholar", "Megalopolis" });
            outlander.DebugTraceToPlay = GetCard(Archangel);
            StartGame();

            Card blow = PutOnDeck(DisarmingBlow);

            //The first time {Outlander} is dealt 4 or more damage from a single source each turn, play the top card of the villain deck.

            //Not Enough damage
            DealDamage(haka, outlander, 3, DamageType.Melee);
            AssertOnTopOfDeck(blow);

            //Enough Damage
            DealDamage(haka, outlander, 4, DamageType.Melee);
            AssertInTrash(blow);

            //Only once per turn
            Card top = outlander.TurnTaker.Deck.TopCard;
            DealDamage(bunker, outlander, 4, DamageType.Melee);
            AssertOnTopOfDeck(top);

            Card traffic = PlayCard("TrafficPileup");
            //To assert irreducible
            PlayCard("TaMoko");
            //At the end of the villain turn, {Outlander} deals each non-villain target irreducible 1 projectile damage.
            QuickHPStorage(haka.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, traffic);
            GoToEndOfTurn(outlander);
            QuickHPCheck(-1, -1, -1, -1);

            GoToStartOfTurn(haka);
            DealDamage(bunker, outlander, 4, DamageType.Melee);
            AssertNotOnTopOfDeck(outlander, top);
        }

        [Test]
        public void TestCrusader()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Parse", "TheScholar", "Megalopolis" });
            outlander.DebugTraceToPlay = GetCard(Crusader);
            StartGame();

            //Increase damage dealt by {Outlander} by 1.
            QuickHPStorage(haka, parse, scholar);
            DealDamage(outlander, haka, 2, DamageType.Melee);
            DealDamage(outlander, parse, 2, DamageType.Melee);
            DealDamage(outlander, scholar, 2, DamageType.Melee);
            QuickHPCheck(-3, -3, -3);

            PlayCard("TaMoko");
            //At the end of the villain turn, {Outlander} deals the 2 non-villain targets with the highest HP 2 irreducible melee damage each.
            QuickHPStorage(haka, parse, scholar);
            GoToEndOfTurn(outlander);
            QuickHPCheck(-3, 0, -3);
        }

        [Test]
        public void TestDimensionalInsinuation()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Parse", "TheScholar", "Megalopolis" });
            outlander.DebugTraceToPlay = GetCard(Archangel);
            StartGame();

            StackAfterShuffle(outlander.TurnTaker.Deck, new string[] { DisarmingBlow });
            //When this card enters play, Search the villain deck for a copy of Anchored Fragment and put it into play. Shuffle the villain deck and play its top card.
            QuickShuffleStorage(outlander);
            Card dim = PlayCard(DimensionalInsinuation);
            AssertIsInPlay(AnchoredFragment);
            AssertInTrash(DisarmingBlow);
            QuickShuffleCheck(1);

            PlayCard("TaMoko");
            PlayCard("FleshToIron");
            //Damage dealt by {Outlander} is irreducible. 
            QuickHPStorage(haka, parse, scholar);
            DealDamage(outlander, haka, 2, DamageType.Melee);
            DealDamage(outlander, parse, 2, DamageType.Melee);
            DealDamage(outlander, scholar, 2, DamageType.Melee);
            QuickHPCheck(-2, -2, -2);

            //At the start of the villain turn, destroy this card.
            GoToStartOfTurn(outlander);
            AssertInTrash(dim);
        }

        [Test]
        public void TestDisarmingBlow()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Parse", "TheScholar", "Megalopolis" });
            outlander.DebugTraceToPlay = GetCard(Archangel);
            StartGame();

            PlayCard("FleshToIron");

            //{Outlander} deals the 2 non-villain targets with the highest HP 3 melee damage each.
            //Any hero damaged this way discards 1 card.
            QuickHPStorage(haka, parse, scholar);
            QuickHandStorage(haka, parse, scholar);
            PlayCard(DisarmingBlow);
            QuickHPCheck(-3, 0, -1);
            QuickHandCheck(-1, 0, -1);

            PlayCard("FleshToIron");

            QuickHPStorage(haka, parse, scholar);
            QuickHandStorage(haka, parse, scholar);
            PlayCard(DisarmingBlow);
            QuickHPCheck(-3, 0, 0);
            QuickHandCheck(-1, 0, 0);
        }

        [Test]
        public void TestDisarmingBlow_NotHeroTakesDamage()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Unity", "TheScholar", "Megalopolis" });
            outlander.DebugTraceToPlay = GetCard(Archangel);
            StartGame();

            SetHitPoints(new TurnTakerController[] { haka, scholar }, 5);
            SetHitPoints(unity, 4);
            Card swift = PlayCard("SwiftBot");

            //{Outlander} deals the 2 non-villain targets with the highest HP 3 melee damage each.
            //Any hero damaged this way discards 1 card.
            QuickHPStorage(swift);
            QuickHandStorage(unity);
            PlayCard(DisarmingBlow);
            QuickHPCheck(-3);
            QuickHandCheck(0);
        }
        [Test]
        public void TestDisarmingBlow_NoDamageDealt()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Unity", "TheScholar", "Megalopolis" });
            outlander.DebugTraceToPlay = GetCard(Archangel);
            StartGame();

            PlayCard("FleshToIron");
            PlayCard("FleshToIron");

            QuickHandStorage(scholar);
            PlayCard(DisarmingBlow);
            QuickHandCheckZero();
        }

        [Test]
        public void TestDragonborn()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Unity", "TheScholar", "Megalopolis" });
            outlander.DebugTraceToPlay = GetCard(Dragonborn);
            StartGame();

            Card traffic = PlayCard("TrafficPileup");

            //The first time {Outlander} is dealt damage each turn, he deals the source of that damage 2 fire damage.
            QuickHPStorage(haka, unity, scholar);
            DealDamage(haka, outlander, 2, DamageType.Melee);
            QuickHPCheck(-2, 0, 0);

            //Once per turn
            QuickHPStorage(haka, unity, scholar);
            DealDamage(unity, outlander, 2, DamageType.Melee);
            QuickHPCheck(0, 0, 0);

            //At the end of the villain turn, {Outlander} deals each non-villain target 1 fire damage.
            QuickHPStorage(haka.CharacterCard, unity.CharacterCard, scholar.CharacterCard, traffic);
            GoToEndOfTurn(outlander);
            QuickHPCheck(-1, -1, -1, -1);

            //New turn
            GoToStartOfTurn(haka);
            QuickHPStorage(haka, unity, scholar);
            DealDamage(unity, outlander, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0);
        }

        [Test]
        public void TestKnightsHatred()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Unity", "TheScholar", "Megalopolis" });
            outlander.DebugTraceToPlay = GetCard(Archangel);
            StartGame();

            Card hate = PlayCard(KnightsHatred);

            QuickHPStorage(haka, outlander);
            DealDamage(outlander, haka, 2, DamageType.Melee);
            DealDamage(haka, outlander, 2, DamageType.Melee);
            //Increase damage dealt by {Outlander} by 1.
            //Reduce damage dealt to {Outlander} by 1.
            QuickHPCheck(-3, -1);

            //At the start of the villain turn, destroy this card.
            GoToStartOfTurn(outlander);
            AssertInTrash(hate);
        }

        [Test]
        public void TestMagekiller()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Unity", "TheScholar", "Megalopolis" });
            outlander.DebugTraceToPlay = GetCard(Magekiller);
            StartGame();

            //The first time a hero one-shot enters play each turn, {Outlander} deals the hero target with the highest HP 1 irreducible lightning damage.

            //Hero Only
            QuickHPStorage(haka, scholar, unity);
            PlayCard("RiftbladeStrikes");
            QuickHPCheck(-4, -2, 0);

            //Not a one-shot
            QuickHPStorage(haka, scholar, unity);
            PlayCard("TaMoko");
            QuickHPCheck(0, 0, 0);

            //First time
            QuickHPStorage(haka, scholar, unity);
            Card smash = PlayCard("ElbowSmash");
            QuickHPCheck(-1, 0, 0);

            //Only first time each turn
            QuickHPStorage(scholar, haka, unity);
            PutInHand(haka, smash);
            PlayCard(smash);
            QuickHPCheck(0, 0, 0);

            SetHitPoints(haka, 17);
            SetHitPoints(unity, 17);
            //At the end of the villain turn, {Outlander} deals the hero target with the highest HP 3 melee damage.
            QuickHPStorage(scholar, haka, unity);
            GoToEndOfTurn(outlander);
            QuickHPCheck(-3, 0, 0);

            GoToStartOfTurn(haka);
            //New turn
            QuickHPStorage(haka, unity, scholar);
            PutInHand(haka, smash);
            PlayCard(smash);
            QuickHPCheck(0, 0, -1);
        }

        [Test]
        public void TestOutOfTouch()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Unity", "TheScholar", "Megalopolis" });
            outlander.DebugTraceToPlay = GetCard(Archangel);
            StartGame();

            //When this card enters play, {Outlander} deals the non-villain target with the highest HP X+3 melee damage, where X is the number of Trace cards in play.
            QuickHPStorage(haka, unity, scholar);
            Card touch = PlayCard(OutOfTouch);
            QuickHPCheck(-4, 0, 0);

            SetHitPoints(scholar, 17);
            //Reduce all HP recovery by 1. 
            QuickHPStorage(haka, scholar);
            PlayCard("VitalitySurge");
            UsePower(scholar);
            QuickHPCheck(1, 0);

            //At the start of the villain turn, destroy this card.
            GoToStartOfTurn(outlander);
            AssertInTrash(touch);
        }

        [Test]
        public void TestOutOfTouch_3X()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Unity", "TheScholar", "Megalopolis" });
            outlander.DebugTraceToPlay = GetCard(Archangel);
            StartGame();

            PlayCard(Dragonborn);
            PlayCard(Magekiller);

            //When this card enters play, {Outlander} deals the non-villain target with the highest HP X+3 melee damage, where X is the number of Trace cards in play.
            QuickHPStorage(haka, unity, scholar);
            Card touch = PlayCard(OutOfTouch);
            QuickHPCheck(-6, 0, 0);
        }

        [Test]
        public void TestRiftbladeStrikes()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Unity", "TheScholar", "Megalopolis" });
            outlander.DebugTraceToPlay = GetCard(Archangel);
            StartGame();

            //{Outlander} deals the non-villain target with the second highest HP 2 fire damage.
            //{Outlander} deals the non-villain target with the highest HP 4 melee damage.
            QuickHPStorage(haka, unity, scholar);
            PlayCard("RiftbladeStrikes");
            QuickHPCheck(-4, 0, -2);

            Card traffic = PlayCard("TrafficPileup");
            Card rail = PlayCard("PlummetingMonorail");
            SetHitPoints(new TurnTakerController[] { haka, unity, scholar }, 4);
            //Check environment valid targets
            QuickHPStorage(traffic, rail, haka.CharacterCard, unity.CharacterCard, scholar.CharacterCard);
            PlayCard("RiftbladeStrikes");
            QuickHPCheck(-4, -2, 0, 0, 0);
        }

        [Test]
        public void TestTransdimensionalOnslaught()
        {
            //862560385
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Unity", "TheScholar", "Megalopolis" });
            outlander.DebugTraceToPlay = GetCard(Archangel);
            StartGame();

            PlayCard("TaMoko");

            Card traffic = PlayCard("TrafficPileup");
            Card rail = PlayCard("PlummetingMonorail");
            //{Outlander} deals each non-villain target X irreducible psychic damage, where X is the number of Trace cards in play.
            DecisionAutoDecideIfAble = true;
            QuickHPStorage(traffic, rail, haka.CharacterCard, unity.CharacterCard, scholar.CharacterCard);
            PlayCard("TransdimensionalOnslaught");
            QuickHPCheck(-1, -1, -1, -1, -1);
        }

        [Test]
        public void TestTransdimensionalOnslaught_3X()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Unity", "TheScholar", "Megalopolis" });
            outlander.DebugTraceToPlay = GetCard(Archangel);
            StartGame();


            PlayCard(Dragonborn);
            PlayCard(Magekiller);
            PlayCard("TaMoko");

            Card traffic = PlayCard("TrafficPileup");
            Card rail = PlayCard("PlummetingMonorail");
            //{Outlander} deals each non-villain target X irreducible psychic damage, where X is the number of Trace cards in play.
            DecisionAutoDecideIfAble = true;
            QuickHPStorage(traffic, rail, haka.CharacterCard, unity.CharacterCard, scholar.CharacterCard);
            PlayCard("TransdimensionalOnslaught");
            QuickHPCheck(-3, -3, -3, -3, -3);
        }

        [Test]
        public void TestWarbrand()
        {
            SetupGameController(new string[] { "Cauldron.Outlander", "Haka", "Unity", "TheScholar", "Megalopolis" });
            outlander.DebugTraceToPlay = GetCard(Warbrand);
            StartGame();

            //The first time {Outlander} deals damage each turn, he then deals the hero target with the highest HP 2 projectile damage.
            QuickHPStorage(haka, unity, scholar);
            DealDamage(outlander, unity, 2, DamageType.Melee);
            QuickHPCheck(-2, -2, 0);

            //At the end of the villain turn, {Outlander} deals the 2 hero targets with the lowest HP 1 projectile damage each.
            //Extra damage from once per turn does not trigger, because it's the same turn
            QuickHPStorage(haka, unity, scholar);
            GoToEndOfTurn(outlander);
            QuickHPCheck(0, -1, -1);

            //New turn
            GoToStartOfTurn(haka);
            QuickHPStorage(haka, unity, scholar);
            DealDamage(outlander, unity, 2, DamageType.Melee);
            QuickHPCheck(-2, -2, 0);
        }
    }
}
