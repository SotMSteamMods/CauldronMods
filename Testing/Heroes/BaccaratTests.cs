using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Baccarat;

namespace CauldronTests
{
    [TestFixture()]
    public class BaccaratTests : CauldronBaseTest
    {
        #region BaccaratHelperFunctions
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(baccarat.CharacterCard, 1);
            DealDamage(villain, baccarat, 2, DamageType.Melee);
        }

        #endregion BaccaratHelperFunctions

        [Test()]
        public void TestLoadBaccarat()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(baccarat);
            Assert.IsInstanceOf(typeof(BaccaratCharacterCardController), baccarat.CharacterCardController);

            Assert.AreEqual(27, baccarat.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestBaccaratInnatePowerOption1()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //Discard the top card of your deck...
            GoToUsePowerPhase(baccarat);
            DecisionSelectFunction = 0;
            UsePower(baccarat.CharacterCard);
            Assert.AreEqual(1, GetNumberOfCardsInTrash(baccarat));
        }

        [Test()]
        public void TestBaccaratInnatePowerOption2Play2()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Card hold1 = GetCard("UnderworldHoldEm", 1);
            Card hold2 = GetCard("UnderworldHoldEm", 2);
            Card saint = GetCard("AceOfSaints");
            IEnumerable<Card> trashCards = new Card[] { saint, hold1, hold2 };

            //...or put up to 2 trick cards with the same name from your trash into play.

            //In case any of these cards start in hand we want to count hand with them
            PutInHand(trashCards);
            QuickHandStorage(baccarat);
            //prep trash
            PutInTrash(trashCards);
            DecisionSelectFunction = 1;
            GoToUsePowerPhase(baccarat);

            //By discarding 3 cards then drawing 2 from the two Hold Em's net -1
            UsePower(baccarat.CharacterCard);
            QuickHandCheck(-1);
        }

        [Test()]
        public void TestBaccaratInnatePowerOption2Play0()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Card hold1 = GetCard("UnderworldHoldEm", 1);
            Card hold2 = GetCard("UnderworldHoldEm", 2);
            Card saint = GetCard("AceOfSaints");
            IEnumerable<Card> trashCards = new Card[] { saint, hold1, hold2 };

            //...or put up to 2 trick cards with the same name from your trash into play.

            //In case any of these cards start in hand we want to count hand with them
            PutInHand(trashCards);
            QuickHandStorage(baccarat);
            //prep trash
            PutInTrash(trashCards);
            DecisionSelectFunction = 1;
            DecisionDoNotSelectCard = SelectionType.PutIntoPlay;
            GoToUsePowerPhase(baccarat);

            //By discarding 3 cards then drawing 0 from the two Hold Em's net -3
            UsePower(baccarat.CharacterCard);
            QuickHandCheck(-3);
        }

        [Test()]
        public void TestBaccaratIncap1Hero()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);
            DiscardTopCards(legacy, 4);

            //Put 2 cards from a trash on the bottom of their deck.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 0);
            //Assert.AreEqual(2, GetNumberOfCardsInTrash(legacy));
            Assert.AreEqual(34, GetNumberOfCardsInDeck(legacy));
        }

        [Test()]
        public void TestBaccaratIncap1Villain()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);
            DiscardTopCards(baron, 4);

            //Put 2 cards from a trash on the bottom of their deck.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 0);
            //Assert.AreEqual(2, GetNumberOfCardsInTrash(baron));
            Assert.AreEqual(22, GetNumberOfCardsInDeck(baron));
        }

        [Test()]
        public void TestBaccaratIncap1Environment()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);
            DiscardTopCards(env, 4);

            //Put 2 cards from a trash on the bottom of their deck.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 0);
            Assert.AreEqual(2, GetNumberOfCardsInTrash(env));
            Assert.AreEqual(13, GetNumberOfCardsInDeck(env));
        }

        [Test()]
        public void TestBaccaratIncap1HeroAndEnvironment_ChooseHero()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);
            DiscardTopCards(env, 4);
            //grab the top 2 cards of the hero deck
            Card heroCard1 = GetTopCardOfDeck(legacy);
            Card heroCard2 = GetTopCardOfDeck(legacy, 1);
            DiscardTopCards(legacy, 4);
            AssertInTrash(heroCard1);
            AssertInTrash(heroCard2);
            //Put 2 cards from a trash on the bottom of their deck.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            //choose to move hero cards
            DecisionSelectCards = new Card[] { heroCard1, heroCard2 };
            UseIncapacitatedAbility(baccarat, 0);
            //assert that the hero cards were moved 
            AssertNotInTrash(heroCard1);
            AssertNotInTrash(heroCard2);
            AssertInDeck(heroCard1);
            AssertInDeck(heroCard2);
        }

        [Test()]
        public void TestBaccaratIncap1HeroAndEnvironment_ChooseEnv()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);
            //grab the top 2 cards of the hero deck
            Card envCard1 = GetTopCardOfDeck(env);
            Card envCard2 = GetTopCardOfDeck(env, 1);
            DiscardTopCards(env, 4);
            DiscardTopCards(legacy, 4);

            AssertInTrash(envCard1);
            AssertInTrash(envCard2);
            //Put 2 cards from a trash on the bottom of their deck.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            //choose to move env cards
            DecisionSelectCards = new Card[] { envCard1, envCard2 };
            UseIncapacitatedAbility(baccarat, 0);
            //assert that the env cards were moved 
            AssertNotInTrash(envCard1);
            AssertNotInTrash(envCard2);
            AssertInDeck(envCard1);
            AssertInDeck(envCard2);
        }

        [Test()]
        public void TestBaccaratIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
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
            int scholarHP = GetHitPoints(scholar);

            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 2);

            Assert.AreEqual(scholarHP - 2, GetHitPoints(scholar));
            QuickHandCheck(1);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestBaccaratIncap3No()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);

            DecisionDoNotSelectFunction = true;

            //Each hero character may deal themselves 3 toxic damage to use a power now.
            QuickHandStorage(bunker);
            QuickHPStorage(bunker);
            int scholarHP = GetHitPoints(scholar);

            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 2);

            Assert.AreEqual(scholarHP, GetHitPoints(scholar));
            QuickHandCheck(0);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestAbyssalSolitaireBeforeNextStart()
        {
            SetupGameController("BaronBlade", "Legacy", "Cauldron.Baccarat", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Card abyssal = GetCard("AbyssalSolitaire");

            //go to end of turn legacy to collect HP accurately
            GoToEndOfTurn(legacy);
            //Until the start of your next turn, reduce damage dealt to {Baccarat} by 1.
            QuickHPStorage(baccarat);
            GoToPlayCardPhase(baccarat);
            PlayCard(abyssal);
            GoToEndOfTurn(baccarat);
            DealDamage(baron, baccarat, 2, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestAbyssalSolitaireAfterNextStart()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Card abyssal = GetCard("AbyssalSolitaire");

            QuickHPStorage(baccarat);

            //Until the start of your next turn, reduce damage dealt to {Baccarat} by 1.
            PlayCard(abyssal);
            GoToStartOfTurn(baccarat);
            DealDamage(baron, baccarat, 2, DamageType.Melee);

            QuickHPCheck(-2);
        }

        [Test()]
        public void TestAceInTheHolePlayCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //put both saint and ace in hand to reduce variance
            Card ace = GetCard("AceInTheHole");
            Card saint = GetCard("AceOfSaints");
            PutInHand(baccarat, saint);
            PutInHand(baccarat, ace);
            DecisionSelectCard = saint;

            //You may play a card.
            QuickHandStorage(baccarat);
            PlayCard(ace);
            //should have 2 fewer cards in hand, 1 for ace being played and 1 for saint being played
            QuickHandCheck(-2);
            //check that saint has been played
            AssertIsInPlay(saint);
        }

        [Test()]
        public void TestAceInTheHoleDontPlayCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //put both ace and saint in hand to reduce variance
            Card ace = GetCard("AceInTheHole");
            Card saint = GetCard("AceOfSaints");
            PutInHand(baccarat, saint);
            PutInHand(baccarat, ace);
            DecisionSelectCard = saint;

            DecisionDoNotSelectCard = SelectionType.PlayCard;

            //You may play a card.
            QuickHandStorage(baccarat);
            PlayCard(ace);
            //should have 1 fewer card in hand, for ace being played
            QuickHandCheck(-1);
            //saint should be still in hand
            AssertInHand(saint);
        }

        [Test()]
        public void TestAceInTheHoleTwoPowerPhase()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
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
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
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

            //not villain targets
            QuickHPStorage(mdp);
            DealDamage(baccarat, mdp, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestAceOfSaintsDestroySelf()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Card saint = GetCard("AceOfSaints");

            //...or this card is destroyed.
            GoToPlayCardPhase(baccarat);
            PlayCard(saint);
            AssertNumberOfCardsInPlay(baccarat, 2);
            AssertIsInPlay(saint);
            GoToStartOfTurn(baccarat);
            AssertNumberOfCardsInPlay(baccarat, 1);
            AssertInTrash(saint);

        }

        [Test()]
        public void TestAceOfSaintsShuffleSame2Cards()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //put saint in hand
            Card saint = GetCard("AceOfSaints");
            PutInHand(saint);
            //discard the rest of bacarrat's deck to make sure there are pairs in the trash
            MoveAllCards(baccarat, baccarat.TurnTaker.Deck, baccarat.TurnTaker.Trash);


            GoToPlayCardPhase(baccarat);
            PlayCard(saint);
            //check that there are 2 cards in play, character card and saint
            AssertNumberOfCardsInPlay(baccarat, 2);
            //verify it is actually saint in play
            AssertIsInPlay(saint);
            //get the number of cards in the trash
            int trash = baccarat.TurnTaker.Trash.NumberOfCards;
            QuickShuffleStorage(baccarat);
            //At the start of your turn, shuffle 2 cards with the same name from your trash into your deck...
            GoToStartOfTurn(baccarat);
            QuickShuffleCheck(1);
            //verify that there are still 2 cards in play, character card and saint
            AssertNumberOfCardsInPlay(baccarat, 2);
            AssertIsInPlay(saint);
            //check that there are 2 fewer cards in the trash
            AssertNumberOfCardsInTrash(baccarat, trash - 2);
            AssertNumberOfCardsInDeck(baccarat, 2);
        }

        [Test()]
        public void TestAceOfSinnersIncreaseDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
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
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Card sinner = GetCard("AceOfSinners");

            //...or this card is destroyed.
            GoToPlayCardPhase(baccarat);
            PlayCard(sinner);
            AssertNumberOfCardsInPlay(baccarat, 2);
            AssertIsInPlay(sinner);
            GoToStartOfTurn(baccarat);
            AssertNumberOfCardsInPlay(baccarat, 1);
            AssertInTrash(sinner);
        }

        [Test()]
        public void TestAceOfSinnersShuffleSame2Cards()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //put sinner in hand
            Card sinner = GetCard("AceOfSinners");
            PutInHand(sinner);
            //discard the rest of bacarrat's deck to make sure there are pairs in the trash
            MoveAllCards(baccarat, baccarat.TurnTaker.Deck, baccarat.TurnTaker.Trash);


            GoToPlayCardPhase(baccarat);
            PlayCard(sinner);
            //check that there are 2 cards in play, character card and sinner
            AssertNumberOfCardsInPlay(baccarat, 2);
            //verify it is actually sinner in play
            AssertIsInPlay(sinner);
            //get the number of cards in the trash
            int trash = baccarat.TurnTaker.Trash.NumberOfCards;
            QuickShuffleStorage(baccarat);
            //At the start of your turn, shuffle 2 cards with the same name from your trash into your deck...
            GoToStartOfTurn(baccarat);
            QuickShuffleCheck(1);
            //verify that there are still 2 cards in play, character card and sinner
            AssertNumberOfCardsInPlay(baccarat, 2);
            AssertIsInPlay(sinner);
            //check that there are 2 fewer cards in the trash
            AssertNumberOfCardsInTrash(baccarat, trash - 2);
            AssertNumberOfCardsInDeck(baccarat, 2);
        }

        [Test()]
        public void TestAfterlifeEuchreIncreaseDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Card euchre = GetCard("AfterlifeEuchre");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectFunction = 0;
            //Increase the next damage dealt by {Baccarat} by 1,
            PlayCard(euchre);
            //Damage is only increased for Baccarat
            QuickHPStorage(baccarat);
            DealDamage(mdp, baccarat, 2, DamageType.Melee);
            QuickHPCheck(-2);
            //+1 damage
            QuickHPStorage(mdp);
            DealDamage(baccarat, mdp, 2, DamageType.Melee);
            QuickHPCheck(-3);
            //should only apply to next damage
            QuickHPStorage(mdp);
            DealDamage(baccarat, mdp, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestAfterlifeEuchreDealDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
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
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //put all in to hand to reduce variability
            Card allin = GetCard("AllIn");
            PutInHand(allin);

            QuickHandStorage(baccarat);
            PlayCard(allin);
            //should be 2 fewer cards in hand, one for discard one for playing all in
            QuickHandCheck(-2);
        }

        [Test()]
        public void TestAllInDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Card allin = GetCard("AllIn");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card battalion = GetCard("BladeBattalion");

            PlayCard(battalion);
            //...{Baccarat} deals each non-hero target 1 infernal damage and 1 radiant damage.

            QuickHPStorage(battalion, mdp, bunker.CharacterCard);
            PlayCard(allin);
            QuickHPCheck(-2, -2, 0);

        }

        [Test()]
        public void TestAllInDamageCantDiscard_NoDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //discard all cards to not have any in hand
            DiscardAllCards(baccarat);

            //put all in to hand
            Card allin = GetCard("AllIn");
            PutInHand(allin);


            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card battalion = GetCard("BladeBattalion");

            PlayCard(battalion);
            //...if you do {Baccarat} deals each non-hero target 1 infernal damage and 1 radiant damage.
            //since there are no cards in hand to discard, no damage should be dealt
            QuickHPStorage(battalion, mdp, bunker.CharacterCard);
            PlayCard(allin);
            QuickHPCheck(0, 0, 0);

        }

        [Test()]
        public void TestBringDownTheHouseShufflePair()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //If all or all but one copy of Cheap Trick is in hand, GetCard() will return the same copy regardless of index and cause the test to fail
            Card trick1 = GetCard("CheapTrick", 1);
            Card trick2 = GetCard("CheapTrick", 2);
            Card saint = GetCard("AceOfSaints");
            IEnumerable<Card> trashCards = new Card[] { trick1, trick2, saint };
            PutInTrash(trashCards);
            DecisionsYesNo = new bool[] { true };

            Card house = GetCard("BringDownTheHouse");
            //Shuffle any number of pairs of cards with the same name from your trash into your deck.
            PlayCard(house);
            AssertNumberOfCardsInTrash(baccarat, 2);
            AssertInTrash(saint);
            AssertInTrash(house);
            AssertInDeck(trick1);
            AssertInDeck(trick2);

        }

        [Test()]
        public void TestBringDownTheHouseDontShuffle()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Card trick1 = GetCard("CheapTrick", 1);
            Card trick2 = GetCard("CheapTrick", 2);
            Card toss1 = GetCard("CardToss", 1);
            Card toss2 = GetCard("CardToss", 2);
            Card ace1 = GetCard("AceInTheHole", 1);
            Card ace2 = GetCard("AceInTheHole", 2);
            Card saint = GetCard("AceOfSaints");
            IEnumerable<Card> trashCards = new Card[] { trick1, trick2, saint, toss1, toss2, ace1, ace2 };
            PutInTrash(trashCards);

            DecisionDoNotSelectCard = SelectionType.ShuffleCardFromTrashIntoDeck;

            Card house = GetCard("BringDownTheHouse");
            //Shuffle any number of pairs of cards with the same name from your trash into your deck.
            PlayCard(house);
            //7 cards already in the trash +1 = 8 cards in trash
            AssertNumberOfCardsInTrash(baccarat, 8);
        }

        [Test()]
        public void TestBringDownTheHouseShuffle3Pairs()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Card trick1 = GetCard("CheapTrick", 1);
            Card trick2 = GetCard("CheapTrick", 2);
            Card toss1 = GetCard("CardToss", 1);
            Card toss2 = GetCard("CardToss", 2);
            Card ace1 = GetCard("AceInTheHole", 1);
            Card ace2 = GetCard("AceInTheHole", 2);
            Card saint = GetCard("AceOfSaints");
            IEnumerable<Card> trashCards = new Card[] { trick1, trick2, saint, toss1, toss2, ace1, ace2 };
            PutInTrash(trashCards);

            DecisionsYesNo = new bool[] { true, true, true };
            QuickShuffleStorage(baccarat.TurnTaker.Deck);
            Card house = GetCard("BringDownTheHouse");
            //Shuffle any number of pairs of cards with the same name from your trash into your deck.
            PlayCard(house);
            QuickShuffleCheck(3);
            //should have 2 cards in the trash, bring down the house and saints
            AssertNumberOfCardsInTrash(baccarat, 2);
            AssertInTrash(saint);
            AssertInTrash(house);
            AssertInDeck(trick1);
            AssertInDeck(trick2);
            AssertInDeck(toss1);
            AssertInDeck(toss2);
            AssertInDeck(ace1);
            AssertInDeck(ace2);
        }

        [Test()]
        public void TestBringDownTheHouseDestroy0Cards()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //Setup Trash
            Card trick1 = GetCard("CheapTrick", 1);
            Card trick2 = GetCard("CheapTrick", 2);
            Card toss1 = GetCard("CardToss", 1);
            Card toss2 = GetCard("CardToss", 2);
            Card bridge1 = GetCard("GraveyardBridge", 1);
            Card bridge2 = GetCard("GraveyardBridge", 2);
            Card saint = GetCard("AceOfSaints");
            IEnumerable<Card> trashCards = new Card[] { trick1, trick2, saint, toss1, toss2, bridge1, bridge2 };
            PutInTrash(trashCards);

            //Setup In play
            Card field = GetCard("LivingForceField");
            PlayCard(field);

            //yes to move cards
            //no to destroy cards
            DecisionsYesNo = new bool[] { true, false };
            DecisionDoNotSelectCard = SelectionType.DestroyCard;

            Card house = GetCard("BringDownTheHouse");
            //Shuffle any number of pairs of cards with the same name from your trash into your deck.
            PlayCard(house);
            //no card should have been destroyed, thus field will still be in play
            AssertIsInPlay(field);
        }

        [Test()]
        public void TestBringDownTheHouseDestroy1Ongoing()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //Setup Trash
            Card trick1 = GetCard("CheapTrick", 1);
            Card trick2 = GetCard("CheapTrick", 2);
            Card toss1 = GetCard("CardToss", 1);
            Card toss2 = GetCard("CardToss", 2);
            Card bridge1 = GetCard("GraveyardBridge", 1);
            Card bridge2 = GetCard("GraveyardBridge", 2);
            Card saint = GetCard("AceOfSaints");
            IEnumerable<Card> trashCards = new Card[] { trick1, trick2, saint, toss1, toss2, bridge1, bridge2 };
            PutInTrash(trashCards);

            //Setup In play
            Card field = GetCard("LivingForceField");
            PlayCard(field);

            DecisionsYesNo = new bool[] { true, true };

            Card house = GetCard("BringDownTheHouse");
            //Shuffle any number of pairs of cards with the same name from your trash into your deck.
            PlayCard(house);
            AssertInTrash(field);
        }

        [Test()]
        public void TestBringDownTheHouseDestroy1Environment()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //Setup Trash
            Card trick1 = GetCard("CheapTrick", 1);
            Card trick2 = GetCard("CheapTrick", 2);
            Card toss1 = GetCard("CardToss", 1);
            Card toss2 = GetCard("CardToss", 2);
            Card bridge1 = GetCard("GraveyardBridge", 1);
            Card bridge2 = GetCard("GraveyardBridge", 2);
            Card saint = GetCard("AceOfSaints");
            IEnumerable<Card> trashCards = new Card[] { trick1, trick2, saint, toss1, toss2, bridge1, bridge2 };
            PutInTrash(trashCards);

            //Setup In play
            Card monorail = GetCard("PlummetingMonorail");
            PlayCard(monorail);

            DecisionsYesNo = new bool[] { true, true };

            Card house = GetCard("BringDownTheHouse");
            //Shuffle any number of pairs of cards with the same name from your trash into your deck.
            PlayCard(house);
            AssertInTrash(monorail);
        }

        [Test()]
        public void TestBringDownTheHouseDestroy2Cards()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //Setup Trash
            Card trick1 = GetCard("CheapTrick", 1);
            Card trick2 = GetCard("CheapTrick", 2);
            Card toss1 = GetCard("CardToss", 1);
            Card toss2 = GetCard("CardToss", 2);
            Card saint = GetCard("AceOfSaints");
            IEnumerable<Card> trashCards = new Card[] { trick1, trick2, saint, toss1, toss2 };
            PutInTrash(trashCards);

            //Setup In play
            Card field = GetCard("LivingForceField");
            Card monorail = GetCard("PlummetingMonorail");
            Card backlash = GetCard("BacklashField");
            Card police = GetCard("PoliceBackup");
            IEnumerable<Card> playCardsToDestroy = new Card[] { field, monorail, backlash };
            PlayCards(playCardsToDestroy);
            PlayCard(police);

            DecisionsYesNo = new bool[] { true, true };
            Card house = GetCard("BringDownTheHouse");
            //Shuffle any number of pairs of cards with the same name from your trash into your deck.
            PlayCard(house);

            //since only 2 pairs were shuffled, 2 cards will be destroyed
            AssertInTrash(field);
            AssertInTrash(backlash);
            AssertIsInPlay(police);
            AssertIsInPlay(monorail);

        }


        [Test()]
        public void TestBringDownTheHouseDestroy3Cards()
        {
            SetupGameController(new string[] { "BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis" });//, randomSeed: 142903984
            StartGame();
            //Setup Trash
            Card trick1 = GetCard("CheapTrick", 1);
            Card trick2 = GetCard("CheapTrick", 2);
            Card toss1 = GetCard("CardToss", 1);
            Card toss2 = GetCard("CardToss", 2);
            Card ace1 = GetCard("AceInTheHole", 1);
            Card ace2 = GetCard("AceInTheHole", 2);
            Card saint = GetCard("AceOfSaints");
            IEnumerable<Card> trashCards = new Card[] { trick1, trick2, saint, toss1, toss2, ace1, ace2 };
            PutInTrash(trashCards);

            //Setup In play
            Card field = GetCard("LivingForceField");
            Card monorail = GetCard("PlummetingMonorail");
            Card backlash = GetCard("BacklashField");
            Card police = GetCard("PoliceBackup");
            IEnumerable<Card> playCardsToDestroy = new Card[] { field, monorail, backlash };
            PlayCards(playCardsToDestroy);
            PlayCard(police);

            DecisionsYesNo = new bool[] { true, true };

            Card house = GetCard("BringDownTheHouse");
            //Shuffle any number of pairs of cards with the same name from your trash into your deck.
            PlayCard(house);
            AssertInTrash(field);
            AssertInTrash(monorail);
            AssertInTrash(backlash);
            AssertIsInPlay(police);
        }

        [Test()]
        public void TestBringDownTheHouseDestroy4Cards()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //Setup Trash

            DiscardAllCards(baccarat);
            ShuffleTrashIntoDeck(baccarat);

            PutInTrash(baccarat.HeroTurnTaker.Deck.Cards.Where(c => c.Identifier == "CheapTrick"));
            PutInTrash(baccarat.HeroTurnTaker.Deck.Cards.Where(c => c.Identifier == "CardToss").Take(2));
            PutInTrash(baccarat.HeroTurnTaker.Deck.Cards.Where(c => c.Identifier == "GraveyardBridge").Take(2));

            Card trick1 = GetCard("CheapTrick", 0);
            Card trick2 = GetCard("CheapTrick", 1);
            Card trick3 = GetCard("CheapTrick", 2);
            Card trick4 = GetCard("CheapTrick", 3);
            Card toss1 = GetCard("CardToss", 1);
            Card toss2 = GetCard("CardToss", 2);
            Card ace1 = GetCard("AceInTheHole", 1);
            Card ace2 = GetCard("AceInTheHole", 2);
            Card saint = GetCard("AceOfSaints");

            IEnumerable<Card> trashCards = baccarat.HeroTurnTaker.Trash.Cards;

            //Setup In play
            Card field = GetCard("LivingForceField");
            Card monorail = GetCard("PlummetingMonorail");
            Card backlash = GetCard("BacklashField");
            Card police = GetCard("PoliceBackup");
            IEnumerable<Card> playCardsToDestroy = new Card[] { field, monorail, backlash, police };
            PlayCards(playCardsToDestroy);

            DecisionsYesNo = new bool[] { true, true };

            Card house = GetCard("BringDownTheHouse");
            //Shuffle any number of pairs of cards with the same name from your trash into your deck.
            PlayCard(house);
            AssertInTrash(field);
            AssertInTrash(monorail);
            AssertInTrash(backlash);
            AssertInTrash(police);
        }

        [Test()]
        public void TestCardTossDealDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Card toss = GetCard("CardToss");
            Card saint = GetCard("AceOfSaints");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            PutInHand(saint);
            DecisionSelectCard = saint;
            DecisionSelectTarget = mdp;

            //{Baccarat} deals 1 target 1 projectile damage.
            QuickHPStorage(mdp);
            PlayCard(toss);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestCardTossPlayCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //put cards in hand to reduce variability
            Card toss = GetCard("CardToss");
            PutInHand(toss);
            Card saint = GetCard("AceOfSaints");
            PutInHand(baccarat, saint);
            DecisionSelectCard = saint;

            //You may play a card.
            QuickHandStorage(baccarat);
            PlayCard(toss);
            //should be 2 fewer, 1 for saints being played and 1 for card toss being played
            QuickHandCheck(-2);
        }

        [Test()]
        public void TestCardTossDontPlayCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //put toss in hand to reduce variability
            Card toss = GetCard("CardToss");
            PutInHand(toss);
            DecisionDoNotSelectCard = SelectionType.PlayCard;

            //You may play a card.
            QuickHandStorage(baccarat);
            PlayCard(toss);
            //toss should have been the only thing played
            QuickHandCheck(-1);
        }

        [Test()]
        public void TestCheapTrick()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Card cheap = GetCard("CheapTrick");
            Card abyssal = GetCard("AbyssalSolitaire");
            Card saint = GetCard("AceOfSaints");
            Card euchre = GetCard("AfterlifeEuchre");
            List<Card> list = new List<Card>() { abyssal, saint, euchre };
            int tricks = 0;
            int cheaps = 0;

            PutOnDeck(baccarat, list);
            QuickShuffleStorage(baccarat);
            PlayCard(cheap);
            QuickShuffleCheck(1);
            //Discard the top card of your deck.
            //Reveal cards from the top of your deck until you reveal a trick. Shuffle the other cards back into your deck and put the trick into play.
            AssertNumberOfCardsInTrash(baccarat, 3);
            foreach (Card c in baccarat.TurnTaker.Trash.Cards)
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
            AssertNumberOfCardsInRevealed(baccarat, 0);
            Assert.IsTrue(cheaps == 1 && tricks == 2);


        }

        [Test()]
        public void TestGraveyardBridge()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DiscardTopCards(baccarat.TurnTaker.Deck, 36);
            DiscardAllCards(baccarat);
            Card abyssal = GetCard("AbyssalSolitaire");
            Card bridge = GetCard("GraveyardBridge");
            PutInHand(bridge);

            GoToPlayCardPhase(baccarat);
            QuickShuffleStorage(baccarat);
            PlayCard(bridge);
            QuickShuffleCheck(2);
            AssertNumberOfCardsInTrash(baccarat, 37);
            //check that all 3 copies are in the deck
            AssertInDeck(GetCard("AbyssalSolitaire", 0));
            AssertInDeck(GetCard("AbyssalSolitaire", 1));
            AssertInDeck(GetCard("AbyssalSolitaire", 2));

            GoToStartOfTurn(baron);


            //check that abyssal solitaire is the one that was played
            QuickHPStorage(baccarat);
            DealDamage(baron, baccarat, 2, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestIFold()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
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
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
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
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
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
            SetupGameController("BaronBlade", "Cauldron.Baccarat", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Card hold = GetCard("UnderworldHoldEm");
            PutInHand(hold);
            DecisionDoNotSelectTurnTaker = true;

            //One player may draw a card.
            int baccaratCards = baccarat.NumberOfCardsInHand;
            QuickHandStorage(bunker);
            PlayCard(hold);
            QuickHandCheck(0);
        }
    }
}
