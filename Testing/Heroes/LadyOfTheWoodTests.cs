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
    public class LadyOfTheWoodTests : BaseTest
    {
        #region LadyOfTheWoodHelperFunctions
        protected HeroTurnTakerController ladyOfTheWood { get { return FindHero("LadyOfTheWood"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(ladyOfTheWood.CharacterCard, 1);
            DealDamage(villain, ladyOfTheWood, 2, DamageType.Melee);
        }

        protected void AssertNumberOfSeasonsInHand(TurnTakerController ttc, int number)
        {
            var cardsInHand = ttc.TurnTaker.GetAllCards().Where(c => c.IsInHand && this.IsSeason(c));
            var actual = cardsInHand.Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in hand, but actually had {2}: {3}", ttc.Name, number, actual, cardsInHand.Select(c => c.Title).ToCommaList()));
        }
        protected void AssertNumberOfSeasonsInPLay(TurnTakerController ttc, int number)
        {
            var cardsInPlay = ttc.TurnTaker.GetAllCards().Where(c => c.IsInPlay && this.IsSeason(c));
            var actual = cardsInPlay.Count();
            Assert.AreEqual(number, actual, String.Format("{0} should have had {1} cards in play, but actually had {2}: {3}", ttc.Name, number, actual, cardsInPlay.Select(c => c.Title).ToCommaList()));
        }


        protected int GetNumberOfSeasonsInHand(TurnTakerController ttc)
        {
            var cardsInHand = ttc.TurnTaker.GetAllCards().Where(c => c.IsInHand && this.IsSeason(c));
            var actual = cardsInHand.Count();
            return actual;
        }
        private bool IsSeason(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "season", false, false);
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

        #endregion

        [Test()]
        public void TestLadyOfTheWoodLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(ladyOfTheWood);
            Assert.IsInstanceOf(typeof(LadyOfTheWoodCharacterCardController), ladyOfTheWood.CharacterCardController);

            Assert.AreEqual(22, ladyOfTheWood.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestLadyOfTheWoodInnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //Select a damage type. {LadyOfTheWood} deals 1 target 1 damage of the chosen type.
            GoToUsePowerPhase(ladyOfTheWood);
            DecisionSelectDamageType = DamageType.Cold;
            DecisionSelectTarget = ra.CharacterCard;
            QuickHPStorage(ra, haka);
            UsePower(ladyOfTheWood.CharacterCard);
            QuickHPCheck(-1, 0);
        }

        [Test()]
        public void TestLadyOfTheWoodInnatePower_DamageType()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //Select a damage type. {LadyOfTheWood} deals 1 target 1 damage of the chosen type.
            GoToUsePowerPhase(ladyOfTheWood);
            DecisionSelectDamageTypes = new DamageType?[] {
                DamageType.Cold, DamageType.Energy, DamageType.Fire,
                DamageType.Infernal, DamageType.Lightning, DamageType.Melee,
                DamageType.Projectile, DamageType.Psychic, DamageType.Radiant,
                DamageType.Sonic, DamageType.Toxic
            };
            DecisionSelectTarget = ra.CharacterCard;

            PrintSeparator("Check for Cold");
            QuickHPStorage(ra);
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Cold, 1);
            UsePower(ladyOfTheWood.CharacterCard);
            QuickHPCheck(0);

            PrintSeparator("Check for Energy");
            QuickHPStorage(ra);
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Energy, 1);
            UsePower(ladyOfTheWood.CharacterCard);
            QuickHPCheck(0);

            PrintSeparator("Check for Fire");
            QuickHPStorage(ra);
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Fire, 1);
            UsePower(ladyOfTheWood.CharacterCard);
            QuickHPCheck(0);

            PrintSeparator("Check for Infernal");
            QuickHPStorage(ra);
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Infernal, 1);
            UsePower(ladyOfTheWood.CharacterCard);
            QuickHPCheck(0);

            PrintSeparator("Check for Lightning");
            QuickHPStorage(ra);
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Lightning, 1);
            UsePower(ladyOfTheWood.CharacterCard);
            QuickHPCheck(0);

            PrintSeparator("Check for Melee");
            QuickHPStorage(ra);
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Melee, 1);
            UsePower(ladyOfTheWood.CharacterCard);
            QuickHPCheck(0);

            PrintSeparator("Check for Projectile");
            QuickHPStorage(ra);
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Projectile, 1);
            UsePower(ladyOfTheWood.CharacterCard);
            QuickHPCheck(0);

            PrintSeparator("Check for Psychic");
            QuickHPStorage(ra);
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Psychic, 1);
            UsePower(ladyOfTheWood.CharacterCard);
            QuickHPCheck(0);

            PrintSeparator("Check for Radiant");
            QuickHPStorage(ra);
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Radiant, 1);
            UsePower(ladyOfTheWood.CharacterCard);
            QuickHPCheck(0);

            PrintSeparator("Check for Sonic");
            QuickHPStorage(ra);
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Sonic, 1);
            UsePower(ladyOfTheWood.CharacterCard);
            QuickHPCheck(0);

            PrintSeparator("Check for Toxic");
            QuickHPStorage(ra);
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Toxic, 1);
            UsePower(ladyOfTheWood.CharacterCard);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestLadyOfTheWoodIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            //Select a damage type. Until the start of your next turn, increase damage dealt of the chosen type by 1.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);

            DecisionSelectDamageType = DamageType.Fire;
            UseIncapacitatedAbility(ladyOfTheWood, 0);

            QuickHPStorage(haka);
            DealDamage(ra, haka, 5, DamageType.Fire);

            //Damage dealt should be 5 +1 => 6
            QuickHPCheck(-6);

            //check only for selected damage type
            QuickHPStorage(haka);
            DealDamage(ra, haka, 5, DamageType.Melee);

            //Damage dealt should be 5,no modifiers
            QuickHPCheck(-5);

            GoToStartOfTurn(ladyOfTheWood);

            //start of turn has passed, trigger should be gone
            QuickHPStorage(haka);
            DealDamage(ra, haka, 5, DamageType.Fire);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestLadyOfTheWoodIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            //give hake room to gain health
            SetHitPoints(haka, 20);

            //1 hero may discard a card to regain 3 HP.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);
            DecisionSelectTurnTaker = haka.HeroTurnTaker;
            QuickHandStorage(haka);
            QuickHPStorage(haka);
            UseIncapacitatedAbility(ladyOfTheWood, 1);
            //should have 1 less card in hand and should have gained 3 HP
            QuickHandCheck(-1);
            QuickHPCheck(3);
            
        }

        [Test()]
        public void TestLadyOfTheWoodIncap2_NoDiscard()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            //give hake room to gain health
            SetHitPoints(haka, 20);

            //1 hero may discard a card to regain 3 HP.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);
            DecisionSelectTurnTaker = haka.HeroTurnTaker;
            DecisionDoNotSelectCard = SelectionType.DiscardCard;
            QuickHandStorage(haka);
            QuickHPStorage(haka);
            UseIncapacitatedAbility(ladyOfTheWood, 1);
            //should have same number of cards in hand and the same hp
            QuickHandCheck(0);
            QuickHPCheck(0);

        }

        [Test()]
        public void TestLadyOfTheWoodIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(ladyOfTheWood);

            PutIntoPlay("PoliceBackup");

            //Destroy an environment card.
            GoToUseIncapacitatedAbilityPhase(ladyOfTheWood);

            int numEnvironmentCardsInPlayBefore = GetNumberOfCardsInPlay(FindEnvironment());
            int numEnvironmentCardsInTrashBefore = GetNumberOfCardsInTrash(FindEnvironment());

            UseIncapacitatedAbility(ladyOfTheWood, 2);

            //1 environment card should have been destroyed, so 1 less card in play, 1 more in trash
            AssertNumberOfCardsInPlay(FindEnvironment(), numEnvironmentCardsInPlayBefore - 1);
            AssertNumberOfCardsInTrash(FindEnvironment(), numEnvironmentCardsInTrashBefore + 1);

        }

        [Test()]
        public void TestCalmBeforeTheStormDiscard4()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //grab the hand besides calm
            IEnumerable<Card> cardsInHand = base.GameController.GetAllCardsInHand(ladyOfTheWood);

            Card calm = PutInHand("CalmBeforeTheStorm");
            //Discard any number of cards.
            // Increase the next damage dealt by LadyOfTheWood by X, where X is 2 plus the number of cards discarded this way.
            GoToPlayCardPhase(ladyOfTheWood);
            QuickHandStorage(ladyOfTheWood);

            DecisionSelectCards = new Card[] { cardsInHand.ElementAt(0), cardsInHand.ElementAt(1), cardsInHand.ElementAt(2), cardsInHand.ElementAt(3)};
            PlayCard(calm);
            //should be -5, 1 for calm before storm, and 4 others discarded
            QuickHandCheck(-5);

            QuickHPStorage(haka);
            DealDamage(ladyOfTheWood, haka, 4, DamageType.Melee);
            //since 4 cards discarded, should be +6 for  10 damage
            QuickHPCheck(-10);

        }

        [Test()]
        public void TestCalmBeforeTheStormDiscard3()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //grab the hand besides calm
            IEnumerable<Card> cardsInHand = base.GameController.GetAllCardsInHand(ladyOfTheWood);

            Card calm = PutInHand("CalmBeforeTheStorm");
            //Discard any number of cards.
            // Increase the next damage dealt by LadyOfTheWood by X, where X is 2 plus the number of cards discarded this way.
            GoToPlayCardPhase(ladyOfTheWood);
            QuickHandStorage(ladyOfTheWood);

            DecisionSelectCards = new Card[] { cardsInHand.ElementAt(0), cardsInHand.ElementAt(1), cardsInHand.ElementAt(2), null };
            PlayCard(calm);
            //should be -4, 1 for calm before storm, and 3 others discarded
            QuickHandCheck(-4);

            QuickHPStorage(haka);
            DealDamage(ladyOfTheWood, haka, 4, DamageType.Melee);
            //since 3 cards discarded, should be +5 for  9 damage
            QuickHPCheck(-9);

        }

        [Test()]
        public void TestCalmBeforeTheStormDiscard2()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //grab the hand besides calm
            IEnumerable<Card> cardsInHand = base.GameController.GetAllCardsInHand(ladyOfTheWood);

            Card calm = PutInHand("CalmBeforeTheStorm");
            //Discard any number of cards.
            // Increase the next damage dealt by LadyOfTheWood by X, where X is 2 plus the number of cards discarded this way.
            GoToPlayCardPhase(ladyOfTheWood);
            QuickHandStorage(ladyOfTheWood);

            DecisionSelectCards = new Card[] { cardsInHand.ElementAt(0), cardsInHand.ElementAt(1), null };
            PlayCard(calm);
            //should be -3, 1 for calm before storm, and 2 others discarded
            QuickHandCheck(-3);

            QuickHPStorage(haka);
            DealDamage(ladyOfTheWood, haka, 4, DamageType.Melee);
            //since 2 cards discarded, should be +4 for 8 damage
            QuickHPCheck(-8);

        }

        [Test()]
        public void TestCalmBeforeTheStormDiscard1()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //grab the hand besides calm
            IEnumerable<Card> cardsInHand = base.GameController.GetAllCardsInHand(ladyOfTheWood);

            Card calm = PutInHand("CalmBeforeTheStorm");
            //Discard any number of cards.
            // Increase the next damage dealt by LadyOfTheWood by X, where X is 2 plus the number of cards discarded this way.
            GoToPlayCardPhase(ladyOfTheWood);
            QuickHandStorage(ladyOfTheWood);

            DecisionSelectCards = new Card[] { cardsInHand.ElementAt(0), null };
            PlayCard(calm);
            //should be -2, 1 for calm before storm, and 1 other discarded
            QuickHandCheck(-2);

            QuickHPStorage(haka);
            DealDamage(ladyOfTheWood, haka, 4, DamageType.Melee);
            //since 1 card discarded, should be +3 for  7 damage
            QuickHPCheck(-7);

        }

        [Test()]
        public void TestCalmBeforeTheStormNoDiscard()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card calm = PutInHand("CalmBeforeTheStorm");
            //Discard any number of cards.
            // Increase the next damage dealt by LadyOfTheWood by X, where X is 2 plus the number of cards discarded this way.
            GoToPlayCardPhase(ladyOfTheWood);
            QuickHandStorage(ladyOfTheWood);
            DecisionDoNotSelectCard = SelectionType.DiscardCard;
            PlayCard(calm);
            //should be -1, 1 for calm before storm, and 0 others discarded
            QuickHandCheck(-1);

            QuickHPStorage(haka);
            DealDamage(ladyOfTheWood, haka, 4, DamageType.Melee);
            //since 0 cards discarded, should be +2 for  6 damage
            QuickHPCheck(-6);

        }

        [Test()]
        public void TestCalmBeforeTheStormCantDiscard()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            DiscardAllCards(ladyOfTheWood);
            PutInHand("CalmBeforeTheStorm");
            Card calm = GetCardFromHand("CalmBeforeTheStorm");
            //Discard any number of cards.
            // Increase the next damage dealt by LadyOfTheWood by X, where X is 2 plus the number of cards discarded this way.
            GoToPlayCardPhase(ladyOfTheWood);
            QuickHandStorage(ladyOfTheWood);
            
            //there shouldn't be any cards to discard
            PlayCard(calm);
            //should be -1, 1 for calm before storm, and 0 others discarded
            QuickHandCheck(-1);

            QuickHPStorage(haka);
            DealDamage(ladyOfTheWood, haka, 4, DamageType.Melee);
            //since 0 cards discarded, should be +2 for  6 damage
            QuickHPCheck(-6);

        }

        [Test()]
        public void TestCrownOfTheFourWinds_4Targets()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, haka.CharacterCard);
            PutInHand("CrownOfTheFourWinds");
            Card crown = GetCardFromHand("CrownOfTheFourWinds");
            GoToPlayCardPhase(ladyOfTheWood); ;
            PlayCard(crown);
            //LadyOfTheWood deals 1 target 1 toxic damage, a second target 1 fire damage, a third target 1 lightning damage, and a fourth target 1 cold damage.
            GoToUsePowerPhase(ladyOfTheWood);
            QuickHPStorage(baron, ladyOfTheWood, ra, haka);
            DecisionSelectCards = new Card[] { baron.CharacterCard, ladyOfTheWood.CharacterCard, ra.CharacterCard, haka.CharacterCard };
            UsePower(crown);
            //every target in play should get 1 damage
            QuickHPCheck(-1, -1, -1, -1);

        }

        [Test()]
        public void TestCrownOfTheFourWinds_DamageTypes()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            //destroy mdp to make baron vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, haka.CharacterCard);
            //add another target to play to make explicit card selection work
            Card battalion = PutIntoPlay("BladeBattalion");

            PutInHand("CrownOfTheFourWinds");
            Card crown = GetCardFromHand("CrownOfTheFourWinds");
            GoToPlayCardPhase(ladyOfTheWood); ;
            PlayCard(crown);
            //LadyOfTheWood deals 1 target 1 toxic damage, a second target 1 fire damage, a third target 1 lightning damage, and a fourth target 1 cold damage.
            GoToUsePowerPhase(ladyOfTheWood);
            QuickHPStorage(baron, ladyOfTheWood, ra, haka);
            DecisionSelectCards = new Card[] {
                baron.CharacterCard, ladyOfTheWood.CharacterCard, ra.CharacterCard, haka.CharacterCard, 
                baron.CharacterCard, ladyOfTheWood.CharacterCard, ra.CharacterCard, haka.CharacterCard,
                baron.CharacterCard, ladyOfTheWood.CharacterCard, ra.CharacterCard, haka.CharacterCard,
                baron.CharacterCard, ladyOfTheWood.CharacterCard, ra.CharacterCard, haka.CharacterCard 
            };
            PrintSeparator("Check for Toxic");
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Toxic, 1);
            UsePower(crown);
            QuickHPCheck(0, -1, -1, -1);

            PrintSeparator("Check for Fire");
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Fire, 1);
            UsePower(crown);
            QuickHPCheck(-1, 0, -1, -1);


            PrintSeparator("Check for Lightning");
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Lightning, 1);
            UsePower(crown);
            QuickHPCheck(-1, -1, 0, -1);

            PrintSeparator("Check for Cold");
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Cold, 1);
            UsePower(crown);
            QuickHPCheck(-1, -1, -1, 0);

        }

        [Test()]
        public void TestCrownOfTheFourWinds_MoreThan4Targets()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, haka.CharacterCard);
            Card battalion = PutIntoPlay("BladeBattalion");

            PutInHand("CrownOfTheFourWinds");
            Card crown = GetCardFromHand("CrownOfTheFourWinds");
            GoToPlayCardPhase(ladyOfTheWood); ;
            PlayCard(crown);
            //LadyOfTheWood deals 1 target 1 toxic damage, a second target 1 fire damage, a third target 1 lightning damage, and a fourth target 1 cold damage.
            GoToUsePowerPhase(ladyOfTheWood);
            QuickHPStorage(baron, ladyOfTheWood, ra, haka);
            DecisionSelectCards = new Card[] { baron.CharacterCard, ladyOfTheWood.CharacterCard, ra.CharacterCard, haka.CharacterCard };
            UsePower(crown);
            //every target in play should get 1 damage
            QuickHPCheck(-1, -1, -1, -1);

        }

        [Test()]
        public void TestCrownOfTheFourWinds_LessThan3Targets()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //incap haka to have only 3 targets
            SetHitPoints(haka.CharacterCard, 1);
            DealDamage(baron, haka, 2, DamageType.Melee);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, haka.CharacterCard);
            PutInHand("CrownOfTheFourWinds");
            Card crown = GetCardFromHand("CrownOfTheFourWinds");
            GoToPlayCardPhase(ladyOfTheWood); ;
            PlayCard(crown);
            //LadyOfTheWood deals 1 target 1 toxic damage, a second target 1 fire damage, a third target 1 lightning damage, and a fourth target 1 cold damage.
            GoToUsePowerPhase(ladyOfTheWood);
            QuickHPStorage(baron, ladyOfTheWood, ra);
            DecisionSelectCards = new Card[] { baron.CharacterCard, ladyOfTheWood.CharacterCard, ra.CharacterCard };

            UsePower(crown);
            //every target in play should get 1 damage (but only 3 targets)
            QuickHPCheck(-1, -1, -1);

        }

        [Test()]
        public void TestEnchantedClearing()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //Reduce damage dealt to LadyOfTheWood by 1.
            Card clearing = PlayCard("EnchantedClearing");

            QuickHPStorage(ladyOfTheWood);
            DealDamage(baron, ladyOfTheWood, 5, DamageType.Melee);

            //reduced by 1, so should be 4
            QuickHPCheck(-4);

            //check that it is gone if enchanted cleared destroyed
            DestroyCard(clearing, baron.CharacterCard);
            QuickHPStorage(ladyOfTheWood);
            DealDamage(baron, ladyOfTheWood, 5, DamageType.Melee);
            //should not be modified, so -5
            QuickHPCheck(-5);

        }

        [Test()]
        public void TestFallDealLightning()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //Whenever LadyOfTheWood deals lightning damage to a target, reduce damage dealt by that target by 1 until the start of your next turn.
            GoToPlayCardPhase(ladyOfTheWood);
            PlayCard("Fall");
           
            DealDamage(ladyOfTheWood, haka, 8, DamageType.Lightning);

            QuickHPStorage(ra);
            DealDamage(haka, ra, 5, DamageType.Projectile);
            //reduced by 1, so should be 4
            QuickHPCheck(-4);

            GoToStartOfTurn(ladyOfTheWood);
            QuickHPStorage(ra);
            DealDamage(haka, ra, 5, DamageType.Projectile);
            //no longer reduced by 1, so should be 5
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestFallDealNotLightning()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //Whenever LadyOfTheWood deals lightning damage to a target, reduce damage dealt by that target by 1 until the start of your next turn.
            GoToPlayCardPhase(ladyOfTheWood);
            PlayCard("Fall");

            DealDamage(ladyOfTheWood, haka, 8, DamageType.Fire);

            QuickHPStorage(ra);
            DealDamage(haka, ra, 5, DamageType.Projectile);
            //not lightning so not reduced by 1, so should be 5
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestFireInTheCloudsOption1()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //LadyOfTheWood deals 1 target 3 fire damage or up to 3 targets 1 lightning damage each.
            GoToPlayCardPhase(ladyOfTheWood);
            //should deal 1 target 3 fire
            DecisionSelectFunction = 0;
            DecisionSelectTarget = mdp;
            QuickHPStorage(mdp);
            PlayCard("FireInTheClouds");
            QuickHPCheck(-3);
            
        }

        [Test()]
        public void TestFireInTheCloudsOption1_DamageType()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //LadyOfTheWood deals 1 target 3 fire damage or up to 3 targets 1 lightning damage each.
            GoToPlayCardPhase(ladyOfTheWood);
            //should deal 1 target 3 fire
            DecisionSelectFunction = 0;
            DecisionSelectTarget = mdp;
            QuickHPStorage(mdp);

            PrintSeparator("Check for fire");
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Fire, 1);
            PlayCard("FireInTheClouds");
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestFireInTheCloudsOption2_3Targets()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);
            //LadyOfTheWood deals 1 target 3 fire damage or up to 3 targets 1 lightning damage each.
            GoToPlayCardPhase(ladyOfTheWood);
            //should deal 3 target 1 lightning
            DecisionSelectFunction = 1;
            DecisionSelectCards = new Card[] { baron.CharacterCard, ladyOfTheWood.CharacterCard, ra.CharacterCard };
            QuickHPStorage(baron, ladyOfTheWood, ra, haka);
            PlayCard("FireInTheClouds");
            QuickHPCheck(-1, -1, -1, 0);
        }

        [Test()]
        public void TestFireInTheCloudsOption2_2Targets()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);
            //LadyOfTheWood deals 1 target 3 fire damage or up to 3 targets 1 lightning damage each.
            GoToPlayCardPhase(ladyOfTheWood);
            //should deal 3 target 1 lightning
            DecisionSelectFunction = 1;
            DecisionSelectCards = new Card[] { baron.CharacterCard, ladyOfTheWood.CharacterCard, null };
            QuickHPStorage(baron, ladyOfTheWood, ra, haka);
            PlayCard("FireInTheClouds");
            QuickHPCheck(-1, -1, 0, 0);
        }

        [Test()]
        public void TestFireInTheCloudsOption2_1Target()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);
            //LadyOfTheWood deals 1 target 3 fire damage or up to 3 targets 1 lightning damage each.
            GoToPlayCardPhase(ladyOfTheWood);
            //should deal 3 target 1 lightning
            DecisionSelectFunction = 1;
            DecisionSelectCards = new Card[] { baron.CharacterCard, null };
            QuickHPStorage(baron, ladyOfTheWood, ra, haka);

            PlayCard("FireInTheClouds");
            QuickHPCheck(-1, 0, 0, 0);
        }

        [Test()]
        public void TestFireInTheCloudsOption2_DamageType()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);
            //LadyOfTheWood deals 1 target 3 fire damage or up to 3 targets 1 lightning damage each.
            GoToPlayCardPhase(ladyOfTheWood);
            //should deal 3 target 1 lightning
            DecisionSelectFunction = 1;
            DecisionSelectCards = new Card[] { baron.CharacterCard, null };
            QuickHPStorage(baron, ladyOfTheWood, ra, haka);
            PrintSeparator("Check for lightning");
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Lightning, 1);
            PlayCard("FireInTheClouds");
            QuickHPCheck(0, 0, 0, 0);
        }

        [Test()]
        public void TestFireInTheCloudsOption2_NoTargets()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);
            //LadyOfTheWood deals 1 target 3 fire damage or up to 3 targets 1 lightning damage each.
            GoToPlayCardPhase(ladyOfTheWood);
            //should deal 3 target 1 lightning
            DecisionSelectFunction = 1;
            DecisionDoNotSelectCard = SelectionType.SelectTarget;
            QuickHPStorage(baron, ladyOfTheWood, ra, haka);
            PlayCard("FireInTheClouds");
            QuickHPCheck(0, 0, 0, 0);
        }

        [Test()]
        public void TestFrostOnThePetalsOption1()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //LadyOfTheWood deals 1 target 3 toxic damage or up to 3 targets 1 cold damage each.
            GoToPlayCardPhase(ladyOfTheWood);
            //should deal 1 target 3 toxic
            DecisionSelectFunction = 0;
            DecisionSelectTarget = mdp;
            QuickHPStorage(mdp);
            PlayCard("FrostOnThePetals");
            QuickHPCheck(-3);
            

        }

        [Test()]
        public void TestFrostOnThePetalsOption1_DamageType()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //LadyOfTheWood deals 1 target 3 toxic damage or up to 3 targets 1 cold damage each.
            GoToPlayCardPhase(ladyOfTheWood);
            //should deal 1 target 3 fire
            DecisionSelectFunction = 0;
            DecisionSelectTarget = mdp;
            QuickHPStorage(mdp);

            PrintSeparator("Check for toxic");
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Toxic, 1);
            PlayCard("FrostOnThePetals");
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestFrostOnThePetalsOption2_DamageType()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);
            //LadyOfTheWood deals 1 target 3 toxic damage or up to 3 targets 1 cold damage each.
            GoToPlayCardPhase(ladyOfTheWood);
            //should deal 3 target 1 lightning
            DecisionSelectFunction = 1;
            DecisionSelectCards = new Card[] { baron.CharacterCard, null };
            QuickHPStorage(baron, ladyOfTheWood, ra, haka);
            PrintSeparator("Check for cold");
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Cold, 1);
            PlayCard("FrostOnThePetals");
            QuickHPCheck(0, 0, 0, 0);
        }

        [Test()]
        public void TestFrostOnThePetalsOption2_3Targets()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);
            //LadyOfTheWood deals 1 target 3 toxic damage or up to 3 targets 1 cold damage each.
            GoToPlayCardPhase(ladyOfTheWood);
            //should deal 3 target 1 cold
            DecisionSelectFunction = 1;
            DecisionSelectCards = new Card[] { baron.CharacterCard, ladyOfTheWood.CharacterCard, ra.CharacterCard };
            QuickHPStorage(baron, ladyOfTheWood, ra, haka);
            PlayCard("FrostOnThePetals");
            QuickHPCheck(-1, -1, -1, 0);
        }

        [Test()]
        public void TestFrostOnThePetalsOption2_2Targets()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);
            //LadyOfTheWood deals 1 target 3 toxic damage or up to 3 targets 1 cold damage each.
            GoToPlayCardPhase(ladyOfTheWood);
            //should deal 3 target 1 cold
            DecisionSelectFunction = 1;
            DecisionSelectCards = new Card[] { baron.CharacterCard, ladyOfTheWood.CharacterCard, null };
            QuickHPStorage(baron, ladyOfTheWood, ra, haka);
            PlayCard("FrostOnThePetals");
            QuickHPCheck(-1, -1, 0, 0);
        }

        [Test()]
        public void TestFrostOnThePetalsOption2_1Target()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);
            //LadyOfTheWood deals 1 target 3 toxic damage or up to 3 targets 1 cold damage each.
            GoToPlayCardPhase(ladyOfTheWood);
            //should deal 3 target 1 cold
            DecisionSelectFunction = 1;
            DecisionSelectCards = new Card[] { baron.CharacterCard, null };
            QuickHPStorage(baron, ladyOfTheWood, ra, haka);
            PlayCard("FrostOnThePetals");
            QuickHPCheck(-1, 0, 0, 0);
        }

        [Test()]
        public void TestFrostOnThePetalsOption2_NoTargets()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);
            //LadyOfTheWood deals 1 target 3 toxic damage or up to 3 targets 1 cold damage each.
            GoToPlayCardPhase(ladyOfTheWood);
            //should deal 3 target 1 cold
            DecisionSelectFunction = 1;
            DecisionDoNotSelectCard = SelectionType.SelectTarget;
            QuickHPStorage(baron, ladyOfTheWood, ra, haka);
            PlayCard("FrostOnThePetals");
            QuickHPCheck(0, 0, 0, 0);
        }

        [Test()]
        public void TestMeadowRushDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //stack deck
            Card spring = StackDeck(ladyOfTheWood, "Spring");


            //Draw a card.
            PutInHand("MeadowRush");
            Card meadow = GetCardFromHand("MeadowRush");

            //choose to not play a card and not to search
            DecisionDoNotSelectCard = SelectionType.PlayCard;
 
            PlayCard(meadow);
            //spring - which was stacked on top of deck, should be now in the hand
            AssertInHand(spring);

        }

        [Test()]
        public void TestMeadowRushSearch()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card meadow = PutInHand("MeadowRush");

            //stack deck to prevent drawing a season
            PutOnDeck("FireInTheClouds");

            int numSeasonsBefore = GetNumberOfSeasonsInHand(ladyOfTheWood);
            //pick a random season from deck
            DecisionSelectCard = (ladyOfTheWood.HeroTurnTaker.Deck.Cards.Where(c => IsSeason(c)).Take(1)).First();
            //Search your deck for a season card, put it into your hand, then shuffle your deck.
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            QuickShuffleStorage(ladyOfTheWood.TurnTaker.Deck);
            PlayCard(meadow);
            QuickShuffleCheck(1);
            AssertNumberOfSeasonsInHand(ladyOfTheWood, numSeasonsBefore + 1);

        }

        [Test()]
        public void TestMeadowRushPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //Discard all cards in hand
            DiscardAllCards(ladyOfTheWood);

            //put "Spring" on the top of the deck, a card that will stay in play
            Card spring = PutOnDeck("Spring");

            Card meadow = PutInHand("MeadowRush");
            //You may play a card.

            PlayCard(meadow);
            //expect just two card in play, character card and played card
            AssertIsInPlay(spring);

        }

        [Test()]
        public void TestNobilityOfDuskIncreaseOnFirstDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //Once per turn when LadyOfTheWood would deal damage you may increase that damage by 2.

            PlayCard("NobilityOfDusk");
            //use effect
            DecisionYesNo = true;
            QuickHPStorage(haka);
            DealDamage(ladyOfTheWood, haka, 5, DamageType.Lightning);
            //damage should have increased by 2, so 7
            QuickHPCheck(-7);

            //try again, this time there should be no option to increase
            QuickHPStorage(haka);
            DealDamage(ladyOfTheWood, haka, 5, DamageType.Lightning);
            //damage should have not been increased by 2, so 5
            QuickHPCheck(-5);

            //at next turn lady, should reset
            GoToNextTurn();
            DecisionYesNo = true;
            QuickHPStorage(haka);
            DealDamage(ladyOfTheWood, haka, 5, DamageType.Lightning);
            //damage should have increased by 2, so 7
            QuickHPCheck(-7);

            //at next turn anyone, should reset
            GoToNextTurn();
            DecisionYesNo = true;
            QuickHPStorage(haka);
            DealDamage(ladyOfTheWood, haka, 5, DamageType.Lightning);
            //damage should have increased by 2, so 7
            QuickHPCheck(-7);


        }

        [Test()]
        public void TestNobilityOfDuskIncreaseOnSecondDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //Once per turn when LadyOfTheWood would deal damage you may increase that damage by 2.

            PlayCard("NobilityOfDusk");
            //use effect
            DecisionsYesNo = new bool[] { false, true, true };
            QuickHPStorage(haka);
            DealDamage(ladyOfTheWood, haka, 5, DamageType.Lightning);
            //damage should not increase, so 5
            QuickHPCheck(-5);

            //try again, this time should be increased
            QuickHPStorage(haka);
            DealDamage(ladyOfTheWood, haka, 5, DamageType.Lightning);
            //damage should have been increased by 2, so 7
            QuickHPCheck(-7);

            //at next turn, should reset
            GoToNextTurn();
            DecisionYesNo = true;
            QuickHPStorage(haka);
            DealDamage(ladyOfTheWood, haka, 5, DamageType.Lightning);
            //damage should have increased by 2, so 7
            QuickHPCheck(-7);

        }

        [Test()]
        public void TestRainpetalPower()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card cloak = PlayCard("RainpetalCloak");
            //  Power: Draw a card.
            GoToUsePowerPhase(ladyOfTheWood);
            QuickHandStorage(ladyOfTheWood);
            UsePower(cloak);
            QuickHandCheck(1);


        }

        [Test()]
        public void TestRainpetalPrevent()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            //give room to gain hp
            SetHitPoints(ladyOfTheWood.CharacterCard, 15);
            PlayCard("RainpetalCloak");

            //  The first time {LadyOfTheWood} would be dealt 1 damage each turn, she regains 1 HP instead.    
            GoToPlayCardPhase(ladyOfTheWood);
            PrintSeparator("Check initial prevention");
            QuickHPStorage(ladyOfTheWood);
            DealDamage(baron, ladyOfTheWood, 1, DamageType.Infernal);
            //should have prevented and gained an HP
            QuickHPCheck(1);

            PrintSeparator("Check doesn't prevent on second damage");
            QuickHPStorage(ladyOfTheWood);
            DealDamage(baron, ladyOfTheWood, 1, DamageType.Infernal);
            //should have not prevented and is normal
            QuickHPCheck(-1);

            GoToNextTurn();
            PrintSeparator("check resets on non-lady of wood start of turn");
            QuickHPStorage(ladyOfTheWood);
            DealDamage(baron, ladyOfTheWood, 1, DamageType.Infernal);
            //should have prevented again and gained an HP
            QuickHPCheck(1);

        }

        [Test()]
        public void TestRebirthPutCardsUnder_Choose3()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //stack trash
            Card spring = PutInTrash("Spring");
            Card fall = PutInTrash("Fall");
            Card summer = PutInTrash("Summer");


            //When this card enters play, put up to 3 cards from your trash beneath it.

            DecisionSelectCards = new Card[] { spring, fall, summer };
            Card rebirth = PlayCard("LadyOfTheWoodsRebirth");

            //check that there are 3 cards under and that they are the cards under are correct
            AssertNumberOfCardsUnderCard(rebirth, 3);
            AssertUnderCard(rebirth, spring);
            AssertUnderCard(rebirth, fall);
            AssertUnderCard(rebirth, summer);
        }

        [Test()]
        public void TestRebirthPutCardsUnder_Choose2()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //stack trash
            Card spring = PutInTrash("Spring");
            Card fall = PutInTrash("Fall");
            Card summer = PutInTrash("Summer");


            //When this card enters play, put up to 3 cards from your trash beneath it.

            DecisionSelectCards = new Card[] { spring, fall, null };
            Card rebirth = PlayCard("LadyOfTheWoodsRebirth");

            //check that there are 2 cards under and that they are the cards under are correct
            AssertNumberOfCardsUnderCard(rebirth, 2);
            AssertUnderCard(rebirth, spring);
            AssertUnderCard(rebirth, fall);
            AssertInTrash(summer);
        }

        [Test()]
        public void TestRebirthPutCardsUnder_Choose1()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //stack trash
            Card spring = PutInTrash("Spring");
            Card fall = PutInTrash("Fall");
            Card summer = PutInTrash("Summer");


            //When this card enters play, put up to 3 cards from your trash beneath it.

            DecisionSelectCards = new Card[] { spring, null };
            Card rebirth = PlayCard("LadyOfTheWoodsRebirth");

            //check that there is 1 card under and that it is correct
            AssertNumberOfCardsUnderCard(rebirth, 1);
            AssertUnderCard(rebirth, spring);
            AssertInTrash(fall, summer);
        }

        [Test()]
        public void TestRebirthPutCardsUnder_Choose0()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //stack trash
            Card spring = PutInTrash("Spring");
            Card fall = PutInTrash("Fall");
            Card summer = PutInTrash("Summer");


            //When this card enters play, put up to 3 cards from your trash beneath it.

            DecisionDoNotSelectCard = SelectionType.MoveCard;
            Card rebirth = PlayCard("LadyOfTheWoodsRebirth");

            //since there are 0 cards moved under this card, it should immediately destroy itself
            AssertInTrash(rebirth);
        }

        [Test()]
        public void TestRebirthPutCardsUnder_NoCardsToMove()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //When this card enters play, put up to 3 cards from your trash beneath it.
            Card rebirth = PlayCard("LadyOfTheWoodsRebirth");

            //since there are no cards to be moved under this card, it should immediately destroy itself
            AssertInTrash(rebirth);
        }

        [Test()]
        public void TestRebirthMoveCardToHand()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card battalion = PlayCard("BladeBattalion");

            //stack trash
            Card spring = PutInTrash("Spring");
            Card fall = PutInTrash("Fall");
            Card summer = PutInTrash("Summer");

            //Whenever LadyOfTheWood destroys a target, put a card from beneath this one into your hand.
            DecisionSelectCards = new Card[] { spring, fall, summer, summer };
            Card rebirth = PlayCard("LadyOfTheWoodsRebirth");

            QuickHandStorage(ladyOfTheWood);
            //have lady of the wood destroy mdp
            DestroyCard(mdp, ladyOfTheWood.CharacterCard);
            //summer should have been moved to hand
            QuickHandCheck(1);
            AssertNumberOfCardsUnderCard(rebirth, 2);
            AssertUnderCard(rebirth, spring);
            AssertUnderCard(rebirth, fall);
            AssertInHand(summer);

            //check doesn't move when another hero destroys a target
            QuickHandStorage(ladyOfTheWood);
            //have lady of the wood destroy mdp
            DestroyCard(battalion, haka.CharacterCard);
            //nothing should have been moved to hand
            QuickHandCheck(0);
            AssertNumberOfCardsUnderCard(rebirth, 2);
            AssertUnderCard(rebirth, spring);
            AssertUnderCard(rebirth, fall);
        }

        [Test()]
        public void TestRebirthDestroy()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //stack trash
            Card spring = PutInTrash("Spring");
            Card fall = PutInTrash("Fall");
            Card summer = PutInTrash("Summer");


            //When there are no cards beneath this one, destroy this card.
            DecisionSelectCards = new Card[] { spring, fall, summer };
            Card rebirth = PlayCard("LadyOfTheWoodsRebirth");
            
            MoveCards(ladyOfTheWood, new Card[] { spring, fall, summer }, ladyOfTheWood.HeroTurnTaker.Hand);
            AssertNotInPlay(rebirth);
            AssertInTrash(rebirth);
        }

        [Test()]
        public void TestSerenityOfDawnDestroyCard()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            //If LadyOfTheWood deals 3 or more damage to a target, destroy this card.
            Card serenity =  PlayCard("SerenityOfDawn");

            DealDamage(ladyOfTheWood, haka, 2, DamageType.Cold);

            //since less than 3 damage has been dealt, serenity should not be destroyed
            AssertIsInPlay(serenity);

            DealDamage(ladyOfTheWood, haka, 5, DamageType.Cold);

            //since more than 3 damage has been dealt, serenity should be destroyed
            AssertNotInPlay(serenity);
            AssertInTrash(serenity);

        }

        [Test()]
        public void TestSerenityOfDawnEndOfTurnHasDealtDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            //At the end of your turn, if {LadyOfTheWood} dealt no damage this turn, she regains 2 HP and you may draw a card.
            GoToPlayCardPhase(ladyOfTheWood);
            PlayCard("SerenityOfDawn");
            DealDamage(ladyOfTheWood, haka, 5, DamageType.Toxic);

            QuickHPStorage(ladyOfTheWood);
            QuickHandStorage(ladyOfTheWood);
            GoToEndOfTurn(ladyOfTheWood);
            //since damage was dealt, no hp should be gained
            QuickHPCheckZero();
            //since damage was dealt no cards should have been drawn
            QuickHandCheck(0);
        }

        [Test()]
        public void TestSerenityOfDawnEndOfTurnHasNotDealtDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //set hp to have room to gain later
            SetHitPoints(ladyOfTheWood.CharacterCard, 15);
            //At the end of your turn, if {LadyOfTheWood} dealt no damage this turn, she regains 2 HP and you may draw a card.
            PlayCard("SerenityOfDawn");
            DecisionYesNo = true;
            QuickHPStorage(ladyOfTheWood);
            QuickHandStorage(ladyOfTheWood);
            GoToEndOfTurn(ladyOfTheWood);
            //since no damage dealt, gain 2 HP, draw 1 card
            QuickHPCheck(2);
            QuickHandCheck(1);
            
        }

        [Test()]
        public void TestSerenityOfDawnEndOfTurnHasNotDealtDamage_DrawOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //set hp to have room to gain later
            SetHitPoints(ladyOfTheWood.CharacterCard, 15);
            //At the end of your turn, if {LadyOfTheWood} dealt no damage this turn, she regains 2 HP and you may draw a card.
            PlayCard("SerenityOfDawn");
            DecisionYesNo = false;
            QuickHPStorage(ladyOfTheWood);
            QuickHandStorage(ladyOfTheWood);
            GoToEndOfTurn(ladyOfTheWood);
            //since no damage dealt, gain 2 HP, draw card was declined, so no new cards in hand
            QuickHPCheck(2);
            QuickHandCheck(0);

        }

        [Test()]
        public void TestSnowshadeGownPower()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //give room to gain HP
            SetHitPoints(ladyOfTheWood, 10);

            Card gown = PlayCard("SnowshadeGown");

            // power: LadyOfTheWood regains 3HP.
            GoToUsePowerPhase(ladyOfTheWood);
            QuickHPStorage(ladyOfTheWood);
            UsePower(gown);
            QuickHPCheck(3);
        }

        [Test()]
        public void TestSnowshadeGownDealDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //give room to gain HP
            SetHitPoints(ladyOfTheWood, 10);

            //destroy mdp so baron is vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card gown = PlayCard("SnowshadeGown");

            // Whenever LadyOfTheWood regains HP, you may select a target that hasn't been dealt damage this turn. LadyOfTheWood deals that target 1 cold damage.

            GoToUsePowerPhase(ladyOfTheWood);


            //Have baron blade and LotW have been dealt damage they aren't available for the reaction
            DealDamage(ra, baron, 5, DamageType.Fire);
            DealDamage(ra, ladyOfTheWood, 3, DamageType.Fire);
            DecisionYesNo = true;
            DecisionSelectCard = ra.CharacterCard;
            QuickHPStorage(ra);
            //Use LotW power to trigger reaction
            UsePower(gown);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestSnowshadeGownCantDealDamageIfNoHPGain()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

 

            //destroy mdp so baron is vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card gown = PlayCard("SnowshadeGown");

            // Whenever LadyOfTheWood regains HP, you may select a target that hasn't been dealt damage this turn. LadyOfTheWood deals that target 1 cold damage.

            GoToUsePowerPhase(ladyOfTheWood);


            //Have baron blade and LotW have been dealt damage they aren't available for the reaction
            DecisionYesNo = true;
            DecisionSelectTarget = ra.CharacterCard;
            QuickHPStorage(ra);
            //Use LotW power to trigger reaction
            UsePower(gown);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestSnowshadeGownCantDealDamageIfPartialHPGain()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //give room to gain 2 HP, but not the full 3HP from the power.
            SetHitPoints(ladyOfTheWood, 20);

            //destroy mdp so baron is vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card gown = PlayCard("SnowshadeGown");

            // Whenever LadyOfTheWood regains HP, you may select a target that hasn't been dealt damage this turn. LadyOfTheWood deals that target 1 cold damage.
            GoToUsePowerPhase(ladyOfTheWood);

            //deal damage to ra with the trigger
            DecisionYesNo = true;
            DecisionSelectTarget = ra.CharacterCard;

            QuickHPStorage(ladyOfTheWood, ra);
            //try to gain 3HP and trigger the reaction
            UsePower(gown);
            QuickHPCheck(2, -1);
        }

        [Test()]
        public void TestSnowshadeGownDealDamage_NoTargets()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //give room to gain HP
            SetHitPoints(ladyOfTheWood, 10);

            //destroy mdp so baron is vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card gown = PlayCard("SnowshadeGown");

            // Whenever LadyOfTheWood regains HP, you may select a target that hasn't been dealt damage this turn. LadyOfTheWood deals that target 1 cold damage.

            GoToUsePowerPhase(ladyOfTheWood);


            //Have baron blade and LotW have been dealt damage they aren't available for the reaction
            DealDamage(ra, baron, 5, DamageType.Fire);
            DealDamage(ra, ladyOfTheWood, 3, DamageType.Fire);
            DealDamage(ra, ra, 2, DamageType.Fire);
            DealDamage(ra, haka, 2, DamageType.Fire);
            QuickHPStorage(ra);
            DecisionSelectCard = ra.CharacterCard;
            //Use LotW power to trigger reaction
            //since there are no targets in play, shouldn't have dealt damage
            UsePower(gown);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestSnowshadeGownDealDamage_Optional()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //give room to gain HP
            SetHitPoints(ladyOfTheWood, 10);

            //destroy mdp so baron is vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card gown = PlayCard("SnowshadeGown");

            // Whenever LadyOfTheWood regains HP, you may select a target that hasn't been dealt damage this turn. LadyOfTheWood deals that target 1 cold damage.

            GoToUsePowerPhase(ladyOfTheWood);

            QuickHPStorage(baron);
            DecisionYesNo = false;

            //Use LotW power to trigger reaction
            //since we declined, shouldn't have dealt damage
            UsePower(gown);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestSnowshadeGownDealDamage_DamageType()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //give room to gain HP
            SetHitPoints(ladyOfTheWood, 10);

            //destroy mdp so baron is vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            Card gown = PlayCard("SnowshadeGown");

            // Whenever LadyOfTheWood regains HP, you may select a target that hasn't been dealt damage this turn. LadyOfTheWood deals that target 1 cold damage.

            GoToUsePowerPhase(ladyOfTheWood);


            DealDamage(ra, baron, 5, DamageType.Fire);
            DealDamage(ra, ladyOfTheWood, 3, DamageType.Fire);
            DecisionSelectCard = ra.CharacterCard;
            QuickHPStorage(ra);
            //Use LotW power to trigger reaction
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Cold, 1);
            UsePower(gown);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestSpring()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //set hp so room to gain more
            SetHitPoints(ladyOfTheWood, 10);
            //put spring in play
            PutIntoPlay("Spring");

            //Whenever LadyOfTheWood deals toxic damage to a target, she regains that much HP.

            PrintSeparator("Check for toxic");
            QuickHPStorage(ladyOfTheWood);
            DealDamage(ladyOfTheWood, ra, 5, DamageType.Toxic);
            //since she dealt 5 toxic damage, she should gain 5 HP
            QuickHPCheck(5);

            PrintSeparator("Check for non-toxic");
            QuickHPStorage(ladyOfTheWood);
            DealDamage(ladyOfTheWood, ra, 5, DamageType.Fire);
            //since she dealt fire damage, she should not gain HP
            QuickHPCheckZero();

            PrintSeparator("Check for others dealing toxic");
            QuickHPStorage(ladyOfTheWood);
            DealDamage(haka, ra, 5, DamageType.Toxic);
            //since not lady of the wood dealt the damage, she should not gain hp
            QuickHPCheckZero();

        }

        [Test()]
        public void TestSummer()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //put summer in play
            PutIntoPlay("Summer");
            //Increase fire damage dealt by LadyOfTheWood by 2.

            PrintSeparator("Check for fire");
            QuickHPStorage(ra);
            DealDamage(ladyOfTheWood, ra, 5, DamageType.Fire);
            //since she dealt 5 fire damage, +2 damage, for 7 total
            QuickHPCheck(-7);

            PrintSeparator("Check for non-fire");
            //check bonus doesn't apply to non fire damage
            QuickHPStorage(ra);
            DealDamage(ladyOfTheWood, ra, 5, DamageType.Toxic);
            //since she dealt not fire damage, no bonus, just 5
            QuickHPCheck(-5);

            PrintSeparator("Check for other target dealing fire");
            //check bonus doesn't apply to other damage sources
            QuickHPStorage(haka);
            DealDamage(ra, haka, 5, DamageType.Fire);
            //since other target, just 5
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestSuncastMantlePower()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //put suncast mantle in play
            Card suncast = PutIntoPlay("SuncastMantle");

            //LadyOfTheWood deals herself 1 fire damage and 1 target 4 fire damage.
            QuickHPStorage(ladyOfTheWood, haka, baron, ra);
            DecisionSelectTarget = haka.CharacterCard;
            UsePower(suncast);
            QuickHPCheck(-1, -4, 0, 0);
        }

        [Test()]
        public void TestSuncastMantlePower_DamageType()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //put suncast mantle in play
            Card suncast = PutIntoPlay("SuncastMantle");

            //LadyOfTheWood deals herself 1 fire damage and 1 target 4 fire damage.
            QuickHPStorage(ladyOfTheWood, haka, baron, ra);
            DecisionSelectTarget = haka.CharacterCard;
            AddReduceDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Fire, 1);
            UsePower(suncast);
            QuickHPCheck(0, -4, 0, 0);
        }

        [Test()]
        public void TestSuncastMantleIncrease()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();
            //set hp to be less than 5
            SetHitPoints(ladyOfTheWood, 3);
            //put suncast mantle in play
            PutIntoPlay("SuncastMantle");
            Card suncast = GetCardInPlay("SuncastMantle");
            //Increase damage dealt by LadyOfTheWood by 3 as long as her HP is 5 or less.
            PrintSeparator("check for lady of the wood damage when hp less than 5");
            QuickHPStorage(haka);
            DecisionSelectTarget = haka.CharacterCard;
            DealDamage(ladyOfTheWood, haka, 2, DamageType.Fire);
            //should be +3 for 5 damage
            QuickHPCheck(-5);

            PrintSeparator("check for others damage when hp less than 5");
            QuickHPStorage(haka);
            DecisionSelectTarget = haka.CharacterCard;
            DealDamage(ra, haka, 2, DamageType.Fire);
            //should be +0 for 2 damage
            QuickHPCheck(-2);

            //check when HP greater than 5
            PrintSeparator("check for when hp greater than 5");
            SetHitPoints(ladyOfTheWood, 7);
            QuickHPStorage(haka);
            DecisionSelectTarget = haka.CharacterCard;
            DealDamage(ladyOfTheWood, haka, 2, DamageType.Fire);
            //should be +0 for 2 damage
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestThundergreyShawlPower()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            GoToUsePowerPhase(ladyOfTheWood);
            Card shawl = PlayCard("ThundergreyShawl");

            // power: LadyOfTheWood deals up to 2 targets 1 lightning damage each."
            DecisionSelectCards = new Card[] { ra.CharacterCard, haka.CharacterCard };
            QuickHPStorage(ra, haka, baron);
            UsePower(shawl);
            QuickHPCheck(-1, -1, 0);

        }

        [Test()]
        public void TestThundergreyShawlPower_Choose0()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            GoToUsePowerPhase(ladyOfTheWood);
            Card shawl = PlayCard("ThundergreyShawl");

            // power: LadyOfTheWood deals up to 2 targets 1 lightning damage each."
            QuickHPStorage(ra, haka, baron);
            DecisionDoNotSelectCard = SelectionType.SelectTarget;
            UsePower(shawl);
            QuickHPCheck(0, 0, 0);

        }

        [Test()]
        public void TestThundergreyShawlPower_DamageType()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            GoToUsePowerPhase(ladyOfTheWood);
            Card shawl = PlayCard("ThundergreyShawl");

            // power: LadyOfTheWood deals up to 2 targets 1 lightning damage each."
            DecisionSelectCards = new Card[] { ra.CharacterCard, haka.CharacterCard };
            QuickHPStorage(ra, haka, baron);
            //use increase damage check since damage will be irreducible
            AddIncreaseDamageOfDamageTypeTrigger(ladyOfTheWood, DamageType.Lightning, 1);
            UsePower(shawl);
            QuickHPCheck(-2, -1, 0);

        }

        [Test()]
        public void TestThundergreyShawlMakeIrreducibleLessThan2()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //destroy mdp so baron is vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            //add a -1 to baron blade
            PlayCard("LivingForceField");

            PlayCard("ThundergreyShawl");
            //  Whenever LadyOfTheWood deals 2 or less damage to a target, that damage is irreducible.
            PrintSeparator("check for lady of the wood less than 2");
            QuickHPStorage(baron);
            DealDamage(ladyOfTheWood, baron, 2, DamageType.Lightning);
            //even though -1 on baron, damage is < 2 so is irreducible
            QuickHPCheck(-2);

            PrintSeparator("check for lady of the wood more than 2");
            QuickHPStorage(baron);
            DealDamage(ladyOfTheWood, baron, 4, DamageType.Lightning);
            //should not be irreducible
            QuickHPCheck(-3);

            //check for other targets not made irreducible
            PrintSeparator("check for other targets deal less than 2");
            QuickHPStorage(baron);
            DealDamage(haka, baron, 2, DamageType.Lightning);
            //should not be irreducible
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestThundergreyShawlStillIrreducibleWhenIncreasedMoreThan2()
        {
            //looks like the irreducibility might linger past being boosted, verifying with tosx
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //set hp less than 5 to trigger suncast mantle
            SetHitPoints(ladyOfTheWood.CharacterCard, 3);
            PlayCard("SuncastMantle");

            //destroy mdp so baron is vulnerable
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DestroyCard(mdp, baron.CharacterCard);

            //add a -1 to baron blade
            PlayCard("LivingForceField");

            PlayCard("ThundergreyShawl");
            //  Whenever LadyOfTheWood deals 2 or less damage to a target, that damage is irreducible.
            QuickHPStorage(baron);
            DealDamage(ladyOfTheWood, baron, 2, DamageType.Lightning);
            //because of suncast mantle, +3 damage, now 5
            //it is still irreducible, so -5
            QuickHPCheck(-5);
        }
        [Test()]
        public void TestThundergreyShawlIrreducibleOrdering()
        {
            //TGS should trigger if the damage totals to 2 or less, with boosts and reductions included
            SetupGameController("Apostate", "Cauldron.LadyOfTheWood", "Legacy", "Haka", "Megalopolis");
            StartGame();

            Card sword = GetCardInPlay("Condemnation");
            Card periapt = PlayCard("PeriaptOfWoe");

            PlayCard("ThundergreyShawl");
            PlayCard("Summer");
            UsePower(legacy);

            //total of +3 to fire damage

            QuickHPStorage(sword);

            DealDamage(ladyOfTheWood, sword, 1, DamageType.Fire);
            DealDamage(ladyOfTheWood, periapt, 1, DamageType.Fire);

            //1 damage +3 boost -2 DR = 2, so Shawl applies and Periapt takes the full 4
            AssertInTrash(periapt);

            //1 damage +3 boost -1 DR = 1, Shawl does not trigger and Condemnation takes 3
            QuickHPCheck(-3);

        }

        [Test()]
        public void TestWinter()
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Haka", "Megalopolis");
            StartGame();

            //put winter in play
            PutIntoPlay("Winter");
            //Whenever {LadyOfTheWood} deals cold damage to a target, draw a card.

            //check draws when cold
            PrintSeparator("check draws when cold");
            QuickHandStorage(ladyOfTheWood);
            DealDamage(ladyOfTheWood, haka, 3, DamageType.Cold);
            QuickHandCheck(1);

            PrintSeparator("check no draws when not cold");
            //check does not draw when not cold
            QuickHandStorage(ladyOfTheWood);
            DealDamage(ladyOfTheWood, haka, 3, DamageType.Toxic);
            QuickHandCheck(0);

            PrintSeparator("check no draws when not lady of the wood");
            //check does not draw when not cold
            QuickHandStorage(ladyOfTheWood);
            DealDamage(ra, haka, 3, DamageType.Cold);
            QuickHandCheck(0);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Season_IsSeason([Values("Spring", "Summer", "Fall", "Winter")] string season)
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(ladyOfTheWood);

            Card card = PlayCard(season);
            AssertInPlayArea(ladyOfTheWood, card);
            AssertCardHasKeyword(card, "season", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Ongoing_IsOngoing([Values("Spring", "Summer", "Fall", "Winter", "LadyOfTheWoodsRebirth", "EnchantedClearing", "SerenityOfDawn", "NobilityOfDusk")] string ongoing)
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Fanatic", "Megalopolis");
            StartGame();
            //put a card in trash so that rebirth doesn't destroy itself
            PutInTrash("SuncastMantle");
            GoToPlayCardPhase(ladyOfTheWood);

            Card card = PlayCard(ongoing);
            AssertInPlayArea(ladyOfTheWood, card);
            AssertCardHasKeyword(card, "ongoing", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Equipment_IsEquipment([Values("RainpetalCloak", "SuncastMantle", "ThundergreyShawl", "SnowshadeGown", "CrownOfTheFourWinds")] string equipment)
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(ladyOfTheWood);

            Card card = PlayCard(equipment);
            AssertInPlayArea(ladyOfTheWood, card);
            AssertCardHasKeyword(card, "equipment", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Limited_IsLimited([Values("RainpetalCloak", "SuncastMantle", "ThundergreyShawl", "SnowshadeGown","Spring", "Summer","Fall","Winter","EnchantedClearing","SerenityOfDawn","NobilityOfDusk", "CrownOfTheFourWinds")] string limited)
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(ladyOfTheWood);

            Card card = PlayCard(limited);
            AssertInPlayArea(ladyOfTheWood, card);
            AssertCardHasKeyword(card, "limited", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Relic_IsRelic([Values("CrownOfTheFourWinds")] string relic)
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(ladyOfTheWood);

            Card card = PlayCard(relic);
            AssertInPlayArea(ladyOfTheWood, card);
            AssertCardHasKeyword(card, "relic", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_OneShot_IsOneShot([Values("MeadowRush", "FrostOnThePetals", "FireInTheClouds", "CalmBeforeTheStorm")] string oneshot)
        {
            SetupGameController("BaronBlade", "Cauldron.LadyOfTheWood", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(ladyOfTheWood);

            Card card = PlayCard(oneshot);
            AssertInTrash(ladyOfTheWood, card);
            AssertCardHasKeyword(card, "one-shot", false);
        }
    }
}
