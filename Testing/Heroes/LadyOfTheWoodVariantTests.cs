using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.LadyOfTheWood;
using System;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller.TheArgentAdept;

namespace CauldronTests
{
    [TestFixture()]
    public class LadyOfTheWoodVariantTests : BaseTest
    {
        #region LadyOfTheWoodHelperFunctions
        protected HeroTurnTakerController ladyOfTheWood { get { return FindHero("LadyOfTheWood"); } }

        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(ladyOfTheWood.CharacterCard, 1);
            DealDamage(villain, ladyOfTheWood, 2, DamageType.Melee);
        }

        private void AddReduceDamageOfDamageTypeTrigger(HeroTurnTakerController httc, DamageType damageType, int amount)
        {
            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(amount);
            reduceDamageStatusEffect.DamageTypeCriteria.AddType(damageType);
            reduceDamageStatusEffect.NumberOfUses = 1;
            this.RunCoroutine(this.GameController.AddStatusEffect(reduceDamageStatusEffect, true, new CardSource(httc.CharacterCardController)));
        }

        private void AddIncreaseDamageOfDamageTypeTrigger(HeroTurnTakerController httc, DamageType damageType, int amount)
        {
            IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(amount);
            increaseDamageStatusEffect.DamageTypeCriteria.AddType(damageType);
            increaseDamageStatusEffect.NumberOfUses = 1;
            this.RunCoroutine(this.GameController.AddStatusEffect(increaseDamageStatusEffect, true, new CardSource(httc.CharacterCardController)));
        }

        private TokenPool ElementTokenPool
        {
            get
            {
                return FindTokenPool(ladyOfTheWood.CharacterCard.Identifier, "LadyOfTheWoodElementPool");
            }
        }            

        #endregion

        [Test()]
        public void TestSeasonsOfChangeLadyOfTheWoodLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/SeasonsOfChangeLadyOfTheWoodCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(ladyOfTheWood);
            Assert.IsInstanceOf(typeof(SeasonsOfChangeLadyOfTheWoodCharacterCardController), ladyOfTheWood.CharacterCardController);

            Assert.AreEqual(25, ladyOfTheWood.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestSeasonsOfChangeLadyOfTheWoodPower()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/SeasonsOfChangeLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            //Discard a card. You may play a card.
            GoToUsePowerPhase(ladyOfTheWood);
            Card toDiscard = PutInHand("Winter");
            Card toPlay = PutInHand("Fall");
            DecisionSelectCards = new Card[] { toDiscard, toPlay };
            QuickHandStorage(ladyOfTheWood);
            UsePower(ladyOfTheWood.CharacterCard);
            AssertInTrash(toDiscard);
            AssertInPlayArea(ladyOfTheWood, toPlay);
            QuickHandCheck(-2);
        }

        [Test()]
        public void TestSeasonsOfChangeLadyOfTheWoodPower_PlayOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/SeasonsOfChangeLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            //Discard a card. You may play a card.
            GoToUsePowerPhase(ladyOfTheWood);
            Card toDiscard = PutInHand("Winter");
            Card toPlay = PutInHand("Fall");
            DecisionSelectCards = new Card[] { toDiscard, null };
            QuickHandStorage(ladyOfTheWood);
            UsePower(ladyOfTheWood.CharacterCard);
            AssertInTrash(toDiscard);
            AssertInHand(ladyOfTheWood, toPlay);
            QuickHandCheck(-1);
        }

        [Test()]
        public void TestSeasonsOfChangeLadyOfTheWoodPower_NoCardsInHand()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/SeasonsOfChangeLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            //Discard a card. You may play a card.
            GoToUsePowerPhase(ladyOfTheWood);
            DiscardAllCards(ladyOfTheWood);
            QuickHandStorage(ladyOfTheWood);
            UsePower(ladyOfTheWood.CharacterCard);
            QuickHandCheck(0);
        }

        [Test()]
        public void TestSeasonsOfChangeLadyOfTheWoodIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/SeasonsOfChangeLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetHitPoints(ra, 15);
            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            //One target regains 1 HP.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);
            QuickHPStorage(ra);
            DecisionSelectCard = ra.CharacterCard;
            UseIncapacitatedAbility(ladyOfTheWood, 0);
            QuickHPCheck(1);
        }

        [Test()]
        public void TestSeasonsOfChangeLadyOfTheWoodIncap2_4InHand()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/SeasonsOfChangeLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            IEnumerable<Card> cardsInHand = FindCardsWhere((Card c) => ra.HeroTurnTaker.Hand.HasCard(c));
            IEnumerable<Card> cardsOnDeck = GetTopCardsOfDeck(ra, cardsInHand.Count());
            //One player may discard their hand and draw the same number of cards.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);
            DecisionSelectTurnTaker = ra.TurnTaker;
            DecisionYesNo = true;
            UseIncapacitatedAbility(ladyOfTheWood, 1);
            AssertInTrash(cardsInHand);
            AssertInHand(cardsOnDeck); 
        }

        [Test()]
        public void TestSeasonsOfChangeLadyOfTheWoodIncap2_4InHand_Optional()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/SeasonsOfChangeLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            IEnumerable<Card> cardsInHand = FindCardsWhere((Card c) => ra.HeroTurnTaker.Hand.HasCard(c));
            IEnumerable<Card> cardsOnDeck = GetTopCardsOfDeck(ra, cardsInHand.Count());
            //One player may discard their hand and draw the same number of cards.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);
            DecisionSelectTurnTaker = ra.TurnTaker;
            DecisionYesNo = false;
            UseIncapacitatedAbility(ladyOfTheWood, 1);
            AssertInDeck(cardsOnDeck);
            AssertInHand(cardsInHand);
        }

        [Test()]
        public void TestSeasonsOfChangeLadyOfTheWoodIncap2_0InHand()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/SeasonsOfChangeLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            DiscardAllCards(ra);

            IEnumerable<Card> cardsInHand = FindCardsWhere((Card c) => ra.HeroTurnTaker.Hand.HasCard(c));
            IEnumerable<Card> cardsOnDeck = GetTopCardsOfDeck(ra, cardsInHand.Count());
            //One player may discard their hand and draw the same number of cards.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);
            DecisionSelectTurnTaker = ra.TurnTaker;
            UseIncapacitatedAbility(ladyOfTheWood, 1);
            AssertInTrash(cardsInHand);
            AssertInHand(cardsOnDeck);
        }

        [Test()]
        public void TestSeasonsOfChangeLadyOfTheWoodIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/SeasonsOfChangeLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);
            Card police = PlayCard("PoliceBackup");
            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            //All damage dealt is irreducible until the start of your turn.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);
            UseIncapacitatedAbility(ladyOfTheWood, 2);
            GoToNextTurn();

            //irreducible for villains
            QuickHPStorage(ra);
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Melee, 1);
            DealDamage(baron, ra, 5, DamageType.Melee);
            QuickHPCheck(-5);

            //irreducible for heroes
            QuickHPStorage(baron);
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Fire, 1);
            DealDamage(ra, baron, 5, DamageType.Fire);
            QuickHPCheck(-5);

            //irreducible for env targets
            QuickHPStorage(baron);
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Projectile, 1);
            DealDamage(police, baron.CharacterCard, 5, DamageType.Projectile);
            QuickHPCheck(-5);

            //expires at start of next turn
            GoToStartOfTurn(ladyOfTheWood);


            // no longer irreducible for villains
            QuickHPStorage(ra);
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Melee, 1);
            DealDamage(baron, ra, 5, DamageType.Melee);
            QuickHPCheck(-4);

            //no longer irreducible for heroes
            QuickHPStorage(baron);
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Fire, 1);
            DealDamage(ra, baron, 5, DamageType.Fire);
            QuickHPCheck(-4);

            //no longer irreducible for env targets
            QuickHPStorage(baron);
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Projectile, 1);
            DealDamage(police, baron.CharacterCard, 5, DamageType.Projectile);
            QuickHPCheck(-4);

        }

        [Test()]
        public void TestMinistryOfStrategicScienceLadyOfTheWoodLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/MinistryOfStrategicScienceLadyOfTheWoodCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(ladyOfTheWood);
            Assert.IsInstanceOf(typeof(MinistryOfStrategicScienceLadyOfTheWoodCharacterCardController), ladyOfTheWood.CharacterCardController);

            Assert.AreEqual(19, ladyOfTheWood.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestMinistryOfStrategicScienceLadyOfTheWoodPower()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/MinistryOfStrategicScienceLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            //Add 2 tokens to your element pool. When {LadyOfTheWood} would deal damage, you may change its type by spending a token.
            GoToUsePowerPhase(ladyOfTheWood);
            AssertTokenPoolCount(ElementTokenPool, 0);
            UsePower(ladyOfTheWood.CharacterCard);
            AssertTokenPoolCount(ElementTokenPool, 2);

            //check optional damage type change when lady of the wood deals damage
            GoToNextTurn();
            DecisionSelectDamageType = DamageType.Infernal;
            DecisionYesNo = true;
            AddIncreaseDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Infernal, 1);
            QuickHPStorage(ra);
            AssertTokenPoolCount(ElementTokenPool, 2);
            DealDamage(ladyOfTheWood, ra, 3, DamageType.Cold);
            QuickHPCheck(-4);
            AssertTokenPoolCount(ElementTokenPool, 1);

            //check that it only applies to lady of the wood
            DecisionSelectDamageType = DamageType.Infernal;
            DecisionYesNo = true;
            AddIncreaseDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Infernal, 1);
            QuickHPStorage(ra);
            AssertTokenPoolCount(ElementTokenPool, 1);
            DealDamage(haka, ra, 3, DamageType.Cold);
            QuickHPCheck(-3);
            AssertTokenPoolCount(ElementTokenPool, 1);

            //check that it never exprires
            GoToStartOfTurn(ladyOfTheWood);
            DecisionSelectDamageType = DamageType.Infernal;
            DecisionYesNo = true;
            AddIncreaseDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Infernal, 1);
            QuickHPStorage(ra);
            AssertTokenPoolCount(ElementTokenPool, 1);
            DealDamage(ladyOfTheWood, ra, 3, DamageType.Cold);
            QuickHPCheck(-5); //2 increases have been given up to here
            AssertTokenPoolCount(ElementTokenPool, 0);

        }

        [Test()]
        public void TestMinistryOfStrategicScienceLadyOfTheWoodPower_GuiseTests()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/MinistryOfStrategicScienceLadyOfTheWoodCharacter", "Ra", "Guise", "Megalopolis");
            StartGame();

            GoToStartOfTurn(guise);
            PlayCard("ICanDoThatToo");

            //check optional damage type change when guise deals damage
            DecisionSelectDamageType = DamageType.Infernal;
            DecisionYesNo = true;
            AddIncreaseDamageOfDamageTypeTrigger(guise, DamageType.Infernal, 1);
            QuickHPStorage(ra);
            AssertTokenPoolCount(ElementTokenPool, 2);
            DealDamage(guise, ra, 3, DamageType.Cold);
            QuickHPCheck(-4);
            AssertTokenPoolCount(ElementTokenPool, 1);

            //check that it only applies to guise and lotw
            DecisionSelectDamageType = DamageType.Infernal;
            DecisionYesNo = true;
            AddIncreaseDamageOfDamageTypeTrigger(guise, DamageType.Infernal, 1);
            QuickHPStorage(ra);
            AssertTokenPoolCount(ElementTokenPool, 1);
            DealDamage(ra, ra, 3, DamageType.Cold);
            QuickHPCheck(-3);
            AssertTokenPoolCount(ElementTokenPool, 1);

            //check that it never exprires
            GoToStartOfTurn(guise);
            DecisionSelectDamageType = DamageType.Infernal;
            DecisionYesNo = true;
            AddIncreaseDamageOfDamageTypeTrigger(guise, DamageType.Infernal, 1);
            QuickHPStorage(ra);
            AssertTokenPoolCount(ElementTokenPool, 1);
            DealDamage(guise, ra, 3, DamageType.Cold);
            QuickHPCheck(-5); //2 increases have been given up to here
            AssertTokenPoolCount(ElementTokenPool, 0);

        }

        [Test()]
        public void TestMinistryOfStrategicScienceLadyOfTheWoodIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/MinistryOfStrategicScienceLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            //One player may play a card.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);
            Card mere = PutInHand("Mere");
            DecisionSelectTurnTaker = haka.TurnTaker;
            DecisionSelectCard = mere;
            UseIncapacitatedAbility(ladyOfTheWood, 0);
            AssertInPlayArea(haka, mere);
        }

        [Test()]
        public void TestMinistryOfStrategicScienceLadyOfTheWoodIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/MinistryOfStrategicScienceLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            //Add 1 token to your element pool.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);
            AssertTokenPoolCount(ElementTokenPool, 0);
            UseIncapacitatedAbility(ladyOfTheWood, 1);
            AssertTokenPoolCount(ElementTokenPool, 1);
        }

        [Test()]
        public void TestMinistryOfStrategicScienceLadyOfTheWoodIncap2_StartWithTokensInPool()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/MinistryOfStrategicScienceLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            UsePower(ladyOfTheWood);
            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            //Add 1 token to your element pool.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);
            AssertTokenPoolCount(ElementTokenPool, 2);
            UseIncapacitatedAbility(ladyOfTheWood, 1);
            AssertTokenPoolCount(ElementTokenPool, 3);
        }

        [Test()]
        public void TestMinistryOfStrategicScienceLadyOfTheWoodIncap3_TokensInPool()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/MinistryOfStrategicScienceLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);

            UsePower(ladyOfTheWood);

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            //Spend 1 token from your element pool. If you do, 1 hero deals 1 target 3 damage of any type.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);
            DecisionSelectTurnTaker = haka.TurnTaker;
            DecisionSelectDamageType = DamageType.Infernal;
            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron);
            AddIncreaseDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Infernal, 1);
            AssertTokenPoolCount(ElementTokenPool, 2);
            UseIncapacitatedAbility(ladyOfTheWood, 2);
            QuickHPCheck(-4);
            AssertTokenPoolCount(ElementTokenPool, 1);
        }

        [Test()]
        public void TestMinistryOfStrategicScienceLadyOfTheWoodIncap3_NoTokensInPool()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/MinistryOfStrategicScienceLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            DestroyCard(GetCardInPlay("MobileDefensePlatform"), baron.CharacterCard);


            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            //Spend 1 token from your element pool. If you do, 1 hero deals 1 target 3 damage of any type.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);
            DecisionSelectTurnTaker = haka.TurnTaker;
            DecisionSelectDamageType = DamageType.Infernal;
            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron);
            AddIncreaseDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Infernal, 1);
            AssertTokenPoolCount(ElementTokenPool, 0);
            UseIncapacitatedAbility(ladyOfTheWood, 2);
            QuickHPCheck(0);
            AssertTokenPoolCount(ElementTokenPool, 0);
        }


        [Test()]
        public void TestFutureLadyOfTheWoodLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/FutureLadyOfTheWoodCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(ladyOfTheWood);
            Assert.IsInstanceOf(typeof(FutureLadyOfTheWoodCharacterCardController), ladyOfTheWood.CharacterCardController);

            Assert.AreEqual(20, ladyOfTheWood.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestFutureLadyOfTheWoodPower()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/FutureLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetHitPoints(ladyOfTheWood, 10);

            //Destroy a season. Play a season and change the next damage dealt by {LadyOfTheWood} to a type listed on it. You may use a power.
            GoToUsePowerPhase(ladyOfTheWood);
            Card seasonToDestroy = PlayCard("Summer");
            Card seasonToPlay = PutInHand("Spring");
            Card cardWithPower = PlayCard("SnowshadeGown");
            QuickHPStorage(ladyOfTheWood);
            DecisionSelectCard = seasonToPlay;
            UsePower(ladyOfTheWood.CharacterCard);

            //check that a season has been destroyed
            AssertInTrash(seasonToDestroy);

            //check that a season has been played
            AssertInPlayArea(ladyOfTheWood, seasonToPlay);

            //check that a power has been used
            QuickHPCheck(3);

            //check that damage has been changed to toxic
            AddIncreaseDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Toxic, 1);
            QuickHPStorage(ra);
            DealDamage(ladyOfTheWood, ra, 3, DamageType.Cold);
            QuickHPCheck(-4);

            //check only next damage
            AddIncreaseDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Toxic, 1);
            QuickHPStorage(ra);
            DealDamage(ladyOfTheWood, ra, 3, DamageType.Cold);
            QuickHPCheck(-3);

        }

        [Test()]
        public void TestFutureLadyOfTheWoodPower_NoSeasonInPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/FutureLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetHitPoints(ladyOfTheWood, 10);

            //Destroy a season. Play a season and change the next damage dealt by {LadyOfTheWood} to a type listed on it. You may use a power.
            GoToUsePowerPhase(ladyOfTheWood);
            Card seasonToPlay = PutInHand("Summer");
            Card cardWithPower = PlayCard("SnowshadeGown");
            QuickHPStorage(ladyOfTheWood);
            DecisionSelectCard = seasonToPlay;
            UsePower(ladyOfTheWood.CharacterCard);

            //check that a season has been played
            AssertInPlayArea(ladyOfTheWood, seasonToPlay);

            //check that a power has been used
            QuickHPCheck(3);

            //check that damage has been changed to fire
            AddIncreaseDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Fire, 1);
            QuickHPStorage(ra);
            DealDamage(ladyOfTheWood, ra, 3, DamageType.Cold);
            QuickHPCheck(-6); //+1 from check, +2 from summer

        }

        [Test()]
        public void TestFutureLadyOfTheWoodIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/FutureLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            //One player may draw a card.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);
            QuickHandStorage(ra);
            DecisionSelectTurnTaker = ra.TurnTaker;
            UseIncapacitatedAbility(ladyOfTheWood, 0);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestFutureLadyOfTheWoodIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/FutureLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            //One target deals itself 1 cold damage.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);
            QuickHPStorage(ra);
            DecisionSelectTarget = ra.CharacterCard;
            AddIncreaseDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Cold, 1);
            UseIncapacitatedAbility(ladyOfTheWood, 1);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestFutureLadyOfTheWoodIncap3_Spring()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/FutureLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);

            //Select 1 of your season cards, its text affects all heroes until your next turn.
            Card spring = FindCardsWhere((Card c) => ladyOfTheWood.CharacterCard.UnderLocation.HasCard(c) && c.Identifier == "Spring").First();
            DecisionSelectCard = spring;
            UseIncapacitatedAbility(ladyOfTheWood, 2);

            GoToPlayCardPhase(haka);
            //Whenever a hero deals toxic damage to a target, they regain that much HP.
            SetHitPoints(haka, 20);
            SetHitPoints(ra, 20);

            //check gain hp on toxic
            QuickHPStorage(haka, ra);
            DealDamage(haka, ra, 3, DamageType.Toxic);
            QuickHPCheck(3, -3);

            //check only for toxic
            QuickHPUpdate();
            DealDamage(haka, ra, 3, DamageType.Fire);
            QuickHPCheck(0, -3);

            //check expires at start of next turn
            GoToStartOfTurn(haka);
            QuickHPUpdate();
            DealDamage(haka, ra, 3, DamageType.Toxic);
            QuickHPCheck(0, -3);

        }

        [Test()]
        public void TestFutureLadyOfTheWoodIncap3_Summer()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/FutureLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);

            //Select 1 of your season cards, its text affects all heroes until your next turn.
            Card summer = FindCardsWhere((Card c) => ladyOfTheWood.CharacterCard.UnderLocation.HasCard(c) && c.Identifier == "Summer").First();
            DecisionSelectCard = summer;
            UseIncapacitatedAbility(ladyOfTheWood, 2);

            GoToPlayCardPhase(haka);
            //Increase fire damage dealt by heroes by 2

            //check gain hp on toxic
            QuickHPStorage(ra);
            DealDamage(haka, ra, 3, DamageType.Fire);
            QuickHPCheck(-5);

            //check only for toxic
            QuickHPUpdate();
            DealDamage(haka, ra, 3, DamageType.Toxic);
            QuickHPCheck(-3);

            //check expires at start of next turn
            GoToStartOfTurn(ladyOfTheWood);
            QuickHPUpdate();
            DealDamage(haka, ra, 3, DamageType.Fire);
            QuickHPCheck(-3);

        }

        [Test()]
        public void TestFutureLadyOfTheWoodIncap3_Fall()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/FutureLadyOfTheWoodCharacter", "Ra", "Haka", "Tachyon", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);

            //Select 1 of your season cards, its text affects all heroes until your next turn.
            Card fall = FindCardsWhere((Card c) => ladyOfTheWood.CharacterCard.UnderLocation.HasCard(c) && c.Identifier == "Fall").First();
            DecisionSelectCard = fall;
            UseIncapacitatedAbility(ladyOfTheWood, 2);

            GoToPlayCardPhase(haka);
            //Whenever a hero deals lightning damage to a target, reduce damage dealt by that target by 1 until the start of your next turn.

            //check reduce effect applied on lightning
            QuickHPStorage(ra);
            DealDamage(haka, ra, 3, DamageType.Lightning);
            QuickHPCheck(-3);
            QuickHPStorage(haka);
            DealDamage(ra, haka, 3, DamageType.Fire);
            QuickHPCheck(-2);

            //check only for lightning
            QuickHPStorage(tachyon);
            DealDamage(haka, tachyon, 3, DamageType.Fire);
            QuickHPCheck(-3);
            QuickHPStorage(haka);
            DealDamage(tachyon, haka, 3, DamageType.Fire);
            QuickHPCheck(-3);

            //check expires at start of next turn
            GoToStartOfTurn(haka);
            QuickHPStorage(haka);
            DealDamage(ra, haka, 3, DamageType.Fire);
            QuickHPCheck(-3);

        }


        [Test()]
        public void TestFutureLadyOfTheWoodIncap3_Winter()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood/FutureLadyOfTheWoodCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);

            //Select 1 of your season cards, its text affects all heroes until your next turn.
            Card winter = FindCardsWhere((Card c) => ladyOfTheWood.CharacterCard.UnderLocation.HasCard(c) && c.Identifier == "Winter").First();
            DecisionSelectCard = winter;
            UseIncapacitatedAbility(ladyOfTheWood, 2);

            GoToPlayCardPhase(haka);
            //Whenever a hero deals cold damage to a target, they draw a card.

            //check for draw on cold
            QuickHPStorage(ra);
            QuickHandStorage(haka);
            DealDamage(haka, ra, 3, DamageType.Cold);
            QuickHPCheck(-3);
            QuickHandCheck(1);

            //check only for cold
            QuickHPUpdate();
            QuickHandUpdate();
            DealDamage(haka, ra, 3, DamageType.Fire);
            QuickHPCheck(-3);
            QuickHandCheck(0);

            //check expires at start of next turn
            GoToStartOfTurn(ladyOfTheWood);
            QuickHPUpdate();
            QuickHandUpdate();
            DealDamage(haka, ra, 3, DamageType.Cold);
            QuickHPCheck(-3);
            QuickHandCheck(0);


        }



    }
}
