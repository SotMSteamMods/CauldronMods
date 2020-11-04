using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Baccarat;

namespace MyModTest
{
    [TestFixture()]
    public class BaccaratTests : BaseTest
    {
        #region BaccaratHelperFunctions
        protected HeroTurnTakerController baccarat { get { return FindHero("Baccarat"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(baccarat.CharacterCard, 1);
            DealDamage(villain, baccarat, 2, DamageType.Melee);
        }

        #endregion BaccaratHelperFunctions

        [Test()]
        public void LoadTestBaccarat()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(baccarat);
            Assert.IsInstanceOf(typeof(BaccaratCharacterCardController), baccarat.CharacterCardController);

            Assert.AreEqual(27, baccarat.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestBaccaratInnatePowerOption1()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");
            StartGame();
            //Discard the top card of your deck...
            GoToUsePowerPhase(baccarat);
            DecisionSelectFunction = 0;
            UsePower(baccarat.CharacterCard);
            Assert.AreEqual(1, GetNumberOfCardsInTrash(baccarat));
        }

        [Test()]
        public void TestBaccaratInnatePowerOption2()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");
            StartGame();
            DiscardTopCards(baccarat, 36);
            //...or put up to 2 trick cards with the same name from your trash into play.
            DecisionSelectFunction = 1;
            GoToUsePowerPhase(baccarat);
            UsePower(baccarat.CharacterCard);
            Assert.IsTrue(false);
            PrintJournal();
        }

        [Test()]
        public void TestBaccaratIncap1Hero()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);
            DiscardTopCards(legacy, 4);
            DiscardTopCards(baron, 4);
            DiscardTopCards(env, 4);

            //Put 2 cards from a trash on the bottom of their deck.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 0);
            PrintJournal();
        }

        [Test()]
        public void TestBaccaratIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //Increase the next damage dealt by a hero target by 2.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 1);

            QuickHPStorage(mdp);
            DealDamage(legacy, mdp, 2, DamageType.Melee);
            QuickHPCheck(-4);

            QuickHPStorage(mdp);
            DealDamage(legacy, mdp, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestBaccaratIncap3Yes()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);

            //Each hero character may deal themselves 3 toxic damage to use a power now.
            QuickHandStorage(bunker);
            QuickHPStorage(bunker);

            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 2);

            QuickHandCheck(1);
            QuickHPCheck(-3);

            PrintJournal();
        }

        [Test()]
        public void TestBaccaratIncap3No()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);

            //Each hero character may deal themselves 3 toxic damage to use a power now.
            QuickHandStorage(bunker);
            QuickHPStorage(bunker);
            //DecisionSelectCard = 2;
            //DecisionSelectFunction = 1;
            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 2);
            QuickHandCheck(0);
            QuickHPCheck(0);

            PrintJournal();
        }

        [Test()]
        public void TestAbyssalSolitaireBeforeNextStart()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");
            StartGame();
            Card abyssal = GetCard("AbyssalSolitaire");

            //Until the start of your next turn, reduce damage dealt to {Baccarat} by 1.
            QuickHPStorage(baccarat);
            GoToPlayCardPhase(baccarat);
            PlayCard(abyssal);
            GoToEndOfTurn(baccarat);
            DealDamage(baron, baccarat, 2, DamageType.Melee);
            QuickHPCheck(-1);
            PrintJournal();
        }

        [Test()]
        public void TestAbyssalSolitaireAfterNextStart()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");
            StartGame();
            Card abyssal = GetCard("AbyssalSolitaire");

            QuickHPStorage(baccarat);

            //Until the start of your next turn, reduce damage dealt to {Baccarat} by 1.
            PlayCard(abyssal);
            GoToStartOfTurn(baccarat);
            DealDamage(baron, baccarat, 2, DamageType.Melee);

            QuickHPCheck(-2);

            PrintJournal();
        }

        [Test()]
        public void TestAceInTheHolePlayCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");
            StartGame();
            Card ace = GetCard("AceInTheHole");
            Card saint = GetCard("AceOfSaints");
            PutInHand(baccarat, saint);
            DecisionSelectCard = saint;

            //You may play a card.
            QuickHandStorage(baccarat);
            PlayCard(ace);
            QuickHandCheck(-1);
        }

        [Test()]
        public void TestAceInTheHoleDontPlayCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");
            StartGame();
            Card ace = GetCard("AceInTheHole");

            DecisionDoNotSelectCard = SelectionType.PlayCard;

            //You may play a card.
            QuickHandStorage(baccarat);
            PlayCard(ace);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestAceInTheHoleTwoPowerPhase()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");
            StartGame();
            Card ace = GetCard("AceInTheHole");
            Card saint = GetCard("AceOfSaints");
            PutInHand(baccarat, saint);
            DecisionSelectCard = saint;

            //You may use {Baccarat}'s innate power twice during your phase this turn.
            GoToPlayCardPhase(baccarat);
            AssertNumberOfUsablePowers(baccarat, 1);
            PlayCard(ace);
            GoToUsePowerPhase(baccarat);
            UsePower(baccarat);
            AssertNumberOfUsablePowers(baccarat, 1);
            UsePower(baccarat);
            AssertNumberOfUsablePowers(baccarat, 0);
        }

        [Test()]
        public void TestAceOfSaintsReduceDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat","Bunker", "Megalopolis");
            StartGame();
            Card saint = GetCard("AceOfSaints");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //Reduce damage dealt to hero targets by 1.
            QuickHPStorage(baccarat);
            PlayCard(saint);
            DealDamage(baron, baccarat, 2, DamageType.Melee);
            QuickHPCheck(-1);

            QuickHPStorage(bunker);
            DealDamage(baron, bunker, 2, DamageType.Melee);
            QuickHPCheck(-1);

            //not villain targetrs
            QuickHPStorage(mdp);
            DealDamage(baccarat, mdp, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestAceOfSaintsDestroySelf()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");
            StartGame();
            Card saint = GetCard("AceOfSaints");

            //...or this card is destroyed.
            GoToPlayCardPhase(baccarat);
            PlayCard(saint);
            AssertNumberOfCardsInPlay(baccarat, 2);
            GoToStartOfTurn(baccarat);
            AssertNumberOfCardsInPlay(baccarat, 1);
        }

        [Test()]
        public void TestAceOfSaintsShuffleSame2Cards()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");
            StartGame();
            Card saint = GetCard("AceOfSaints");
            DiscardTopCards(baccarat, 36);

            //At the start of your turn, shuffle 2 cards with the same name from your trash into your deck...
            GoToPlayCardPhase(baccarat);
            PlayCard(saint);
            AssertNumberOfCardsInPlay(baccarat, 2);
            GoToStartOfTurn(baccarat);
            AssertNumberOfCardsInPlay(baccarat, 2);
            AssertNumberOfCardsInTrash(baccarat, 34);

            Assert.IsFalse(true);
        }

        [Test()]
        public void TestAceOfSinnersIncreaseDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat","Bunker", "Megalopolis");
            StartGame();
            Card sinner = GetCard("AceOfSinners");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //Increase damage dealt by hero targets by 1.
            PlayCard(sinner);

            QuickHPStorage(mdp);
            DealDamage(baccarat, mdp, 2, DamageType.Melee);
            QuickHPCheck(-3);

            QuickHPStorage(mdp);
            DealDamage(bunker, mdp, 2, DamageType.Melee);
            QuickHPCheck(-3);

            //Not villain damage
            QuickHPStorage(baccarat);
            DealDamage(mdp, baccarat, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestAceOfSinnersDestroySelf()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");
            StartGame();
            Card sinner = GetCard("AceOfSinners");

            //...or this card is destroyed.
            GoToPlayCardPhase(baccarat);
            PlayCard(sinner);
            AssertNumberOfCardsInPlay(baccarat, 2);
            GoToStartOfTurn(baccarat);
            AssertNumberOfCardsInPlay(baccarat, 1);
        }

        [Test()]
        public void TestAceOfSinnersShuffleSame2Cards()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");
            StartGame();
            Card sinner = GetCard("AceOfSinners");
            DiscardTopCards(baccarat, 36);

            //At the start of your turn, shuffle 2 cards with the same name from your trash into your deck...
            GoToPlayCardPhase(baccarat);
            PlayCard(sinner);
            AssertNumberOfCardsInPlay(baccarat, 2);
            GoToStartOfTurn(baccarat);
            AssertNumberOfCardsInPlay(baccarat, 2);
            AssertNumberOfCardsInTrash(baccarat, 34);

            Assert.IsFalse(true);
        }

        [Test()]
        public void TestAfterlifeEuchreIncreaseDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");
            StartGame();
            Card euchre = GetCard("AfterlifeEuchre");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //Increase the next damage dealt by {Baccarat} by 1,
            PlayCard(euchre);
            QuickHPStorage(mdp);
            DealDamage(baccarat, mdp, 2, DamageType.Melee);
            QuickHPCheck(-3);

            QuickHPStorage(mdp);
            DealDamage(baccarat, mdp, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestAfterlifeEuchreDealDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");
            StartGame();
            Card euchre = GetCard("AfterlifeEuchre");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectFunction = 1;
            DecisionSelectTarget = mdp;

            //{Baccarat} deals 1 target 2 toxic damage
            QuickHPStorage(mdp);
            PlayCard(euchre);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestAllInDiscard()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Megalopolis");
            StartGame();
            Card allin = GetCard("AllIn");

            QuickHandStorage(baccarat);
            PlayCard(allin);
            QuickHandCheck(-1);
        }

        [Test()]
        public void TestAllInDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Bunker", "Megalopolis");
            StartGame();
            Card allin = GetCard("AllIn");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card battalion = GetCard("BladeBattalion");

            PlayCard(battalion);
            //...{Baccarat} deals each non-hero target 1 infernal damage and 1 radiant damage.
            int bunkerHP = GetHitPoints(bunker);
            int mdpHP = GetHitPoints(mdp);
            QuickHPStorage(battalion);
            PlayCard(allin);
            QuickHPCheck(-2);
            Assert.AreEqual(mdpHP - 2, GetHitPoints(mdp));
            Assert.AreEqual(bunkerHP, GetHitPoints(bunker));
        }

        [Test()]
        public void TestBringDownTheHouseShufflePair()
        {
            //Shuffle any number of pairs of cards with the same name from your trash into your deck.
            Assert.IsTrue(false);
        }

        [Test()]
        public void TestBringDownTheHouseDontShuffle()
        {
            //Shuffle any number of pairs of cards with the same name from your trash into your deck.
            Assert.IsTrue(false);
        }

        [Test()]
        public void TestBringDownTheHouseShuffle3Pairs()
        {
            //Shuffle any number of pairs of cards with the same name from your trash into your deck.
            Assert.IsTrue(false);
        }

        [Test()]
        public void TestBringDownTheHouseDestroy0Cards()
        {
            //You may destroy up to X ongoing or environment cards, where X is the number of pairs you shuffled this way.
            Assert.IsTrue(false);
        }

        [Test()]
        public void TestBringDownTheHouseDestroy1Ongoing()
        {
            //You may destroy up to X ongoing or environment cards, where X is the number of pairs you shuffled this way.
            Assert.IsTrue(false);
        }

        [Test()]
        public void TestBringDownTheHouseDestroy1Environment()
        {
            //You may destroy up to X ongoing or environment cards, where X is the number of pairs you shuffled this way.
            Assert.IsTrue(false);
        }

        [Test()]
        public void TestBringDownTheHouseDestroy3Cards()
        {
            //You may destroy up to X ongoing or environment cards, where X is the number of pairs you shuffled this way.
            Assert.IsTrue(false);
        }

        [Test()]
        public void TestCardTossDealDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Bunker", "Megalopolis");
            StartGame();
            Card toss = GetCard("CardToss");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectTarget = mdp;

            //{Baccarat} deals 1 target 1 projectile damage.
            QuickHPStorage(mdp);
            PlayCard(toss);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestCardTossPlayCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Bunker", "Megalopolis");
            StartGame();
            Card toss = GetCard("CardToss");
            Card saint = GetCard("AceOfSaints");
            PutInHand(baccarat, saint);
            DecisionSelectCard = saint;

            //You may play a card.
            QuickHandStorage(baccarat);
            PlayCard(toss);
            QuickHandCheck(-1);
        }

        [Test()]
        public void TestCardTossDontPlayCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Bunker", "Megalopolis");
            StartGame();
            Card toss = GetCard("CardToss");

            DecisionDoNotSelectCard = SelectionType.PlayCard;

            //You may play a card.
            QuickHandStorage(baccarat);
            PlayCard(toss);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestCheapTrick()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Bunker", "Megalopolis");
            StartGame();
            Card cheap = GetCard("CheapTrick");
            Card abyssal = GetCard("AbyssalSolitaire");
            Card saint = GetCard("AceOfSaints");
            Card euchre = GetCard("AfterlifeEuchre");
            List<Card> list = new List<Card>() { abyssal, saint, euchre };
            int tricks = 0;
            int cheaps = 0;

            PutOnDeck(baccarat, list);
            PlayCard(cheap);
            //Discard the top card of your deck.
            //Reveal cards from the top of your deck until you reveal a trick. Shuffle the other cards back into your deck and put the trick into play.
            AssertNumberOfCardsInTrash(baccarat, 3);
            foreach(Card c in baccarat.TurnTaker.Trash.Cards)
            {
                if (c.DoKeywordsContain("trick"))
                {
                    tricks++;
                }
                else if (c.Identifier == "CheapTrick")
                {
                    cheaps++;
                }
            }
            Assert.IsTrue(cheaps == 1 && tricks == 2);
        }

        [Test()]
        public void TestGraveyardBridge()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Bunker", "Megalopolis");
            StartGame();
            DiscardTopCards(baccarat.TurnTaker.Deck, 36);
            DiscardAllCards(baccarat);
            Card abyssal = GetCard("AbyssalSolitaire");
            Card bridge = GetCard("GraveyardBridge");
            PutInHand(bridge);
            DecisionSelectCard = abyssal;

            AssertNumberOfCardsInTrash(baccarat, 39);
            PlayCard(bridge);
            AssertNumberOfCardsInTrash(baccarat, 37);

            QuickHPStorage(baccarat);
            DealDamage(baron, baccarat, 2, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestIFold()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Bunker", "Megalopolis");
            StartGame();
            Card fold = GetCard("IFold");
            PutInHand(fold);
            PlayCard(fold);

            //Discard your hand and draw 3 cards.
            AssertNumberOfCardsInHand(baccarat, 3);
            AssertNumberOfCardsInTrash(baccarat, 5);
        }

        [Test()]
        public void TestUnderworldHoldEmSelfDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Bunker", "Megalopolis");
            StartGame();
            Card hold = GetCard("UnderworldHoldEm");
            PutInHand(hold);

            //One player may draw a card.
            QuickHandStorage(baccarat);
            PlayCard(hold);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestUnderworldHoldEmOtherDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Bunker", "Megalopolis");
            StartGame();
            Card hold = GetCard("UnderworldHoldEm");
            PutInHand(hold);
            DecisionSelectTurnTaker = bunker.TurnTaker;

            //One player may draw a card.
            QuickHandStorage(bunker);
            PlayCard(hold);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestUnderworldHoldEmDontDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Bunker", "Megalopolis");
            StartGame();
            Card hold = GetCard("UnderworldHoldEm");
            PutInHand(hold);
            DecisionDoNotSelectTurnTaker = true;

            //One player may draw a card.
            int baccaratCards = baccarat.NumberOfCardsInHand;
            QuickHandStorage(bunker);
            PlayCard(hold);
            QuickHandCheck(0);
            Assert.AreEqual(baccaratCards, baccarat.NumberOfCardsInHand + 1);
        }
    }
}
