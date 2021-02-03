using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.UnitTest;

using Cauldron.Dynamo;

using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture]
    public class DynamoTests : CauldronBaseTest
    {
        protected const string BankHeist = "BankHeist";
        protected const string CatharticDemolition = "CatharticDemolition";
        protected const string Copperhead = "Copperhead";
        protected const string CrimeSpree = "CrimeSpree";
        protected const string EnergyConversion = "EnergyConversion";
        protected const string HardenedCriminals = "HardenedCriminals";
        protected const string HelmetedCharge = "HelmetedCharge";
        protected const string HeresThePlan = "HeresThePlan";
        protected const string ImperviousAdvance = "ImperviousAdvance";
        protected const string KineticEnergyBeam = "KineticEnergyBeam";
        protected const string Python = "Python";
        protected const string SlipperyThief = "SlipperyThief";
        protected const string Stranglehold = "Stranglehold";
        protected const string TakeItOutside = "TakeItOutside";
        protected const string WantonDestruction = "WantonDestruction";

        [Test()]
        public void TestDynamo_Load()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(dynamo);
            Assert.IsInstanceOf(typeof(DynamoCharacterCardController), dynamo.CharacterCardController);

            foreach (Card card in dynamo.TurnTaker.GetAllCards())
            {
                CardController cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(60, dynamo.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestDynamo_Decklist()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");

            AssertHasKeyword("one-shot", new string[] { HelmetedCharge, HeresThePlan, ImperviousAdvance, KineticEnergyBeam, SlipperyThief, Stranglehold, TakeItOutside });

            AssertHasKeyword("ongoing", new string[] { BankHeist, CatharticDemolition, CrimeSpree, EnergyConversion, HardenedCriminals, WantonDestruction });

            AssertHasKeyword("plot", new string[] { BankHeist, CrimeSpree, WantonDestruction });

            AssertHasKeyword("rogue", new string[] { Copperhead, Python });

            AssertMaximumHitPoints(GetCard(Copperhead), 18);
            AssertMaximumHitPoints(GetCard(Python), 10);
        }

        [Test]
        public void TestDynamo_Front_Start_3H()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //At the start of the villain turn, discard the top card of the villain deck and {Dynamo} deals the hero target with the highest HP {H} energy damage.
            AssertNumberOfCardsInTrash(dynamo, 1);
            AssertHitPoints(haka.CharacterCard, haka.CharacterCard.MaximumHitPoints - 3 ?? 0);
        }

        [Test]
        public void TestDynamo_Front_Start_4H()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "Unity", "TheScholar", "Megalopolis");
            StartGame();

            //At the start of the villain turn, discard the top card of the villain deck and {Dynamo} deals the hero target with the highest HP {H} energy damage.
            AssertNumberOfCardsInTrash(dynamo, 1);
            AssertHitPoints(haka.CharacterCard, haka.CharacterCard.MaximumHitPoints - 4 ?? 0);
        }

        [Test]
        public void TestDynamo_Front_Start_5H()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "Unity", "Ra", "TheScholar", "Megalopolis");
            StartGame();

            //At the start of the villain turn, discard the top card of the villain deck and {Dynamo} deals the hero target with the highest HP {H} energy damage.
            AssertNumberOfCardsInTrash(dynamo, 1);
            AssertHitPoints(haka.CharacterCard, haka.CharacterCard.MaximumHitPoints - 5 ?? 0);
        }

        [Test]
        public void TestDynamo_Flip()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StackDeck(dynamo, new string[] { Copperhead, Python, BankHeist, CatharticDemolition, CrimeSpree, EnergyConversion, HardenedCriminals, WantonDestruction });
            StartGame();


            //Front: At the end of the villain turn, if there are at least 6 cards in the villain trash, flip {Dynamo}'s villain character card.
            AssertNumberOfCardsInTrash(dynamo, 1);
            GoToEndOfTurn(dynamo);
            AssertNotFlipped(dynamo);

            GoToEndOfTurn(dynamo);
            AssertNumberOfCardsInTrash(dynamo, 2);
            AssertNotFlipped(dynamo);

            GoToEndOfTurn(dynamo);
            AssertNumberOfCardsInTrash(dynamo, 3);
            AssertNotFlipped(dynamo);

            GoToEndOfTurn(dynamo);
            AssertNumberOfCardsInTrash(dynamo, 4);
            AssertNotFlipped(dynamo);

            GoToEndOfTurn(dynamo);
            AssertNumberOfCardsInTrash(dynamo, 5);
            AssertNotFlipped(dynamo);

            //Make sure game doesn't end
            SetHitPoints(new TurnTakerController[] { haka, bunker, scholar }, 40);

            //Back: When Dynamo flips to this side, play the top 2 cards of the villain deck. Then, shuffle the villain trash, put it on the bottom of the villain deck, and flip {Dynamo}'s villain character cards.
            GoToEndOfTurn(dynamo);
            AssertIsInPlay(HardenedCriminals, WantonDestruction);
            AssertNumberOfCardsInTrash(dynamo, 0);
            AssertNotFlipped(dynamo);
        }

        [Test]
        public void TestDynamo_Advanced()
        {
            SetupGameController(new string[] { "Cauldron.Dynamo", "Haka", "Unity", "TheScholar", "Megalopolis" }, true);
            StartGame();

            AssertNumberOfCardsInTrash(dynamo, 1);
            //Front-Advanced: Whenever a villain target enters play, discard the top card of the villain deck.
            Card copper = PlayCard(Copperhead);
            AssertNumberOfCardsInTrash(dynamo, 2);
            AssertIsInPlay(copper);

            DiscardTopCards(dynamo.TurnTaker.Deck, 10);

            StackDeck(HelmetedCharge, HardenedCriminals);
            QuickHPStorage(haka, unity, scholar);
            GoToEndOfTurn(dynamo);
            //Back-Advanced: Increase damage dealt by villain targets by 1.
            //Helmeted Charge: If Copperhead is in play, he deals each hero target 2 melee damage.

            //This damage is not increased because Villain has flipped back already
            //Copperhead: At the end of the villain turn, this card deals the 2 hero targets with the highest HP {H} melee damage each.
            QuickHPCheck(-6, -3, -6);
        }

        [Test]
        public void TestBankHeist_Discard()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            int dynamoTrash = dynamo.TurnTaker.Trash.NumberOfCards;
            PlayCard(BankHeist);

            //At the end of the villain turn, each player may discard 1 card. If fewer than {H - 1} cards were discarded this way, discard the top card of the villain deck.
            GoToEndOfTurn(dynamo);
            AssertNumberOfCardsInTrash(dynamo, dynamoTrash);
        }

        [Test]
        public void TestBankHeist_NoDiscard()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DecisionDoNotSelectCard = SelectionType.DiscardCard;
            int dynamoTrash = dynamo.TurnTaker.Trash.NumberOfCards;
            PlayCard(BankHeist);

            //At the end of the villain turn, each player may discard 1 card. If fewer than {H - 1} cards were discarded this way, discard the top card of the villain deck.
            GoToEndOfTurn(dynamo);
            AssertNumberOfCardsInTrash(dynamo, dynamoTrash + 1);
        }

        [Test]
        public void TestCatharticDemolition_1X()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card traffic = PlayCard("TrafficPileup");
            Card cat = PlayCard(CatharticDemolition);

            QuickHPStorage(haka.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, traffic);
            //At the start of the villain turn, destroy all Plot cards and this card.
            GoToStartOfTurn(dynamo);
            //When this card is destroyed, {Dynamo} deals each non-villain target X energy damage, where X is 2 times the number of villain cards destroyed this turn.
            //Dynamo deals highest H
            QuickHPCheck(-5, -2, -2, -2);
            AssertInTrash(cat);
        }

        [Test]
        public void TestCatharticDemolition_2X()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card traffic = PlayCard("TrafficPileup");
            Card cat = PlayCard(CatharticDemolition);
            GoToEndOfTurn(env);
            Card bank = PlayCard(BankHeist);

            QuickHPStorage(haka.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, traffic);
            //At the start of the villain turn, destroy all Plot cards and this card.
            GoToStartOfTurn(dynamo);
            //When this card is destroyed, {Dynamo} deals each non-villain target X energy damage, where X is 2 times the number of villain cards destroyed this turn.
            //Dynamo deals highest H
            QuickHPCheck(-7, -4, -4, -4);
            AssertInTrash(cat, bank);
        }

        [Test]
        public void TestCatharticDemolition_3X()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card traffic = PlayCard("TrafficPileup");
            Card cat = PlayCard(CatharticDemolition);
            GoToEndOfTurn(env);
            Card bank = PlayCard(BankHeist);
            Card crime = PlayCard(CrimeSpree);

            QuickHPStorage(haka.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, traffic);
            //At the start of the villain turn, destroy all Plot cards and this card.
            GoToStartOfTurn(dynamo);
            //When this card is destroyed, {Dynamo} deals each non-villain target X energy damage, where X is 2 times the number of villain cards destroyed this turn.
            //Dynamo deals highest H
            QuickHPCheck(-9, -6, -6, -6);
            AssertInTrash(cat, bank, crime);
        }

        [Test]
        public void TestCopperhead()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(dynamo);
            Card cop = PlayCard(Copperhead);

            //At the end of the villain turn, this card deals the 2 hero targets with the highest HP {H} melee damage each.
            QuickHPStorage(haka, bunker, scholar);
            GoToEndOfTurn(dynamo);
            QuickHPCheck(-3, 0, -3);

            //If this card has 10 or fewer HP, increase damage it deals by 2.
            SetHitPoints(cop, 10);
            QuickHPStorage(cop);
            DealDamage(cop, cop, 2, DamageType.Melee);
            QuickHPCheck(-4);

            QuickHPStorage(haka);
            DealDamage(cop, haka, 2, DamageType.Melee);
            QuickHPCheck(-4);
        }

        [Test]
        public void TestCrimeSpree_NoPlay()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Card traffic = StackDeck("TrafficPileup");

            PlayCard(CrimeSpree);
            int dynamoTrash = dynamo.TurnTaker.Trash.NumberOfCards;

            //At the end of the villain turn, the players may choose to play the top card of the environment deck. If they do not, discard the top card of the villain deck and {Dynamo} deals each hero target 1 energy damage.
            QuickHPStorage(haka, bunker, scholar);
            GoToEndOfTurn(dynamo);
            AssertNumberOfCardsInTrash(dynamo, dynamoTrash + 1);
            QuickHPCheck(-1, -1, -1);
            AssertOnTopOfDeck(traffic);
        }

        [Test]
        public void TestCrimeSpree_Play()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Card traffic = StackDeck("TrafficPileup");

            PlayCard(CrimeSpree);
            DecisionYesNo = true;
            int dynamoTrash = dynamo.TurnTaker.Trash.NumberOfCards;

            //At the end of the villain turn, the players may choose to play the top card of the environment deck. If they do not, discard the top card of the villain deck and {Dynamo} deals each hero target 1 energy damage.
            QuickHPStorage(haka, bunker, scholar);
            GoToEndOfTurn(dynamo);
            AssertNumberOfCardsInTrash(dynamo, dynamoTrash);
            QuickHPCheckZero();
            AssertIsInPlay(traffic);
        }

        [Test]
        public void TestEnergyConversion()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            int dynamoTrash = dynamo.TurnTaker.Trash.NumberOfCards;

            //When this card enters play, discard the top card of the villain deck.
            Card energy = PlayCard(EnergyConversion);
            AssertNumberOfCardsInTrash(dynamo, dynamoTrash + 1);
            dynamoTrash = dynamo.TurnTaker.Trash.NumberOfCards;

            //When {Dynamo} is dealt 4 or more damage from a single source, discard the top card of the villain deck and {Dynamo} deals each hero target {H} energy damage. Then, destroy this card.

            //needs to be 4 or more
            QuickHPStorage(haka, bunker, scholar);
            DealDamage(haka, dynamo, 2, DamageType.Melee);
            AssertNumberOfCardsInTrash(dynamo, dynamoTrash);
            QuickHPCheckZero();
            AssertIsInPlay(energy);

            //needs to be all at once
            QuickHPStorage(haka, bunker, scholar);
            DealDamage(haka, dynamo, 2, DamageType.Melee);
            AssertNumberOfCardsInTrash(dynamo, dynamoTrash);
            QuickHPCheckZero();
            AssertIsInPlay(energy);

            //4 or more
            QuickHPStorage(haka, bunker, scholar);
            DealDamage(haka, dynamo, 4, DamageType.Melee);
            //Discard and destroy
            AssertNumberOfCardsInTrash(dynamo, dynamoTrash + 2);
            QuickHPCheck(-3, -3, -3);
            AssertInTrash(energy);
        }

        [Test]
        public void TestHardenedCriminals()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card pyt = PlayCard(Python);
            Card cop = PlayCard(Copperhead);

            //Reduce damage dealt to villain targets by 1.
            PlayCard(HardenedCriminals);

            QuickHPStorage(dynamo.CharacterCard, pyt, cop);
            DealDamage(haka, dynamo, 2, DamageType.Melee);
            DealDamage(bunker, pyt, 2, DamageType.Melee);
            DealDamage(pyt, cop, 2, DamageType.Melee);
            QuickHPCheck(-1, -1, -1);
        }

        [Test]
        public void TestHelmetedCharge_InDeck()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card cop = GetCard(Copperhead);

            QuickHPStorage(haka, bunker, scholar);
            PlayCard(HelmetedCharge);
            //If Copperhead is in play, he deals each hero target 2 melee damage.
            QuickHPCheckZero();

            //Otherwise, seach the villain deck and trash for Copperhead and put him into play. If you searched the villain deck, shuffle it.
            AssertIsInPlay(cop);
        }

        [Test]
        public void TestHelmetedCharge_InTrash()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card cop = PutInTrash(Copperhead);

            QuickHPStorage(haka, bunker, scholar);
            PlayCard(HelmetedCharge);
            //If Copperhead is in play, he deals each hero target 2 melee damage.
            QuickHPCheckZero();

            //Otherwise, seach the villain deck and trash for Copperhead and put him into play. If you searched the villain deck, shuffle it.
            AssertIsInPlay(cop);
        }

        [Test]
        public void TestHelmetedCharge_InPlay()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card cop = PlayCard(Copperhead);

            QuickHPStorage(haka, bunker, scholar);
            PlayCard(HelmetedCharge);
            //If Copperhead is in play, he deals each hero target 2 melee damage.
            QuickHPCheck(-2, -2, -2);
        }

        [Test]
        public void TestHeresThePlan()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            QuickHPStorage(haka, bunker, scholar);
            PlayCard(HeresThePlan);

            //Reveal cards from the top of the villain deck until a Plot is revealed. Put it into play and shuffle the other revealed cards back into the villain deck.
            AssertNumberOfCardsInPlay((Card c) => c.DoKeywordsContain("plot"), 1);

            //The villain target with the highest HP deals each hero target {H - 1} melee damage.
            QuickHPCheck(-2, -2, -2);
        }

        [Test]
        public void TestHeresThePlan_DynamoNotHighest()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            AddCannotDealNextDamageTrigger(dynamo, dynamo.CharacterCard);
            SetHitPoints(dynamo, 1);
            PlayCard(Copperhead);

            QuickHPStorage(haka, bunker, scholar);
            PlayCard(HeresThePlan);

            //Reveal cards from the top of the villain deck until a Plot is revealed. Put it into play and shuffle the other revealed cards back into the villain deck.
            AssertNumberOfCardsInPlay((Card c) => c.DoKeywordsContain("plot"), 1);

            //The villain target with the highest HP deals each hero target {H - 1} melee damage.
            QuickHPCheck(-2, -2, -2);
        }

        [Test]
        public void TestImperviousAdvance_NoCopperhead()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //The villain target with the highest HP deals the hero target with the second highest HP {H} melee damage
            QuickHPStorage(haka, bunker, scholar);
            PlayCard(ImperviousAdvance);
            QuickHPCheck(0, 0, -3);

            //If Copperhead is in play, reduce damage dealt to villain targets by 1 until the start of the next villain turn.
            QuickHPStorage(dynamo);
            DealDamage(haka, dynamo, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test]
        public void TestImperviousAdvance_Copperhead()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            AddCannotDealNextDamageTrigger(dynamo, dynamo.CharacterCard);
            SetHitPoints(dynamo, 6);
            Card cop = PlayCard(Copperhead);

            //The villain target with the highest HP deals the hero target with the second highest HP {H} melee damage
            QuickHPStorage(haka, bunker, scholar);
            PlayCard(ImperviousAdvance);
            QuickHPCheck(0, 0, -3);

            //If Copperhead is in play, reduce damage dealt to villain targets by 1 until the start of the next villain turn.
            QuickHPStorage(dynamo.CharacterCard, cop);
            DealDamage(haka, cop, 2, DamageType.Melee);
            DealDamage(haka, dynamo, 2, DamageType.Melee);
            QuickHPCheck(-1, -1);

            GoToStartOfTurn(dynamo);
            QuickHPStorage(dynamo.CharacterCard, cop);
            DealDamage(haka, cop, 2, DamageType.Melee);
            DealDamage(haka, dynamo, 2, DamageType.Melee);
            QuickHPCheck(-2, -2);
        }

        [Test]
        public void TestKineticEnergyBeam()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card traffic = PlayCard("TrafficPileup");

            //{Dynamo} deals the hero target with the second highest HP {H} energy damage.
            QuickHPStorage(haka, bunker, scholar);
            PlayCard(KineticEnergyBeam);
            QuickHPCheck(0, 0, -3);

            //Increase damage dealt to that target by environment cards by 1 until the start of the next villain turn.
            QuickHPStorage(haka, scholar);
            //only environment
            DealDamage(dynamo, scholar, 2, DamageType.Melee);
            //only target
            DealDamage(traffic, haka, 2, DamageType.Melee);
            DealDamage(traffic, scholar, 2, DamageType.Melee);
            QuickHPCheck(-2, -5);

            GoToStartOfTurn(dynamo);
            QuickHPStorage(scholar);
            DealDamage(traffic, scholar, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test]
        public void TestPython()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card pyt = PlayCard(Python);

            //The first time a hero target deals damage to this card each turn, reduce damage dealt by that target by 1 until the start of the next villain turn.
            DealDamage(haka, pyt, 1, DamageType.Melee);

            QuickHPStorage(dynamo.CharacterCard, pyt);
            DealDamage(haka, dynamo, 2, DamageType.Melee);
            DealDamage(bunker, pyt, 2, DamageType.Melee);
            QuickHPCheck(-1, -2);

            QuickHPStorage(dynamo.CharacterCard, pyt);
            DealDamage(bunker, pyt, 2, DamageType.Melee);
            QuickHPCheck(0, -1);

            //Whenever a One-shot enters the villain trash, this card deals the 2 hero targets with the lowest HP {H - 2} toxic damage each.
            QuickHPStorage(haka, bunker, scholar);
            PutInTrash(SlipperyThief);
            QuickHPCheck(0, -1, -1);
        }

        [Test]
        public void TestSlipperyThief_NoPython()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            QuickHPStorage(haka, bunker, scholar);
            //If Python is in play, he deals each hero target 1 toxic damage, regains {H} HP, and discards the top card of the villain deck.
            //The villain target with the lowest HP deals the hero target with the lowest HP {H - 2} melee damage.
            PlayCard(SlipperyThief);
            QuickHPCheck(0, -1, 0);
        }

        [Test]
        public void TestSlipperyThief_Python()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            StackDeck(HardenedCriminals);

            AddCannotDealNextDamageTrigger(dynamo, dynamo.CharacterCard);
            Card pyt = PlayCard(Python);
            SetHitPoints(pyt, 3);
            int dynamoTrash = dynamo.TurnTaker.Trash.NumberOfCards;

            QuickHPStorage(haka.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, pyt);
            //If Python is in play, he deals each hero target 1 toxic damage, regains {H} HP, and discards the top card of the villain deck.
            //The villain target with the lowest HP deals the hero target with the lowest HP {H - 2} melee damage.
            PlayCard(SlipperyThief);

            //Python hits all for 1 becasue he's in play
            //Slippery Thief hits lowest (Bunker) for 1
            //Because a One-Shot entered the trash Python's card deals 1 to Bunker and Scholar
            QuickHPCheck(-1, -3, -2, 3);

            //card played and top card discarded
            AssertNumberOfCardsInTrash(dynamo, dynamoTrash + 2);
        }

        [Test]
        public void TestStranglehold_InDeck()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card mere = PlayCard("Mere");
            Card moko = PlayCard("TaMoko");
            Card flak = PlayCard("FlakCannon");

            PlayCard(Stranglehold);
            //If Python is in play, destroy {H} hero ongoing and/or equipment cards.
            AssertIsInPlay(flak, moko, mere);
            //Otherwise, search the villain deck and trash for Python and put him into play. If you searched the villain deck, shuffle it.
            AssertIsInPlay(Python);
        }

        [Test]
        public void TestStranglehold_InTrash()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card mere = PlayCard("Mere");
            Card moko = PlayCard("TaMoko");
            Card flak = PlayCard("FlakCannon");

            Card pyt = PutInTrash(Python);

            PlayCard(Stranglehold);
            //If Python is in play, destroy {H} hero ongoing and/or equipment cards.
            AssertIsInPlay(flak, moko, mere);
            //Otherwise, search the villain deck and trash for Python and put him into play. If you searched the villain deck, shuffle it.
            AssertIsInPlay(pyt);
        }

        [Test]
        public void TestStranglehold_InPlay()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card mere = PlayCard("Mere");
            Card moko = PlayCard("TaMoko");
            Card flak = PlayCard("FlakCannon");

            PlayCard(Python);

            PlayCard(Stranglehold);
            //If Python is in play, destroy {H} hero ongoing and/or equipment cards.
            AssertInTrash(flak, moko, mere);
        }

        [Test]
        public void TestTakeItOutside_TakesDamage()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card traffic = PlayCard("TrafficPileup");

            QuickHPStorage(haka, bunker, scholar);
            PlayCard(TakeItOutside);
            //{Dynamo} deals the hero target with the highest HP 5 energy damage. If a hero target takes damage this way, destroy 1 environment card.
            AssertInTrash(traffic);
            //{Dynamo} deals each other hero target 1 sonic damage.
            QuickHPCheck(-5, -1, -1);
        }

        [Test]
        public void TestTakeItOutside_NoTakeDamage()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card traffic = PlayCard("TrafficPileup");
            SetHitPoints(new TurnTakerController[] { haka, bunker, scholar }, 17);

            AddCannotDealNextDamageTrigger(dynamo, dynamo.CharacterCard);
            QuickHPStorage(haka, bunker, scholar);
            PlayCard(TakeItOutside);
            //{Dynamo} deals the hero target with the highest HP 5 energy damage. If a hero target takes damage this way, destroy 1 environment card.
            AssertIsInPlay(traffic);
            //{Dynamo} deals each other hero target 1 sonic damage.
            QuickHPCheck(0, -1, -1);
        }

        [Test]
        public void TestWantonDestruction_NoDestroy()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            int dynamoTrash = dynamo.TurnTaker.Trash.NumberOfCards;
            PlayCard(WantonDestruction);

            //At the end of the villain turn, each player may destroy 1 of their non-character cards. If fewer than {H - 1} cards were destroyed this way, discard the top card of the villain deck.
            GoToEndOfTurn(dynamo);
            AssertNumberOfCardsInTrash(dynamo, dynamoTrash + 1);
        }

        [Test]
        public void TestWantonDestruction_Destroy()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            int dynamoTrash = dynamo.TurnTaker.Trash.NumberOfCards;
            PlayCard(WantonDestruction);

            Card mere = PlayCard("Mere");
            Card flak = PlayCard("FlakCannon");
            Card iron = PlayCard("FleshToIron");

            //At the end of the villain turn, each player may destroy 1 of their non-character cards. If fewer than {H - 1} cards were destroyed this way, discard the top card of the villain deck.
            GoToEndOfTurn(dynamo);
            AssertNumberOfCardsInTrash(dynamo, dynamoTrash);
            AssertInTrash(mere, flak, iron);
        }

        [Test]
        public void TestWantonDestruction_OnlyDestroy1PerHero()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            int dynamoTrash = dynamo.TurnTaker.Trash.NumberOfCards;
            PlayCard(WantonDestruction);

            Card mere = PlayCard("Mere");
            Card moko = PlayCard("TaMoko");

            //At the end of the villain turn, each player may destroy 1 of their non-character cards. If fewer than {H - 1} cards were destroyed this way, discard the top card of the villain deck.
            GoToEndOfTurn(dynamo);
            AssertNumberOfCardsInTrash(dynamo, dynamoTrash + 1);
            AssertInTrash(mere);
            AssertIsInPlay(moko);
        }
    }
}
