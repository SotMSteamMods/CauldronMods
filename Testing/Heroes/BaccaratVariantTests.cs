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
    public class BaccaratVariantTests : BaseTest
    {
        #region BaccaratHelperFunctions

        protected HeroTurnTakerController baccarat { get { return FindHero("Baccarat"); } }

        private void SetupIncap(TurnTakerController source)
        {
            SetHitPoints(baccarat.CharacterCard, 1);
            DealDamage(source, baccarat, 2, DamageType.Melee);
        }

        #endregion BaccaratHelperFunctions

        [Test()]
        public void TestLoadBaccaratAceOfSwords()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(baccarat);
            Assert.IsInstanceOf(typeof(AceOfSwordsBaccaratCharacterCardController), baccarat.CharacterCardController);

            Assert.AreEqual(30, baccarat.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsInnatePower1EuchreInTrash()
        {
            SetupGameController("Apostate", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //Play X copies of Afterlife Euchre from your trash (up to 3). Discard the top 3 - X cards of your deck.
            DecisionSelectNumber = 1;
            GoToUsePowerPhase(baccarat);
            //Pick deal damage on Afterlife Euchre
            DecisionSelectFunction = 1;
            PutInTrash("AfterlifeEuchre", 0);
            QuickHPStorage(apostate);
            UsePower(baccarat.CharacterCard);
            QuickHPCheck(-2);
            AssertNumberOfCardsInTrash(baccarat, 3);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsInnatePower2EuchreInTrash()
        {
            SetupGameController("Apostate", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //Play X copies of Afterlife Euchre from your trash (up to 3). Discard the top 3 - X cards of your deck.
            DecisionSelectNumber = 2;
            GoToUsePowerPhase(baccarat);
            //Pick deal damage on Afterlife Euchre
            DecisionSelectFunction = 1;
            PutInTrash("AfterlifeEuchre", 0);
            PutInTrash("AfterlifeEuchre", 1);
            QuickHPStorage(apostate);
            UsePower(baccarat.CharacterCard);
            QuickHPCheck(-4);
            AssertNumberOfCardsInTrash(baccarat, 3);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsInnatePower3EuchreInTrash()
        {
            SetupGameController(new string[] { "Apostate", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis" });//, randomSeed: 1961543406
            StartGame();
            //Play X copies of Afterlife Euchre from your trash (up to 3). Discard the top 3 - X cards of your deck.
            DecisionSelectNumber = 3;
            GoToUsePowerPhase(baccarat);
            //Pick deal 2 damage on Afterlife Euchre
            DecisionSelectFunction = 1;
            PutInTrash("AfterlifeEuchre", 0);
            PutInTrash("AfterlifeEuchre", 1);
            PutInTrash("AfterlifeEuchre", 2);
            QuickHPStorage(apostate);
            UsePower(baccarat.CharacterCard);
            QuickHPCheck(-6);
            AssertNumberOfCardsInTrash(baccarat, 3);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsInnatePowerPick2ToPlay()
        {
            SetupGameController("Apostate", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //Play X copies of Afterlife Euchre from your trash (up to 3). Discard the top 3 - X cards of your deck.
            DecisionSelectNumber = 2;
            GoToUsePowerPhase(baccarat);
            //Pick deal 2 damage on Afterlife Euchre
            DecisionSelectFunction = 1;
            PutInTrash("AfterlifeEuchre", 0);
            PutInTrash("AfterlifeEuchre", 1);
            PutInTrash("AfterlifeEuchre", 2);
            PutInTrash("AfterlifeEuchre", 3);
            QuickHPStorage(apostate);
            UsePower(baccarat.CharacterCard);
            QuickHPCheck(-4);
            AssertNumberOfCardsInTrash(baccarat, 5);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsIncap1Hero()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Unity", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);
            Card bot1 = GetCard("RaptorBot", 0);
            Card bot2 = GetCard("RaptorBot", 1);
            PlayCard(bot1);
            PutInTrash(bot2);

            //Select a card in a trash. Destroy a card in play with the same name.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 0);
            AssertInTrash(bot1, bot2);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsIncap1Villain()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Unity", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);
            Card mdp = GetCardInPlay("MobileDefensePlatform", 0);
            Card mdp2 = GetCard("MobileDefensePlatform", 1);
            PlayCard(mdp);
            PutInTrash(mdp2);

            //Select a card in a trash. Destroy a card in play with the same name.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 0);
            AssertInTrash(mdp, mdp2);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsIncap1EnvNonTarget()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Unity", "TheScholar", "Magmaria");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);
            Card throng = GetCard("MagmarianThrong", 0);
            Card throng2 = GetCard("MagmarianThrong", 1);
            PlayCard(env, throng);
            PutInTrash(env, throng2);

            //Select a card in a trash. Destroy a card in play with the same name.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 0);
            AssertInTrash(throng, throng2);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsIncap1NotFailOnNoDestroy()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Unity", "TheScholar", "Magmaria");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);

            //Select a card in a trash. Destroy a card in play with the same name.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 0);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsIncap2Hero()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);

            GoToUseIncapacitatedAbilityPhase(baccarat);
            //One target deals itself 1 energy damage.
            DecisionSelectCard = bunker.CharacterCard;
            QuickHPStorage(bunker);
            UseIncapacitatedAbility(baccarat, 1);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsIncap2Villain()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            GoToUseIncapacitatedAbilityPhase(baccarat);
            //One target deals itself 1 energy damage.
            DecisionSelectCard = mdp;
            QuickHPStorage(mdp);
            UseIncapacitatedAbility(baccarat, 1);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsIncap3Yes()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);

            //Each hero character may deal themselves 1 toxic damage to draw a card now.
            QuickHandStorage(legacy, bunker, scholar);
            QuickHPStorage(legacy, bunker, scholar);

            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 2);

            QuickHandCheck(1, 1, 1);
            QuickHPCheck(-1, -1, -1);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsIncap3No()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(baccarat);

            DecisionDoNotSelectFunction = true;

            //Each hero character may deal themselves 1 toxic damage to draw a card now.
            QuickHandStorage(legacy, bunker, scholar);
            QuickHPStorage(legacy, bunker, scholar);

            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 2);

            QuickHandCheck(0, 0, 0);
            QuickHPCheck(0, 0, 0);
        }

        [Test()]
        public void TestBaccaratAceOfSwordsAceInTheHoleTwoPowerPhase()
        {
            SetupGameController("AkashBhuta", "Cauldron.Baccarat/AceOfSwordsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            Card ace = GetCard("AceInTheHole");
            //Pick deal damage on Afterlife Euchre
            DecisionSelectFunction = 1;
            PutInTrash("AfterlifeEuchre", 0);
            PutInTrash("AfterlifeEuchre", 1);
            //Ensuring the first card discarded isn't another Afterlife Euchre
            PutOnDeck("AceOfSaints");
            //Don't play card
            DecisionDoNotSelectCard = SelectionType.PlayCard;

            //Play max number of cards
            DecisionSelectNumber = 2;
            //You may use {Baccarat}'s innate power twice during your phase this turn.
            QuickHPStorage(akash);
            GoToPlayCardPhase(baccarat);
            AssertNumberOfUsablePowers(baccarat, 1);
            PlayCard(ace);
            GoToUsePowerPhase(baccarat);
            UsePower(baccarat);
            AssertNumberOfUsablePowers(baccarat, 1);
            UsePower(baccarat);
            AssertNumberOfUsablePowers(baccarat, 0);
            QuickHPCheck(-8);
            //Ace In The Hole, 2 Afterlife Euchre, 2 Discard
            AssertNumberOfCardsInTrash(baccarat, 5);
        }

        [Test()]
        public void TestLoadBaccaratAceOfSorrows()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(baccarat);
            Assert.IsInstanceOf(typeof(AceOfSorrowsBaccaratCharacterCardController), baccarat.CharacterCardController);

            Assert.AreEqual(28, baccarat.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestBaccaratAceOfSorrowsInnatePowerPlay0Trick()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DiscardAllCards(baccarat);
            Card hold0 = PutInHand(baccarat, "UnderworldHoldEm");
            Card solitare = PutInHand(baccarat, "AbyssalSolitaire");
            Card euchre = PutInHand(baccarat, "AfterlifeEuchre");
            Card hold1 = PutInHand(baccarat, "UnderworldHoldEm", 1);

            //Play 0 tricks
            DecisionYesNo = false;
            //Play any number of tricks from your hand
            UsePower(baccarat);
            AssertInHand(new Card[] { hold0, hold1, solitare, euchre });
        }

        [Test()]
        public void TestBaccaratAceOfSorrowsInnatePowerPlay1Trick()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DiscardAllCards(baccarat);
            Card hold0 = PutInHand(baccarat, "UnderworldHoldEm");
            Card solitare = PutInHand(baccarat, "AbyssalSolitaire");
            Card euchre = PutInHand(baccarat, "AfterlifeEuchre");
            Card hold1 = PutInHand(baccarat, "UnderworldHoldEm", 1);

            //Play 1 trick
            DecisionYesNo = true;
            DecisionSelectCards = new Card[] { hold0, null };
            //Play any number of tricks from your hand
            UsePower(baccarat);
            AssertInHand(new Card[] { hold1, solitare, euchre });
            AssertInTrash(new Card[] { hold0 });
        }

        [Test()]
        public void TestBaccaratAceOfSorrowsInnatePowerPlay2Trick()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DiscardAllCards(baccarat);
            Card hold0 = PutInHand(baccarat, "UnderworldHoldEm");
            Card solitare = PutInHand(baccarat, "AbyssalSolitaire");
            Card euchre = PutInHand(baccarat, "AfterlifeEuchre");
            Card hold1 = PutInHand(baccarat, "UnderworldHoldEm", 1);

            //Play 2 tricks
            DecisionYesNo = true;
            DecisionSelectCards = new Card[] { hold0, hold1, null };
            //Play any number of tricks from your hand
            UsePower(baccarat);
            AssertInHand(new Card[] { solitare, euchre });
            AssertInTrash(new Card[] { hold1, hold0 });
        }

        [Test()]
        public void TestBaccaratAceOfSorrowsInnatePowerPlay3Trick()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DiscardAllCards(baccarat);
            Card hold0 = PutInHand(baccarat, "UnderworldHoldEm");
            Card solitare = PutInHand(baccarat, "AbyssalSolitaire");
            Card euchre = PutInHand(baccarat, "AfterlifeEuchre");
            Card hold1 = PutInHand(baccarat, "UnderworldHoldEm", 1);

            //Play 3 tricks
            DecisionYesNo = true;
            DecisionSelectCards = new Card[] { solitare, hold0, hold1, null };
            //Play any number of tricks from your hand
            UsePower(baccarat);
            AssertInHand(new Card[] { euchre });
            AssertInTrash(new Card[] { solitare, hold1, hold0 });
        }

        [Test()]
        public void TestBaccaratAceOfSorrowsInnatePowerPlay4Trick()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DiscardAllCards(baccarat);
            Card hold0 = PutInHand(baccarat, "UnderworldHoldEm");
            Card solitare = PutInHand(baccarat, "AbyssalSolitaire");
            Card euchre = PutInHand(baccarat, "AfterlifeEuchre");
            Card hold1 = PutInHand(baccarat, "UnderworldHoldEm", 1);

            //Play 4 tricks
            DecisionYesNo = true;
            DecisionSelectCards = new Card[] { solitare, hold0, hold1, euchre, null };
            //Play any number of tricks from your hand
            UsePower(baccarat);
            AssertInTrash(new Card[] { solitare, hold1, hold0, euchre });
        }

        [Test()]
        public void TestBaccaratAceOfSorrowsInnatePowerTricksToHand0TricksInTrash()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DecisionSelectFunction = 1;
            //Put 2 tricks from your trash into your hand.
            UsePower(baccarat);
        }

        [Test()]
        public void TestBaccaratAceOfSorrowsInnatePowerTricksToHand1TrickInTrash()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card hold = PutInTrash("UnderworldHoldEm");
            Card saint = PutInTrash("AceOfSaints");

            DecisionSelectFunction = 1;
            //Put 2 tricks from your trash into your hand.
            UsePower(baccarat);
            AssertInHand(hold);
            AssertInTrash(new Card[] { saint });
        }

        [Test()]
        public void TestBaccaratAceOfSorrowsInnatePowerTricksToHand2TricksInTrash()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card hold = PutInTrash("UnderworldHoldEm");
            Card saint = PutInTrash("AceOfSaints");
            Card euchre = PutInTrash("AfterlifeEuchre");

            DecisionSelectFunction = 1;
            //Put 2 tricks from your trash into your hand.
            UsePower(baccarat);
            AssertInHand(hold, euchre);
            AssertInTrash(new Card[] { saint });
        }

        [Test()]
        public void TestBaccaratAceOfSorrowsInnatePowerTricksToHand3TricksInTrash()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card hold = PutInTrash("UnderworldHoldEm");
            Card saint = PutInTrash("AceOfSaints");
            Card euchre = PutInTrash("AfterlifeEuchre");
            Card solitare = PutInTrash("AbyssalSolitaire");

            DecisionSelectFunction = 1;
            //Put 2 tricks from your trash into your hand.
            UsePower(baccarat);
            AssertInHand(solitare, euchre);
            AssertInTrash(saint, hold);
        }

        [Test()]
        public void TestBaccaratAceOfSorrowsIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baccarat);
            //One player may shuffle a card from their trash into their deck, then put all cards with the same name from their trash into their hand.
        }

        [Test()]
        public void TestBaccaratAceOfSorrowsIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baccarat);

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DealDamage(baron, mdp, 2, DamageType.Melee);
            DealDamage(baron, legacy, 2, DamageType.Melee);
            DealDamage(baron, bunker, 2, DamageType.Melee);
            DealDamage(baron, scholar, 2, DamageType.Melee);

            //Each target regains 1 HP.
            QuickHPStorage(mdp, bunker.CharacterCard, legacy.CharacterCard, scholar.CharacterCard);
            UseIncapacitatedAbility(baccarat, 1);
            QuickHPCheck(1, 1, 1, 1);
        }

        [Test()]
        public void TestBaccaratAceOfSorrowsIncap3Yes()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baccarat);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card flak = PutInHand("FlakCannon");
            Card ring = PutInHand("TheLegacyRing");
            Card iron = PutInHand("FleshToIron");

            DecisionSelectCards = new Card[] { legacy.CharacterCard, ring, bunker.CharacterCard, flak, iron };

            //Each hero character may deal themselves 3 toxic damage to play a card now.
            QuickHPStorage(mdp, bunker.CharacterCard, legacy.CharacterCard, scholar.CharacterCard);
            UseIncapacitatedAbility(baccarat, 2);
            QuickHPCheck(0, -3, -3, -3);
            AssertIsInPlay(new Card[] { ring, flak, iron });
        }

        [Test()]
        public void TestBaccaratAceOfSorrowsIncap3No()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            SetupIncap(baccarat);

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //Don't use this effect
            DecisionDoNotSelectFunction = true;

            //Each hero character may deal themselves 3 toxic damage to play a card now.
            QuickHPStorage(mdp, bunker.CharacterCard, legacy.CharacterCard, scholar.CharacterCard);
            UseIncapacitatedAbility(baccarat, 2);
            QuickHPCheck(0, 0, 0, 0);
        }

        [Test()]
        public void TestBaccaratAceOfSorrowssAceInTheHoleTwoPowerPhase()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/AceOfSorrowsBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DiscardAllCards(baccarat);
            Card hold0 = PutInHand(baccarat, "UnderworldHoldEm");
            Card solitare = PutInHand(baccarat, "AbyssalSolitaire");
            Card euchre = PutInHand(baccarat, "AfterlifeEuchre");
            Card hold1 = PutInHand(baccarat, "UnderworldHoldEm", 1);


            DecisionYesNo = true;
            DecisionSelectCards = new Card[] { null, hold0, null, solitare, null };
            //Play any number of tricks from your hand
            GoToUsePowerPhase(baccarat);
            //Ace in the Hole allows Baccarat to use his power twice
            PlayCard("AceInTheHole");
            AssertNumberOfUsablePowers(baccarat, 1);
            UsePower(baccarat);
            AssertNumberOfUsablePowers(baccarat, 1);
            UsePower(baccarat);
            AssertNumberOfUsablePowers(baccarat, 0);
            AssertInHand(new Card[] { hold1, euchre });
            AssertInTrash(new Card[] { solitare, hold0 });
        }

        [Test()]
        public void TestLoadBaccarat1929()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/PastBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(baccarat);
            Assert.IsInstanceOf(typeof(PastBaccaratCharacterCardController), baccarat.CharacterCardController);

            Assert.AreEqual(24, baccarat.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestBaccaratInnatePowerPlayTrick()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/PastBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card abyssal = PutInHand("AbyssalSolitaire");

            //Play or draw a card. If you played a trick this way, draw a card.
            DecisionSelectCardToPlay = abyssal;
            GoToUsePowerPhase(baccarat);
            QuickHandStorage(baccarat);
            UsePower(baccarat);
            //Played Abyssal Solitare then drew a card
            QuickHandCheck(0);
        }

        [Test()]
        public void TestBaccaratInnatePowerPlayNotTrick()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/PastBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Card saint = PutInHand("AceOfSaints");

            //Play or draw a card. If you played a trick this way, draw a card.
            DecisionSelectCardToPlay = saint;
            GoToUsePowerPhase(baccarat);
            QuickHandStorage(baccarat);
            UsePower(baccarat);
            //Played Ace of Saints
            QuickHandCheck(-1);
        }

        [Test()]
        public void TestBaccaratInnatePowerDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/PastBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //Play or draw a card. If you played a trick this way, draw a card.
            DecisionSelectFunction = 1;
            GoToUsePowerPhase(baccarat);
            QuickHandStorage(baccarat);
            UsePower(baccarat);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestBaccaratIncapOption1()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/PastBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            SetupIncap(baccarat);
            Card fort = PutInHand("Fortitude");
            DecisionSelectCardToPlay = fort;

            //One player may play a card now.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            UseIncapacitatedAbility(baccarat, 0);
            AssertIsInPlay(fort);
        }

        [Test()]
        public void TestBaccaratIncapOption2()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/PastBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            SetupIncap(baccarat);
            DecisionSelectTurnTaker = bunker.TurnTaker;

            //One player may use a power now.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            QuickHandStorage(bunker);
            UseIncapacitatedAbility(baccarat, 1);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestBaccaratIncapOption3()
        {
            SetupGameController("BaronBlade", "Cauldron.Baccarat/PastBaccaratCharacter", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            SetupIncap(baccarat);
            DecisionSelectTurnTaker = bunker.TurnTaker;

            //One player may use a power now.
            GoToUseIncapacitatedAbilityPhase(baccarat);
            QuickHandStorage(bunker);
            UseIncapacitatedAbility(baccarat, 2);
            QuickHandCheck(1);
        }
    }
}
