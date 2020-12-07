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




    }
}
